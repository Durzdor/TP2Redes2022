using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Instantiator : MonoBehaviour
{
    [SerializeField] Transform[] spawnPoints;
    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
            Destroy(gameObject);
    }
    void Start()
    {
        GameObject ballObject = PhotonNetwork.Instantiate("Ball", spawnPoints[0].position, Quaternion.identity);
        MasterGameManager.Instance.SetBallObject(ballObject);
        for (int i = 1; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            GameObject playerObject = PhotonNetwork.Instantiate("Player" + i.ToString(), spawnPoints[i].position, Quaternion.identity);
            MasterGameManager.Instance.AddPlayerCharacter(PhotonNetwork.PlayerList[i].NickName, playerObject);
        }
    }
}
