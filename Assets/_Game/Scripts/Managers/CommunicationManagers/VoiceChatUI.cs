using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Voice.Unity;
using Photon.Realtime;

public class VoiceChatUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    Dictionary<Speaker, Player> _dic = new Dictionary<Speaker, Player>();
    public void AddSpeaker(Speaker speaker, Player player)
    {
        _dic[speaker] = player;
    }
    private void Update()
    {
        string voiceChat = "";

        foreach (var item in _dic)
        {
            var speaker = item.Key;
            if (speaker.IsPlaying)
            {
                voiceChat += item.Value.NickName + "\n";
            }
        }
        text.text = voiceChat;
    }
}