using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Service.SaveLoadServices;
using Time = ProjectMuseum.Models.Time;
namespace Godot4CS.ProjectMuseum.Service.MuseumServices;
public partial class TimeServices: Node
{
    private Time _timeDatabase;

    public override void _Ready()
    {
        base._Ready();
        _timeDatabase = SaveLoadService.Load().Time;
        
    }
    

    public Time GetTime()
    {
        return _timeDatabase;
    }

    public Time Update(Time time)
    {
        _timeDatabase = time;
        return _timeDatabase;
    }
}