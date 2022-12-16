using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LocalInstantiator : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        //Transform[] paints = GameObject.Find("Instantiator").GetComponent<Instantiator>().spawnPoints;
        //for (int i = 0; i < paints.Length; i++)
        //{
        //    spawnPoints[i] = paints[i];
        //}
        //spawnPoints[1] = GameObject.Find("Player1Spawn").transform;
        //spawnPoints[2] = GameObject.Find("Player2Spawn").transform;
        //spawnPoints[3] = GameObject.Find("Player3Spawn").transform;
        //spawnPoints[4] = GameObject.Find("Player4Spawn").transform;
    }

    public void SpawnPlayer(int player)
    {
        var playerObject = PhotonNetwork.Instantiate("Player" + player.ToString(),
            GameObject.Find("Player" + player.ToString() + "Spawn").transform.position, Quaternion.identity);
        MasterGameManager.Instance.RPCMasterCall("AddPlayerObj", PhotonNetwork.PlayerList[player].NickName,
            playerObject.name);
    }

    public void SpawnBall()
    {
        var ballObject = PhotonNetwork.Instantiate("Ball", spawnPoints[0].position, Quaternion.identity);
        MasterGameManager.Instance.SetBallObject(ballObject);
    }

    public void CustomSpawn(string prefabname, Vector3 spawnPosition, Vector3 rotation)
    {
        PhotonNetwork.Instantiate(prefabname, spawnPosition,
            rotation == Vector3.zero ? Quaternion.identity : Quaternion.Euler(rotation));
    }
}