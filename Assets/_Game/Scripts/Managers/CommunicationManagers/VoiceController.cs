using UnityEngine;
using Photon.Pun;
using Photon.Voice.PUN;

public class VoiceController : MonoBehaviour
{
    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //Destroy(Camera.main.gameObject);
            Destroy(this);
        }
    }
    private void Start()
    {
        //if (PhotonNetwork.IsMasterClient) return;
        PhotonNetwork.Instantiate("VoiceObject", Vector3.zero, Quaternion.identity);
        PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled = false;
    }

    private void Update()
    {
        //if (PhotonNetwork.IsMasterClient) return;
        if (Input.GetKeyDown(KeyCode.V))
        {
            PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled = true;
        }
        else if (Input.GetKeyUp(KeyCode.V))
        {
            PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled = false;
        }
    }
}