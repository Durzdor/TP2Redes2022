using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

public class VoiceUI : MonoBehaviourPun
{
    [SerializeField] private Speaker speaker;
    private MicUI _micUI;

    private void Start()
    {
        if (photonView.IsMine)
            _micUI = FindObjectOfType<MicUI>();
        else
            FindObjectOfType<VoiceChatUI>().AddSpeaker(speaker, photonView.Owner);
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            var v = PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled;
            _micUI.Show(v);
        }
    }
}