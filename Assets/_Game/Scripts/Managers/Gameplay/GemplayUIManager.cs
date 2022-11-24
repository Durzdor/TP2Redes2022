using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GemplayUIManager : MonoBehaviourPun
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI team1Score;
    [SerializeField] TextMeshProUGUI team2Score;
    [SerializeField] TextMeshProUGUI team1Players;
    [SerializeField] TextMeshProUGUI team2Players;
    [SerializeField] int gamePlayTimer;
    List<string> playerNames;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(Countdown(gamePlayTimer));
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
        string team1text = "";
        string team2text = "";
        playerNames = MasterGameManager.Instance.getPlayersName();
        for (int i = 0; i < playerNames.Count; i++)
        {
            if (i == 0 || i == 2)
            {
                team1text += (i + 1).ToString() + " - " + playerNames[i] + "\n";
            }
            else
            {
                team2text += (i + 1).ToString() + " - " + playerNames[i] + "\n";
            }
        }
        photonView.RPC("UpdateTeams", RpcTarget.All, team1text, team2text);
    }
    [PunRPC]
    void UpdateGamePlayTimer(int count)
    {
        timerText.text = $"{count}";
    }
    [PunRPC]
    void UpdateScores(int team1Score, int team2Score)
    {
        this.team1Score.text = team1Score.ToString();
        this.team2Score.text = team2Score.ToString();
    }
    [PunRPC]
    void UpdateTeams(string team1, string team2)
    {
        team1Players.text = team1;
        team2Players.text = team2;
    }
    [PunRPC]
    void LoadGameplayLevel()
    {
        PhotonNetwork.LoadLevel(3);
    }
}
