using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Voice;
using Photon.Voice.Unity;

public class MicSelectorManager : MonoBehaviour
{
    public Recorder rec;
    public Dropdown dropdown;
    private void Awake()
    {
        var list = new List<string>(Microphone.devices);
        dropdown.AddOptions(list);
    }
    public void SetMic(int i)
    {
        //rec.UnityMicrophoneDevice = mic;
        string mic = Microphone.devices[i];
        rec.MicrophoneDevice = new DeviceInfo(mic);
    }
}