using System.Collections.Generic;
using UnityEngine;
public class User
{
    private List<GameObject> _baracks;

    public User()
    {
        Baracks = new List<GameObject>();
        Units = new List<GameObject>();
        Defence = new Turret[7];
    }

    public List<GameObject> Baracks
    {
        get { return _baracks; }
        set { _baracks = value; }
    }

    public List<GameObject> Units { get; set; }

    public Turret[] Defence { get; set; }

    public int GetFreeSpot()
    {
        for (var i = 0; i < 7; i++)
        {
            if (Defence[i] == null)
            {
                return i + 1;
            }
        }

        return 1;
    }

    public void AddBarack(GameObject barack)
    {
        Baracks.Add(barack);
    }

    public void AddUnit(GameObject unit)
    {
        Units.Add(unit);
    }

    public void AddDefence(Turret turret)
    {
        Defence[GetFreeSpot() - 1] = turret;
    }

    public Turret getActiveTurret()
    {
        for (var i = 0; i < 7; i++)
        {
            if (Defence[i] != null)
            {
                return Defence[i];
            }
        }

        return null;
    }

    public void DestoryTurret()
    {
        for (var i = 0; i < 7; i++)
        {
            if (Defence[i] != null)
            {
                Object.Destroy(Defence[i].TurretObject);
                Defence[i] = null;
                return;
            }
        }
    }
}