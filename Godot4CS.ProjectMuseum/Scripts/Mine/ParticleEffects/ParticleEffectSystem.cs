using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.ParticleEffects;

public partial class ParticleEffectSystem : Node2D
{
    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;

    private RandomNumberGenerator _randomNumberGenerator;

    #region Initializers

    private void InitializeDiInstaller()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void SubscribeToActions()
    {
        MineActions.OnSuccessfulDigActionCompleted += MakeMineWallDepletedParticleEffect;
    }
    
    public override void _Ready()
    {
        InitializeDiInstaller();
        SubscribeToActions();
        _randomNumberGenerator = new RandomNumberGenerator();
    }

    #endregion
    
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

    public override void _ExitTree()
    {
        MineActions.OnSuccessfulDigActionCompleted -= MakeMineWallDepletedParticleEffect;
    }
    
}