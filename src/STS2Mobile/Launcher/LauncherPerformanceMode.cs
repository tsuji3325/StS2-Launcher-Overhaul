namespace STS2Mobile.Launcher;

internal static class LauncherPerformanceMode
{
    internal const string Standard = "standard";
    internal const string Balanced = "balanced";
    internal const string Low = "low";

    internal static string Normalize(string mode)
        => mode switch
        {
            Balanced => Balanced,
            Low => Low,
            _ => Standard,
        };
}
