using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletController : MonoBehaviour
{

    public float bulletSpeed = 100f;
    Rigidbody2D rigidBD2D;

    public float bulletTimeLife = 5f;
    float bulletTimeCount;

    public float bulletDamage = 10f;

    // Use this for initialization
    void Start()
    {
        rigidBD2D = GetComponent<Rigidbody2D>();

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
            //collision.GetComponent<PlayerController>().TakeDamage(-bulletDamage, GetComponent<PhotonView>().Owner);

            this.GetComponent<PhotonView>().RPC("BulletDestroy", RpcTarget.AllViaServer);
        }

        Destroy(this.gameObject);

    }



}