using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Godot;

namespace STS2Mobile.Steam;

// Opt-in alternative when Steam's subscription-list RPC returns InvalidParam
// and the Community fallback redirects to a login page. Store ONLY numeric
// Workshop IDs; never keep the copied URL or any clipboard/authentication data.
internal static class SteamWorkshopManualLinks
{
    private const int MaxItems = 50;
    private const string IdFileName = "manually_added_workshop_ids.txt";
    private static readonly Regex WorkshopLink = new(
        @"^https?://(?:www\.)?steamcommunity\.com/(?:sharedfiles|workshop)/filedetails/\?[^#]*[?&]?id=(?<id>[0-9]{5,20})(?:&|#|$)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant
    );

    private static string IdFile =>
        Path.Combine(AppPaths.AppPrivateDataDir, "workshop_mods", IdFileName);

    internal static bool TryAddFromClipboard(out string message)
    {
        message = "Workshopリンクをコピーしてから、もう一度押してください。";
        string clipboard;
        try
        {
            clipboard = DisplayServer.ClipboardGet()?.Trim() ?? "";
        }
        catch (Exception ex)
        {
            PatchHelper.Log($"[Workshop] Clipboard unavailable: {ex.GetType().Name}");
            message = "クリップボードを読み取れません。";
            return false;
        }

        if (!TryParseWorkshopId(clipboard, out var id))
            return false;

        try
        {
            var ids = ReadIds().ToHashSet();
            if (ids.Contains(id))
            {
                message = "このMODは登録済みです。再同期します。";
                return true;
            }

            if (ids.Count >= MaxItems)
            {
                message = "登録できるMODは最大50個です。";
                return false;
            }

            ids.Add(id);
            var dir = Path.GetDirectoryName(IdFile)!;
            Directory.CreateDirectory(dir);
            var temporary = IdFile + ".tmp";
            File.WriteAllLines(temporary, ids.OrderBy(value => value).Select(value => value.ToString()));
            File.Move(temporary, IdFile, overwrite: true);
            message = "MODリンクを登録しました。Workshopから取得します。";
            PatchHelper.Log($"[Workshop] Manually added Workshop item {id}");
            return true;
        }
        catch (Exception ex)
        {
            PatchHelper.Log($"[Workshop] Unable to save Workshop ID: {ex.GetType().Name}: {ex.Message}");
            message = "MODリンクの保存に失敗しました。";
            return false;
        }
    }

    internal static IReadOnlyCollection<ulong> ReadIds()
    {
        try
        {
            if (!File.Exists(IdFile))
                return Array.Empty<ulong>();

            return File.ReadLines(IdFile)
                .Select(line => ulong.TryParse(line.Trim(), out var value) ? value : 0)
                .Where(value => value != 0)
                .Distinct()
                .Take(MaxItems)
                .ToArray();
        }
        catch (Exception ex)
        {
            PatchHelper.Log($"[Workshop] Could not read manually added IDs: {ex.GetType().Name}");
            return Array.Empty<ulong>();
        }
    }

    internal static bool TryParseWorkshopId(string text, out ulong id)
    {
        id = 0;
        // Do not scan generic clipboard text (which might be sensitive).
        if (string.IsNullOrWhiteSpace(text))
            return false;

        // Uri.TryCreate and query parsing avoid accepting lookalike host names,
        // ID strings pasted alone, non-HTTPS protocols, or unrelated page URLs.
        if (!Uri.TryCreate(text, UriKind.Absolute, out var uri)
            || !string.Equals(uri.Scheme, "https", StringComparison.OrdinalIgnoreCase)
            || !string.Equals(uri.Host, "steamcommunity.com", StringComparison.OrdinalIgnoreCase)
               && !string.Equals(uri.Host, "www.steamcommunity.com", StringComparison.OrdinalIgnoreCase)
            || !(
                string.Equals(uri.AbsolutePath, "/sharedfiles/filedetails/", StringComparison.OrdinalIgnoreCase)
                || string.Equals(uri.AbsolutePath, "/sharedfiles/filedetails", StringComparison.OrdinalIgnoreCase)
                || string.Equals(uri.AbsolutePath, "/workshop/filedetails/", StringComparison.OrdinalIgnoreCase)
                || string.Equals(uri.AbsolutePath, "/workshop/filedetails", StringComparison.OrdinalIgnoreCase)
            ))
            return false;

        var match = Regex.Match(uri.Query, @"(?:^\?|&)id=(?<id>\d{5,20})(?:&|$)", RegexOptions.CultureInvariant);
        return match.Success && ulong.TryParse(match.Groups["id"].Value, out id) && id != 0;
    }
}
