using Godot;
using STS2Mobile.Launcher;
using STS2Mobile.Launcher.Components;

namespace STS2Mobile.Launcher.Sections;

internal sealed partial class ActionSection
{
    private (
        VBoxContainer Group,
        Button AutoButton,
        Button VulkanButton,
        Button OpenGlButton
    ) BuildRendererControls(float scale, bool compact)
    {
        var group = new VBoxContainer
        {
            Name = "GraphicsSection",
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
        };
        group.AddThemeConstantOverride(
            "separation",
            LauncherViewLayoutMetrics.ScaleInt(compact ? 6 : 8, scale)
        );

        var label = new StyledLabel(
            "Graphics",
            scale,
            compact ? 13 : 14,
            HorizontalAlignment.Left
        );
        label.Name = "GraphicsSectionLabel";
        label.AddThemeColorOverride("font_color", LauncherComponentTheme.TextSecondary);
        group.AddChild(label);

        var row = new HBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
        };
        row.AddThemeConstantOverride(
            "separation",
            LauncherViewLayoutMetrics.ScaleInt(6, scale)
        );
        group.AddChild(row);

        var buttonGroup = new ButtonGroup
        {
            AllowUnpress = false,
        };
        var auto = AddRendererButton(row, buttonGroup, "Auto", LauncherRendererMode.Auto, scale, compact);
        var vulkan = AddRendererButton(row, buttonGroup, "Vulkan", LauncherRendererMode.Vulkan, scale, compact);
        var openGl = AddRendererButton(row, buttonGroup, "OpenGL", LauncherRendererMode.OpenGl, scale, compact);
        auto.Name = "RendererAuto";
        vulkan.Name = "RendererVulkan";
        openGl.Name = "RendererOpenGL";

        AddPerformanceControls(group, scale, compact);

        return (group, auto, vulkan, openGl);
    }

    private Button AddRendererButton(
        Container parent,
        ButtonGroup buttonGroup,
        string text,
        string mode,
        float scale,
        bool compact
    )
    {
        var button = new StyledButton(
            text,
            scale,
            compact ? 13 : 14,
            compact
                ? LauncherSectionMetrics.CompactDrawerToggleHeight
                : LauncherSectionMetrics.SecondaryButtonHeight
        )
        {
            ToggleMode = true,
            ButtonGroup = buttonGroup,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            AccessibilityName = LauncherJapanese.Text($"Renderer {text}"),
            AccessibilityDescription = mode == LauncherRendererMode.Auto
                ? "Recommended for normal game starts."
                : $"Use {text} for graphics troubleshooting.",
        };
        LauncherButtonStyles.ApplySupportAction(button, scale);
        button.Pressed += () => ApplyRendererMode(mode, notify: true);
        parent.AddChild(button);
        return button;
    }

    private void AddPerformanceControls(VBoxContainer group, float scale, bool compact)
    {
        var spacer = new Control
        {
            CustomMinimumSize = new Vector2(
                0,
                LauncherViewLayoutMetrics.ScaleInt(compact ? 4 : 6, scale)
            ),
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        group.AddChild(spacer);

        var label = new StyledLabel(
            "Performance",
            scale,
            compact ? 13 : 14,
            HorizontalAlignment.Left
        )
        {
            Name = "PerformanceSectionLabel",
        };
        label.AddThemeColorOverride("font_color", LauncherComponentTheme.TextSecondary);
        group.AddChild(label);

        var row = new HBoxContainer
        {
            Name = "PerformanceModeRow",
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
        };
        row.AddThemeConstantOverride(
            "separation",
            LauncherViewLayoutMetrics.ScaleInt(6, scale)
        );
        group.AddChild(row);

        var buttonGroup = new ButtonGroup
        {
            AllowUnpress = false,
        };

        var standard = AddPerformanceButton(
            row,
            buttonGroup,
            "Standard",
            LauncherPerformanceMode.Standard,
            scale,
            compact
        );
        var balanced = AddPerformanceButton(
            row,
            buttonGroup,
            "Balanced",
            LauncherPerformanceMode.Balanced,
            scale,
            compact
        );
        var low = AddPerformanceButton(
            row,
            buttonGroup,
            "Low",
            LauncherPerformanceMode.Low,
            scale,
            compact
        );

        standard.Name = "PerformanceStandard";
        balanced.Name = "PerformanceBalanced";
        low.Name = "PerformanceLow";

        var current = LauncherPreferences.ReadPerformanceMode();
        standard.ButtonPressed = current == LauncherPerformanceMode.Standard;
        balanced.ButtonPressed = current == LauncherPerformanceMode.Balanced;
        low.ButtonPressed = current == LauncherPerformanceMode.Low;

        var note = new StyledLabel(
            "Standard: original / Balanced: 45 FPS / Low: 30 FPS",
            scale,
            compact ? 12 : 13,
            HorizontalAlignment.Left
        )
        {
            Name = "PerformanceModeDescription",
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
        };
        note.AddThemeColorOverride("font_color", LauncherComponentTheme.TextMuted);
        group.AddChild(note);
    }

    private Button AddPerformanceButton(
        Container parent,
        ButtonGroup buttonGroup,
        string text,
        string mode,
        float scale,
        bool compact
    )
    {
        var button = new StyledButton(
            text,
            scale,
            compact ? 12 : 13,
            compact
                ? LauncherSectionMetrics.CompactDrawerToggleHeight
                : LauncherSectionMetrics.SecondaryButtonHeight
        )
        {
            ToggleMode = true,
            ButtonGroup = buttonGroup,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            AccessibilityName = LauncherJapanese.Text($"Performance {text}"),
        };

        LauncherButtonStyles.ApplySupportAction(button, scale);
        button.Pressed += () =>
        {
            LauncherPreferences.SavePerformanceMode(mode);
            PatchHelper.Log($"Performance preference changed: {mode}");
        };
        parent.AddChild(button);
        return button;
    }
}
