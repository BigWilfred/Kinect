using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightHandController : MonoBehaviour {

	private Vector3 mousePos;
	public DragAndDrop drop;

	// Use this for initialization
	void Start () {
	}



	// Update is called once per frame
	void Update () {
		mousePos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y,1f));
		transform.position = mousePos;

		if (HasInput) {
			drop.DragOrPickUp ();
		} else {
			if (drop.getDrag()) {
				drop.DropItem();
			}
		}
	}

	private bool HasInput
	{
		get
		{
			// returns true if either the mouse button is down or at least one touch is felt on the screen
			return Input.GetMouseButton(0);
		}
	}
		
}
