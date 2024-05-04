shader_type canvas_item;

uniform vec4 color : source_color;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec4 originalColor = texture(TEXTURE, UV);
	if (originalColor == vec4(1.0, 0.0, 0.0, 1.0)) {
		COLOR = color;
	} else {
		COLOR = originalColor;
	}
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
