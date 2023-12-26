using System.Threading.Tasks;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI;

public partial class MinePopUp : PanelContainer
{
    [Export] private RichTextLabel _popUpText;
    

    public async Task ShowPopUp(string message)
    {
        Show();
        _popUpText.Text = message;
        var panelSize = Size;
        panelSize.Y = _popUpText.Size.Y + 20;
        Size = panelSize;
        await Task.Delay(3000);
        _popUpText.Text = "";
        Hide();
    }
    
    public async Task ShowPopUp(string message, int seconds)
    {
        Show();
        _popUpText.Text = message;
        var panelSize = Size;
        panelSize.Y = _popUpText.Size.Y + 20;
        Size = panelSize;
        await Task.Delay(seconds * 1000);
        _popUpText.Text = "";
        Hide();
    } 
}