using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    [SerializeField] private TextMeshProUGUI content;
    [SerializeField] private TMP_InputField inputField;
    private ChatClient _chatClient;
    private readonly List<string> _mutedPlayers = new List<string>();

    private string _channel;

    private void Start()
    {
        _chatClient = new ChatClient(this);
        _chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
            PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion,
            new AuthenticationValues(PhotonNetwork.NickName));
    }

    private void Update()
    {
        _chatClient.Service();
    }

    public void ChatSendMessage()
    {
        var message = inputField.text;
        if (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message)) return;
        var words = message.Split(' ');

        if (SceneManager.GetActiveScene().buildIndex != 2) return;

        //Gameplay commands
        if (words.Length == 3 && (words[0] == ChatCommands.MoveSpeed || words[0] == ChatCommands.MoveSpeedLong))
        {
            DoCommandMoveSpeed(words);
        }
        else if (words.Length == 3 &&
                 (words[0] == ChatCommands.ImpactForce || words[0] == ChatCommands.ImpactForceLong))
        {
            DoCommandImpactForce(words);
        }
        else if (words.Length == 3 &&
                 (words[0] == ChatCommands.GoalModify || words[0] == ChatCommands.GoalModifyLong))
        {
            DoCommandGoalModify(words);
        }
        else if (words.Length == 2 &&
                 (words[0] == ChatCommands.TimerModify || words[0] == ChatCommands.TimerModifyLong))
        {
            DoCommandTimerModify(words);
        }
        // Local commands?
        else if (words.Length > 2 && (words[0] == ChatCommands.Whisper || words[0] == ChatCommands.WhisperLong))
        {
            DoCommandWhisper(words);
        }
        else if (words.Length == 2 && (words[0] == ChatCommands.Mute || words[0] == ChatCommands.MuteLong))
        {
            DoCommandMute(words);
        }
        else if (words.Length == 2 &&
                 (words[0] == ChatCommands.BackgroundColor || words[0] == ChatCommands.BackgroundColorLong))
        {
            DoCommandBackgroundColorModify(words);
        }
        else if (words[0] == ChatCommands.Help || words[0] == ChatCommands.HelpLong)
        {
            DoCommandHelp();
        }
        else if (words[0] == ChatCommands.Players || words[0] == ChatCommands.PlayersLong)
        {
            DoCommandConnectedPlayers();
        }
        else
        {
            _chatClient.PublishMessage(_channel, message);
            inputField.text = "";
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {
    }

    public void OnChatStateChange(ChatState state)
    {
    }

    public void OnConnected()
    {
        content.text += "Connected to Chat." + "\n" + "Use /h to check commands available!" + "\n";
        _channel = PhotonNetwork.CurrentRoom.Name;
        _chatClient.Subscribe(_channel);
    }

    public void OnDisconnected()
    {
        content.text += "Disconnected from Chat." + "\n";
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (var i = 0; i < senders.Length; i++)
        {
            var currSenders = senders[i];
            if (_mutedPlayers.Contains(currSenders)) continue;
            string color;
            if (PhotonNetwork.NickName == currSenders)
            {
                color = "<color=blue>";
                currSenders = "Me";
            }
            else
            {
                color = "<color=red>";
            }

            content.text += color + currSenders + ": " + "</color>" + messages[i] + "\n";
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        content.text += "<color=yellow>" + sender + ": " + "</color>" + message + "\n";
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        for (var i = 0; i < channels.Length; i++) content.text += "Subscribed to " + channels[i] + "\n";
    }

    public void OnUnsubscribed(string[] channels)
    {
        for (var i = 0; i < channels.Length; i++) content.text += "Unsubscribed to " + channels[i] + "\n";
    }

    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }

    private void DoCommandWhisper(string[] words)
    {
        // /w <player> <msg>
        var target = words[1];
        foreach (var currPlayer in PhotonNetwork.PlayerList)
            if (target == currPlayer.NickName)
            {
                var currMessage = string.Join(" ", words, 2, words.Length - 2);
                _chatClient.SendPrivateMessage(target, currMessage);
                inputField.text = "";
                return;
            }

        content.text += "<color=orange>" + "Target not valid." + "</color>" + "\n";
        inputField.text = "";
    }

    private void DoCommandMute(string[] words)
    {
        // /m <player/all>
        inputField.text = "";
        var target = words[1];
        foreach (var currPlayer in PhotonNetwork.PlayerList)
        {
            if (currPlayer.NickName == PhotonNetwork.NickName) continue;
            if (target == currPlayer.NickName)
            {
                CheckMute(target);
                return;
            }

            if (target != "all") continue;
            CheckMute(currPlayer.NickName);
        }

        if (target == "all") return;
        content.text += "<color=orange>" + "Target not valid." + "</color>" + "\n";
        inputField.text = "";
    }

    private void CheckMute(string target)
    {
        if (_mutedPlayers.Contains(target))
        {
            _mutedPlayers.Remove(target);
            content.text += $"<color=orange> {target} is no longer muted! </color> \n";
            return;
        }

        _mutedPlayers.Add(target);
        content.text += $"<color=orange> {target} is muted! </color> \n";
    }

    private void DoCommandHelp()
    {
        // /h
        inputField.text = "";
        content.text += "Available commands:" + "\n"
                                              + ChatCommands.WhisperDescription +
                                              "\n"
                                              + ChatCommands.MuteDescription +
                                              "\n"
                                              + ChatCommands.PlayersDescription +
                                              "\n"
                                              + ChatCommands.MoveSpeedDescription +
                                              "\n"
                                              + ChatCommands.ImpactForceDescription +
                                              "\n"
                                              + ChatCommands.GoalModifyDescription +
                                              "\n"
                                              + ChatCommands.TimerModifyDescription +
                                              "\n"
                                              + ChatCommands.BackgroundColorDescription +
                                              "\n";
    }

    private void DoCommandMoveSpeed(string[] words)
    {
        // /speed
        var target = words[1];
        if (float.TryParse(words[2], out var newSpeedInput))
        {
            for (var i = 1; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (target != PhotonNetwork.PlayerList[i].NickName) continue;
                var objName = $"Player{i}(Clone)";
                var obj = GameObject.Find(objName);
                var objModel = obj.GetComponent<PlayerModel>();
                objModel.ChangeSpeed(newSpeedInput);
                content.text += $"<color=orange> Speed set to {newSpeedInput}. </color> \n";
                inputField.text = "";
                return;
            }

            content.text += "<color=orange>" + "Target not valid." + "</color>" + "\n";
            inputField.text = "";
        }
        else
        {
            content.text += "<color=orange>" + "Value not valid." + "</color>" + "\n";
            inputField.text = "";
        }
    }

    private void DoCommandImpactForce(string[] words)
    {
        // /impact
        var target = words[1];
        if (float.TryParse(words[2], out var newImpactForceInput))
        {
            for (var i = 1; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (target != PhotonNetwork.PlayerList[i].NickName) continue;
                var objName = $"Player{i}(Clone)";
                var obj = GameObject.Find(objName);
                var objModel = obj.GetComponent<PlayerModel>();
                objModel.ChangeImpact(newImpactForceInput);
                content.text += $"<color=orange> ImpactForce set to {newImpactForceInput}. </color> \n";
                inputField.text = "";
                return;
            }

            content.text += "<color=orange>" + "Target not valid." + "</color>" + "\n";
            inputField.text = "";
        }
        else
        {
            content.text += "<color=orange>" + "Value not valid." + "</color>" + "\n";
            inputField.text = "";
        }
    }

    private void DoCommandConnectedPlayers()
    {
        // /players
        content.text += $"Connected Players: \n";
        for (var i = 1; i < PhotonNetwork.PlayerList.Length; i++)
        {
            var playerNickname = PhotonNetwork.PlayerList[i].NickName;
            content.text += $"{i})<color=orange>{playerNickname}</color> \n";
        }

        inputField.text = "";
    }

    private void DoCommandGoalModify(string[] words)
    {
        // /goal
        if (Enum.TryParse(words[1], out Goals.TeamGoal teamEnum))
        {
            if (int.TryParse(words[2], out var extraGoals))
            {
                MasterGameManager.Instance.RPCMasterCall("ChangeTeamScore", extraGoals, teamEnum);
                //MasterGameManager.Instance.ChangeTeamScore(extraGoals, teamEnum);
                content.text += $"<color=orange> {teamEnum} got {extraGoals} extra goals. </color> \n";
                inputField.text = "";
            }
            else
            {
                content.text += "<color=orange>" + "Goals not valid." + "</color>" + "\n";
                inputField.text = "";
            }
        }
        else
        {
            content.text += "<color=orange>" + "Team not valid." + "</color>" + "\n";
            inputField.text = "";
        }
    }

    private void DoCommandTimerModify(string[] words)
    {
        // /t
        if (int.TryParse(words[1], out var newTimer))
        {
            MasterGameManager.Instance.RPCMasterCall("ChangeCountdownTimer", newTimer);
            //MasterGameManager.Instance.ChangeCountdownTimer(newTimer);
            content.text += $"<color=orange> Timer set to {newTimer} seconds remaining. </color> \n";
            inputField.text = "";
        }
        else
        {
            content.text += "<color=orange>" + "Time not valid." + "</color>" + "\n";
            inputField.text = "";
        }
    }

    private void DoCommandBackgroundColorModify(string[] words)
    {
        // /bg
        // black blue grey green magenta cyan red white yellow
        switch (words[1])
        {
            case "grey":
            case "Grey":
                CameraBackgroundChange(Color.gray);
                break;
            case "green":
            case "Green":
                CameraBackgroundChange(Color.green);
                break;
            case "black":
            case "Black":
                CameraBackgroundChange(Color.black);
                break;
            case "blue":
            case "Blue":
                CameraBackgroundChange(Color.blue);
                break;
            case "magenta":
            case "Magenta":
                CameraBackgroundChange(Color.magenta);
                break;
            case "cyan":
            case "Cyan":
                CameraBackgroundChange(Color.cyan);
                break;
            case "red":
            case "Red":
                CameraBackgroundChange(Color.red);
                break;
            case "white":
            case "White":
                CameraBackgroundChange(Color.white);
                break;
            case "yellow":
            case "Yellow":
                CameraBackgroundChange(Color.yellow);
                break;
            default:
                content.text += "<color=orange>" + "Color not valid." + "</color>" + "\n";
                inputField.text = "";
                break;
        }
    }

    private void CameraBackgroundChange(Color color)
    {
        foreach (var cam in Camera.allCameras) cam.backgroundColor = color;
        content.text += $"<color=orange> Background color set to {color}. </color> \n";
        inputField.text = "";
    }
    
    
}