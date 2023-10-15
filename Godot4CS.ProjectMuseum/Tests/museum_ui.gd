extends Control  # Replace with the appropriate node type for your UI

var item1 = preload("res://item_1.tscn")
var item2 = preload("res://item_2.tscn")



func _on_exhibit_0_pressed():
	var instance = item1.instantiate()
	get_tree().get_root().add_child(instance)


func _on_exhibit_3_pressed():
	var instance = item2.instantiate()
	get_tree().get_root().add_child(instance)
