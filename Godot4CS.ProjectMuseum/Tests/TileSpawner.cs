using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ProjectMuseum.Models;

public partial class TileSpawner : TileMap
{
	public override void _Ready()
	{
		HttpRequest httpRequest = GetNode<HttpRequest>("HTTPRequest");
		httpRequest.Request("http://localhost:5178/api/MuseumTile/GetAllMuseumTilesForNewGame");
	}
	
	private void OnRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var listOfMuseumTiles = JsonSerializer.Deserialize<List<MuseumTile>>(jsonStr);
		foreach (var museumTile in listOfMuseumTiles)
		{
			SetCell(0, new Vector2I( museumTile.XPosition, museumTile.YPosition), museumTile.TileSetNumber, Vector2I.Zero);
		}
	}
}
