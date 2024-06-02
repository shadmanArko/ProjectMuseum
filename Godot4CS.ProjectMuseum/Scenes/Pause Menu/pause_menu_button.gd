extends Button

@onready var pause_menu_button: Button = $"."
@onready var pause_menu: ColorRect = $"../pause_menu"


func _on_pressed() -> void:
	pause_menu.visible = true
	pause_menu_button.visible = false
	get_tree().paused = true
