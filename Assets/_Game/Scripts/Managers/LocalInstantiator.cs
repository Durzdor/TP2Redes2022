using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LocalInstantiator : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
    }
    public void SpawnPlayer(int player)
    {
        GameObject playerObject = PhotonNetwork.Instantiate("Player" + player.ToString(), spawnPoints[player].position, Quaternion.identity);
        MasterGameManager.Instance.RPCMasterCall("AddPlayerObj", PhotonNetwork.PlayerList[player].NickName, playerObject);
    }
    public void SpawnBall()
    {
        GameObject ballObject = PhotonNetwork.Instantiate("Ball", spawnPoints[0].position, Quaternion.identity);
        MasterGameManager.Instance.SetBallObject(ballObject);
    }
    public void CustomSpawn(string prefabname, Vector3 spawnPosition, Vector3 rotation)
    {
        PhotonNetwork.Instantiate(prefabname, spawnPosition,
            rotation == Vector3.zero ? Quaternion.identity : Quaternion.Euler(rotation));
    }
}
