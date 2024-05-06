extends Node2D

@onready var player_template = preload("res://templates/player.tscn")

var COLORS = [
	Color.from_string("#ec7063", Color.CORNFLOWER_BLUE),
	Color.from_string("#28b463", Color.CORNFLOWER_BLUE),
	Color.from_string("#3498db", Color.CORNFLOWER_BLUE),
]


func _ready():
	pass # Replace with function body.

func update_ui():
	$UI/CenterContainer/VBoxContainer/HBoxContainer/Remove.disabled = \
		LevelManager.num_keyboard_players == 0
	$UI/CenterContainer/VBoxContainer/HBoxContainer/Add.disabled = \
		LevelManager.num_keyboard_players == 3
	$UI/CenterContainer/VBoxContainer/HBoxContainer/Label.text =              \
		str(LevelManager.num_keyboard_players) + " keyboard player"           \
		+ ("" if LevelManager.num_keyboard_players == 1 else "s")             \
		+ ", "                                                                \
		+ str(LevelManager.num_joystick_players) + " joystick player"         \
		+ ("" if LevelManager.num_joystick_players == 1 else "s")

func _on_add_pressed():
	LevelManager.add_player(LevelManager.ControlType.KEYBOARD)
	update_ui()


func _on_remove_pressed():
	LevelManager.remove_last_keyboard_player()
	update_ui()


func _on_load_king_of_the_mountain_pressed():
	LevelManager.load_level("king-of-the-mountain")
