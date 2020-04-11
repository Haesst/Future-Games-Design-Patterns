using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectable
{
    void StartEffect(IEffect effect);
    IEnumerator ApplyEffect(IEffect effect);
    List<IEffect> ActiveEffects { get; }
}
