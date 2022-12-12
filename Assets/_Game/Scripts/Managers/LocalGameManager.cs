using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LocalGameManager : MonoBehaviourPun
{
    private LocalInstantiator instanceManager;

    public static LocalGameManager Instance;

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
    }
    public void SpawnPlayer()
    {
        Debug.Log("Spawning Player");
        if (photonView.IsMine)
        {
            Debug.Log("Is Mine");
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.LocalPlayer.NickName)
                {
                    Debug.Log("Instantiate");
                    instanceManager.SpawnPlayer(i);
                }
            }
        }
    }
    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            Debug.Log("Start on GameplayScene");
            instanceManager = GameObject.Find("Instantiator").GetComponent<LocalInstantiator>();
            SpawnPlayer();
        }
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Debug.Log("Start on RoomScene");
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        }
    }
}
