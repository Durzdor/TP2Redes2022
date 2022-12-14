using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BallModel : MonoBehaviourPun
{
    Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void AddPush(Vector3 dir, float impactForce)
    {
        rb.AddForce(new Vector2(dir.x, dir.y) * impactForce, ForceMode2D.Impulse);
    }
    [PunRPC]
    void GettingShockWaved(Vector3 dir, float impactForce)
    {
        AddPush(dir, impactForce);
    }
}
