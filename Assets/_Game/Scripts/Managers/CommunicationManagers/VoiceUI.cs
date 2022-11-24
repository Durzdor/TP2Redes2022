using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;

public class VoiceUI : MonoBehaviourPun
{
    public Speaker speaker;
    MicUI _micUI;
    void Start()
    {
        if (photonView.IsMine)
        {
            _micUI = FindObjectOfType<MicUI>();
        }
        else
        {
            FindObjectOfType<VoiceChatUI>().AddSpeaker(speaker, photonView.Owner);
        }
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