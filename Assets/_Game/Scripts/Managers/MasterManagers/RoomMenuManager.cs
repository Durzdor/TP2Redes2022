using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomMenuManager : MonoBehaviourPun
{
    [SerializeField] TextMeshProUGUI roomName;
    [SerializeField] TextMeshProUGUI countDown;
    [SerializeField] Button startGameButton;
    [SerializeField] GameObject[] playerSlots;
    private int autoStartTimer = 25;

    private void Start()
    {
        roomName.text = PhotonNetwork.CurrentRoom.Name;
    }
    private IEnumerator Countdown(int duration)
    {
        var count = duration;
        while (count > 0)
        {
            yield return new WaitForSeconds(1);
            count--;
            photonView.RPC("UpdateTimer", RpcTarget.All, count);
        }

        //Load level
    }
    [PunRPC]
    private void UpdateTimer(int count)
    {
        countDown.text = $"{count}s";
    }
}
