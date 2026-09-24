using Godot;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;

namespace STS2Mobile.Launcher;

internal static class LauncherPerformanceSettings
{
    internal static void ApplyAfterSettingsLoaded()
    {
        if (!OperatingSystem.IsAndroid())
            return;

        var mode = LauncherPreferences.ReadPerformanceMode();
        var settings = SaveManager.Instance.SettingsSave;
        if (settings == null)
            return;

        var changed = false;
        switch (mode)
        {
            case LauncherPerformanceMode.Low:
                changed |= SetVSync(settings, VSyncType.Off);
                changed |= SetMsaa(settings, 0);
                Engine.MaxFps = 30;
                break;

            case LauncherPerformanceMode.Balanced:
                changed |= SetVSync(settings, VSyncType.Off);
                changed |= SetMsaa(settings, 0);
                Engine.MaxFps = 45;
                break;

            default:
                changed |= SetVSync(settings, VSyncType.On);
                Engine.MaxFps = 0;
                break;
        }

        if (changed)
            SaveManager.Instance.SaveSettings();

        PatchHelper.Log(
            $"Performance mode applied: {mode}, MaxFps={Engine.MaxFps}, VSync={settings.VSync}, Msaa={settings.Msaa}"
        );
    }

    private static bool SetVSync(SettingsSave settings, VSyncType value)
    {
        if (settings.VSync == value)
            return false;

        settings.VSync = value;
        return true;
    }

    private static bool SetMsaa(SettingsSave settings, int value)
    {
        if (settings.Msaa == value)
            return false;

        settings.Msaa = value;
        return true;
    }
}
