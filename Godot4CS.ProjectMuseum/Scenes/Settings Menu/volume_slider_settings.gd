extends HBoxContainer

@onready var label: Label = $Label as Label
@onready var h_slider: HSlider = $HSlider as HSlider

@export_enum("Master", "Music", "SFX", "UI", "Ambiance") var bus_name : String

var bus_index : int = 0

# Called when the node enters the scene tree for the first time.
func _ready():
	h_slider.value_changed.connect(on_value_changed)
	get_bus_name_by_index()
	set_name_lebel_text()
	set_slider_value()
	
	
func set_name_lebel_text() -> void:
	label.text = str(bus_name) + "Volume"
	
func get_bus_name_by_index() -> void:
	bus_index = AudioServer.get_bus_index(bus_name)
	
func set_slider_value() -> void:
	h_slider.value = db_to_linear(AudioServer.get_bus_volume_db(bus_index))

func on_value_changed(value : float) -> void:
	AudioServer.set_bus_volume_db(bus_index, linear_to_db(value))

