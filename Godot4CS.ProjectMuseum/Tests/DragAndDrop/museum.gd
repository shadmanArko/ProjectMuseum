extends Node2D
#@onready var item1 = $Item1
#@onready var item2 = $Item2


# Called when the node enters the scene tree for the first time.
func _ready():
	GameManager.tilemap = $TileMap

