extends Control

func _ready():

	# Create an HTTP request node and connect its completion signal.
	var http_request = HTTPRequest.new()
	add_child(http_request)
	http_request.request_completed.connect(self._on_http_request_request_completed)

	# Perform a GET request. The URL below returns JSON as of writing.
	var error = http_request.request("http://localhost:5178/api/MuseumTile")
	if error != OK:
		push_error("An error occurred in the HTTP request.")

	# Perform a POST request. The URL below returns JSON as of writing.
	# Note: Don't make simultaneous requests using a single HTTPRequest node.
	# The snippet below is provided for reference only.
	var data = {
	  "id": "string",
	  "xPosition": 999,
	  "yPosition": 45,
	  "tileSetNumber": 0,
	  "tileAtlasCoOrdinateX": 0,
	  "tileAtlasCoOrdinateY": 0,
	  "layer": 0,
	  "flooring": "GoDotFlooring",
	  "decoration": "GG"
	}
	var body = JSON.new().stringify(data)
	error = http_request.request("http://localhost:5178/api/MuseumTile", [], HTTPClient.METHOD_POST, body)
	if error != OK:
		push_error("An error occurred in the HTTP request.")

func _on_putDataOnAPI_button_up():
	


	var http_request = HTTPRequest.new()
	add_child(http_request)
	var data = {
	  "id": "string",
	  "xPosition": 999,
	  "yPosition": 45,
	  "tileSetNumber": 0,
	  "tileAtlasCoOrdinateX": 0,
	  "tileAtlasCoOrdinateY": 0,
	  "layer": 0,
	  "flooring": "GoDotFlooring",
	  "decoration": "GG"
	}
	
	var query = JSON.stringify(data);
	var header = ["Content-Type: application/json"];
	var error = http_request.request("http://localhost:5178/api/MuseumTile", header, HTTPClient.METHOD_POST,query);
	


func _on_http_request_request_completed(result, response_code, headers, body):
	var json = JSON.new()
	json.parse(body.get_string_from_utf8())
	var response = json.get_data()
	# Will print the user agent string used by the HTTPRequest node (as recognized by httpbin.org).
	
