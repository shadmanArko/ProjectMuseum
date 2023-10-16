using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public partial class TileSpawner : TileMap
{
	public override void _Ready()
	{
		HttpRequest httpRequest = GetNode<HttpRequest>("HTTPRequest");
		httpRequest.Request("http://localhost:5178/api/MuseumTile/GetAllMuseumTiles");
	}
	
	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var museumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(jsonStr);
		foreach (var museumTile in museumTiles)
		{
			SetCell(0, new Vector2I( museumTile.XPosition, museumTile.YPosition), museumTile.TileSetNumber, Vector2I.Zero);
		}
	}
}
