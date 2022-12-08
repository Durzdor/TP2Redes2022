using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class MasterGameManager : MonoBehaviourPun
{
    int team1Goals = 0;
    int team2Goals = 0;
    GemplayUIManager gameplayUIManager;
    GameObject ballObject;
    public int listCount;
    Dictionary<string, GameObject> PlayerList = new Dictionary<string, GameObject>();

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
    public void SetBallObject(GameObject ball)
    {
        ballObject = ball;
    }
    public void AddTeamScore(int score, Goals.TeamGoal team)
    {
        switch (team)
        {
            case Goals.TeamGoal.Team1:
                team2Goals += score;
                break;
            case Goals.TeamGoal.Team2:
                team1Goals += score;
                break;
            default:
                break;
        }
        if (!gameplayUIManager)
            gameplayUIManager = GameObject.Find("GameplayUIManagerCanvas").GetComponent<GemplayUIManager>();
        gameplayUIManager.photonView.RPC("UpdateScores", RpcTarget.All, team1Goals, team2Goals);

        ballObject.transform.position = Vector3.zero;
        ballObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
    public List<string> getPlayersName()
    {
        List<string> players = new List<string>();
        foreach (var player in PlayerList)
        {
            players.Add(player.Key);
        }
        return players;
    }
    public (int, int, int) getPostGameScores()
    {
        int winner = 0;
        if (team1Goals > team2Goals)
            winner = 1;
        if (team2Goals > team1Goals)
            winner = 2;
        return (team1Goals, team2Goals, winner);
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
            if (PlayerList.ContainsKey(player.NickName))
            {
                PlayerList.Remove(player.NickName);
                UpdatedPlayerList?.Invoke();
                ReadyToStartGame?.Invoke(PhotonNetwork.PlayerList.Length >= minPlayerToStart);
            }
        }
        else if (scene == 2)
        {
            photonView.RPC("RemovePlayerChar", RpcTarget.MasterClient, PlayerList[player.NickName].gameObject.name);
            PlayerList.Remove(player.NickName);
            if (!gameplayUIManager)
                gameplayUIManager = GameObject.Find("GameplayUIManagerCanvas").GetComponent<GemplayUIManager>();
            gameplayUIManager.UpdateTeamsNames();
        }
    }
    [PunRPC]
    void RemovePlayerChar(string playerChar)
    {
        GameObject charObj = GameObject.Find(playerChar);
        if (charObj)
            PhotonNetwork.Destroy(charObj);
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
    [PunRPC]
    void RequestShockWave(string client)
    {
        if (PlayerList.ContainsKey(client))
        {
            var charModel = PlayerList[client].GetComponent<PlayerModel>();
            charModel.ShockWave();
        }
    }
    [PunRPC]
    void AddPlayerObj(string playerName, string playerObjName)
    {
        GameObject playerObj = GameObject.Find(playerObjName);
        AddPlayerCharacter(playerName, playerObj);
    }
}