﻿using UnityEngine;
using System.Collections;

public class Looking : MonoBehaviour {

	public GUIText reticule;

	public Color neutral;
	public Color select;
	public Color holding;

	public float holdDistance = 1f;

	public GameObject blueprint;

	bool holdingItem = false;
	GameObject itemHolding;

	float throwforce;
	float holdtimer;
	
	// Update is called once per frame
	void Update () {
		Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
		RaycastHit hit;
		if (holdingItem){
			reticule.color = holding;
			reticule.fontSize = 15;
			holdtimer += Time.deltaTime;
			ScaleHeldThing();
			if (holdtimer > .25f){
				if (Input.GetMouseButton(0)){
					throwforce += Time.deltaTime * 50f;
					throwforce = Mathf.Clamp(throwforce, 0f, 75f);
				}else if (Input.GetMouseButtonUp(0)){
					if (throwforce < 7f) throwforce = 0f;
					itemHolding.rigidbody.velocity += Camera.main.transform.forward * throwforce;
					DropThing();
				}
			}else{
				if (Input.GetMouseButtonDown(0)){
					DropThing();
				}
			}

			if (Input.GetMouseButtonDown(1)){
				Destroy(itemHolding);
				DropThing();
			}

		}else if (Physics.Raycast(ray, out hit, 5f, 1 << 8)){
			reticule.color = select;
			reticule.fontSize = 25;
			if (Input.GetMouseButtonDown(0)){
				PickupThing(hit.collider.gameObject);
				holdtimer = 0;
			}else if (Input.GetMouseButtonDown(1)){
				Destroy(hit.collider.gameObject);
			}
		}else{
			reticule.color = neutral;
			reticule.fontSize = 20;
			if (Input.GetMouseButtonDown(1)){
				MakeNewThing();
			}
		}
	}

	void FixedUpdate(){
		if (holdingItem) HoldThing();
	}

	void PickupThing(GameObject g){
		itemHolding = g;
		holdingItem = true;
	}

	void DropThing(){
		itemHolding = null;
		holdingItem = false;
		throwforce = 0f;
	}

	void HoldThing(){
		Vector3 targetPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
		targetPoint += Camera.main.transform.forward * holdDistance + GetComponent<CharacterController>().velocity.normalized;

		Vector3 force = targetPoint - itemHolding.transform.position;

		itemHolding.rigidbody.AddForce(force * 100f);

		itemHolding.rigidbody.velocity *= Mathf.Min(1.0f, force.magnitude / 2f);
	
	}

	void MakeNewThing(){
		GameObject newthing = Instantiate(blueprint, transform.position + Vector3.up * 2f, Quaternion.identity) as GameObject;

		PickupThing(newthing);
	}

	void ScaleHeldThing(){
		itemHolding.transform.localScale += Vector3.one * (Input.GetAxis("Mouse ScrollWheel") + Input.GetAxis("Alt Scroll"));
	}

	void OnControllerColliderHit(ControllerColliderHit hit){
		Rigidbody r = hit.rigidbody;
		
		if (r == null || r.isKinematic) { return; }
		if (hit.moveDirection.y < -0.3) { return; }
		
		Vector3 push = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
		
		r.velocity += push * 2f;
	}
}
