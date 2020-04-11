using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Effect : IEffect
{
    public abstract float Duration { get; }
    public abstract bool Stackable { get; }
    public abstract Coroutine DisableRoutine { get; set; }
    public abstract bool TopStack { get; set; }

    public abstract void ApplyEffect(GameObject target);
    public abstract void DisableEffect(GameObject target);
    public abstract void StackEffect(GameObject target);
}
