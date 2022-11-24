using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PostGameUIManager : MonoBehaviourPun
{
    [SerializeField] TextMeshProUGUI winnerTeamText;
    [SerializeField] TextMeshProUGUI team1ScoreText;
    [SerializeField] TextMeshProUGUI team2ScoreText;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var scores = MasterGameManager.Instance.getPostGameScores();
            photonView.RPC("UpdateWinner", RpcTarget.All, scores.Item1, scores.Item2, scores.Item3);
        }
    }
    [PunRPC]
    void UpdateWinner(int team1Score, int team2Score, int winner)
    {
        winnerTeamText.text = winner == 0 ? "BOTH!" : "TEAM " + winner.ToString();
        team1ScoreText.text = team1Score.ToString();
        team2ScoreText.text = team2Score.ToString();
    }
}
