using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    private List<GameObject> _baracks;
    private List<GameObject> _units;

    public Enemy()
    {
        Baracks = new List<GameObject>();
        Units = new List<GameObject>();
        HitPoints = 18;
    }

    public List<GameObject> Baracks
    {
        get { return _baracks; }
        set { _baracks = value; }
    }

    public List<GameObject> Units
    {
        get { return _units; }
        set { _units = value; }
    }

    public int HitPoints { get; set; }

    public void AddBarack(GameObject barack)
    {
        Baracks.Add(barack);
    }

    public void AddUnit(GameObject unit)
    {
        Units.Add(unit);
    }
}