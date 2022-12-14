using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Instantiator : MonoBehaviour
{
    public Transform[] spawnPoints;

    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
            Destroy(gameObject);
    }

    private void Start()
    {
        var ballObject = PhotonNetwork.Instantiate("Ball", spawnPoints[0].position, Quaternion.identity);
        MasterGameManager.Instance.SetBallObject(ballObject);
        PhotonNetwork.Instantiate("Splat", Vector3.zero, Quaternion.identity);
        //for (int i = 1; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        //{
        //    GameObject playerObject = PhotonNetwork.Instantiate("Player" + i.ToString(), spawnPoints[i].position, Quaternion.identity);
        //    MasterGameManager.Instance.AddPlayerCharacter(PhotonNetwork.PlayerList[i].NickName, playerObject);
        //}
    }
}