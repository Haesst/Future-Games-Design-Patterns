using UnityEngine;

public static class GameTime
{
    public static bool IsPaused { get; set; }
    public static float DeltaTime { get { return IsPaused ? 0 : Time.deltaTime; } }
}