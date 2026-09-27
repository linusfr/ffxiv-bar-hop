using System;
using System.Globalization;

using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace BarHop;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static IPluginLog              Log             { get; private set; } = null!;
    [PluginService] internal static ICommandManager         CommandManager  { get; private set; } = null!;
    [PluginService] internal static IGameGui                GameGui         { get; private set; } = null!;
    [PluginService] internal static IChatGui                ChatGui         { get; private set; } = null!;

    private const string Cmd = "/barhop";

    internal Configuration Config { get; }

    private readonly CrossHotbar _hotbar;

    public Plugin()
    {
        Config  = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        _hotbar = new CrossHotbar(GameGui);

        CommandManager.AddHandler(Cmd, new CommandInfo(OnCommand)
        {
            HelpMessage = "Step the cross hotbar: \"/barhop up\", \"/barhop down 3\", \"/barhop set 5\".",
        });

        Log.Info("Bar Hop loaded.");
    }

    private void OnCommand(string command, string args)
    {
        var parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var verb  = parts.Length > 0 ? parts[0].ToLowerInvariant() : "";

        switch (verb)
        {
            case "up":
                Hop(Count(parts, Config.Step));
                break;
            case "down":
                Hop(-Count(parts, Config.Step));
                break;
            case "set":
                Apply(Count(parts, 0));
                break;
            case "try":
                Try(parts);
                break;
            default:
                Say($"on set {_hotbar.Current()} (hotbar {_hotbar.RawId()}). Use up, down or set.");
                break;
        }
    }

    private static int Count(string[] parts, int fallback) =>
        parts.Length > 1 && int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var n)
            ? n
            : fallback;

    /// <summary>Moves <paramref name="step"/> sets from wherever the bar is.</summary>
    private void Hop(int step)
    {
        var current = _hotbar.Current();
        if (current == 0)
        {
            Say("the cross hotbar is not on screen.");
            return;
        }

        Apply(CrossHotbar.Step(current, step, Config.Wrap));
    }

    private void Apply(int set)
    {
        if (set < CrossHotbar.First || set > CrossHotbar.Last)
        {
            Say($"sets run {CrossHotbar.First} to {CrossHotbar.Last}.");
            return;
        }

        var command = $"{CrossHotbar.ChangeCommand()} change {set}";
        Shell.Run(command);
        Log.Info($"Bar Hop: {command}, bar now reads set {_hotbar.Current()}.");
    }

    /// <summary>
    /// Writes the hotbar index into the addon directly. Kept while the text
    /// command is still being trusted: it moves the bar's own idea of the set
    /// without telling the game, which is the fallback if "change" ever stops
    /// working.
    /// </summary>
    private unsafe void Try(string[] parts)
    {
        var bar = _hotbar.Bar();
        if (bar is null || parts.Length < 3 || parts[1].ToLowerInvariant() != "id"
            || !int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
        {
            Say("/barhop try id <hotbar index>, with the cross hotbar on screen.");
            return;
        }

        var before = $"set {_hotbar.Current()} (hotbar {_hotbar.RawId()})";
        bar->RaptureHotbarId = (byte)id;
        Say($"id {id}: was {before}, now set {_hotbar.Current()} (hotbar {_hotbar.RawId()}).");
    }

    private static void Say(string message) => ChatGui.Print($"[Bar Hop] {message}");

    internal void SaveConfig() => PluginInterface.SavePluginConfig(Config);

    public void Dispose() => CommandManager.RemoveHandler(Cmd);
}
