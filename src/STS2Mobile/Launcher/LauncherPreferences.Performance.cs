namespace STS2Mobile.Launcher;

internal static partial class LauncherPreferences
{
    private const string PerformanceModePreferenceKey = "performance_mode";
    private static readonly PreferenceFile PerformanceModePreference =
        new(PerformanceModePreferenceKey);

    internal static string ReadPerformanceMode()
        => LauncherPerformanceMode.Normalize(
            PerformanceModePreference.ReadText(LauncherPerformanceMode.Standard)
        );

    internal static void SavePerformanceMode(string mode)
        => PerformanceModePreference.WriteText(LauncherPerformanceMode.Normalize(mode));
}
