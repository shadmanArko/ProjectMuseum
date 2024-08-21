using System;
using System.Collections.Generic;
using System.Linq;

namespace Godot4CS.ProjectMuseum.Scripts.DutyManager;


public class DutyManager
{
    private List<Duty> GetAllTypeOfDuty()
    {
        var duties = new List<Duty>();
        return duties;
    }

    private void GenerateNewDuty()
    {
        var duties = GetAllTypeOfDuty();
        var newDuty = duties.OrderBy(x => Guid.NewGuid()).First();
        newDuty.Id = Guid.NewGuid().ToString();
        newDuty.ZoneId = GetSuitableZoneForNewDuty();
        newDuty.AssignedStaffId = GetStaffForNewDuty(newDuty);
    }

    private string GetSuitableZoneForNewDuty()
    {
        throw new NotImplementedException();
    }

    private string GetStaffForNewDuty(Duty newDuty)
    {
        throw new NotImplementedException();
    }
}