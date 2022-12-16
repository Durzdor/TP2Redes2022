using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerModel : MonoBehaviourPun
{
    [SerializeField] private float speed;
    [SerializeField] private float impactForce;
    private Rigidbody2D rb;
    private Animation anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animation>();
    }

    public void Move(Vector3 dir)
    {
        //dir *= speed;
        dir = Vector3.ClampMagnitude(dir, 1);
        transform.position += dir * (speed * Time.deltaTime);
    }

    public void ShockWave()
    {
        //push mechanic
        var hits = Physics2D.OverlapCircleAll(transform.position, .81f);
        foreach (var hitObject in hits)
            if (hitObject.gameObject.layer == 6)
            {
                var dir = hitObject.transform.position - transform.position;
                var objRb = hitObject.GetComponent<BallModel>();
                objRb.photonView.RPC("GettingShockWaved", RpcTarget.MasterClient, dir, impactForce);
                Debug.Log($"Hitted {hitObject.name}");
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

    public void ChangeSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void ChangeImpact(float newImpact)
    {
        impactForce = newImpact;
    }
}