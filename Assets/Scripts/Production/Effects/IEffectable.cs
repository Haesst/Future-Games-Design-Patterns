using System.Collections;
using System.Collections.Generic;
public interface IEffectable
{
    void StartEffect(IEffect effect);
    IEnumerator ApplyEffect(IEffect effect);
    List<IEffect> ActiveEffects { get; }
}
