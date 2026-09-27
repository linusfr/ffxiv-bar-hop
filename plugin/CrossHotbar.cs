using System;

using Dalamud.Plugin.Services;

using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace BarHop;

/// <summary>
/// The game's cross hotbar, as far as stepping through its sets goes. Which set
/// is on screen is readable from the addon; changing it is the part the game
/// only offers by name, which is why a macro cannot step.
/// </summary>
internal sealed class CrossHotbar
{
    internal const string Addon = "_ActionCross";

    /// <summary>Cross hotbar sets, as the game numbers them in the UI.</summary>
    internal const int First = 1;
    internal const int Last = 8;

    /// <summary>
    /// RaptureHotbarModule keeps all hotbars in one list: ten normal ones, then
    /// the eight cross sets. The addon reports its index into that list.
    /// </summary>
    private const int FirstCrossHotbar = 10;

    private readonly IGameGui _gui;

    internal CrossHotbar(IGameGui gui) => _gui = gui;

    /// <summary>The addon, or null when the cross hotbar is not on screen.</summary>
    internal unsafe AddonActionBarBase* Bar()
    {
        var bar = (AddonActionBarBase*)_gui.GetAddonByName(Addon).Address;
        return bar is not null && ((AtkUnitBase*)bar)->IsVisible ? bar : null;
    }

    /// <summary>The raw hotbar index the addon draws, or -1 when it is not up.</summary>
    internal unsafe int RawId()
    {
        var bar = Bar();
        return bar is null ? -1 : bar->RaptureHotbarId;
    }

    /// <summary>The set on screen, or 0 when the cross hotbar is not up.</summary>
    internal int Current() => SetOf(RawId());

    /// <summary>The UI's set number for a raw hotbar index.</summary>
    internal static int SetOf(int raptureHotbarId)
    {
        if (raptureHotbarId < 0)
            return 0;

        var index = raptureHotbarId >= FirstCrossHotbar ? raptureHotbarId - FirstCrossHotbar : raptureHotbarId;
        return index + First;
    }

    /// <summary>
    /// The text command that switches sets. PvP swaps the whole cross hotbar
    /// for a separate one with its own command.
    /// </summary>
    internal static unsafe string ChangeCommand() =>
        RaptureHotbarModule.Instance()->PvPHotbarsActive ? "/pvpchotbar" : "/chotbar";

    /// <summary>
    /// Where <paramref name="step"/> sets away lands. Wrapping walks the sets in
    /// a circle so holding a direction never dead-ends; without it the ends stop.
    /// </summary>
    internal static int Step(int from, int step, bool wrap)
    {
        if (!wrap)
            return Math.Clamp(from + step, First, Last);

        var count = Last - First + 1;
        var index = (from - First + step) % count;
        if (index < 0)
            index += count;
        return index + First;
    }
}
