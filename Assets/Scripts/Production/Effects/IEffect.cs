using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffect
{
    float Duration { get; }
    bool Stackable { get; }
    bool TopStack { get; set; }
    Coroutine DisableRoutine { get; set; }

    void ApplyEffect(GameObject target);
    void DisableEffect(GameObject target);
    void StackEffect(GameObject target);
}
