extends Node

@onready var keybind_resource : PlayerKeyBindResource = preload("res://Plugins/Keybind/PlayerKeyBind.tres")

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
