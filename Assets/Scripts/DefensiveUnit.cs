using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DefensiveUnit : MonoBehaviour{

	//public Transform UnitFace;
	public int[] costs= new int[5];
	public int damageAbility1=0;
	public int damageAbility2=0;

	bool started=false;
	bool canBeClicked;
	bool activeMarker=false;
	string tempName;

	UnitStructure structure;

	// Use this for initialization
	void Start () {
		structure = this.GetComponent<UnitStructure> ();
		structure.HP = 500;
		structure.HPMax = 500;
		attributeCosts ();
		//Debug.Log (structure.costs [0]);
		structure.colorUnit = gameObject.GetComponent<Renderer>().material.color;
		structure.isInConstruction = true;
		StartCoroutine (structure.waitConstruction (20f,structure.colorUnit));
		BaseManager.resources -= structure.costs [0];

		structure.healthBar = GameObject.Find ("HealthBarfor" + gameObject.name);
		structure.HP_Bar = structure.healthBar.GetComponent<Slider> ();
		structure.HP_Bar.minValue = 0;
		structure.HP_Bar.maxValue = structure.HPMax;
		structure.HP_Bar.value = structure.HP;

		structure.name = "Defensive Unit";
		GameObject temp=GameObject.Find("Base");
		structure.BaseUnit=temp.GetComponent<BaseManager>();
		
		tempName=gameObject.name.Substring(0,9);
		//Debug.Log(tempName);
		structure.panel = GameObject.Find ("BuildPanelfor"+tempName);
		changePanel ();
		structure.panel.SetActive(activeMarker);


	}
	

	// Update is called once per frame
	void Update () {
		if (!structure.isInConstruction && !structure.isUnderRepair) 
		{

			if (structure.HP <= 0f) {	
				Destroy (gameObject, 0.1f);
				structure.BaseUnit.reCheckShield ();
			}
			
		//	Debug.Log (gameObject.tag + " " + structure.HP);
			structure.HP_Bar.value = structure.HP;
		}

		structure.status = status ();
	}
	

	void OnMouseEnter(){
		if (!structure.isUnderRepair && !UnitStructure.TeamLookingForTarget)
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
		btnText.text = "Set Cloud";
		btn.onClick.AddListener (() => setCloud ());
		
		tempOBj= GameObject.Find("BuildPanelfor"+tempName+"/buildAtck");
		tempOBj.SetActive (false);
		
		tempOBj= GameObject.Find("BuildPanelfor"+tempName+"/BuildSpec");
		Button btn2 = tempOBj.GetComponent<Button> ();
		Text btn2text = btn2.GetComponentInChildren<Text> ();
		btn2text.text = "Throw Rock";
		btn2.onClick.AddListener (() => throwRock ());
	}

	/// <summary>
	/// Sets the cloud.
	/// one cloud lasts 10 sec, the ability has 2 charges and 30 sec cool-down. 
	/// The clouds form like little tornado's and can be used to mask portions (60 units radius) 
	/// of the map above friendly units. The cast sets the center of the cloud.  
	/// This will prevent enemy units from targeting them.
	/// </summary>
	void setCloud(){
		activeMarker = false;
		structure.panel.SetActive(activeMarker);
		Debug.Log ("cloud is above");
	}

	/// <summary>
	/// Throws the rock.
	/// the rock is thrown at a certain target and makes a little bit of mess :  
	/// 20% damage of max health of the enemy unit it hits and creates a cloud of 
	/// dust around the same that lasts for 10 sec.  
	/// The cloud temporarily incapacitates the enemy unit. 
	/// The ability has 1 min cool down
	/// </summary>
	void throwRock()
	{
		activeMarker = false;
		structure.panel.SetActive(activeMarker);
		Debug.Log ("threw rock");
	}

	void attributeCosts(){
		structure.costs [0] = 40;
		structure.costs [1] = 15;
		structure.costs [2] = 35;
		structure.costs [3] = 100;
		structure.costs [4] = 225;
	}

	public void upgrade(){
		Debug.Log ("upgrading defensive");
	}


	public string status(){
		return "status";
	}

}
