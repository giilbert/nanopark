extends Node

const KeyboardControls = preload("res://scripts/keyboard-controls.gd")
const player_template = preload("res://templates/player.tscn")

const platformer_player = preload("res://templates/platformer-player.tscn")

var PLAYER_COLORS = [
	Color.from_string("#ec7063", Color.CORNFLOWER_BLUE),
	Color.from_string("#28b463", Color.CORNFLOWER_BLUE),
	Color.from_string("#3498db", Color.CORNFLOWER_BLUE),
]

enum ControlType {
	KEYBOARD,
	JOYSTICK
}

var current_level_id = "main-menu"
var current_id = 0

var players = {}

var num_keyboard_players:
	get:
		var amount = 0
		for player in players.values():
			if player.control_type == ControlType.KEYBOARD:
				amount += 1
		return amount

var num_joystick_players:
	get:
		var amount = 0
		for player in players.values():
			if player.control_type == ControlType.JOYSTICK:
				amount += 1
		return amount

func get_spawn_points(scene = get_tree().current_scene):
	var spawn_points_node = scene.get_node("Spawn Points")
	var positions = []
	for child in spawn_points_node.get_children():
		positions.append(child.position)
	return positions

func add_player(control_type: ControlType):
	var id = self.current_id
	self.current_id += 1
	
	var player = player_template.instantiate()
	player.id = id
	player.control_type = control_type
	if control_type == ControlType.KEYBOARD:
		player.controls = KeyboardControls.new(num_keyboard_players + 1)
	
	self.players[id] = player
	return player

func remove_last_keyboard_player():
	var greatest_id = int("-inf")
	for player in players.values():
		if player.id > greatest_id and player.control_type == ControlType.KEYBOARD:
			greatest_id = player.id
	players.erase(greatest_id)

func load_level(id: String):
	var level_scene = load("res://scenes/levels/" + id + ".tscn").instantiate()
	var current_scene = get_tree().current_scene
	get_tree().root.add_child(level_scene)
	current_scene.queue_free()
	
	# spawn the players again
	var spawn_points = get_spawn_points(level_scene)
	var spawn_point_index = 0
	
	var camera_node = level_scene.get_node("Camera")
	
	for player in players.values():
		var actor = platformer_player.instantiate()
		player.attach_actor(actor)
		camera_node.add_point_of_interest(actor)
		actor.position = spawn_points[spawn_point_index]
		spawn_point_index = (spawn_point_index + 1) % len(spawn_points)
		level_scene.add_child(actor)

func reset_players():
	for player in players:
		pass
