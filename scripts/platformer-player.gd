extends CharacterBody2D


const SPEED = 300.0
const JUMP_VELOCITY = -450.0

var _core = null
var core:
	get:
		return _core.get_ref()

var g = ProjectSettings.get_setting("physics/2d/default_gravity")

@onready var material_clone = $AnimatedSprite2D.material.duplicate()


func _ready():
	material_clone.set_shader_parameter("color", self.core.color)
	$AnimatedSprite2D.material = material_clone

var can_double_jump = true

func _physics_process(delta):
	if not is_on_floor():
		velocity.y += g * delta
		if self.core.was_up_just_pressed() and can_double_jump:
			$GPUParticles2D.emitting = true
			velocity.y = JUMP_VELOCITY * 1.1
			can_double_jump = false
	else:
		can_double_jump = true
		if self.core.was_up_just_pressed():
			velocity.y = JUMP_VELOCITY
	
	var direction = self.core.get_horizontal_axis()
	if direction:
		velocity.x = direction * SPEED
		$AnimatedSprite2D.flip_h = direction == -1
		$AnimatedSprite2D.animation = "walk"
	else:
		$AnimatedSprite2D.animation = "idle"
		velocity.x = move_toward(velocity.x, 0, SPEED)
	
	if self.core.was_action_just_pressed():
		print("action just pressed")
	
	move_and_slide()
