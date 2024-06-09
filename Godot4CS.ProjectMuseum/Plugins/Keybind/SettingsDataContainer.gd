extends Node

var master_volume : float = 0.0
var music_volume : float = 0.0
var sfx_volume : float = 0.0
var ui_volume : float = 0.0
var ambiance_volume : float = 0.0
@onready var settings_menu: Control = $"."
@onready var exit_button: Button = $"MarginContainer/VBoxContainer/HBoxContainer/Exit Button"
@onready var keybind_resource : PlayerKeyBindResource = preload("res://Plugins/Keybind/PlayerKeyBind.tres")

var loaded_data : Dictionary = {}

func _ready():
	handle_signals()
	create_storage_dictionary()
	
		
func on_master_volume_set(value : float) -> void:
	master_volume = value
	
func on_music_volume_set(value : float) -> void:
	music_volume = value
	
func on_sfx_volume_set(value : float) -> void:
	sfx_volume = value
	
func on_ui_volume_set(value : float) -> void:
	ui_volume = value
	
func on_ambiance_volume_set(value : float) -> void:
	ambiance_volume = value
	
#func get -> void:
	#if loaded_data == {}:
		#return
	#return
	
func handle_signals() -> void:
	SettingsSignalBus.on_master_sound_set.connect(on_master_volume_set)
	SettingsSignalBus.on_music_sound_set.connect(on_music_volume_set)
	SettingsSignalBus.on_sfx_sound_set.connect(on_sfx_volume_set)
	SettingsSignalBus.on_ui_sound_set.connect(on_ui_volume_set)
	SettingsSignalBus.on_ambiance_sound_set.connect(on_ambiance_volume_set)
	SettingsSignalBus.load_settings_data.connect(on_settings_data_loaded)

func create_storage_dictionary() -> Dictionary:
	var settings_container_dict = {
		"master_volume" : master_volume,
		"music_volume" : music_volume,
		"sfx_volume" : sfx_volume,
		"ui_volume" : ui_volume,
		"ambiance_volume" : ambiance_volume,
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
	print(loaded_data)
	on_keybind_loaded(loaded_data.keybinds)
	


func _on_exit_button_pressed() -> void:
	settings_menu.visible = false
