extends RigidBody2D


const ACCELERATION = 250.0
const MAX_SPEED = 800.0

var _core = null
var core:
	get:
		return _core.get_ref()

var dx = Vector2.ZERO

@onready var material_clone = $AnimatedSprite2D.material.duplicate()

func _ready():
	material_clone.set_shader_parameter("color", self.core.color)
	$AnimatedSprite2D.material = material_clone

func _physics_process(delta):
	var direction = Vector2(
		self.core.get_horizontal_axis(),
		-self.core.get_vertical_axis()
	)
	
	if self.linear_velocity.length() < MAX_SPEED:
		# no need to multiply by dt here since godot's physics simulation accounts for it
		self.apply_force(direction * ACCELERATION * self.mass)

