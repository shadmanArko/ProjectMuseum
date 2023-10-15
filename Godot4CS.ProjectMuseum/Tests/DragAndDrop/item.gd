extends Sprite2D

var selectedItem = false
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(delta):
	var mouse_tile = GameManager.tilemap.local_to_map(get_global_mouse_position())
	
	var local_pos = GameManager.tilemap.map_to_local(mouse_tile)
	var world_pos = GameManager.tilemap.to_global(local_pos)
	global_position = world_pos

func _unhandled_input(event):
	if event is InputEventMouseButton and event.is_pressed() and event.button_index == MOUSE_BUTTON_LEFT:
		set_physics_process(false)
		set_process_unhandled_input(false)
		$Area2D.monitoring = false
