﻿using UnityEngine;
using System.Collections;
using System.Net.Sockets;

public class Gun : MonoBehaviour
{
	public Rigidbody2D rocket;				// Prefab of the rocket.
	public float speed = 20f;				// The speed the rocket will fire at.


	private PlayerControl playerCtrl;		// Reference to the PlayerControl script.
	private Animator anim;					// Reference to the Animator component.

    private Socket clientSocket;

    void Awake()
	{
		//获取socket对象
        GameObject netObj = GameObject.FindGameObjectWithTag("netSocket");
        clientSocket = netObj.GetComponent<NetWorks>().getClientSocket();
        // Setting up the references.
        anim = transform.root.gameObject.GetComponent<Animator>();
		playerCtrl = transform.root.GetComponent<PlayerControl>();
	}


	void Update ()
	{
		// If the fire button is pressed...
		if(Input.GetButtonDown("Fire1"))
		{
            byte[] sendBuff = new byte[1024];
			//发送fire协议
            sendBuff = System.Text.Encoding.Default.GetBytes("Fire ");
            clientSocket.Send(sendBuff);
            // ... set the animator Shoot trigger parameter and play the audioclip.
            anim.SetTrigger("Shoot");
			GetComponent<AudioSource>().Play();

			// If the player is facing right...
			if(playerCtrl.facingRight)
			{
				// ... instantiate the rocket facing right and set it's velocity to the right. 
				Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0,0,0))) as Rigidbody2D;
				bulletInstance.velocity = new Vector2(speed, 0);
			}
			else
			{
				// Otherwise instantiate the rocket facing left and set it's velocity to the left.
				Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0,0,180f))) as Rigidbody2D;
				bulletInstance.velocity = new Vector2(-speed, 0);
			}
		}
	}
}
