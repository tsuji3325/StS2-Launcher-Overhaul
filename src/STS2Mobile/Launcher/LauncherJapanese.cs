using System;
using System.Collections.Generic;

namespace STS2Mobile.Launcher;

// Small Japanese presentation layer for the Android launcher.
// Internal state values, Steam branch names, logs, and protocol strings stay English.
internal static class LauncherJapanese
{
    private static readonly Dictionary<string, string> Exact = new(StringComparer.Ordinal)
    {
        ["Home"] = "ホーム",
        ["Saves"] = "セーブ",
        ["Versions"] = "バージョン",
        ["Mods"] = "MOD",
        ["Help"] = "ヘルプ",
        ["Play"] = "プレイ",
        ["Launch the game, choose a version, and manage mods."] = "ゲームを起動し、バージョンやMODを管理します。",
        ["Play safely"] = "安全に起動",
        ["Your next climb starts here."] = "次の冒険をここから始めます。",
        ["Keep your progress within reach."] = "セーブデータを管理します。",
        ["Choose the build you want to play."] = "プレイするゲームバージョンを選択します。",
        ["Make the next run your own."] = "MODを使ってプレイ内容を変更できます。",
        ["Get back to your game."] = "ゲームの起動や問題解決を行います。",

        ["Manage saves"] = "セーブデータ管理",
        ["Sync progress with Steam"] = "Steamと進行状況を同期",
        ["Game version"] = "ゲームバージョン",
        ["Choose, update, or repair your download"] = "バージョンの選択・更新・修復",
        ["Mods & play style"] = "MOD・プレイ方式",
        ["Switch between Vanilla and Modded"] = "バニラとMODありを切り替え",

        ["Play Vanilla"] = "バニラでプレイ",
        ["Play Modded"] = "MODありでプレイ",
        ["Vanilla"] = "バニラ",
        ["Modded"] = "MODあり",
        ["Vanilla saves"] = "バニラのセーブ",
        ["Modded saves"] = "MOD用セーブ",
        ["Uses Vanilla saves"] = "バニラのセーブを使用",
        ["Uses Modded saves"] = "MOD用セーブを使用",
        ["Not synced yet"] = "未同期",

        ["Syncing..."] = "同期中...",
        ["Sync now"] = "今すぐ同期",
        ["Get saves from Steam"] = "Steamからセーブを取得",
        ["Send saves to Steam"] = "Steamへセーブを送信",
        ["Vanilla and modded saves are separate. Both sync automatically."] = "バニラとMOD用のセーブは別々です。どちらも自動同期されます。",

        ["Game Install"] = "ゲームのインストール",
        ["Choose the Steam branch, download files, and keep separate installs isolated."] = "Steamブランチを選び、ゲームファイルをダウンロードします。",
        ["Local files"] = "ローカルファイル",
        ["Download Game Files"] = "ゲームファイルをダウンロード",
        ["Download Game"] = "ゲームをダウンロード",
        ["Downloading..."] = "ダウンロード中...",
        ["Refresh Game Versions"] = "ゲームバージョンを更新",
        ["Refresh list"] = "一覧を更新",
        ["Check for updates"] = "アップデートを確認",
        ["Check for Updates"] = "アップデートを確認",
        ["Update selected Steam branch"] = "選択中のバージョンを更新",
        ["Selected version redownload"] = "選択中バージョンの再ダウンロード",
        ["Redownload selected version"] = "選択中のバージョンを再ダウンロード",
        ["Redownload removes only the selected version's downloaded files. Saves and other versions stay in place."] = "選択中バージョンのゲームファイルだけを再取得します。セーブや他のバージョンは残ります。",

        ["Update Workshop mods"] = "Workshop MODを更新",
        ["Remove downloaded Workshop mods..."] = "ダウンロード済みWorkshop MODを削除...",
        ["Enabled"] = "有効",
        ["Disabled"] = "無効",

        ["Safe Start"] = "セーフスタート",
        ["Graphics"] = "グラフィック",
        ["Auto"] = "自動",
        ["Performance"] = "パフォーマンス",
        ["Standard"] = "標準",
        ["Balanced"] = "バランス",
        ["Low"] = "軽量",
        ["Standard: original / Balanced: 45 FPS / Low: 30 FPS"] = "標準：制限なし / バランス：45 FPS / 軽量：30 FPS",
        ["App updates"] = "ランチャー更新",
        ["Check for app updates"] = "ランチャーの更新を確認",
        ["Diagnostics"] = "診断",
        ["Create support report"] = "サポートレポートを作成",
        ["Report a bug on GitHub"] = "GitHubで不具合を報告",
        ["View last error"] = "最後のエラーを表示",
        ["Copy launcher log"] = "ランチャーログをコピー",
        ["Show technical details"] = "技術情報を表示",
        ["Hide technical details"] = "技術情報を隠す",
        ["Show details"] = "詳細を表示",
        ["Hide details"] = "詳細を隠す",
        ["Details"] = "詳細",
        ["Hide"] = "隠す",
        ["If preparation requires a redownload, use Redownload selected version; do not uninstall or clear app data. If the game starts but does not appear, try Safe Start. Create a support report if either problem repeats."] =
            "再ダウンロードが必要な場合は「選択中のバージョンを再ダウンロード」を使用してください。アプリの削除やデータ消去はしないでください。ゲームが起動しても表示されない場合は「セーフスタート」を試してください。問題が続く場合はサポートレポートを作成してください。",

        ["Steam Guard"] = "Steam Guard",
        ["Current code"] = "現在のコード",
        ["Enter Steam Guard code"] = "Steam Guardコードを入力",
        ["Steam Guard code"] = "Steam Guardコード",
        ["Verify Code"] = "コードを確認",
        ["Use current Steam Guard code"] = "現在のSteam Guardコードを使用",
        ["One-shot submit; code is not stored"] = "コードは1回だけ送信され、保存されません",
        ["Authenticating..."] = "認証中...",
        ["Working"] = "処理中",
        ["Ready"] = "準備完了",
        ["Warning"] = "注意",
        ["Error"] = "エラー",

        ["Steam username"] = "Steamユーザー名",
        ["Steam password"] = "Steamパスワード",
        ["Sign in"] = "サインイン",
        ["Login"] = "ログイン",
        ["Cancel"] = "キャンセル",
        ["Continue"] = "続行",
        ["Retry"] = "再試行",
        ["Start here"] = "ここから開始",
        ["Setup guide"] = "セットアップガイド",
    };

    internal static string Text(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        if (text.Contains('\n'))
        {
            var lines = text.Split('\n');
            for (var i = 0; i < lines.Length; i++)
                lines[i] = Text(lines[i]);
            return string.Join("\n", lines);
        }

        if (Exact.TryGetValue(text, out var translated))
            return translated;

        const string installedSuffix = " (installed)";
        if (text.EndsWith(installedSuffix, StringComparison.Ordinal))
            return text[..^installedSuffix.Length] + "（インストール済み）";

        const string notInstalledSuffix = " (not installed)";
        if (text.EndsWith(notInstalledSuffix, StringComparison.Ordinal))
            return text[..^notInstalledSuffix.Length] + "（未インストール）";

        return text;
    }
}
