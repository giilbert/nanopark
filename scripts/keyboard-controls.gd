extends Object

var up_axis_name = null
var down_axis_name = null
var left_axis_name = null
var right_axis_name = null
var action_axis_name = null

func _init(id: int):
	var _id = str(id)
	self.up_axis_name = "up-" + _id
	self.down_axis_name = "down-" + _id
	self.left_axis_name = "left-" + _id
	self.right_axis_name = "right-" + _id
	self.action_axis_name = "action-" + _id

func get_horizontal_axis():
	return Input.get_axis(left_axis_name, right_axis_name)

func get_vertical_axis():
	return Input.get_axis(down_axis_name, up_axis_name)

func is_action_just_pressed():
	return Input.is_action_just_pressed(action_axis_name)
