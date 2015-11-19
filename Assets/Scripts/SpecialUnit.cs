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

	RaycastHit hit; 
	Ray ray; 
	Transform target;
	bool repairDeployedTeam=false;
	bool repairReady=true;
	bool upgradeReady=true;
	bool upgradeDeployedTeam=false;

	float gathered;

	public GameObject[] resourceFields;
	// Use this for initialization
	void Start () {
		resourceFields = new GameObject[3];
		resourceFields = GameObject.FindGameObjectsWithTag("resource");

		structure = this.GetComponent<UnitStructure> ();
		structure.HP = 200;
		structure.HPMax = 200;
		attributeCosts ();
		structure.colorUnit = gameObject.GetComponent<Renderer>().material.color;
		structure.isInConstruction = true;
		StartCoroutine (structure.waitConstruction (20f,structure.colorUnit));
		BaseManager.resources -= structure.costs [0];
		
		structure.healthBar = GameObject.Find ("HealthBarfor" + gameObject.name);
		structure.HP_Bar = structure.healthBar.GetComponent<Slider> ();
		structure.HP_Bar.minValue = 0;
		structure.HP_Bar.maxValue = structure.HPMax;
		structure.HP_Bar.value = structure.HP;

		structure.name = "Special Unit";
		GameObject temp=GameObject.Find("Base");
		structure.BaseUnit=temp.GetComponent<BaseManager>();

		tempName=gameObject.name.Substring(0,9);
		//Debug.Log(tempName);
		structure.panel = GameObject.Find ("BuildPanelfor"+tempName);
		changePanel ();
		structure.panel.SetActive(activeMarker);
		gatherRescources ();


	}


	// Update is called once per frame
	void Update () 
	{
		if (!structure.isInConstruction && !structure.isUnderRepair) {
			if (repairDeployedTeam && Input.GetMouseButtonUp (0)) {
				ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
				//Debug.Log(Physics.Raycast (ray, out hit, 10000f)+" "+ray);
				if (Physics.Raycast (ray, out hit, 10000f)) {
					//Debug.Log (hit.transform.tag);
				
					if (hit.transform.tag == "attacking" || 
						hit.transform.tag == "defense" || 
						hit.transform.tag == "special") {
						target = hit.transform;
						Debug.Log(hit.transform.name+" "+hit.transform.tag+" is under repair");
						target.GetComponent<UnitStructure> ().isUnderRepair = true;
						StartCoroutine (repairDeployed (target));
						repairDeployedTeam = false;
						repairReady = false;
					}		
				}
			}

			if (upgradeDeployedTeam && Input.GetMouseButtonUp (0)) {
				ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
				//Debug.Log(Physics.Raycast (ray, out hit, 10000f)+" "+ray);
				if (Physics.Raycast (ray, out hit, 10000f)) {
					//Debug.Log (hit.transform.tag);
					
					if (hit.transform.tag == "attacking" || 
					    hit.transform.tag == "defense" || 
					    hit.transform.tag == "special") {
						target = hit.transform;
						Debug.Log(hit.transform.name+" "+hit.transform.tag+" is under upgrade");
						target.GetComponent<UnitStructure> ().isUnderRepair = true;
						StartCoroutine (upgradeDeployed (target.transform));
						upgradeDeployedTeam = false;
						upgradeReady = false;
					}		
				}
			}
		}

		structure.status = status ();
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

		tempOBj= GameObject.Find("BuildPanelfor"+tempName+"/buildAtck");
		Button btn3 = tempOBj.GetComponent<Button> ();
		Text btn3Text = btn3.GetComponentInChildren<Text> ();
		btn3Text.text = "Send Scout";
		btn3.onClick.AddListener (() => sendScout());

		tempOBj= GameObject.Find("BuildPanelfor"+tempName+"/BuildDef");
		Button btn = tempOBj.GetComponent<Button> ();
		Text btnText = btn.GetComponentInChildren<Text> ();
		btnText.text = "Repair Unit";
		btn.onClick.AddListener (() => repair());

		tempOBj= GameObject.Find("BuildPanelfor"+tempName+"/BuildSpec");
		Button btn2 = tempOBj.GetComponent<Button> ();
		Text btn2text = btn2.GetComponentInChildren<Text> ();
		btn2text.text = "Upgrade Unit";
		btn2.onClick.AddListener (() => upgradeUnits());
	}

	void repair()
	{	
		if (repairReady) {
			repairDeployedTeam = true;
			UnitStructure.TeamLookingForTarget = true;
		} else
			Debug.Log ("repairs not ready yet");

		activeMarker = false;
		structure.panel.SetActive (activeMarker);
		Debug.Log ("sent repair");
	}

		IEnumerator repairDeployed(Transform target)
	{	
		UnitStructure.TeamLookingForTarget = false;
		//Debug.Log ("started cor");
		yield return new WaitForSeconds (20f);
			if (target) 
			{
				target.GetComponent<UnitStructure>().isUnderRepair = false;
				float repairedHealth =target.GetComponent<UnitStructure> ().HP * 1.2f;	
				if (repairedHealth > target.GetComponent<UnitStructure> ().HPMax)
					target.GetComponent<UnitStructure> ().HP=target.GetComponent<UnitStructure> ().HPMax;
				else target.GetComponent<UnitStructure> ().HP=repairedHealth;
			}
			
			yield return new WaitForSeconds (20f);
			repairReady =true;


	}
		

	void upgradeUnits()
	{
		
		if (upgradeReady) {
			upgradeDeployedTeam = true;
			UnitStructure.TeamLookingForTarget = true;
		} else
			Debug.Log ("upgrades not ready yet");
		activeMarker = false;
		structure.panel.SetActive(activeMarker);
		//Debug.Log ("sent upgrade");
	}

	void gatherRescources(){
		gathered=0f;
		for (int i=0; i<resourceFields.Length; i++)
			gathered = resourceFields [i].GetComponent<ResourceField> ().speed;
		StartCoroutine (addResources ());

	}
	IEnumerator addResources(){
		yield return new WaitForSeconds (1f);
		BaseManager.resources+= (int)gathered;
		gatherRescources ();
	}

	IEnumerator upgradeDeployed(Transform target)
	{	
		UnitStructure.TeamLookingForTarget = false;
		//Debug.Log ("started cor");

		if (target) 
		{
			target.GetComponent<UnitStructure>().isUnderRepair = false;
			Debug.Log("in cor: "+target.tag);
			switch (target.tag)
			{
			case "attacking": target.GetComponent<AttackingUnit>().upgrade();
				break;
			case "defense": target.GetComponent<DefensiveUnit>().upgrade();
				break;
			case "special": target.GetComponent<SpecialUnit>().upgrade();
				break;
			}
		}
		yield return new WaitForSeconds (60f);
		upgradeReady =true;
		Debug.Log ("done upgrading");
		
	}
	void sendScout(){
		activeMarker = false;
		structure.panel.SetActive(activeMarker);
		Debug.Log ("scout out");
	}

	void attributeCosts(){
		structure.costs [0] = 20; //to build
		structure.costs [1] = 15; //to repair stuff + 10 % cost 1,3,4
		structure.costs [2] = 0; //upgrading costs for this unit
		structure.costs [3] = 60; // +0.25/sec  - send scout on mission
		structure.costs [4] = 60; //revive scout
	}
	public void upgrade()
	{
		Debug.Log ("upgrading special");
	}

	public string status(){
		return "status";
	}

}
