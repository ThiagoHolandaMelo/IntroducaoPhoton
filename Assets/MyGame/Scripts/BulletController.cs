using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletController : MonoBehaviour
{

    public float bulletSpeed = 100f;

    Rigidbody2D rigidBD2D;
    PhotonView photonView;

    public float bulletTimeLife = 5f;
    float bulletTimeCount = 0f;

    public float bulletDamage = 10f;

    // Use this for initialization
    void Start()
    {
        rigidBD2D = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();

        rigidBD2D.AddForce(transform.up * bulletSpeed, ForceMode2D.Force);
    }

    // Update is called once per frame
    void Update()
    {

        if (bulletTimeCount >= bulletTimeLife)
        {
            Destroy(this.gameObject);
        }

        bulletTimeCount += Time.deltaTime;
        
    }

    [PunRPC]
    void BulletDestroy()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetComponent<PlayerController>() && collision.GetComponent<PhotonView>().IsMine)
        {
            Debug.Log("PlayerID: " + collision.GetComponent<PhotonView>().Owner.ActorNumber + " PlayerName: " + collision.GetComponent<PhotonView>().Owner.NickName);

            collision.GetComponent<PlayerController>().TakeDamage(-bulletDamage, collision.GetComponent<PhotonView>().Owner); //, GetComponent<PhotonView>().Owner);

            if (this.GetComponent<PhotonView>() != null && collision.GetComponent<PhotonView>() != null)
            {
                this.GetComponent<PhotonView>().RPC("BulletDestroy", RpcTarget.AllViaServer);
            }
        }

        Destroy(this.gameObject);
    }

}