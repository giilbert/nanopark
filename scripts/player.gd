extends Node


var controls = null
var control_type = null
var id = null

var actor = null

func attach_actor(node: Node2D):
	self.actor = node
	self.actor._core = weakref(self)


func get_horizontal_axis():
	return self.controls.get_horizontal_axis()

func get_vertical_axis():
	return self.controls.get_vertical_axis()

func get_action_axis():
	return self.controls.get_action_axis()

var dict = {}
func was_just_pressed(name: String, f: Callable):
	var last = dict.get(name, false)
	var f_result = f.call()
	var r = f_result and not last
	dict[name] = f_result
	return r

func was_up_just_pressed():
	return was_just_pressed(
		"up",
		func(): return get_vertical_axis() > 0.5
	)
