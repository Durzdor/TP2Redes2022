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
    private int team1Goals = 0;
    private int team2Goals = 0;
    private GameplayUIManager gameplayUIManager;
    private GameObject ballObject;
    public int listCount;
    private Dictionary<string, GameObject> PlayerList = new Dictionary<string, GameObject>();
    [SerializeField] private GameObject localGm;
    public int minPlayerToStart;
    public event Action<bool> ReadyToStartGame;
    public event Action UpdatedPlayerList;

    [SerializeField] private GameObject instantiatorPrefab;
    private LocalInstantiator instanceManager;

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

    private void Awake()
    {
        MakeSingleton();
    }

    private void Start()
    {
        NetworkManager.Instance.OnPlayerConnect += PlayerConnected;
        NetworkManager.Instance.OnPlayerDisconnect += PlayerDisconnected;
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
        ChangeTeamScore(score, team);

        ballObject.transform.position = Vector3.zero;
        ballObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    [PunRPC]
    public void ChangeTeamScore(int score, Goals.TeamGoal team)
    {
        switch (team)
        {
            case Goals.TeamGoal.Team1:
                team1Goals += score;
                team1Goals = Math.Max(0, team1Goals);
                break;
            case Goals.TeamGoal.Team2:
                team2Goals += score;
                team2Goals = Math.Max(0, team2Goals);
                break;
            default:
                break;
        }

        if (!gameplayUIManager)
            gameplayUIManager = GameObject.Find("GameplayUIManagerCanvas").GetComponent<GameplayUIManager>();
        gameplayUIManager.photonView.RPC("UpdateScores", RpcTarget.All, team1Goals, team2Goals);
    }

    public List<string> getPlayersName()
    {
        var players = new List<string>();
        foreach (var player in PlayerList) players.Add(player.Key);
        return players;
    }

    public (int, int, int) getPostGameScores()
    {
        var winner = 0;
        if (team1Goals > team2Goals)
            winner = 1;
        if (team2Goals > team1Goals)
            winner = 2;
        return (team1Goals, team2Goals, winner);
    }

    private void OnLevelWasLoaded(int level)
    {
        if (PhotonNetwork.IsMasterClient) return;
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            //Instantiate(localGm, Vector2.zero, Quaternion.identity);
            Debug.Log("Start on GameplayScene");
            instanceManager = Instantiate(instantiatorPrefab).GetComponent<LocalInstantiator>();
            if (instanceManager)
                SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        Debug.Log("Spawning Player");
        if (photonView.IsMine)
        {
        }

        Debug.Log("Is Mine");
        for (var i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                Debug.Log("Instantiate");
                instanceManager.SpawnPlayer(i);
            }
    }

    [PunRPC]
    private void PlayerConnectedRPC(Player player)
    {
        var scene = SceneManager.GetActiveScene().buildIndex;
        if (scene == 1)
        {
            PlayerList.Add(player.NickName, null);
            UpdatedPlayerList?.Invoke();
            ReadyToStartGame?.Invoke(PhotonNetwork.PlayerList.Length >= minPlayerToStart);
        }
    }

    [PunRPC]
    private void PlayerDisconnectedRPC(Player player)
    {
        var scene = SceneManager.GetActiveScene().buildIndex;
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
                gameplayUIManager = GameObject.Find("GameplayUIManagerCanvas").GetComponent<GameplayUIManager>();
            gameplayUIManager.UpdateTeamsNames();
        }
    }

    [PunRPC]
    private void RemovePlayerChar(string playerChar)
    {
        var charObj = GameObject.Find(playerChar);
        if (charObj)
            PhotonNetwork.Destroy(charObj);
    }

    [PunRPC]
    private void RequestMove(string client, Vector3 dir)
    {
        if (PlayerList.ContainsKey(client))
        {
            var charModel = PlayerList[client].GetComponent<PlayerModel>();
            charModel.Move(dir);
        }
    }

    [PunRPC]
    private void RequestShockWave(string client)
    {
        if (PlayerList.ContainsKey(client))
        {
            var charModel = PlayerList[client].GetComponent<PlayerModel>();
            charModel.ShockWave();
        }
    }

    [PunRPC]
    private void AddPlayerObj(string playerName, string playerObjName)
    {
        var playerObj = GameObject.Find(playerObjName);
        AddPlayerCharacter(playerName, playerObj);
    }

    public List<GameObject> getPlayersObj()
    {
        var players = new List<GameObject>();
        foreach (var player in PlayerList) players.Add(player.Value);
        return players;
    }

    public GameObject getPlayerObjFromNickname(string nickname)
    {
        return PlayerList[nickname];
    }

    [PunRPC]
    public void ChangeCountdownTimer(int newCountdownTimer)
    {
        if (!gameplayUIManager)
            gameplayUIManager = GameObject.Find("GameplayUIManagerCanvas").GetComponent<GameplayUIManager>();
        gameplayUIManager.ChangeCountdownTimer(newCountdownTimer);
    }

    [PunRPC]
    public void SplatActivation()
    {
        var obj = GameObject.Find("Splat(Clone)");
        var child = obj.transform.GetChild(0).gameObject;
        var newState = !child.activeInHierarchy;
        child.SetActive(newState);
    }
}