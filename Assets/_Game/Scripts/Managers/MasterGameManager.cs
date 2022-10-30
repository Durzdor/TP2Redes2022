using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MasterGameManager : MonoBehaviourPun
{
    public int listCount;
    List<Player> PlayerList = new List<Player>();

    public int minPlayerToStart;
    public event Action ReadyToStartGame;
    public event Action<List<Player>> NewPlayerAdded;

    public static MasterGameManager Instance;
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
        NetworkManager.Instance.OnPlayerConnect += PlayerConnected;
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            return;
    }
    private void Update()
    {
        listCount = PlayerList.Count;
    }
    public void RPCMasterCall(string _rpcName, params object[] _params)
    {
        photonView.RPC(_rpcName, PhotonNetwork.MasterClient, _params);
    }
    public void PlayerConnected(Player player)
    {
        RPCMasterCall("PlayerConnectedRPC", player);
    }
    [PunRPC]
    void PlayerConnectedRPC(Player player)
    {
        PlayerList.Add(player);
        NewPlayerAdded?.Invoke(PlayerList);
        if (PlayerList.Count >= minPlayerToStart)
        {
            ReadyToStartGame?.Invoke();
        }
    }
}