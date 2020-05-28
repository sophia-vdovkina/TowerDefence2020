using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkTower : Tower
{
    [SerializeField]
    private float tickTime;

    [SerializeField]
    private float tickDamage;

    public float TickTime { get => tickTime;}
    public float TickDamage { get => tickDamage;}

    private void Start()
    {
        ElementType = Element.PINK;

        Upgrades = new TowerUpgrade[]
        {
            new TowerUpgrade((int)(Price*1.5) , Damage*2, (int)(DebuffDuration*1.5) , 80, TickTime-1, TickDamage+1),
            new TowerUpgrade(Price*3 , (int)(Damage*2.5), DebuffDuration*3 , 95, TickTime-2, TickDamage+2),
        };

    }

    public override Debuff GetDebuff()
    {
        return new PinkDebuff(TickDamage, TickTime, DebuffDuration, Target);
    }

    public override string GetStats()
    {
        if (NextUpgrade != null)
        {
            return string.Format("<color=#df4e9e><size=20><b> Башня кролика </b></size></color>\n{0}\nУрон от горения: {1} <color=#00ff00ff>+{2}</color>", base.GetStats(), TickDamage, NextUpgrade.TickDamage - TickDamage);
        }
        return string.Format("<color=#df4e9e><size=20><b> Башня кролика </b></size></color>\n{0}\nУрон от горения: {1}", base.GetStats(), TickDamage);
    }

    public override void Upgrade()
    {
        tickDamage = NextUpgrade.TickDamage;
        tickTime = NextUpgrade.TickTime;
        base.Upgrade();
    }
}
