using UnityEngine;

public static class GameTime
{
    public static bool m_IsPaused = false;
    public static float m_DeltaTime { get { return m_IsPaused ? 0 : Time.deltaTime; } }
}