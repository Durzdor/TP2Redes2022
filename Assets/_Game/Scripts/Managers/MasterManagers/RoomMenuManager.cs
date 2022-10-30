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
    [SerializeField] GameObject countDownLabel;
    [SerializeField] GameObject startGameButton;
    [SerializeField] GameObject[] playerSlots;
    private int autoStartTimer = 25;

    void Start()
    {
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            return;
        MasterGameManager.Instance.ReadyToStartGame += StartCountDown;
        MasterGameManager.Instance.NewPlayerAdded += UpdateList;
    }
    public void StartCountDown()
    {
        photonView.RPC("ActivateCountDownObjects", RpcTarget.All);
        StartCoroutine(Countdown(autoStartTimer));
    }
    public void UpdateList(List<Player> playerList)
    {
        for (int i = 1; i < playerList.Count; i++)
        {
            photonView.RPC("SetPlayerName", RpcTarget.All, playerList[i], i - 1);
        }
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
    public void SetPlayerName(Player player, int playerTextSlot)
    {
        playerSlots[playerTextSlot].SetActive(true);
        TextMeshProUGUI nametext = playerSlots[playerTextSlot].GetComponentInChildren<TextMeshProUGUI>();
        nametext.text = player.NickName;
    }
    [PunRPC]
    private void UpdateTimer(int count)
    {
        countDown.text = $"{count}s";
    }
    [PunRPC]
    void ActivateCountDownObjects()
    {
        if (!countDownLabel.activeSelf)
            countDownLabel.SetActive(true);
        if (!startGameButton.activeSelf)
            startGameButton.SetActive(true);
    }
}
