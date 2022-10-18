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

    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI status;
    [SerializeField] private TMP_InputField nickname;
    [SerializeField] private TMP_InputField maxPlayersCount;
    [SerializeField] private TMP_InputField roomName;

    public event Action OnRoomJoinedSuccessfully;
    public event Action OnRoomLeftSuccessfully;
    public event Action<int> OnPlayerConnect;

    private RoomOptions _roomOptions;

    public void Connect()
    {
        if (string.IsNullOrEmpty(roomName.text) || string.IsNullOrWhiteSpace(roomName.text))
            return;
        if (string.IsNullOrEmpty(nickname.text) || string.IsNullOrWhiteSpace(nickname.text))
            return;
        if (string.IsNullOrEmpty(maxPlayersCount.text) || string.IsNullOrWhiteSpace(maxPlayersCount.text))
            return;

        PhotonNetwork.ConnectUsingSettings();
        button.interactable = false;
        status.text = "Connecting to Master...";
        PhotonNetwork.NickName = nickname.text;

        _roomOptions = new RoomOptions
        {
            MaxPlayers = byte.Parse(maxPlayersCount.text),
            IsOpen = true,
            IsVisible = true
        };
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        status.text = "Connecting to Lobby...";
    }

    public override void OnJoinedLobby()
    {
        status.text = "Connected to Lobby.";
        PhotonNetwork.JoinOrCreateRoom(roomName.text, _roomOptions, TypedLobby.Default);
    }


    public override void OnCreatedRoom()
    {
        status.text = "Created Room.";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        status.text = "Created Room failed." + returnCode + "|" + message;
        button.interactable = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        status.text = "Connection Failed." + cause;
        button.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        status.text = "Joined Room Successfully.";
        CheckUniqueName();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        button.interactable = true;
        PhotonNetwork.Disconnect();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnRoomLeftSuccessfully?.Invoke();
    }

    private void CheckUniqueName()
    {
        var uniqueName = true;
        foreach (var player in PhotonNetwork.PlayerListOthers)
        {
            if (PhotonNetwork.LocalPlayer.NickName != player.NickName) continue;
            uniqueName = false;
            PhotonNetwork.Disconnect();
        }

        if (uniqueName)
        {
            OnRoomJoinedSuccessfully?.Invoke();
            OnPlayerConnect?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
        }
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel(0);
    }
}
