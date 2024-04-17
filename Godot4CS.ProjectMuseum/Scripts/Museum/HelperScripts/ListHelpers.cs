using System;
using System.Collections.Generic;
using System.Linq;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;

public static class ListHelpers
{
    public static List<T> Shuffle<T>(this IList<T> list)
    {
        Random rnd = new Random();
        var res = new T[list.Count];

        res[0] = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            int j = rnd.Next(i);
            res[i] = res[j];
            res[j] = list[i];
        }
        return res.ToList();
    }
}