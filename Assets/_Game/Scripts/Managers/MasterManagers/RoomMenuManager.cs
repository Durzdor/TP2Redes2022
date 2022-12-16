using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomMenuManager : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private TextMeshProUGUI countDown;
    [SerializeField] private GameObject countDownLabel;
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private GameObject[] playerSlots;
    private int autoStartTimer = 5;
    private Coroutine timer;

    private void Start()
    {
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            return;
        MasterGameManager.Instance.ReadyToStartGame += StartStopCountDown;
        MasterGameManager.Instance.UpdatedPlayerList += UpdateList;
    }

    public void StartStopCountDown(bool ready)
    {
        if (ready)
        {
            if (timer != null)
                StopCoroutine(timer);
            photonView.RPC("ActivateCountDownObjects", RpcTarget.All);
            timer = StartCoroutine(Countdown(autoStartTimer));
        }
        else
        {
            photonView.RPC("DeactivateCountDownObjects", RpcTarget.All);
            if (timer != null)
                StopCoroutine(timer);
        }
    }

    public void UpdateList()
    {
        for (var i = 0; i < playerSlots.Length; i++)
            if (i < PhotonNetwork.PlayerList.Length - 1)
                photonView.RPC("UpdateListAddPlayer", RpcTarget.All, PhotonNetwork.PlayerList[i + 1], i);
            else
                photonView.RPC("UpdateListRemovePlayer", RpcTarget.All, i);
    }

    public void StartGameButton()
    {
        photonView.RPC("LoadGameplayLevel", RpcTarget.All);
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

        photonView.RPC("LoadGameplayLevel", RpcTarget.All);
    }

    [PunRPC]
    private void LoadGameplayLevel()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(2);
    }

    [PunRPC]
    private void UpdateListAddPlayer(Player player, int playerTextSlot)
    {
        playerSlots[playerTextSlot].SetActive(true);
        var nametext = playerSlots[playerTextSlot].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        nametext.text = player.NickName;
    }

    [PunRPC]
    private void UpdateListRemovePlayer(int playerTextSlot)
    {
        if (playerSlots[playerTextSlot].activeSelf)
            playerSlots[playerTextSlot].SetActive(false);
    }

    [PunRPC]
    private void UpdateTimer(int count)
    {
        countDown.text = $"{count}s";
    }

    [PunRPC]
    private void ActivateCountDownObjects()
    {
        if (!countDownLabel.activeSelf)
            countDownLabel.SetActive(true);
        if (!startGameButton.activeSelf && PhotonNetwork.IsMasterClient)
            startGameButton.SetActive(true);
    }

    [PunRPC]
    private void DeactivateCountDownObjects()
    {
        if (countDownLabel.activeSelf)
            countDownLabel.SetActive(false);
        if (startGameButton.activeSelf && PhotonNetwork.IsMasterClient)
            startGameButton.SetActive(false);
    }
}