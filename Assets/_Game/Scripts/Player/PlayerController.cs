using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    PlayerModel charModel;
    void Start()
    {
        charModel = GetComponent<PlayerModel>();
        if (!charModel.photonView.IsMine)
            Destroy(this);
    }
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, v, 0);
        if (dir != Vector3.zero)
            charModel.Move(dir);
        if (Input.GetKeyDown(KeyCode.Space))
            charModel.ShockWave();
    }
}