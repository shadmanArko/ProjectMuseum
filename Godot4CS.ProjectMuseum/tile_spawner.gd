extends TileMap

var ZERO:int = 0

# Called when the node enters the scene tree for the first time.
func _ready():
	
	for x in range(32):
		for y in range(80):
			var tile : Vector2 = Vector2(x, y)
			set_cell(ZERO, tile, ZERO, Vector2i.ZERO)

