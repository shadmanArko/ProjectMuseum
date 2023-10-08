extends Camera2D

var zoomSpd: float = 0.05
var Minzoom: float = 0.001
var Maxzoom: float = 2.0
var dragSen: float = 1.0

func _input(event):
	if event is InputEventMouseMotion:
		if Input.is_mouse_button_pressed(MOUSE_BUTTON_RIGHT):
			position -= event.relative * dragSen / zoom

	if event is InputEventMouseButton: # for zooming
		if event.button_index == MOUSE_BUTTON_WHEEL_UP:
			zoom += Vector2(zoomSpd, zoomSpd)
		elif event.button_index == MOUSE_BUTTON_WHEEL_DOWN:
			zoom -= Vector2(zoomSpd, zoomSpd)
		zoom = clamp(zoom, Vector2(Minzoom, Minzoom), Vector2(Maxzoom, Maxzoom))

