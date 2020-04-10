public class WaveData
{
    public float m_Timer;
    public uint  m_Spawned;
    public uint  m_ToSpawn;

    public WaveData(uint toSpawn)
    {
        m_Timer = 0.0f;
        m_Spawned = 0;
        m_ToSpawn = toSpawn;
    }

    public void ResetWaveData(float timerValue = 0.0f)
    {
        SetTimer(timerValue);
        m_Spawned = 0;
    }

    public void SubtractFromTimer(float time)
    {
        m_Timer -= time;
    }

    public void SpawnDone(float timerValue)
    {
        SetTimer(timerValue);
        m_Spawned += 1;
    }

    public void SetTimer(float time)
    {
        m_Timer = time;
    }

    public bool ReadyToSpawn()
    {
        return m_Timer <= 0.0f && m_Spawned < m_ToSpawn;
    }

    public bool WaveUnitDone()
    {
        return m_Spawned >= m_ToSpawn;
    }
}
