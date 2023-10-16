extends Sprite2D

var selectedItem = false
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(delta):
	if !selectedItem: return
	var mouse_tile = GameManager.tilemap.local_to_map(get_global_mouse_position())
	
	var local_pos = GameManager.tilemap.map_to_local(mouse_tile)
	var world_pos = GameManager.tilemap.to_global(local_pos)
	#check if the tile is eligible for this item placement
	#apply effect based on eligibility
	global_position = world_pos
	if selectedItem and Input.is_action_pressed("ui_left_click"):
		selectedItem = false
	if selectedItem and Input.is_action_pressed("ui_right_click"):
		queue_free()

