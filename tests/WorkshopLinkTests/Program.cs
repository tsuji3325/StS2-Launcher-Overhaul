using System;
using System.IO;
using System.Linq;
using STS2Mobile.Steam;

var root = Path.Combine(Path.GetTempPath(), "sts2-workshop-tests-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(root);
STS2Mobile.AppPaths.AppPrivateDataDir = root;
var assertions = 0;
void Require(bool ok, string why) { if (!ok) throw new InvalidOperationException(why); assertions++; }

try
{
    Require(
        SteamWorkshopManualLinks.TryParseWorkshopId("https://steamcommunity.com/sharedfiles/filedetails/?id=3747503308&searchtext=a", out var a)
            && a == 3747503308UL,
        "Sharedfiles link"
    );
    Require(
        SteamWorkshopManualLinks.TryParseWorkshopId("https://steamcommunity.com/workshop/filedetails/?id=1234567890", out var b)
            && b == 1234567890UL,
        "Workshop link"
    );
    Require(!SteamWorkshopManualLinks.TryParseWorkshopId("http://steamcommunity.com/sharedfiles/filedetails/?id=12345678", out _), "HTTPS required");
    Require(!SteamWorkshopManualLinks.TryParseWorkshopId("https://steamcommunity.com.evil.test/sharedfiles/filedetails/?id=12345678", out _), "Host must match");
    Require(!SteamWorkshopManualLinks.TryParseWorkshopId("3747503308", out _), "Plain IDs disallowed");
    Require(!SteamWorkshopManualLinks.TryParseWorkshopId("https://steamcommunity.com/app/2868840/workshop/", out _), "Browse URL is not an item");

    Godot.DisplayServer.NextClipboard = "https://steamcommunity.com/sharedfiles/filedetails/?id=3747503308";
    Require(SteamWorkshopManualLinks.TryAddFromClipboard(out _), "First import");
    Require(SteamWorkshopManualLinks.TryAddFromClipboard(out _), "Duplicate reimport is safe");
    Require(SteamWorkshopManualLinks.ReadIds().Count == 1, "No duplicated IDs");
    Godot.DisplayServer.NextClipboard = "https://steamcommunity.com/workshop/filedetails/?id=1234567890";
    Require(SteamWorkshopManualLinks.TryAddFromClipboard(out _), "Second import");
    Require(SteamWorkshopManualLinks.ReadIds().Count == 2, "Both IDs persist");

    Godot.DisplayServer.NextClipboard = "sensitive clipboard text";
    Require(!SteamWorkshopManualLinks.TryAddFromClipboard(out _), "Do not import unrelated clipboard");
    var stored = File.ReadAllText(Path.Combine(root, "workshop_mods", "manually_added_workshop_ids.txt"));
    Require(!stored.Contains("steamcommunity", StringComparison.Ordinal), "No URLs stored");
    Require(stored.Split('\n', StringSplitOptions.RemoveEmptyEntries).All(line => ulong.TryParse(line.Trim(), out _)), "Only numeric IDs stored");
    Console.WriteLine($"PASS: {assertions} Workshop link tests");
}
finally
{
    Directory.Delete(root, recursive: true);
}

namespace Godot
{
    public static class DisplayServer
    {
        public static string NextClipboard = "";
        public static string ClipboardGet() => NextClipboard;
    }
}

namespace STS2Mobile
{
    internal static class AppPaths
    {
        internal static string AppPrivateDataDir { get; set; } = "";
    }
    internal static class PatchHelper
    {
        internal static void Log(string text) { }
    }
}
