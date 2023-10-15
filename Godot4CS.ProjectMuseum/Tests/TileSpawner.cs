using Godot;
using System;
using System.Threading.Tasks;

public partial class TileSpawner : TileMap
{
	private int ZERO = 0;

	[Export] public int numberOfTilesInX = 32;
	[Export] public int numberOfTilesInY = 20;
	[Export] public int originStartsX = 55;
	[Export] public int originStartsY = 22;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		
		
		
		for (int x = originStartsX; x > originStartsX - numberOfTilesInX; x--)
		{
			for (int y = originStartsY; y > originStartsY - numberOfTilesInY; y--)
			{
				var tileSetId = GD.RandRange(0, 2); 
				SetCell(0, new Vector2I( x, y), tileSetId, Vector2I.Zero);
				string body = Json.Stringify(new Godot.Collections.Dictionary
				{
					{ "id", "string" },
					{"xPosition", x},
					{"yPosition", y},
					{"tileSetNumber", tileSetId},
					{"tileAtlasCoOrdinateX", 0},
					{"tileAtlasCoOrdinateY", 0},
					{"layer", 0},
					{"wallId", "string"},
					{"exhibitId", "string"},
					{"hangingLightId", "string"}
				});
				var httpRequest = new HttpRequest();
				AddChild(httpRequest);
				string[] headers = { "Content-Type: application/json"};
				Error error = httpRequest.Request("http://localhost:5178/api/MuseumTile", headers, HttpClient.Method.Post,body);
				await Task.Delay(10);
				if (error != Error.Ok)
				{
					GD.PushError("An error occurred in the HTTP request.");
				}
				GD.Print($".{x}, {y}"); 
			}
		}
	}
}