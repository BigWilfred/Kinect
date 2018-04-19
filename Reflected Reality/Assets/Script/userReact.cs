using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class userReact : MonoBehaviour {

	private Animator anim;
	private bool collisionObject;

	// Use this for initialization
	void Start () {
		anim = GetComponentInParent<Animator> ();
		collisionObject = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/*
	void OnTriggerEnter2D (Collider2D col)
	{
		if (collisionObject == false) {
			collisionObject = true;
			anim.Play (name);
		}
	}

	void OnTriggerExit2D (Collider2D col)
	{
			collisionObject = false;
			anim.Play ("userIdle");
	}*/

		
}
