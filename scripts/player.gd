extends CharacterBody2D


const SPEED = 250.0
const JUMP_VELOCITY = -400.0

var g = ProjectSettings.get_setting("physics/2d/default_gravity")

@export var left_axis = "left-1"
@export var right_axis = "right-1"
@export var jump_axis = "jump-1"
@export var color: Color = Color.GHOST_WHITE

@onready var material_clone = $AnimatedSprite2D.material.duplicate() 

func _ready():
	self.safe_margin = 0.0001
	material_clone.set_shader_parameter("color", color)
	$AnimatedSprite2D.material = material_clone

var can_double_jump = true

func _physics_process(delta):
	if not is_on_floor():
		velocity.y += g * delta
		if Input.is_action_just_pressed(jump_axis) and can_double_jump:
			$GPUParticles2D.emitting = true
			velocity.y = JUMP_VELOCITY * 1.1
			can_double_jump = false
	else:
		can_double_jump = true
		if Input.is_action_just_pressed(jump_axis):
			velocity.y = JUMP_VELOCITY
	
	var direction = Input.get_axis(left_axis, right_axis)
	if direction:
		velocity.x = direction * SPEED
		$AnimatedSprite2D.flip_h = direction == -1
		$AnimatedSprite2D.animation = "walk"
	else:
		$AnimatedSprite2D.animation = "idle"
		velocity.x = move_toward(velocity.x, 0, SPEED)

	move_and_slide()
