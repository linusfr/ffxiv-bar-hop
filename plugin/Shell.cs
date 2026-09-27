using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;

namespace BarHop;

/// <summary>
/// Runs a text command the way the game runs one typed into chat, minus the
/// chat box. The cross hotbar only answers to "/chotbar change N", so stepping
/// means working out N and handing it over.
/// </summary>
internal static class Shell
{
    internal static unsafe void Run(string command)
    {
        var message = Utf8String.FromString(command);
        try
        {
            RaptureShellModule.Instance()->ExecuteCommandInner(message, UIModule.Instance());
        }
        finally
        {
            message->Dtor(true);
        }
    }
}
