using UnityEngine;

public class Turret
{

    private GameObject _turretObject;

    public Turret(GameObject turretObject, int hitPoints)
    {
        TurretObject = turretObject;
        HitPoints = hitPoints;
    }

    private int _hitPoints;

    public GameObject TurretObject
    {
        get { return _turretObject; }
        set { _turretObject = value; }
    }

    public int HitPoints
    {
        get { return _hitPoints; }
        set { _hitPoints = value; }
    }
}