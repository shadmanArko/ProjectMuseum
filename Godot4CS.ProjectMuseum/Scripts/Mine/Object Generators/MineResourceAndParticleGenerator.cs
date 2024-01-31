using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.ParticleEffects;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Resource = ProjectMuseum.Models.MIne.Resource;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Object_Generators;

public partial class MineResourceAndParticleGenerator : Node
{
	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;

	private HttpRequest _sendResourceFromMineToInventoryHttpRequest;
	
	private RandomNumberGenerator _randomNumberGenerator;
	
	public override void _Ready()
	{
		CreateHttpRequests();
		InitializeDiInstaller();
		SubscribeToActions();
		_randomNumberGenerator = new RandomNumberGenerator();
	}

	private void CreateHttpRequests()
	{
		_sendResourceFromMineToInventoryHttpRequest = new HttpRequest();
		AddChild(_sendResourceFromMineToInventoryHttpRequest);
		_sendResourceFromMineToInventoryHttpRequest.RequestCompleted += OnSendResourceFromMineToInventoryHttpRequestCompleted;
	}

	

	private void InitializeDiInstaller()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	private void SubscribeToActions()
	{
		MineActions.OnSuccessfulDigActionCompleted += MakeMineWallDepletedParticleEffect;
		MineActions.OnSuccessfulDigActionCompleted += CollectResources;
	}

	#region Wall Particle Effects

	private void MakeMineWallDepletedParticleEffect()
	{
		var particleEffectPath = ReferenceStorage.Instance.DepletedParticleExplosion;
		var particle = ResourceLoader.Load<PackedScene>(particleEffectPath).Instantiate() as DepletedParticleExplosion;
		if (particle == null) return;

		var position = _mineGenerationVariables.MineGenView.LocalToMap(_playerControllerVariables.Position);
		position += _playerControllerVariables.MouseDirection;
		particle.Position = position * _mineGenerationVariables.Mine.CellSize;
        
		var cellSize = _mineGenerationVariables.Mine.CellSize;
		var rand = _randomNumberGenerator.RandfRange(cellSize / 4f,cellSize);
        
		switch (_playerControllerVariables.MouseDirection)
		{
			case (1, 0):
				particle.Position += new Vector2(0, rand);
				particle.EmitParticle(_playerControllerVariables.MouseDirection);
				break;
			case (-1, 0):
				particle.Position += new Vector2(cellSize, rand);
				particle.EmitParticle(_playerControllerVariables.MouseDirection);
				break;
			case (0, -1):
				particle.Position += new Vector2(rand, cellSize);
				particle.EmitParticle(_playerControllerVariables.MouseDirection);
				break;
			case (0, 1):
				particle.Position += new Vector2(rand, 0);
				particle.EmitParticle(_playerControllerVariables.MouseDirection);
				break;
		}

		_mineGenerationVariables.MineGenView.AddChild(particle);
		var direction = _playerControllerVariables.MouseDirection * -1;
		particle.EmitParticle(direction);
	}

	#endregion
	
	#region Instantiate Resource Objects

	private void CollectResources()
	{
		var tilePos = _mineGenerationVariables.MineGenView.LocalToMap(_playerControllerVariables.Position);
		tilePos += _playerControllerVariables.MouseDirection;
		var cell = _mineGenerationVariables.GetCell(tilePos);
		
		if(cell.HitPoint > 0) return;
		if(!cell.HasResource) return;
		var resource = _mineGenerationVariables.Mine.Resources.FirstOrDefault(tempResource =>
			tempResource.PositionX == cell.PositionX && tempResource.PositionY == cell.PositionY);

		if (resource == null) return;
		SendResourceFromMineToInventory(resource.Id);
		cell.HasResource = false;
		
	}

	#endregion

	#region Collect Resource

	private void SendResourceFromMineToInventory(string resourceId)
	{
		var url = ApiAddress.MineApiPath + "SendResourceFromMineToInventory/" + resourceId;
		_sendResourceFromMineToInventoryHttpRequest.Request(url);
	}
	
	private void OnSendResourceFromMineToInventoryHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		MineActions.OnInventoryUpdate?.Invoke();
	}

	#endregion


	public override void _ExitTree()
	{
		MineActions.OnSuccessfulDigActionCompleted -= MakeMineWallDepletedParticleEffect;
		MineActions.OnSuccessfulDigActionCompleted += CollectResources;
	}
}