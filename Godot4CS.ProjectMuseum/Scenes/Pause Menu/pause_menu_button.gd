extends Button

@onready var pause_menu_button: Button = $"."
@onready var pause_menu: TextureRect = $"../pause_menu"

#func _on_pressed() -> void:
	#if get_tree().paused == false:
		#pause_menu.visible = true
		#pause_menu_button.visible = false
		#get_tree().paused = true
