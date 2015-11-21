using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class UnitConstruction : NetworkBehaviour {

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
		int constructionCost = unit.GetComponent<UnitStructure>().costs[0];
		if (BaseManager.resources - constructionCost >= 0) 
		{
			hpbar.SetActive (true);
			BaseUnit.UnitsBuilt [index - 49] = unit;

			BaseUnit.reCheckShield ();
			unit.transform.LookAt (GameObject.FindWithTag ("Enemy").transform.position);
            GameObject myUnit = (GameObject)Instantiate(unit.gameObject, gameObject.transform.position, Quaternion.identity);
            CmdBuildUnit(myUnit);
            Destroy(gameObject);
		}

	}

    [Command]
    void CmdBuildUnit(GameObject unit)
    {
        NetworkServer.Spawn(unit);
    }

}
