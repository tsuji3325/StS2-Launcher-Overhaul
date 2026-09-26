"""Guardrail for the experimental Downfall/Android custom-character patch.
This is a source-level scope check. Passing does not prove Android game behavior.
"""
from pathlib import Path
import re

src = Path("src/STS2Mobile/Patches/ModLoaderPatches.cs").read_text()
match = re.search(
    r"BaseLibDownfallCharacterPatchTypes\s*\{\s*get;\s*\}\s*=\s*new\[\]\s*\{(?P<items>.*?)\};",
    src,
    re.S,
)
assert match, "Explicit Downfall character-hook list missing"
hooks = re.findall(r'"([^"]+)"', match.group("items"))
expected = [
    "BaseLib.Patches.Content.PrefixIdPatch",
    "BaseLib.Patches.Content.AddCustomCharacters",
    "BaseLib.Abstracts.CharacterSelectIconPath",
    "BaseLib.Abstracts.CharacterSelectLockedIconPath",
    "BaseLib.Abstracts.CustomCharacterSelectBg",
]
assert "BaseLib.Patches.UI.ScrollCharSelectPatch" not in hooks
assert hooks == expected, f"Unexpected patch scope: {hooks}"
assert "IsDownfallSelectedForCurrentLaunch()" in src
assert 'LauncherModLaunchPlan.Resolve(selection)' in src
assert 'string.Equals(mod.ManifestId, "Downfall", StringComparison.OrdinalIgnoreCase)' in src
assert 'string.Equals(mod.Id, "Downfall"' not in src
assert "LauncherModSelectionState.IsModdedModeFor(selection)" in src
assert "harmony.CreateClassProcessor(patchType).Patch()" in src
assert "foreach (var patchType in BaseLibDownfallCharacterPatchTypes)" in src
assert "BaseLib.Patches.Content.TheBigPatchToCardPileCmdAdd.Patch" in src
assert "BaseLib Android-safe initializer skipped BaseLib PatchAll" in src
assert "MainHarmony.TryPatchAll(assembly);" not in src, "Unsafe full BaseLib patching re-enabled"
print("PASS: 13 Downfall whitelist/gating source checks")
