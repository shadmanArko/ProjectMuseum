extends Node

signal on_master_sound_set(value : float)
signal on_music_sound_set(value : float)
signal on_sfx_sound_set(value : float)
signal on_ui_sound_set(value : float)
signal on_ambiance_sound_set(value : float)
signal load_settings_data(settings_dict : Dictionary)
signal set_settings_dictionary(settings_dict : Dictionary)

func emit_load_settings_data(settings_dict : Dictionary) -> void:
	load_settings_data.emit(settings_dict)

func emit_set_settings_dictionary(settings_dict : Dictionary) -> void:
	set_settings_dictionary.emit(settings_dict)

func emit_on_master_sound_set(value : float) -> void:
	on_master_sound_set.emit(value)
	
func emit_on_music_sound_set(value : float) -> void:
	on_music_sound_set.emit(value)
	
func emit_on_ui_sound_set(value : float) -> void:
	on_ui_sound_set.emit(value)
	
func emit_on_sfx_sound_set(value : float) -> void:
	on_sfx_sound_set.emit(value)
	
func emit_on_ambiance_sound_set(value : float) -> void:
	on_ambiance_sound_set.emit(value)
