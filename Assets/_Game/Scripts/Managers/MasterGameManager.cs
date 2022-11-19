using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class MasterGameManager : MonoBehaviourPun
{
    public int listCount;
    List<Player> PlayerList = new List<Player>(); //diccionario de player que referencia modelo como hizo el profe

    public int minPlayerToStart;
    public event Action<bool> ReadyToStartGame;
    public event Action<List<Player>> UpdatedPlayerList;

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
        NetworkManager.Instance.OnPlayerDisconnect += PlayerDisconnected;
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            return;
    }
    private void Update()
    {
        listCount = PlayerList.Count;
        if(PhotonNetwork.LevelLoadingProgress > 100)
        {
            Debug.Log($"Level Loaded {SceneManager.GetActiveScene().name}");
        }
    }
    public void RPCMasterCall(string _rpcName, params object[] _params)
    {
        photonView.RPC(_rpcName, PhotonNetwork.MasterClient, _params);
    }
    public void PlayerConnected(Player player)
    {
        RPCMasterCall("PlayerConnectedRPC", player);
    }
    public void PlayerDisconnected(Player player)
    {
        RPCMasterCall("PlayerDisconnectedRPC", player);
    }
    [PunRPC]
    void PlayerConnectedRPC(Player player)
    {
        PlayerList.Add(player);
        UpdatedPlayerList?.Invoke(PlayerList);
        ReadyToStartGame?.Invoke(PlayerList.Count >= minPlayerToStart);
    }
    [PunRPC]
    void PlayerDisconnectedRPC(Player player)
    {
        PlayerList.Remove(player);
        UpdatedPlayerList?.Invoke(PlayerList);
        ReadyToStartGame?.Invoke(PlayerList.Count >= minPlayerToStart);
    }
}