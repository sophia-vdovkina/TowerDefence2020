using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUpgrade
{
    public int Price { get; private set; }

    public int Damage { get; private set; }

    public float DebuffDuration { get; private set; }

    public float Proc { get; private set; }

    public float SlowingFactor { get; private set; }

    public float TickTime { get; private set; }

    public float TickDamage { get; private set; }

    public TowerUpgrade(int price, int damage, float debuffDuration, float proc)
    {
        Price = price;
        Damage = damage;
        DebuffDuration = debuffDuration;
        Proc = proc;
    }

    public TowerUpgrade(int price, int damage, float debuffDuration, float proc, float slowingFactor)
    {
        Price = price;
        Damage = damage;
        DebuffDuration = debuffDuration;
        Proc = proc;
        SlowingFactor = slowingFactor;
    }

    public TowerUpgrade(int price, int damage, float debuffDuration, float proc, float tickTime, float tickDamage)
    {
        Price = price;
        Damage = damage;
        DebuffDuration = debuffDuration;
        Proc = proc;
        TickTime = tickTime;
        TickDamage = tickDamage;
    }

}
