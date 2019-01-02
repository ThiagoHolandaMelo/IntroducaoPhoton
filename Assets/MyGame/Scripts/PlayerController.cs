using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	public float playerSpeed = 5f;
	
	Rigidbody2D rigidbody2D;
	
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
		PlayerTurn();
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
}
