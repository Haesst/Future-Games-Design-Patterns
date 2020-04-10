using System.Collections.Generic;

public class UnitMethods
{
    public static IReadOnlyDictionary<BoxymonType, int> IdByType { get; } = new Dictionary<BoxymonType, int>
    {
        { BoxymonType.SmallBoxymon, 0 },
        { BoxymonType.BigBoxymon, 1 }
    };
    public static IReadOnlyDictionary<int, BoxymonType> TypeById { get; } = new Dictionary<int, BoxymonType>
    {
        { 0, BoxymonType.SmallBoxymon },
        { 1, BoxymonType.BigBoxymon }
    };
    public static IReadOnlyDictionary<BoxymonType, float> TimeBetweenSpawnsByType { get; } = new Dictionary<BoxymonType, float>
    {
        { BoxymonType.SmallBoxymon, 0.8f },
        { BoxymonType.BigBoxymon, 1.2f }
    };
}

