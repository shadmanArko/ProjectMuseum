using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.ParticleEffects;

public partial class DepletedParticleExplosion : GpuParticles2D
{
	public void EmitParticle(Vector2 dir)
	{
		OneShot = true;
		ProcessMaterial.Set("direction", dir);
		Emitting = true;
	}
	
	private void OnEffectCompleted()
	{
		QueueFree();
	}
}