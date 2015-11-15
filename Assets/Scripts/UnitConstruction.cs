﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class UnitConstruction : MonoBehaviour {

	public GameObject cs;
	bool activeMarker=false;
	Vector3 tempPosition;
	GameObject panel;
	GameObject hpbar;
	bool canBeClicked;
	BaseManager BaseUnit;

	// Use this for initialization
	void Start () {
		if (cs != null) {
			panel = GameObject.Find ("BuildPanelfor"+gameObject.name);
			panel.SetActive (false);
			hpbar=GameObject.Find("HealthBarfor"+gameObject.name+"(Clone)");
			hpbar.SetActive(false);
			GameObject temp=GameObject.Find("Base");
			BaseUnit=temp.GetComponent<BaseManager>();
		}
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnMouseEnter(){
		canBeClicked = true;
	}
	void OnMouseExit(){
		canBeClicked = false;
	}

	void OnMouseUp()
	{	if (canBeClicked) {
			panel.SetActive (activeMarker);
			activeMarker = !activeMarker;
			//Debug.Log (activeMarker);
		}
	}

	public void build(Transform unit)
	{
		unit.name = gameObject.name;
		int index = (int)(gameObject.name[gameObject.name.Length - 1]);
		//Debug.Log (gameObject.name[gameObject.name.Length - 1]+","+index);
		hpbar.SetActive (true);
		BaseUnit.UnitsBuilt[index-49]=unit;
		BaseUnit.reCheckShield ();
		Instantiate (unit, gameObject.transform.position, Quaternion.identity);
		//panel.SetActive (false);
		Destroy(gameObject);
	}

}
