using UnityEngine;

public class SpeedModifierEffect : IEffect
{
    private float m_OriginalSpeed;
    private float m_SpeedMultiplier;

    public float Duration { get; }
    public bool Stackable { get; }
    public bool TopStack { get; set; }
    public Coroutine DisableRoutine { get; set; }

    public SpeedModifierEffect(float speedMultiplier, float duration, bool shouldStack)
    {
        m_SpeedMultiplier = speedMultiplier;
        Duration = duration;
        Stackable = shouldStack;
    }

    public void ApplyEffect(GameObject target)
    {
        Boxymon boxymon = target.GetComponent<Boxymon>();

        if(boxymon != null)
        {
            m_OriginalSpeed = boxymon.MoveSpeed;
            boxymon.MoveSpeed = m_OriginalSpeed * m_SpeedMultiplier;

            if(boxymon.MoveSpeed < 0.0f)
            {
                boxymon.MoveSpeed = 0.0f;
            }
        }
    }

    public void DisableEffect(GameObject target)
    {
        Boxymon boxymon = target.GetComponent<Boxymon>();

        if (boxymon != null)
        {
            boxymon.MoveSpeed = m_OriginalSpeed;
        }
    }

    public void StackEffect(GameObject target)
    {
        Boxymon boxymon = target.GetComponent<Boxymon>();

        if (boxymon != null)
        {
            m_OriginalSpeed = boxymon.MoveSpeed;
        }
    }
}
