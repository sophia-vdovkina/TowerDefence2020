﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Debuff
{
    protected Monster target;

    private float duration;

    protected float elapsed;

    public Debuff(Monster target, float duration)
    {
        this.target = target;

        this.duration = duration;
    }

    public virtual void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed >= duration)
        {
            Remove();
        }
    }

    public virtual void Remove()
    {
        if (target != null)
        {
            target.RemoveDebuff(this);
        }
    }
}
