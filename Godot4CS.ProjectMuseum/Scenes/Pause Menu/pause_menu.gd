extends ColorRect
@onready var pause_menu_button: Button = $"../pause_menu_button"
@onready var pause_menu: ColorRect = $"."
@onready var back_button: Button = $MarginContainer/VBoxContainer/back_button
@onready var settings_button: Button = $MarginContainer/VBoxContainer/settings_button
@onready var settings_menu: Control = $"../settings_menu"
@onready var setting_menu_close_button: Button = $"../setting_menu_close_button"





func _on_back_button_pressed() -> void:
	pause_menu.visible = false
	pause_menu_button.visible = true
	


func _on_settings_button_pressed() -> void:
	settings_menu.visible = true
	setting_menu_close_button.visible = true
	pause_menu.visible = false
	


func _on_setting_menu_close_button_pressed() -> void:
	settings_menu.visible = false
	setting_menu_close_button.visible = false
	pause_menu.visible = true
