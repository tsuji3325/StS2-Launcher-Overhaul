using Mono.Cecil;
using Mono.Cecil.Cil;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: CustomApkPatch <input STS2Mobile.dll> <output STS2Mobile.dll>");
    return 2;
}

var input = args[0];
var output = args[1];

var translations = new Dictionary<string, string>(StringComparer.Ordinal)
{
    ["Home"] = "ホーム",
    ["Saves"] = "セーブ",
    ["Versions"] = "バージョン",
    ["Mods"] = "MOD",
    ["Help"] = "ヘルプ",
    ["Your next climb starts here."] = "次の冒険をここから始めます。",
    ["Keep your progress within reach."] = "セーブデータを管理します。",
    ["Choose the build you want to play."] = "プレイするゲームバージョンを選択します。",
    ["Make the next run your own."] = "MODを使ってプレイ内容を変更できます。",
    ["Get back to your game."] = "ゲームの起動や問題解決を行います。",

    ["Play Vanilla"] = "バニラでプレイ",
    ["Play Modded"] = "MODありでプレイ",
    ["Vanilla"] = "バニラ",
    ["Modded"] = "MODあり",
    ["Vanilla saves"] = "バニラのセーブ",
    ["Modded saves"] = "MOD用セーブ",
    ["Not synced yet"] = "未同期",
    ["Manage saves"] = "セーブデータ管理",
    ["Sync progress with Steam"] = "Steamと進行状況を同期",
    ["Game version"] = "ゲームバージョン",
    ["Choose, update, or repair your download"] = "バージョンの選択・更新・修復",
    ["Mods & play style"] = "MOD・プレイ方式",
    ["Switch between Vanilla and Modded"] = "バニラとMODありを切り替え",

    ["Syncing..."] = "同期中...",
    ["Sync now"] = "今すぐ同期",
    ["Get saves from Steam"] = "Steamからセーブを取得",
    ["Send saves to Steam"] = "Steamへセーブを送信",
    ["Vanilla and modded saves are separate. Both sync automatically."] =
        "バニラとMOD用のセーブは別々です。どちらも自動同期されます。",

    ["Game Install"] = "ゲームのインストール",
    ["Local files"] = "ローカルファイル",
    ["Download Game Files"] = "ゲームファイルをダウンロード",
    ["Download Game"] = "ゲームをダウンロード",
    ["Downloading..."] = "ダウンロード中...",
    ["Check for updates"] = "アップデートを確認",
    ["Check for Updates"] = "アップデートを確認",
    ["Refresh list"] = "一覧を更新",
    ["Refresh Game Versions"] = "ゲームバージョンを更新",
    ["Update selected Steam branch"] = "選択中のバージョンを更新",
    ["Selected version redownload"] = "選択中バージョンの再ダウンロード",
    ["Redownload selected version"] = "選択中のバージョンを再ダウンロード",

    ["Update Workshop mods"] = "Workshop MODを更新",
    ["Remove downloaded Workshop mods..."] = "ダウンロード済みWorkshop MODを削除...",
    ["Uses Vanilla saves"] = "バニラのセーブを使用",
    ["Uses Modded saves"] = "MOD用セーブを使用",

    ["Safe Start"] = "セーフスタート",
    ["Graphics"] = "グラフィック（30FPS軽量化）",
    ["Auto"] = "自動",
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

    ["If preparation requires a redownload, use Redownload selected version; do not uninstall or clear app data. If the game starts but does not appear, try Safe Start. Create a support report if either problem repeats."] =
        "再ダウンロードが必要な場合は「選択中のバージョンを再ダウンロード」を使用してください。アプリの削除やデータ消去はしないでください。ゲームが起動しても表示されない場合は「セーフスタート」を試してください。問題が続く場合はサポートレポートを作成してください。",
    ["Redownload removes only the selected version's downloaded files. Saves and other versions stay in place."] =
        "選択中バージョンのゲームファイルだけを再取得します。セーブや他のバージョンは残ります。",
};

using var assembly = AssemblyDefinition.ReadAssembly(input, new ReaderParameters
{
    ReadWrite = false,
    InMemory = true,
});

var translated = 0;
MethodReference? maxFpsSetter = null;
MethodDefinition? settingsPostfix = null;

foreach (var type in AllTypes(assembly.MainModule.Types))
{
    foreach (var method in type.Methods)
    {
        if (type.FullName == "STS2Mobile.Patches.SettingsPatches"
            && method.Name == "InitSettingsDataPostfix")
        {
            settingsPostfix = method;
        }

        if (!method.HasBody)
            continue;

        foreach (var instruction in method.Body.Instructions)
        {
            if (instruction.OpCode == OpCodes.Ldstr
                && instruction.Operand is string value
                && translations.TryGetValue(value, out var replacement))
            {
                instruction.Operand = replacement;
                translated++;
            }

            if (instruction.Operand is MethodReference mr
                && mr.DeclaringType.FullName == "Godot.Engine"
                && mr.Name == "set_MaxFps")
            {
                maxFpsSetter ??= mr;
            }
        }
    }
}

if (settingsPostfix is null || !settingsPostfix.HasBody)
    throw new InvalidOperationException("SettingsPatches.InitSettingsDataPostfix was not found.");

if (maxFpsSetter is null)
    throw new InvalidOperationException("Godot.Engine.MaxFps setter reference was not found.");

var il = settingsPostfix.Body.GetILProcessor();
var first = settingsPostfix.Body.Instructions[0];
il.InsertBefore(first, il.Create(OpCodes.Ldc_I4, 30));
il.InsertBefore(first, il.Create(OpCodes.Call, assembly.MainModule.ImportReference(maxFpsSetter)));

Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
assembly.Write(output);

Console.WriteLine($"Translated string literals: {translated}");
Console.WriteLine("Injected Android launcher/game 30 FPS cap.");
return 0;

static IEnumerable<TypeDefinition> AllTypes(IEnumerable<TypeDefinition> roots)
{
    foreach (var type in roots)
    {
        yield return type;
        foreach (var nested in AllTypes(type.NestedTypes))
            yield return nested;
    }
}
