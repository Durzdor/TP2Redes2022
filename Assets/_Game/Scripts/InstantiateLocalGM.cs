using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InstantiateLocalGM : MonoBehaviour
{
    [SerializeField] GameObject localGm;
    void Start()
    {
        Instantiate(localGm, Vector2.zero, Quaternion.identity);
    }
}
