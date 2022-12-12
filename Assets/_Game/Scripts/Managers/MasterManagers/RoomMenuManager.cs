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
    int autoStartTimer = 5;
    Coroutine timer;

    void Start()
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
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (i < PhotonNetwork.PlayerList.Length - 1)
            {
                photonView.RPC("UpdateListAddPlayer", RpcTarget.All, PhotonNetwork.PlayerList[i + 1], i);

            }
            else
            {
                photonView.RPC("UpdateListRemovePlayer", RpcTarget.All, i);
            }
        }
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
    void LoadGameplayLevel()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(3);
    }
    [PunRPC]
    void UpdateListAddPlayer(Player player, int playerTextSlot)
    {
        playerSlots[playerTextSlot].SetActive(true);
        TextMeshProUGUI nametext = playerSlots[playerTextSlot].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        nametext.text = player.NickName;
    }
    [PunRPC]
    void UpdateListRemovePlayer(int playerTextSlot)
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
    void ActivateCountDownObjects()
    {
        if (!countDownLabel.activeSelf)
            countDownLabel.SetActive(true);
        if (!startGameButton.activeSelf && PhotonNetwork.IsMasterClient)
            startGameButton.SetActive(true);
    }
    [PunRPC]
    void DeactivateCountDownObjects()
    {
        if (countDownLabel.activeSelf)
            countDownLabel.SetActive(false);
        if (startGameButton.activeSelf && PhotonNetwork.IsMasterClient)
            startGameButton.SetActive(false);
    }
}
