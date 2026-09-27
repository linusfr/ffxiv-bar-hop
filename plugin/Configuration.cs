using Dalamud.Configuration;

namespace BarHop;

[System.Serializable]
public sealed class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    /// How far a plain "/barhop up" moves. Two, because the cross hotbar's sets
    /// are usually filled in pairs.
    public int Step { get; set; } = 2;

    /// Walk off the end and come back at the other. Off stops at 1 and 8.
    public bool Wrap { get; set; } = true;
}
