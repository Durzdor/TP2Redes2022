using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameplayUIManager : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI team1Score;
    [SerializeField] private TextMeshProUGUI team2Score;
    [SerializeField] private TextMeshProUGUI team1Players;
    [SerializeField] private TextMeshProUGUI team2Players;
    [SerializeField] private int gamePlayTimer;
    private List<string> playerNames;
    private Coroutine countdownRoutine;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("UpdateGamePlayTimer", RpcTarget.All, gamePlayTimer);
            countdownRoutine = StartCoroutine(Countdown(gamePlayTimer));
            photonView.RPC("UpdateScores", RpcTarget.All, 0, 0);

            UpdateTeamsNames();
        }
    }

    private IEnumerator Countdown(int duration)
    {
        var count = duration;
        while (count > 0)
        {
            yield return new WaitForSeconds(1);
            count--;
            photonView.RPC("UpdateGamePlayTimer", RpcTarget.All, count);
        }

        photonView.RPC("LoadGameplayLevel", RpcTarget.All);
    }

    public void UpdateTeamsNames()
    {
        var team1text = "";
        var team2text = "";
        playerNames = MasterGameManager.Instance.getPlayersName();
        for (var i = 0; i < playerNames.Count; i++)
            if (i == 0 || i == 2)
                team1text += (i + 1).ToString() + " - " + playerNames[i] + "\n";
            else
                team2text += (i + 1).ToString() + " - " + playerNames[i] + "\n";
        photonView.RPC("UpdateTeams", RpcTarget.All, team1text, team2text);
    }

    [PunRPC]
    private void UpdateGamePlayTimer(int count)
    {
        timerText.text = $"{count}";
    }

    [PunRPC]
    private void UpdateScores(int team1Score, int team2Score)
    {
        this.team1Score.text = team1Score.ToString();
        this.team2Score.text = team2Score.ToString();
    }

    [PunRPC]
    private void UpdateTeams(string team1, string team2)
    {
        team1Players.text = team1;
        team2Players.text = team2;
    }

    [PunRPC]
    private void LoadGameplayLevel()
    {
        PhotonNetwork.LoadLevel(3);
    }

    public void ChangeCountdownTimer(int newCountdownValue)
    {
        StopCoroutine(countdownRoutine);
        countdownRoutine = StartCoroutine(Countdown(newCountdownValue));
        photonView.RPC("UpdateGamePlayTimer", RpcTarget.All, newCountdownValue);
    }
}