extends Control

func _draw() -> void:
	var bounds := Rect2(Vector2.ZERO, size)
	draw_rect(bounds, Color(0.075, 0.13, 0.12, 0.97), true)
	for i in range(1, 6):
		var x := size.x * float(i) / 6.0
		var y := size.y * float(i) / 6.0
		draw_line(Vector2(x, 0), Vector2(x, size.y), Color(0.24, 0.34, 0.27, 0.55), 1.0)
		draw_line(Vector2(0, y), Vector2(size.x, y), Color(0.24, 0.34, 0.27, 0.55), 1.0)
	draw_polyline(PackedVector2Array([Vector2(0, size.y * 0.76), Vector2(size.x * 0.24, size.y * 0.60), Vector2(size.x * 0.48, size.y * 0.65), Vector2(size.x * 0.70, size.y * 0.32), Vector2(size.x, size.y * 0.18)]), Color(0.55, 0.53, 0.37, 0.9), 3.0)
	draw_arc(size * 0.52, minf(size.x, size.y) * 0.38, 0, TAU, 64, Color(0.22, 0.82, 0.64, 0.95), 2.0)
	draw_arc(size * 0.52, minf(size.x, size.y) * 0.20, 0, TAU, 64, Color(0.92, 0.83, 0.44, 0.9), 1.5)
	draw_circle(size * 0.52, 4.0, Color(1.0, 0.82, 0.12))
	draw_colored_polygon(PackedVector2Array([size * Vector2(0.52, 0.24), size * Vector2(0.48, 0.62), size * Vector2(0.56, 0.62)]), Color(1.0, 0.78, 0.12))
