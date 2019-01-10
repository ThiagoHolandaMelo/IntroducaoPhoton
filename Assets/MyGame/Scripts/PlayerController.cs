using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

	public float playerSpeed = 5f;
	
	Rigidbody2D rigidbody2D;
	PhotonView photonView;
	
	[Header("Health")]
	public float playerHealthMax = 100;
	float playerHealthCurrent;
	public Image playerHealthFill;
	
	[Header("Bullet")]
	public GameObject bulletGO; // Bala que vai ser processada localmente em cada player
	public GameObject spawnBullet;	// Local onde vai ser criado as balas 
	public GameObject bulletPhotonView;
	
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();

        HealthManager( playerHealthMax );
    }

    // Update is called once per frame
    void Update()
    {
		if( photonView.IsMine ){
			PlayerMove();
			PlayerTurn();
            Shooting();
        }

        

        /*
        if (Input.GetMouseButton(0)){
			HealthManager(-10f);
		}
        */

    }
	
	void PlayerMove(){
		var x = Input.GetAxis("Horizontal");
		var y = Input.GetAxis("Vertical");
		
		rigidbody2D.velocity = new Vector2(x, y);
	}
	
	void PlayerTurn(){
		Vector3 mousePosition = Input.mousePosition;
		
		mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
		
		Vector2 direction = new Vector2(
			mousePosition.x - transform.position.x,
			mousePosition.y - transform.position.y
		);
		
		transform.up = direction;
	}
	
	void HealthManager(float value){
		playerHealthCurrent += value;
		playerHealthFill.fillAmount = playerHealthCurrent/100f;
		
		if( playerHealthCurrent > playerHealthMax){
			playerHealthCurrent = playerHealthMax;
		}
		
		Debug.Log("Passei aqui");
	}
	
	void Shooting(){
		
		//Botão Esquerdo
		if( Input.GetMouseButtonDown(0) ){
			Instantiate( bulletGO, spawnBullet.transform.position, spawnBullet.transform.rotation);
		}
		
		//Botão Direito
		if( Input.GetMouseButtonDown(1) ){
            PhotonNetwork.Instantiate(bulletPhotonView.name, spawnBullet.transform.position, spawnBullet.transform.rotation, 0);
        }
	}
}
