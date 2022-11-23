using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerClientController : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, v, 0);
        if (dir != Vector3.zero)
            MasterGameManager.Instance.RPCMasterCall("RequestMove", PhotonNetwork.LocalPlayer.NickName, dir);
        if (Input.GetKeyDown(KeyCode.Space))
            MasterGameManager.Instance.RPCMasterCall("RequestShockWave", PhotonNetwork.LocalPlayer.NickName);
    }
}
