using System.Collections.Generic;
using UnityEngine;
using Photon.Voice;
using Photon.Voice.Unity;
using TMPro;

public class MicSelectorManager : MonoBehaviour
{
    [SerializeField] private Recorder rec;
    [SerializeField] private TMP_Dropdown dropdown;
    private void Awake()
    {
        var list = new List<string>(Microphone.devices);
        dropdown.AddOptions(list);
    }
    public void SetMic(int i)
    {
        var mic = Microphone.devices[i];
        rec.MicrophoneDevice = new DeviceInfo(mic);
    }
}