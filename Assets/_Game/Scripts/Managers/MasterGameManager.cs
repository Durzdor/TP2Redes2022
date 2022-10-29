using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MasterGameManager : MonoBehaviourPun
{
    [SerializeField] private NetworkManager netManager;
    public int listCount;
    List<Player> PlayerList = new List<Player>();

    public int minPlayerToStart;
    public List<int> PlayerTablePositions { get; private set; }

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
        netManager.OnPlayerConnect += PlayerConnected;
    }
    private void Update()
    {
        listCount = PlayerList.Count;
    }
    public void RPCCall(string _rpcName, params object[] _params)
    {
        photonView.RPC(_rpcName, PhotonNetwork.MasterClient, _params);
    }
    public void PlayerConnected(Player player)
    {
        PlayerList.Add(player);
    }
}