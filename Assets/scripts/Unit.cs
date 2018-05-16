using UnityEngine;

public class Unit
{
    private GameObject unitObject;
    private int HitPoints;
    private int Damage;
    private UnitType type;

    public Unit(GameObject unitObject, int hitPoints, int damage, UnitType type)
    {
        this.unitObject = unitObject;
        HitPoints = hitPoints;
        Damage = damage;
        this.type = type;
    }
}