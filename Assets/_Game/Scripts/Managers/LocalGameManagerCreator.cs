using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LocalGameManagerCreator : MonoBehaviour
{
    void Start()
    {
        PhotonNetwork.Instantiate("LocalGamemanagerCreator", Vector2.zero, Quaternion.identity);
    }
}
