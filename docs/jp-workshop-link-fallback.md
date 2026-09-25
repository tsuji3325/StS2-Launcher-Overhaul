# JP edition: Workshop link fallback (experimental)

For the JP build with the stable package identity, **install as an update**. Do not
uninstall, clear app data, or redownload the game to test the Workshop fix.

This is an opt-in fallback for Steam accounts where the legacy
`PublishedFile.GetUserFiles` requests return `InvalidParam` and the
Community HTML fallback responds with HTTP 302. It is **not** a general fix
for Steam's automatic subscribed-item listing.

## On Android

1. Open the subscribed Slay the Spire 2 mod's individual Steam Workshop page
   in your mobile browser (not the Workshop browse page).
2. Copy its entire URL. It must look like
   `https://steamcommunity.com/sharedfiles/filedetails/?id=1234567890`.
3. In the JP launcher, open **MOD** and tap **コピーしたWorkshopリンクを追加**.
4. The launcher stores only the numeric Workshop ID and calls the normal
   Workshop metadata/dependency/download flow. Confirm that the mod is listed,
   enable it, select MODあり, and launch.
5. Repeat steps 1-4 for each mod. Imported IDs remain across app updates.

The experimental button never stores the copied URL or arbitrary clipboard
content; it accepts only HTTPS Steam Community item detail URLs. The existing
launcher selection, cached downloads, and separate modded saves are preserved.
It does not solve Android-incompatible mods or guarantee that Steam's
GetDetails/download endpoints work on every account; verify on the actual
device.

The unchanged original Steam auto-sync path remains available when no manual
Workshop IDs have been imported. Existing upstream login, saved credentials,
and installed game files are not changed.

### Validation
- Source assembly built against matching v0.2.432 APK references.
- Isolated link parser / duplicate / ID-only persistence tests.
- Local updated APK derived from **the exact user's JP stable APK** with the
  same package ID and private signing certificate and increased versionCode.
- Phone-based Workshop download and mod activation still require user testing.
