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

    // Background color /bgcolor
    public const string BackgroundColor = "/bg";
    public const string BackgroundColorLong = "/background";

    public const string BackgroundColorDescription =
        "/bg or /background <color> - Use this to change the background color to \n (black blue grey green magenta cyan red white yellow) <color>";

    // Player color /c
    public const string PlayerColor = "/c";
    public const string PlayerColorLong = "/color";

    public const string PlayerColorDescription =
        "/c or /color <target> <color> - Use this to change the <target> color to \n (black blue grey green magenta cyan red white yellow) <color>";
}