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
    Dictionary<string, GameObject> PlayerList = new Dictionary<string, GameObject>(); //diccionario de player que referencia modelo como hizo el profe

    public int minPlayerToStart;
    public event Action<bool> ReadyToStartGame;
    public event Action UpdatedPlayerList;

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
    public void AddPlayerCharacter(string key, GameObject playerChar)
    {
        PlayerList[key] = playerChar;
    }
    [PunRPC]
    void PlayerConnectedRPC(Player player)
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        if (scene == 1)
        {
            PlayerList.Add(player.NickName, null);
            UpdatedPlayerList?.Invoke();
            ReadyToStartGame?.Invoke(PhotonNetwork.PlayerList.Length >= minPlayerToStart);
        }
    }
    [PunRPC]
    void PlayerDisconnectedRPC(Player player)
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        if (scene == 1)
        {
            PlayerList.Remove(player.NickName);
            UpdatedPlayerList?.Invoke();
            ReadyToStartGame?.Invoke(PhotonNetwork.PlayerList.Length >= minPlayerToStart);
        }
        else if (scene == 2)
        {
            photonView.RPC("RemovePlayerChar", RpcTarget.MasterClient, PlayerList[player.NickName].gameObject.name);
            PlayerList.Remove(player.NickName);
        }
    }
    [PunRPC]
    void RemovePlayerChar(string playerChar)
    {
        PhotonNetwork.Destroy(GameObject.Find(playerChar));
    }
    [PunRPC]
    void RequestMove(string client, Vector3 dir)
    {
        if (PlayerList.ContainsKey(client))
        {
            var charModel = PlayerList[client].GetComponent<PlayerModel>();
            charModel.Move(dir);
        }
    }
}