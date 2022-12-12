using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using TMPro;
using UnityEngine;

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

        if (words.Length > 3 && (words[0] == ChatCommands.Team || words[0] == ChatCommands.TeamLong))
            DoCommandChangeTeam(words);
        if (words.Length > 2 && (words[0] == ChatCommands.Whisper || words[0] == ChatCommands.WhisperLong))
        {
            DoCommandWhisper(words);
        }
        else if (words.Length > 1 && (words[0] == ChatCommands.Mute || words[0] == ChatCommands.MuteLong))
        {
            DoCommandMute(words);
        }
        else if (words[0] == ChatCommands.Help || words[0] == ChatCommands.HelpLong)
        {
            DoCommandHelp();
        }
        else if (words[0] == ChatCommands.Restart || words[0] == ChatCommands.RestartLong)
        {
            DoCommandRestart();
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
            content.text += target + " " + "is no longer Muted!" + "\n";
            return;
        }

        _mutedPlayers.Add(target);
        content.text += target + " " + "is now Muted!" + "\n";
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
                                              + ChatCommands.MuteAllDescription +
                                              "\n";
    }

    private void DoCommandRestart()
    {
        // /r
        PhotonNetwork.LoadLevel(2);
    }

    private void DoCommandChangeTeam(string[] words)
    {
        // /t red, /t blue
    }
}