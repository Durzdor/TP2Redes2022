using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerModel : MonoBehaviourPun
{
    [SerializeField] float speed;
    [SerializeField] float impactForce;
    Rigidbody2D rb;
    Animation anim;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animation>();
    }
    public void Move(Vector3 dir)
    {
        dir *= speed;
        transform.position += dir * Time.deltaTime;
    }
    public void ShockWave()
    {
        //push mechanic
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, .81f);
        foreach (Collider2D hitObject in hits)
        {
            if (hitObject.gameObject.layer == 6)
            {
                var dir = hitObject.transform.position - transform.position;
                var objRb = hitObject.GetComponent<Rigidbody2D>();
                objRb.AddForce(new Vector2(dir.x, dir.y) * impactForce, ForceMode2D.Impulse);
                Debug.Log($"Hitted {hitObject.name}");
            }
        }
        photonView.RPC("ShockWaveAnimation", RpcTarget.All);
    }
    [PunRPC]
    public void Dead()
    {
        PhotonNetwork.Destroy(gameObject);
    }
    [PunRPC]
    public void ShockWaveAnimation()
    {
        anim.Play();
    }
}
