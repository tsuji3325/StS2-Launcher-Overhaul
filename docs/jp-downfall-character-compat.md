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
