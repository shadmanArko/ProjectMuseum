extends Control  # Replace with the appropriate node type for your UI

var item1 = preload("res://Scenes/Museum/Sub Scenes/exhibitItemNode_1.tscn")
var item2 = preload("res://Scenes/Museum/Sub Scenes/exhibitItemNode_2.tscn")



func _on_exhibit_0_pressed():
	var instance = item1.instantiate()
	get_tree().get_root().add_child(instance)
	var script_instance = instance.get_node(".")  # Replace "path_to_script_node" with the actual path to the script node

	if script_instance:
		# Now you can access properties or call methods on the script instance
		script_instance.selectedItem = true
	else: print("Item script not found")


func _on_exhibit_3_pressed():
	var instance = item2.instantiate()
	get_tree().get_root().add_child(instance)
	var script_instance = instance.get_node(".")  # Replace "path_to_script_node" with the actual path to the script node

	if script_instance:
		# Now you can access properties or call methods on the script instance
		script_instance.selectedItem = true
	else: print("Item script not found")
