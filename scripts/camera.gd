extends Camera2D


var points_of_interest = []

func _ready():
	pass

func add_point_of_interest(node):
	points_of_interest.append(node)
	print(points_of_interest)

func remove_point_of_interest(node):
	var index = points_of_interest.find(node)
	points_of_interest.remove_at(index)

func _physics_process(_delta):
	# minimizing x and minimizing y
	var top_left_extrema = points_of_interest[0].position
	# maximizing x and maximizing y
	var bottom_right_extrema = points_of_interest[0].position
	
	for point_of_interest in points_of_interest:
		var px = point_of_interest.position.x
		var py = point_of_interest.position.y
		
		if py < top_left_extrema.y:
			top_left_extrema.y = py
		elif py > bottom_right_extrema.y:
			bottom_right_extrema.y = py
		if px < top_left_extrema.x:
			top_left_extrema.x = px
		elif px > bottom_right_extrema.x:
			bottom_right_extrema.x = px
		
		if bottom_right_extrema.y == 0:
			print(points_of_interest)
	
	
	var target_position = (top_left_extrema + bottom_right_extrema) / 2
	
	var room_size = (top_left_extrema - bottom_right_extrema).abs()
	var zoom_unnormal = room_size / Vector2(get_viewport().size) 
	var z = clampf(max(zoom_unnormal.x, zoom_unnormal.y) + 0.4, 0.5, 6.0)
	
	print(top_left_extrema, " ", bottom_right_extrema)
	
	self.zoom = lerp(self.zoom, Vector2(1/z, 1/z), 0.05)
	self.position = lerp(self.position, target_position, 0.1)


