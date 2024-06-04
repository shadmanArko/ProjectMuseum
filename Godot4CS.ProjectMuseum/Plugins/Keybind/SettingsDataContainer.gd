extends Node

var master_volume : float = 0.0
var music_volume : float = 0.0
var sfx_volume : float = 0.0
var ui_volume : float = 0.0
var ambiance_volume : float = 0.0

@onready var keybind_resource : PlayerKeyBindResource = preload("res://Plugins/Keybind/PlayerKeyBind.tres")

var loaded_data : Dictionary = {}

func _ready():
	create_storage_dictionary()
	
func on_master_volume_set(toggled : bool) -> void:
	pass
	
func on_music_volume_set(toggled : bool) -> void:
	pass
	
func on_sfx_volume_set(toggled : bool) -> void:
	pass
	
func on_ui_volume_set(toggled : bool) -> void:
	pass
	
func on_ambiance_volume_set(toggled : bool) -> void:
	pass
	
	
func handle_signals() -> void:
	pass

func create_storage_dictionary() -> Dictionary:
	var settings_container_dict = {
		"keybind" : create_keybind_dictionary()
	}
	return settings_container_dict

func create_keybind_dictionary() -> Dictionary:
	var keybind_container_dict = {
		keybind_resource.MOVE_LEFT : keybind_resource.move_left_key,
		keybind_resource.MOVE_RIGHT : keybind_resource.move_right_key
	}
	return keybind_container_dict
	
	
func set_keybind(action : String, event) -> void:
	match action:
		keybind_resource.MOVE_LEFT:
			keybind_resource.move_left_key = event
		keybind_resource.MOVE_RIGHT:
			keybind_resource.move_right_key = event
	
	
func on_keybind_loaded(data : Dictionary) -> void:
	var	loaded_move_left = InputEventKey.new()
	var loaded_move_right = InputEventKey.new()
	
	loaded_move_left.set_physical_keycode(int(data.move_left))
	loaded_move_right.set_physical_keycode(int(data.move_right))
	
	keybind_resource.move_left_key = loaded_move_left
	keybind_resource.move_right_key = loaded_move_right
	

func on_settings_data_loaded(data : Dictionary) -> void:
	loaded_data = data
	on_keybind_loaded(loaded_data.keybinds)
	
