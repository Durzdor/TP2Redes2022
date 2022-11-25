using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    [SerializeField] private Button joinButton;
    [SerializeField] private Button createButton;
    [SerializeField] private TextMeshProUGUI status;
    [SerializeField] private TMP_InputField nickname;
    [SerializeField] private TMP_InputField maxPlayersCount;
    [SerializeField] private TMP_InputField minPlayersCount;
    [SerializeField] private TMP_InputField roomName;

    public event Action OnRoomJoinedSuccessfully;
    public event Action OnRoomLeftSuccessfully;
    public event Action<Player> OnPlayerConnect;
    public event Action<Player> OnPlayerDisconnect;

    private RoomOptions _roomOptions;

    public static NetworkManager Instance;

    public void MakeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Awake()
    {
        MakeSingleton();
    }
    void Start()
    {
        GetClientInput();
        Connect();
    }
    void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
        if (joinButton)
            joinButton.interactable = false;
        if (createButton)
            createButton.interactable = false;
        status.text = "Connecting to Master...";
    }
    void GetClientInput()
    {
        GameObject[] masterClientObjects = GameObject.FindGameObjectsWithTag("MasterClient");
        GameObject[] clientObjects = GameObject.FindGameObjectsWithTag("Client");
        if (masterClientObjects.Length > 0)
        {
            PhotonNetwork.LocalPlayer.NickName = "Server";
            foreach (GameObject masterObject in masterClientObjects)
            {
                switch (masterObject.name)
                {
                    case "RoomNameInput":
                        roomName = masterObject.GetComponent<TMP_InputField>();
                        break;
                    case "MaxPlayersInput":
                        maxPlayersCount = masterObject.GetComponent<TMP_InputField>();
                        break;
                    case "MinPlayerStart":
                        minPlayersCount = masterObject.GetComponent<TMP_InputField>();
                        break;
                    case "CreateButton":
                        createButton = masterObject.GetComponent<Button>();
                        break;
                    case "ConectionStatus":
                        status = masterObject.GetComponent<TextMeshProUGUI>();
                        break;
                    default:
                        break;
                }
            }
        }
        if (clientObjects.Length > 0)
        {
            foreach (GameObject clientObject in clientObjects)
            {
                switch (clientObject.name)
                {
                    case "NicknameInput":
                        nickname = clientObject.GetComponent<TMP_InputField>();
                        break;
                    case "RoomNameInput":
                        roomName = clientObject.GetComponent<TMP_InputField>();
                        break;
                    case "JoinButton":
                        createButton = clientObject.GetComponent<Button>();
                        break;
                    case "ConectionStatus":
                        status = clientObject.GetComponent<TextMeshProUGUI>();
                        break;
                    default:
                        break;
                }
            }
        }
    }
    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel(0);
    }
    public void JoinRoom()
    {
        if (NicknameIsEmpty() || RoomNameIsEmpty())
            return;

        joinButton.interactable = false;
        PhotonNetwork.NickName = nickname.text;
        PhotonNetwork.JoinRoom(roomName.text);
    }
    public void CreateRoom()
    {
        if (RoomNameIsEmpty() || MaxPlayerIsEmpty())
            return;

        createButton.interactable = false;
        int maxPlayers = Mathf.Clamp(int.Parse(maxPlayersCount.text), 1, 4);
        _roomOptions = new RoomOptions
        {
            MaxPlayers = (byte)(maxPlayers + 1),
            IsOpen = true,
            IsVisible = true
        };
        PhotonNetwork.CreateRoom(roomName.text, _roomOptions, TypedLobby.Default);
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        status.text = "Connecting to Lobby...";
    }
    public override void OnJoinedLobby()
    {
        status.text = "Connected to Lobby.";
        if (joinButton)
            joinButton.interactable = true;
        if (createButton)
            createButton.interactable = true;
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LoadLevel(0);
        status.text = "Disconnected: " + cause;
        if (joinButton)
            joinButton.interactable = true;
        if (createButton)
            createButton.interactable = true;
    }
    public override void OnCreatedRoom()
    {
        status.text = "Created Room.";
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        status.text = "Created Room failed." + returnCode + "|" + message;
        if (createButton)
            createButton.interactable = true;
    }
    public override void OnJoinedRoom()
    {
        status.text = "Joined Room Successfully.";
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            if (!IsNameUnique())
            {
                PhotonNetwork.Disconnect();
                status.text = $"Nickname '{nickname.text}' already connected";
                return;
            }
        }
        else
        {
            MasterGameManager.Instance.minPlayerToStart = MinPlayerParse(minPlayersCount.text);
        }

        OnPlayerConnect?.Invoke(PhotonNetwork.LocalPlayer);
        PhotonNetwork.LoadLevel(1);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        status.text = "Created Room failed." + returnCode + "|" + message;
        if (joinButton)
            joinButton.interactable = true;
        if (createButton)
            createButton.interactable = true;
        PhotonNetwork.Disconnect();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnPlayerDisconnect?.Invoke(otherPlayer);
    }
    bool IsNameUnique()
    {
        foreach (Player player in PhotonNetwork.PlayerListOthers)
        {
            if (PhotonNetwork.LocalPlayer.NickName != player.NickName) continue;
            return false;
        }
        return true;
    }
    bool RoomNameIsEmpty()
    {
        if (string.IsNullOrEmpty(roomName.text) || string.IsNullOrWhiteSpace(roomName.text))
        {
            status.text = $"Invalid Room name '{roomName.text}'";
            return true;
        }
        return false;
    }
    bool MaxPlayerIsEmpty()
    {
        if (string.IsNullOrEmpty(maxPlayersCount.text) || string.IsNullOrWhiteSpace(maxPlayersCount.text))
        {
            status.text = $"Invalid Max players option '{maxPlayersCount.text}'";
            return true;
        }
        return false;
    }
    bool NicknameIsEmpty()
    {
        if (string.IsNullOrEmpty(nickname.text) || string.IsNullOrWhiteSpace(nickname.text))
        {
            status.text = $"Invalid Nickname '{nickname.text}'";
            return true;
        }
        return false;
    }
    int MinPlayerParse(string minPlayerQuantity)
    {
        if (string.IsNullOrEmpty(minPlayerQuantity) || string.IsNullOrWhiteSpace(minPlayerQuantity))
        {
            return 2;
        }
        else
        {
            int minPlayers = Mathf.Clamp(int.Parse(minPlayerQuantity), 1, 4);
            return minPlayers + 1;
        }
    }
}
