using Godot;
using Godot4CS.ProjectMuseum.Scripts;


public partial class ProceduralMineGenerator : TileMap
{
	private FastNoiseLite _perlinNoise = new();  
	private Vector2 _initialBlockPosition;
	private Cell[,] _grid;
	
	[Export] private PackedScene _playerScene;
	[Export] private PackedScene _cellPrefab;
	
	[Export] private TileMap _tileMap;
	[Export] private int _totalTiles;
	
	[Export] private int _cellSize = 512;
	[Export] private int _width = 9;
	[Export] private int _length = 9; //must be 64
	
	private RandomNumberGenerator _randomNumberGenerator = new();
	
	[Export] private float _cellBreakThreshold = 0.9f;
	public override void _Ready()
	{
		//_cellPrefab = GD.Load<PackedScene>(_cellPath);
		GenerateGrid();
	}
	
	private void GenerateGrid()
	{
		_grid = new Cell[_width, _length];
	
		for (var y = 0; y < _length; y++)
		{
			for (var x = 0; x < _width; x++)
			{
				//var tilePos = _tileMap.LocalToMap(_grid[x, y].Pos);
				//_tileMap.SetCell(0,tilePos,4,new Vector2I(6,1));
				
				Node newCell = _cellPrefab.Instantiate();
				var cell = new Cell(new Vector2(x * _cellSize, y * _cellSize), false, false, 3);
				newCell.Name = $"cell{x}{y}";
				//var tilePos = _tileMap.LocalToMap(_grid[x, y].Pos);
				//_tileMap.SetCell(0,tilePos,4,new Vector2I(6,1));
				GD.Print($"cell generated at ");
				AddChild(newCell);
				newCell.Set("position", cell.Pos);
				
				_grid[x, y] = cell;
				
				GD.Print($"cell instantiated at {_grid[x,y].Pos}");
			}
		}
		
		//InstantiatePlayer();
	}
	
	private Cell InstantiateCell(int width, int height)
	{
		var newCell = _cellPrefab.Instantiate();
		var cell = newCell as Cell;
		cell = new Cell(new Vector2(width * _cellSize, height * _cellSize), false, false,
			_randomNumberGenerator.RandiRange(1, 3));
		newCell.Name = $"cell{width}{height}";
		GD.Print($"cell generated at {cell.Pos}");
		AddChild(newCell);
		return cell;
	}

	private void InstantiatePlayer()
	{
		var temp = _playerScene.Instantiate();
		AddChild(temp);
		var playerScript = temp as PlayerSideView;
		GD.Print($"playerScript null = {playerScript is null}");
		GD.Print(playerScript._maxSpeed);
		playerScript.Position = _grid[0, 0].Pos;
		GD.Print("Instantiated player at "+_grid[0,0].Pos);
	}
	
}
