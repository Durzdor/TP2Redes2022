using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Goals : MonoBehaviour
{
    [SerializeField] private TeamGoal team;
    public TeamGoal Team => team;

    public enum TeamGoal
    {
        Team1 = 1,
        Team2 = 2
    }

    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
            Destroy(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6) MasterGameManager.Instance.AddTeamScore(1, team);
    }
}