using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenDebuff : Debuff
{
    private float slowingFactor;

    private bool applied;

    public GreenDebuff(float slowingFactor, float duration, Monster target) : base(target, duration)
    {
        this.slowingFactor = slowingFactor;

    }

    public override void Update()
    {
        if (target != null)
        {
            if (!applied)
            {
                applied = true;

                if (target.Speed > (target.MaxSpeed * slowingFactor) / 100)
                {
                    target.Speed -= (target.MaxSpeed * slowingFactor) / 100;
                }

                
            }
        }

        base.Update();
    }

    public override void Remove()
    {
        if (target != null)
        {
            target.Speed = target.MaxSpeed;

            base.Remove();
        }
        
    }
}
