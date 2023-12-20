using System.Threading.Tasks;
using Godot;


namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI;

public partial class SceneTransition : CanvasLayer
{
    [Export] private Godot.ColorRect _colorRect;

    public async Task FadeIn()
    {
        Visible = true;
        _colorRect.Show();
        _colorRect.Modulate = Colors.Transparent;
        
        while (_colorRect.Modulate.A < 1)
        {
            var color = _colorRect.Modulate;
            color.A += 0.5f;
            _colorRect.Modulate = color;
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        }
    }

    public async Task FadeOut()
    {
        _colorRect.Show();
        _colorRect.Modulate = Colors.Black;
        
        while (_colorRect.Modulate.A > 0)
        {
            var color = _colorRect.Modulate;
            color.A -= 0.5f;
            _colorRect.Modulate = color;
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        }
        
        Visible = false;
    }
}