using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpecialUnit : MonoBehaviour {

	UnitStructure structure;

	public int[] costs= new int[5];
	public int damageAbility1=0;
	public int damageAbility2=0;
	
	bool started=false;
	bool canBeClicked;
	bool activeMarker=false;
	string tempName;

	// Use this for initialization
	void Start () {
	
		structure = this.GetComponent<UnitStructure> ();
		structure.HP = 500;
		structure.HPMax = 500;
		structure.colorUnit = gameObject.GetComponent<Renderer>().material.color;
		structure.isInConstruction = true;
		StartCoroutine (structure.waitConstruction (20f,structure.colorUnit));
		
		structure.healthBar = GameObject.Find ("HealthBarfor" + gameObject.name);
		structure.HP_Bar = structure.healthBar.GetComponent<Slider> ();
		structure.HP_Bar.minValue = 0;
		structure.HP_Bar.maxValue = 500;
		structure.HP_Bar.value = structure.HP;
		GameObject temp=GameObject.Find("Base");
		structure.BaseUnit=temp.GetComponent<BaseManager>();
		
		tempName=gameObject.name.Substring(0,9);
		Debug.Log(tempName);
		structure.panel = GameObject.Find ("BuildPanelfor"+tempName);
		changePanel ();
		structure.panel.SetActive(activeMarker);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseEnter(){
		canBeClicked = true;
	}
	void OnMouseExit(){
		canBeClicked = false;
	}
	
	void OnMouseUp()
	{	if (canBeClicked) {
			structure.panel.SetActive (activeMarker);
			activeMarker = !activeMarker;
			//Debug.Log (activeMarker);
		}
	}

	void changePanel()
	{ 	
		//	Debug.Log ("BuildPanelfor" + tempName + "/buildAtck");
		GameObject tempOBj = GameObject.Find ("BuildPanelfor" + tempName + "/Text");
		Text panelTitle = tempOBj.GetComponent<Text> ();
		panelTitle.text = "Abilities";
		
		tempOBj= GameObject.Find("BuildPanelfor"+tempName+"/BuildDef");
		Button btn = tempOBj.GetComponent<Button> ();
		Text btnText = btn.GetComponentInChildren<Text> ();
		btnText.text = "Repair Unit";
		btn.onClick.AddListener (() => repair());
		
		tempOBj= GameObject.Find("BuildPanelfor"+tempName+"/buildAtck");
		Button btn3 = tempOBj.GetComponent<Button> ();
		Text btn3Text = btn.GetComponentInChildren<Text> ();
		btn3Text.text = "Send Scout";
		btn3.onClick.AddListener (() => sendScout());
		
		tempOBj= GameObject.Find("BuildPanelfor"+tempName+"/BuildSpec");
		Button btn2 = tempOBj.GetComponent<Button> ();
		Text btn2text = btn2.GetComponentInChildren<Text> ();
		btn2text.text = "Upgrade Unit";
		btn2.onClick.AddListener (() => upgrade());
	}

	void repair(){
		activeMarker = false;
		structure.panel.SetActive(activeMarker);
		Debug.Log ("sent repair");
	}

	void upgrade(){
		activeMarker = false;
		structure.panel.SetActive(activeMarker);
		Debug.Log ("sent upgrade");
	}

	void sendScout(){
		activeMarker = false;
		structure.panel.SetActive(activeMarker);
		Debug.Log ("scout out");
	}
}
