using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Instantiator : MonoBehaviour
{
    [SerializeField] Transform[] playersSpawnPoints;
    private void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
            Destroy(gameObject);
    }
    void Start()
    {
        for (int i = 1; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            GameObject playerObject = PhotonNetwork.Instantiate("Player" + i.ToString(), playersSpawnPoints[i - 1].position, Quaternion.identity);
            MasterGameManager.Instance.AddPlayerCharacter(PhotonNetwork.PlayerList[i].NickName, playerObject);
        }
    }
}
