extends TextureRect
@onready var pause_menu_button: Button = $"../pause_menu_button"
@onready var pause_menu: TextureRect = $"."
@onready var back_button: Button = $MarginContainer/VBoxContainer/back_button
@onready var settings_button: Button = $MarginContainer/VBoxContainer/settings_button
@onready var settings_menu: Control = $"../settings_menu"
@onready var setting_menu_close_button: Button = $"../settings_menu/MarginContainer/VBoxContainer/HBoxContainer/setting_menu_close_button"
@onready var main_menu_button: Button = $MarginContainer/VBoxContainer/main_menu_button
@onready var exit_to_desktop_button: Button = $MarginContainer/VBoxContainer/exit_to_desktop_button





func _on_back_button_pressed() -> void:
	pause_menu.visible = false
	pause_menu_button.visible = true
	get_tree().paused = false
	SettingsSignalBus.emit_set_settings_dictionary(SettingsDataContainer.create_storage_dictionary())
	


func _on_settings_button_pressed() -> void:
	settings_menu.visible = true
	setting_menu_close_button.visible = true
	pause_menu.visible = false
	


func _on_setting_menu_close_button_pressed() -> void:
	settings_menu.visible = false
	setting_menu_close_button.visible = false
	pause_menu.visible = true


func _on_main_menu_button_pressed() -> void:
	var main_menu_scene = load("res://Scenes/MainMenu/Main Scene/MainMenu.tscn")
	if main_menu_scene:
		get_tree().change_scene_to(main_menu_scene)


func _on_exit_to_desktop_button_pressed() -> void:
	get_tree().quit()
