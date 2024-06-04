class_name PlayerKeyBindResource
extends Resource

const MOVE_LEFT : String = "move_left"
const MOVE_RIGHT : String = "move_right"

@export var DEFAULT_MOVE_LEFT_KEY = InputEventKey.new()
@export var DEFAULT_MOVE_RIGHT_KEY = InputEventKey.new()

var move_left_key = InputEventKey.new()
var move_right_key = InputEventKey.new()
