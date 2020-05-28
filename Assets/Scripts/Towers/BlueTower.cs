using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueTower : Tower
{
    private void Start()
    {
        ElementType = Element.BLUE;
        Upgrades = new TowerUpgrade[]
        {
            new TowerUpgrade((int)(Price*1.5) , Damage*2, DebuffDuration*2, 45),
            new TowerUpgrade(Price*3 , Damage*3, DebuffDuration*3 , 85),
        };
    }

    public override Debuff GetDebuff()
    {
        return new BlueDebuff(Target, DebuffDuration);
    }

    public override string GetStats()
    {
        return string.Format("<color=#4e9edf><size=20><b> Башня зайца </b></size></color>\n{0}", base.GetStats());
    }

}

