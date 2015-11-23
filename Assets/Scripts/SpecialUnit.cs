using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpecialUnit : MonoBehaviour
{
	
	UnitStructure structure;
	bool started = false;
	bool canBeClicked;
	bool activeMarker = false;
	string tempName;
	RaycastHit hit;
	Ray ray;
	Transform target;
	bool repairDeployedTeam = false;
	bool repairReady = true;
	bool upgradeReady = true;
	bool upgradeDeployedTeam = false;
	float gathered;
	public float lastGathered;
	bool startedGathering = false;
	float upgradeDuration;
	float repDur;
	float repCD;
	public GameObject[] resourceFields;
	
	bool scoutReady=true;
	Vector3 origin;
	bool scoutTriggered=false;
	Vector3 newPosition;
	float scoutDistance;
	public GameObject scout;
	Coroutine sc;
	int scoutCurrentStep=0;
	int steps=0;
	bool scisRunning=false;
	float timeScoutSpawned;
	float durationBetweenSteps;
	GameObject scoutInstantiated;

	List <Vector3> positions=new List<Vector3>();
	string message;
	Vector3 newPositionForScout;
	// Use this for initialization
	void Start()
	{
		resourceFields = new GameObject[3];
		resourceFields = GameObject.FindGameObjectsWithTag("resource");
		
		structure = this.GetComponent<UnitStructure>();
		structure.HP = 200;
		structure.HPMax = 200;
		attributeCosts();
		structure.colorUnit = gameObject.GetComponent<Renderer>().material.color;
		structure.isInConstruction = true;
		structure.statusUpdater = status();
//		Debug.Log(structure.statusUpdater);
		StartCoroutine(structure.waitConstruction(20f, structure.colorUnit));
		BaseManager.resources -= structure.costs [0];
		
		structure.healthBar = GameObject.Find("HealthBarfor" + gameObject.name);
		structure.HP_Bar = structure.healthBar.GetComponent<Slider>();
		structure.HP_Bar.minValue = 0;
		structure.HP_Bar.maxValue = structure.HPMax;
		structure.HP_Bar.value = structure.HP;
		
		structure.name = "Special Unit";
		GameObject temp = GameObject.Find("Base");
		structure.BaseUnit = temp.GetComponent<BaseManager>();
		
		tempName = gameObject.name.Substring(0, 9);
		//Debug.Log(tempName);
		structure.panel = GameObject.Find("BuildPanelfor" + tempName);
		changePanel();
		structure.panel.SetActive(activeMarker);
		
		upgradeDuration = 30f;
		repDur = 30f;
		repCD = 60f;
	}
	
	
	// Update is called once per frame
	void Update()
	{
		if (!structure.isInConstruction && !structure.isUnderRepair && !structure.isDisoriented)
		{
			if (repairDeployedTeam && Input.GetMouseButtonUp(0))
			{
				ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
				//Debug.Log(Physics.Raycast (ray, out hit, 10000f)+" "+ray);
				if (Physics.Raycast(ray, out hit, 10000f))
				{
					//Debug.Log (hit.transform.tag);
					
					if (hit.transform.tag == "attacking" || 
					    hit.transform.tag == "defense" || 
					    hit.transform.tag == "special")
					{   target = hit.transform;
						float repairCost=target.GetComponent<UnitStructure>().costs[0];
						if (target.GetComponent<UnitStructure>().upgrades==2)
							repairCost+=target.GetComponent<UnitStructure>().costs[3]+target.GetComponent<UnitStructure>().costs[4];
						else if (target.GetComponent<UnitStructure>().upgrades==1)
							repairCost+=target.GetComponent<UnitStructure>().costs[3];
						repairCost*=0.1f;
						
						if(BaseManager.resources-repairCost>=0)
						{
							BaseManager.resources-=(int)repairCost;
							BaseManager.notEnough=" ";
							
							//Debug.Log(hit.transform.name + " " + hit.transform.tag + " is under repair");
							target.GetComponent<UnitStructure>().isUnderRepair = true;
							StartCoroutine(repairDeployed(target, repDur, repCD));
							repairDeployedTeam = false;
							repairReady = false;
						}   
						else  BaseManager.notEnough="not enough resources";
					}  
				}
			}
			
			if (upgradeDeployedTeam && Input.GetMouseButtonUp(0))
			{
				ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
				//Debug.Log(Physics.Raycast (ray, out hit, 10000f)+" "+ray);
				if (Physics.Raycast(ray, out hit, 10000f))
				{
					//Debug.Log (hit.transform.tag);
					
					if (hit.transform.tag == "attacking" || 
					    hit.transform.tag == "defense" || 
					    hit.transform.tag == "special")
					{
						target = hit.transform;
						//Debug.Log(hit.transform.name + " " + hit.transform.tag + " is under upgrade");
						target.GetComponent<UnitStructure>().isUnderRepair = true;
						StartCoroutine(upgradeDeployed(target.transform));
						upgradeDeployedTeam = false;
						upgradeReady = false;
					}       
				}
			}
			
			if ( scoutTriggered && Input.GetMouseButtonDown(0))
			{
				Vector3 positionAtClickInWorld=new Vector3(Input.mousePosition.x,Screen.height-Input.mousePosition.y,Input.mousePosition.z);

				//positionAtClickInWorld=Camera.main.ScreenToWorldPoint(positionAtClickInWorld);
				positions.Add(positionAtClickInWorld);	
			}

			if (scoutTriggered && Input.GetMouseButtonDown(1))
			{
				scoutTriggered=false;
				Debug.Log(scoutDistance);
				//scoutDistance =Mathf.Sqrt(scoutDistance);

				float scoutMissionDuration=scoutDistance/10f;

				int ID =Instantiate(scout,transform.position,Quaternion.identity).GetInstanceID();
				GameObject [] scouts=GameObject.FindGameObjectsWithTag("Scout");
			
				for (int i=0;i<scouts.Length;i++)
					if (scouts[i].GetInstanceID()==ID)
						scoutInstantiated=scouts[i];
				//Debug.Log("ID:"+ scoutInstantiated.GetInstanceID() +" "+ ID);
				timeScoutSpawned=Time.realtimeSinceStartup;
				steps=positions.Count;
				durationBetweenSteps=2*scoutMissionDuration/(float)steps;
				scoutCurrentStep=0;
				Debug.Log("duration:"+scoutMissionDuration);
				newPositionForScout=origin;
				StartCoroutine(scoutOut(scoutMissionDuration));

			}

			if (scisRunning)
			{	
				Debug.Log("coroutine is running");
				if(scoutCurrentStep<=steps)
				{	
					Debug.Log(scoutCurrentStep +" "+ steps + "timeScoutSpawned:" + timeScoutSpawned+"duration betweenSteps:"+durationBetweenSteps);
					float timeToCompare=timeScoutSpawned+durationBetweenSteps*(scoutCurrentStep+1);
					Debug.Log((int)timeToCompare+" "+(int)Time.realtimeSinceStartup);
					scoutInstantiated.transform.position=Vector3.MoveTowards(scoutInstantiated.transform.position, 
					                                                         newPositionForScout, 30f/durationBetweenSteps);

					if ((int)timeToCompare==(int)Time.realtimeSinceStartup)
					{ int j=0;
						foreach(Vector3 v in positions)
						{
							if (scoutCurrentStep==j)
							{// float distanceToCarry=Vector3.SqrMagnitude(scoutInstantiated.transform.position-v);
								newPositionForScout=Camera.main.ScreenToWorldPoint(v);
								newPositionForScout=new Vector3(newPositionForScout.x,newPositionForScout.z,0f);
								Debug.Log("assigned new position" + newPositionForScout);

							}
							j++;
						}
						scoutCurrentStep++;
					}
				}
			}
			if (scoutInstantiated && (positions.Count!=0))
				{	 
				//Debug.Log(scoutInstantiated.transform.position+" "+positions[positions.Count-1]);
				if ((scoutInstantiated.transform.position.x==positions[positions.Count-1].x ) &&
				    (scoutInstantiated.transform.position.z==positions[positions.Count-1].z )
				    )
					Destroy(scoutInstantiated);
							
				if(scisRunning==false)
				{
					Destroy(scoutInstantiated);

				}
				}

			
//			Debug.Log(origin+" "+newPosition);
			
			if (!startedGathering)
			{
				gatherResources();
				startedGathering = true;
			}
			
			
		}
		
		structure.statusUpdater = status();
//		Debug.Log(structure.statusUpdater);
	}
	
	void OnGUI(){
		if (scoutTriggered)
		{
			Debug.Log(origin+" "+newPosition);
			float width = 3.0f;
			Color color = Color.magenta;
			Vector3 previousStep=new Vector3();
			scoutDistance=0f;
			previousStep=origin;
			foreach (Vector3 v in positions)
			{	Drawing.DrawLine(previousStep,v,color,width);
				//Vector3 difference=v-previousStep;
				scoutDistance+=Vector3.Distance(Camera.main.ScreenToWorldPoint(previousStep),Camera.main.ScreenToWorldPoint(v));
				previousStep=v;
			}
			Debug.Log("newly calculated distance:"+scoutDistance);
		}
		if (scisRunning) {
			float width = 3.0f;
			Color color = Color.cyan;
			Vector3 previousStep=origin;
			foreach (Vector3 v in positions) {
				Drawing.DrawLine (previousStep, v, color, width);
				Vector3 difference = v - previousStep;
				//scoutDistance += Vector3.SqrMagnitude (difference);
				previousStep = v;
			}
		}
	}
	
	void OnMouseEnter()
	{
		if (!structure.isInConstruction && !structure.isDisoriented && !structure.isUnderRepair && !UnitStructure.TeamLookingForTarget)
			canBeClicked = true;
	}
	
	void OnMouseExit()
	{
		canBeClicked = false;
	}
	
	void OnMouseUp()
	{
		if (canBeClicked)
		{
			structure.panel.SetActive(activeMarker);
			activeMarker = !activeMarker;
			//Debug.Log (activeMarker);
		}
	}
	
	
	
	void changePanel()
	{   
		//  Debug.Log ("BuildPanelfor" + tempName + "/buildAtck");
		GameObject tempOBj = GameObject.Find("BuildPanelfor" + tempName + "/Text");
		Text panelTitle = tempOBj.GetComponent<Text>();
		panelTitle.text = "Abilities";
		
		tempOBj = GameObject.Find("BuildPanelfor" + tempName + "/buildAtck");
		Button btn3 = tempOBj.GetComponent<Button>();
		Text btn3Text = btn3.GetComponentInChildren<Text>();
		btn3Text.text = "Send Scout";
		btn3.onClick.AddListener(() => sendScout());
		
		tempOBj = GameObject.Find("BuildPanelfor" + tempName + "/BuildDef");
		Button btn = tempOBj.GetComponent<Button>();
		Text btnText = btn.GetComponentInChildren<Text>();
		btnText.text = "Repair Unit";
		btn.onClick.AddListener(() => repair());
		
		tempOBj = GameObject.Find("BuildPanelfor" + tempName + "/BuildSpec");
		Button btn2 = tempOBj.GetComponent<Button>();
		Text btn2text = btn2.GetComponentInChildren<Text>();
		btn2text.text = "Upgrade Unit";
		btn2.onClick.AddListener(() => upgradeUnits());
	}
	
	void repair()
	{   
		if (repairReady)
		{
			repairDeployedTeam = true;
			UnitStructure.TeamLookingForTarget = true;
		} else
			//Debug.Log("repairs not ready yet");
		
		activeMarker = false;
		structure.panel.SetActive(activeMarker);
		//Debug.Log("sent repair");
	}
	
	IEnumerator repairDeployed(Transform target, float repairDuration, float repairCooldown)
	{   
		UnitStructure.TeamLookingForTarget = false;
		
		//Debug.Log("started cor" + target + "" + repairDuration + " " + repairCooldown + " " + target.GetComponent<UnitStructure>().colorUnit);
		
		if (target)
		{   
			StartCoroutine(target.GetComponent<UnitStructure>().waitConstruction(repairDuration,
			                                                                     target.GetComponent<UnitStructure>().colorUnit));
			yield return new WaitForSeconds(repairDuration);
			target.GetComponent<UnitStructure>().isUnderRepair = false;
			float repairedHealth = target.GetComponent<UnitStructure>().HP * 1.2f;  
			if (repairedHealth > target.GetComponent<UnitStructure>().HPMax)
				target.GetComponent<UnitStructure>().HP = target.GetComponent<UnitStructure>().HPMax;
			else
				target.GetComponent<UnitStructure>().HP = repairedHealth;
		}
		
		yield return new WaitForSeconds(repairCooldown - repairDuration);
		repairReady = true;
		
		
	}
	
	void upgradeUnits()
	{
		
		if (upgradeReady)
		{
			upgradeDeployedTeam = true;
			UnitStructure.TeamLookingForTarget = true;
		} else
			//Debug.Log("upgrades not ready yet");
		activeMarker = false;
		structure.panel.SetActive(activeMarker);
		//Debug.Log ("sent upgrade");
	}
	
	IEnumerator upgradeDeployed(Transform target)
	{   
		UnitStructure.TeamLookingForTarget = false;
		//Debug.Log ("started cor");
		
		if (target)
		{
			
			//Debug.Log("in cor: " + target.tag);
			switch (target.tag)
			{
			case "attacking":
				target.GetComponent<AttackingUnit>().upgrade(upgradeDuration);
				break;
			case "defense":
				target.GetComponent<DefensiveUnit>().upgrade(upgradeDuration);
				break;
			case "special":
				target.GetComponent<SpecialUnit>().upgrade(upgradeDuration);
				break;
			}
		}
		yield return new WaitForSeconds(60f);
		upgradeReady = true;
		target.GetComponent<UnitStructure>().isUnderRepair = false;
		//Debug.Log("done upgrading");
		
	}
	
	void gatherResources()
	{
		gathered = 0f;
		for (int i=0; i<resourceFields.Length; i++)
			gathered += resourceFields [i].GetComponent<ResourceField>().speed;
		lastGathered = gathered;
//		Debug.Log(lastGathered);
		StartCoroutine(addResources());
		
	}
	
	IEnumerator addResources()
	{
		yield return new WaitForSeconds(1f);
		BaseManager.resources += (int)gathered;
		gatherResources();
	}
	
	
	void sendScout()
	{
		
		if (scoutReady)
		{
			if (BaseManager.resources - structure.costs [4] >= 0)
			{
				BaseManager.resources -= structure.costs [4];
				BaseManager.notEnough = "";
				scoutTriggered=true;
				positions=new List<Vector3> ();
				//origin = new Vector3(Screen.width-Input.mousePosition.x,Screen.height-Input.mousePosition.y,Input.mousePosition.z);
				origin=Camera.main.WorldToScreenPoint(this.transform.position);
				origin=new Vector3(origin.x,Screen.height-origin.y,origin.z);
				newPositionForScout=origin;
				positions.Add(origin);
				scoutReady = false;
				activeMarker = false;
				structure.panel.SetActive(activeMarker);
			//	Debug.Log("scout out");
			} else
				BaseManager.notEnough = "not enough resources";
		}
	}

	IEnumerator scoutOut(float duration){

			scisRunning = true;
			yield return new WaitForSeconds (2*duration);
			scisRunning = false;
	}

	void attributeCosts()
	{
		structure.costs [0] = 20; //to build
		structure.costs [1] = 15; //to repair stuff + 10 % cost 1,3,4
		structure.costs [2] = 0; //upgrading costs for this unit
		structure.costs [3] = 60; // +0.25/sec  - send scout on mission
		structure.costs [4] = 60; //to update
	}
	
	public void upgrade(float upgradeDur)
	{
		//Debug.Log("upgrading special, step" + structure.upgrades);
		structure.upgrades++;
		if (structure.upgrades == 1)
		{
			if (BaseManager.resources - structure.costs [4] >= 0)
			{
				BaseManager.resources -= structure.costs [4];
				BaseManager.notEnough = "";
				for (int i=0; i<resourceFields.Length; i++)
					resourceFields [i].GetComponent<ResourceField>().speed = 3;
				StartCoroutine(structure.waitConstruction(upgradeDur, structure.colorUnit)); //needs to be 30 for upgrades;
				upgradeDuration = 20f;
				repDur = 20f;
			} else
				BaseManager.notEnough = "not enough resources";
		}
		if (structure.upgrades == 2)
		{   
			if (BaseManager.resources - structure.costs [4] >= 0)
			{
				BaseManager.resources -= structure.costs [4];
				BaseManager.notEnough = "";
				StartCoroutine(structure.waitConstruction(upgradeDur, structure.colorUnit));
				for (int i=0; i<resourceFields.Length; i++)
					resourceFields [i].GetComponent<ResourceField>().speed = 5;
				repDur = 15f;
				upgradeDuration = 15f;
			} else
				BaseManager.notEnough = "not enough resources";
		}
		
		
		//Debug.Log("upgrading special");
	}
	
	public string status()
	{message = " ";
		
		if (structure) {
			if (structure.isInConstruction) {
				message = "building";
				return message;
			} else if (structure.isUnderRepair) {
				if (structure.upgrades == 0) {
					message = "repairing";
					return message;
				} else {
					message = "upgrading";
					return message;
				}
			} else {
				if (scoutReady)
					message += " scout ready;";
				else
					message += " scout recuperating;";
				
				if (repairReady)
					message += " repair crew ready;";
				else
					message += " repair crew regenarating;";

				if (upgradeReady)
					message += " upgrade crew ready;";
				else
					message += " upgrade crew regenarating;";

				return message;
			}
		} else
			return message;

	}
	
}
