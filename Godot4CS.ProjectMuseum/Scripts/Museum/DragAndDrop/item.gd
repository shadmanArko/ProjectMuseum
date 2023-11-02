extends Sprite2D

var selectedItem = false
@export var itemType = "small"
# Called when the node enters the scene tree for the first time.
func _ready():
	pass
#	$HTTPRequest.request_completed.connect(_on_http_request_request_completed)
#	var url = "http://localhost:5178/api/MuseumTile/" + itemType + "/" 
#	$HTTPRequest.request(url) 


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



func _on_http_request_request_completed(result, response_code, headers, body):
	print(result)	
