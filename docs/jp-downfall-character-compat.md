# Downfall custom-character hooks on Android (experimental)

## Repro from Sony SOG05
- Vanilla: game starts and a run can be played.
- BaseLib only: no character-selection hang observed by tester.
- BaseLib + Downfall: title menu reports two loaded mods; Downfall UI is visible, but no added characters appear; selecting a vanilla character may lead to a black screen.
- Prior launcher intentionally disables BaseLib full PatchAll/CustomPile patches due to an observed Android freeze. Do **not** re-enable them to test this.

## Change
When, and only when, Downfall is explicitly enabled in Modded mode,
apply three **individual** BaseLib Harmony classes:
1. `PrefixIdPatch`
2. `AddCustomCharacters` (adds `CustomContentDictionary.CustomCharacters` to `ModelDb.AllCharacters`)
3. `ScrollCharSelectPatch` (makes enlarged character set scrollable).

The baseline BaseLib-only path is untouched. Log the selection decision,
each resolved class and actual Harmony patch count; if a class is unavailable
or raises, preserve the Android-safe initializer and log the failure. Do not
run BaseLib PatchAll, enable extended saves, or enable the known-hanging
CustomPile compatibility patch.

## Test steps (physical device needed)
1. Install as an in-place update over the existing JP WorkshopFix build:
   same manifest (package/versionCode) and same private certificate. Never
   uninstall or clear data; the APK intentionally contains no game files.
2. Verify Vanilla starts; then BaseLib only can start a new run.
3. Enable BaseLib+Downfall. Check if the Downfall characters become visible;
   do **not** assume that visibility implies compatibility.
4. Try one vanilla character with both mods and one custom character.
   If either hangs, capture a **redacted** launcher log filtered for
   `Downfall character hook`, `BaseLib`, `Downfall`, `[Mods]`,
   `Exception`; include whether the background continued animating.
5. If the build regresses, **reinstall the prior signed JP WorkshopFix APK**
   over it. Do not uninstall; same versionCode and same signing certificate
   allow reinstatement without discarding the already-downloaded game.

## Validation scope
Managed assembly compilation and the existing Workshop import test plus
source-level allowlist/gating checks pass in GitHub Actions. Physical Android
visibility, Downfall game startup and combat remain **unverified**.
No signing credentials or personal data are stored in the repository.

## Fix after first in-place test (Workshop identity mismatch)
The first phone test still booted but did not show custom characters. Source audit
found that the opt-in gate compared `LauncherKnownMod.Id` against `Downfall`.
Workshop `Id` is actually a numeric Steam PublishedFileId, so that gate was
always false for the user's Workshop installation. The current revision uses
`LauncherModLaunchPlan.Resolve(selection)` and checks the resolved manifest's
`ManifestId` instead. A regression source test asserts that the erroneous
numeric-ID comparison is not reintroduced. This fixes the gate, but actual
character registration and Downfall run compatibility remain device tests.

## v3 actual device evidence (2026-09-27 JST)
- With BaseLib+Downfall, all eight mod character select assets were logged as loading:
  `res://Automaton/images/character/char_select.png`,
  `res://Awakened/...`, `Champ`, `Guardian`, `Hermit`,
  `Hexaghost`, `SlimeBoss`, and `Snecko` (the original log lists eight names;
  preserve the log as the authority).
- Immediately before main-menu startup completed, BaseLib logged
  `More than 8 selection options, enabling character select scroll` and loaded
  `res://BaseLibScenes/NHorizontalScrollContainer.cs`. Process exited via native
  signal 6 soon afterward; logs do not prove this script caused the signal.
- v4 diagnostic: remove only `ScrollCharSelectPatch` from the Downfall-specific
  whitelist. Retain PrefixIdPatch, AddCustomCharacters, and the three model
  icon/background path getter patches. If v4 runs, implement scrolling using
  Android-safe built-in Godot controls and test in isolation.
- Without scroll the extended character row might not fit on a phone screen.
  Check whether game can open character selection without crashing first.
- Do not delete game files, re-download them, clear app data, or reinstall.
