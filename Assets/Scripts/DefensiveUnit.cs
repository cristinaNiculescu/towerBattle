using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DefensiveUnit : MonoBehaviour {

	public float HP=500;
	public bool isInConstruction=false;
	public int upgrades=0;
	//public Transform UnitFace;
	public int[] costs= new int[5];
	public int damageAbility1=0;
	public int damageAbility2=0;

	bool started=false;

	public GameObject healthBar;
	Slider HP_Bar;
	BaseManager BaseUnit;
	
	GameObject panel;
	bool canBeClicked;
	bool activeMarker=false;
	string tempName;


	// Use this for initialization
	void Start () {
	
		isInConstruction = true;
		StartCoroutine (waitConstruction ());

		healthBar = GameObject.Find ("HealthBarfor" + gameObject.name);
		HP_Bar = healthBar.GetComponent<Slider> ();
		HP_Bar.minValue = 0;
		HP_Bar.maxValue = 500;
		HP = 500;
		HP_Bar.value = HP;
		GameObject temp=GameObject.Find("Base");
		BaseUnit=temp.GetComponent<BaseManager>();
		
		tempName=gameObject.name.Substring(0,9);
		Debug.Log(tempName);
		panel = GameObject.Find ("BuildPanelfor"+tempName);
		changePanel ();
		panel.SetActive(activeMarker);
	}

	IEnumerator waitConstruction(){
		yield return new WaitForSeconds (20f);
		isInConstruction = false;
	}

	// Update is called once per frame
	void Update () {
		Debug.Log (isInConstruction);
		if (!isInConstruction) 
		{

			if (this.HP <= 0f) {	
				Destroy (gameObject, 0.1f);
				BaseUnit.reCheckShield ();
			}
			
			Debug.Log (gameObject.tag + " " + HP);
			HP_Bar.value = HP;
		}
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
	
	void changePanel()
	{ 	
		Debug.Log ("BuildPanelfor" + tempName + "/buildAtck");
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

	}

}
