using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenTower : Tower
{
    [SerializeField]
    private float slowingFactor;

    public float SlowingFactor { get => slowingFactor;}

    private void Start()
    {
        ElementType = Element.GREEN;
        Upgrades = new TowerUpgrade[]
        {
            new TowerUpgrade((int)(Price*1.5) , (int)(Damage*1.5), (int)(DebuffDuration*1.5), 50, SlowingFactor+15),
            new TowerUpgrade(Price*3 , Damage*2, DebuffDuration*3 , 95, SlowingFactor+15),
        };
    }

    public override Debuff GetDebuff()
    {
        return new GreenDebuff(SlowingFactor, DebuffDuration,Target);
    }

    public override string GetStats()
    {
        if (NextUpgrade != null)
        {
            return string.Format("<color=#339a49><size=20><b> Башня кошки </b></size></color>\n{0}\nЗамедление: {1}% <color=#00ff00ff>+{2}%</color>", base.GetStats(), SlowingFactor, NextUpgrade.SlowingFactor - SlowingFactor);
        }
        return string.Format("<color=#339a49><size=20><b> Башня кошки </b></size></color>\n{0}\nЗамедление: {1}%", base.GetStats(), SlowingFactor);
    }

    public override void Upgrade()
    {
        slowingFactor = NextUpgrade.SlowingFactor;
        base.Upgrade();
    }
}
