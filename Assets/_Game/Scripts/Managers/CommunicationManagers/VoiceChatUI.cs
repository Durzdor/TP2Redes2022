using System.Collections.Generic;
using Photon.Realtime;
using Photon.Voice.Unity;
using TMPro;
using UnityEngine;

public class VoiceChatUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private readonly Dictionary<Speaker, Player> _dic = new Dictionary<Speaker, Player>();

    public void AddSpeaker(Speaker speaker, Player player)
    {
        _dic[speaker] = player;
    }

    private void Update()
    {
        var voiceChat = "";

        foreach (var item in _dic)
        {
            var speaker = item.Key;
            if (speaker.IsPlaying) voiceChat += item.Value.NickName + "\n";
        }

        text.text = voiceChat;
    }
}