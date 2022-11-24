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
    private const string WhisperCommand = "/w";

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

        if (words.Length > 2 && words[0] == WhisperCommand)
        {
            DoCommandWhisper(words);
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
        content.text += "Connected to Chat." + "\n" + "Use /w <target> <msg> to send private messages!" + "\n";
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
            string color;
            if (PhotonNetwork.NickName == currSenders)
                color = "<color=blue>";
            else
                color = "<color=red>";
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
        var target = words[1];
        foreach (var currPlayer in PhotonNetwork.PlayerList)
            if (target == currPlayer.NickName)
            {
                var currMessage = string.Join(" ", words, 2, words.Length - 2);
                _chatClient.SendPrivateMessage(target, currMessage);
                return;
            }

        content.text += "<color=pink>" + "Target not valid." + "</color>" + "\n";
        inputField.text = "";
    }
}