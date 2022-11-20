using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerModel : MonoBehaviourPun
{
    [SerializeField] float speed;
    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Move(Vector3 dir)
    {
        dir *= speed;
        //rb.velocity = dir;
        transform.position += dir;
    }
    public void PushAround()
    {
        //push mechanic
        //play push animation
    }
    [PunRPC]
    public void Dead()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
