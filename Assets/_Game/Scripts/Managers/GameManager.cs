using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviourPun
{
    [SerializeField] private NetworkManager netManager;


    public List<string> PlayerList { get; private set; }
    public List<int> PlayerTablePositions { get; private set; }

    public static GameManager Instance;

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
    public void StartGame()
    {
        if (!photonView.IsMine) return;
    }
}