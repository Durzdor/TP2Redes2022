using UnityEngine;

public class ChatCommands : MonoBehaviour
{
    // Whisper /w
    public const string Whisper = "/w";
    public const string WhisperLong = "/whisper";

    public const string WhisperDescription =
        "/w or /whisper <target> <message> - use this to send a private <message> to the <target>";

    // Mute /m
    public const string Mute = "/m";
    public const string MuteLong = "/mute";

    public const string MuteDescription =
        "/m or /mute <target> or all - Use this to mute <target>, preventing further messages from being recieved";

    // Help /h
    public const string Help = "/h";
    public const string HelpLong = "/help";

    // Speed /speed
    public const string MoveSpeed = "/speed";
    public const string MoveSpeedLong = "/movespeed";

    public const string MoveSpeedDescription =
        "/speed or /movespeed <target> <value> - Use this to change the <target> movement speed to the new <value>";

    // Players connected /p
    public const string Players = "/p";
    public const string PlayersLong = "/players";
    public const string PlayersDescription = "/p or /players - Use this to display all connected players";

    // Impact force /force
    public const string ImpactForce = "/impact";
    public const string ImpactForceLong = "/impactforce";

    public const string ImpactForceDescription =
        "/impact or /impactforce <target> <value> - Use this to change the <target> impact force to the new <value>";

    //  Goals /goal
    public const string GoalModify = "/g";
    public const string GoalModifyLong = "/goal";

    public const string GoalModifyDescription =
        "/g or /goal <targetteam> <value> - Use this to add <value> goals to <targetteam>";

    //  Timer /timer
    public const string TimerModify = "/t";
    public const string TimerModifyLong = "/timer";

    public const string TimerModifyDescription =
        "/t or /timer <value> - Use this to change the remaining time in the game to <value> (in seconds)";
}