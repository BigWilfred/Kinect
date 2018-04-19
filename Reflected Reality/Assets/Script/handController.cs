using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handController : MonoBehaviour {

	private Animator userAnim;
	private Collider2D collisionObject;
	private GameObject item;
	private Vector3 touchOffset;
	private Vector3 mousePos;
	private bool move; 
	private float smoothSpeed = 0.5f;
	private float speed = 2f;
	private float maxRotation = 45f;
	private Vector3 lastPosition = Vector3.zero;
	private Quaternion originalRotation;
	private GameObject rotate;
	private bool restoreRotation = false;
	private float startTime = 0f;
	private float endTime;

	// Use this for initialization
	void Start () {
		rotate = GameObject.Find("rotationPoint");
		originalRotation = rotate.transform.rotation;
		collisionObject = null;
		move = false;
		if (GameObject.Find ("PersonInNeed") != null) {
			userAnim = GameObject.Find ("PersonInNeed").GetComponent<Animator>();
		} 
	}



	// Update is called once per frame
	void Update () {
		mousePos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y,1f));
		transform.position = mousePos;
		Debug.Log (restoreRotation);
		if (move == true) {
			rotate.transform.position = mousePos;
			rotate.transform.rotation = Quaternion.Euler (0f, 0f, maxRotation * Mathf.Sin (Time.time * speed));
			/*
			rotate.transform.position = mousePos;
			InvokeRepeating ("checkSpeed", 2f, 2f);
			if (restoreRotation) {
				float time = setTime ();
				rotate.transform.rotation = Quaternion.Lerp (rotate.transform.rotation, originalRotation, time);
				if (rotate.transform.rotation == originalRotation) {
					restoreRotation = false;
					maxRotation = 0f;
					startTime = 0f;
				}
			}else if (!restoreRotation && lastPosition!= transform.position){
				rotate.transform.rotation = Quaternion.Euler (0f, 0f, maxRotation * Mathf.Sin (Time.time * speed));
			}
			lastPosition = transform.position;
			*/
	}
	}

	float setTime(){
		float rate = 1f/10f;
		if (startTime < 1.0) {
			startTime += rate+0.1f;
		} 
		return startTime;
	}

	void checkSpeed(){
		if (lastPosition == transform.position && restoreRotation == false) {
			restoreRotation = true;
		}
	}


	void OnTriggerEnter2D (Collider2D col)
	{
		if (userAnim != null && (collisionObject == null || col != collisionObject)) {
			collisionObject = col;
			userAnim.Play (col.name);
		} else if (col.gameObject.name == "rotationPoint" && move == false) {
			move = true;
		}
	}

	void OnTriggerExit2D (Collider2D col)
	{
		if (collisionObject == col && userAnim != null) {
			collisionObject = null;
			userAnim.Play ("userIdle");
		}
	}
		
}
