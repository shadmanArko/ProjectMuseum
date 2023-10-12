using Godot;


namespace Godot4CS.ProjectMuseum.Scripts.MineScripts;

public partial class ProceduralMineGenerator : TileMap
{
	private FastNoiseLite _perlinNoise = new();  
	private Vector2 _initialBlockPosition;
	private Cell[,] _grid;

	[Export] private TileMap _tileMap;
	[Export] private int _totalTiles;

	//[Export] private float _noiseScale = 0.1f;

	[Export] private int _cellSize = 64;
	[Export] private int _width = 25;
	[Export] private int _length = 32; //must be 64
	
	private PackedScene _cellPrefab;
	[Export] private string _cellPath; 

	private RandomNumberGenerator _randomNumberGenerator = new();

	[Export] private float _cellBreakThreshold = 0.9f;
	public override void _Ready()
	{
		_cellPrefab = (PackedScene)ResourceLoader.Load(_cellPath);
		GenerateGrid();
	}
<<<<<<< Updated upstream:Godot4CS.ProjectMuseum/ProceduralMineGenerator.cs

	private void GenerateGrid()
=======
	
	private async void GenerateGrid()
>>>>>>> Stashed changes:Godot4CS.ProjectMuseum/Scripts/MineScripts/ProceduralMineGenerator.cs
	{
		_grid = new Cell[_width, _length];

		for (var y = 0; y < _length; y++)
		{
			for (var x = 0; x < _width; x++)
			{
<<<<<<< Updated upstream:Godot4CS.ProjectMuseum/ProceduralMineGenerator.cs
				_grid[x, y] = InstantiateCell(x,y);
				var tilePos = _tileMap.LocalToMap(_grid[x, y].Pos);
				_tileMap.SetCell(0,tilePos,4,new Vector2I(6,1));
			}
		}
=======
				//var tilePos = _tileMap.LocalToMap(_grid[x, y].Pos);
				//_tileMap.SetCell(0,tilePos,4,new Vector2I(6,1));
				
				
				_grid[x, y] = InstantiateCell(x,y);
				
				//GD.Print($"cell instantiated at {_grid[x,y].Pos}");
			}
		}
		
		//InstantiatePlayer();
	}
	
	private Cell InstantiateCell(int width, int height)
	{
		Node newCell = _cellPrefab.Instantiate();
		var cell = new Cell(new Vector2(width * _cellSize, height * _cellSize), false, false, 3);
		newCell.Name = $"cell{width}{height}";
		//var tilePos = _tileMap.LocalToMap(_grid[x, y].Pos);
		//_tileMap.SetCell(0,tilePos,4,new Vector2I(6,1));
		//GD.Print($"cell generated at ");
		AddChild(newCell);
		newCell.Set("position", cell.Pos);
		return cell;
>>>>>>> Stashed changes:Godot4CS.ProjectMuseum/Scripts/MineScripts/ProceduralMineGenerator.cs
	}

	private Cell InstantiateCell(int width, int height)
	{
<<<<<<< Updated upstream:Godot4CS.ProjectMuseum/ProceduralMineGenerator.cs
		var breakStrength = _randomNumberGenerator.RandiRange(1, 3);
		var cell = new Cell(false, false, breakStrength)
		{
			Pos = new Vector2(width * _cellSize, height * _cellSize)
		};

		return cell;
	}
}
=======
		var temp = _playerScene.Instantiate();
		AddChild(temp);
		var playerScript = temp as Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts.PlayerController;
		//GD.Print($"playerScript null = {playerScript is null}");
		//GD.Print(playerScript._maxSpeed);
		playerScript.Position = _grid[0, 0].Pos;
		//GD.Print("Instantiated player at "+_grid[0,0].Pos);
	}
	
}
>>>>>>> Stashed changes:Godot4CS.ProjectMuseum/Scripts/MineScripts/ProceduralMineGenerator.cs
