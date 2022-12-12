using UnityEngine;

public class ChatCommands : MonoBehaviour
{
    // Whisper /w
    public const string Whisper = "/w";
    public const string WhisperLong = "/whisper";

    public const string WhisperDescription =
        "/w <target> <message> - use this to send a private message to another player";

    // Mute /m
    public const string Mute = "/m";
    public const string MuteLong = "/mute";

    public const string MuteDescription =
        "/m <target> - Use this to mute a player, preventing further messages from being recieved";

    public const string MuteAllDescription =
        "/m all - Use this to mute all players, preventing further messages from being recieved";

    // Help /h
    public const string Help = "/h";
    public const string HelpLong = "/help";

    // Restart /r
    public const string Restart = "/r";
    public const string RestartLong = "/restart";

    // Change Team /t
    public const string Team = "/t";
    public const string TeamLong = "/team";
}