using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject joinPanel;
    [SerializeField] GameObject createPanel;
    [SerializeField] TMP_InputField joinNickname;
    [SerializeField] TMP_InputField joinRoomName;
    [SerializeField] TMP_InputField createNickname;
    [SerializeField] TMP_InputField createRoomName;
    public void OpenCreateRoomPanel()
    {
        joinPanel.SetActive(false);
        createPanel.SetActive(true);
        PassJoinToCreate();
    }
    public void BackToStartPanel()
    {
        createPanel.SetActive(false);
        joinPanel.SetActive(true);
        PassCreateToJoin();
    }
    public void PassJoinToCreate()
    {
        createNickname.text = joinNickname.text;
        createRoomName.text = joinRoomName.text;
    }
    public void PassCreateToJoin()
    {
        joinNickname.text = createNickname.text;
        joinRoomName.text = createRoomName.text;
    }
}
