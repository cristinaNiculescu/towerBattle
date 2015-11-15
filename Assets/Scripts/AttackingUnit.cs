using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AttackingUnit : MonoBehaviour {

	public float HP=250;
	public bool isInConstruction=false;
	public int upgrades=0;
	//public Transform UnitFace;
	public int[] costs= new int[5];
	public int damageAbility1=0;
	public int damageAbility2=0;
	public Transform projectile;
	int RocksMin;
	int RocksMax;
	bool started=false;
	bool topToBottom=true;
	float startAngle;
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
		RocksMin = 20;
		RocksMax = 40;
		startAngle = gameObject.transform.rotation.z;
		healthBar = GameObject.Find ("HealthBarfor" + gameObject.name);
		HP_Bar = healthBar.GetComponent<Slider> ();
		HP_Bar.minValue = 0;
		HP_Bar.maxValue = 250;
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

		if (!isInConstruction) 
		{
			if ((Time.realtimeSinceStartup >= 20) && !started) {
				started = true;
				//Debug.Log("about to autocast");
				StartCoroutine (rockFlurr ());
			}
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
		btnText.text = "Launch Missile";
		btn.onClick.AddListener (() => missileLaunch ());

		tempOBj= GameObject.Find("BuildPanelfor"+tempName+"/buildAtck");
		tempOBj.SetActive (false);

		tempOBj= GameObject.Find("BuildPanelfor"+tempName+"/BuildSpec");
		Button btn2 = tempOBj.GetComponent<Button> ();
		Text btn2text = btn2.GetComponentInChildren<Text> ();
		btn2text.text = "Throw Mud";
		btn2.onClick.AddListener (() => mudSplatter ());
	}
	/// <summary>
	/// Attributes the costs.
	/// 0 - to build: 40 resources;
	/// 1 - to cast ability 2:  15 resources/charge;
	/// 2- to cast ability 3:  35 resources;
	/// 3- to upgrade - step 1: 100 resources;
	/// 4- to upgrade step 2: 225 resources;
	/// </summary>
	void attributeCosts(){
		costs [0] = 40;
		costs [1] = 15;
		costs [2] = 35;
		costs [3] = 100;
		costs [4] = 225;
	}


	IEnumerator rockFlurr(){

		int noRocks = Random.Range (RocksMin, RocksMax);
		float delayBetweenRockThrows = 20f/(float)noRocks;
		//Debug.Log ("shoots every "+delayBetweenRockThrows);
		float shootPeriod = 20f;
		while (shootPeriod-delayBetweenRockThrows>=0) 
		{
			yield return new WaitForSeconds(delayBetweenRockThrows);
			rockFlurrShooting();
			shootPeriod=shootPeriod-delayBetweenRockThrows;

		}
		yield return new WaitForSeconds(20);
		yield return new WaitForSeconds(20);
		StartCoroutine (rockFlurr());
	}

	/// <summary>
	/// Rocks the flurr.
	/// every 20 sec, the unit will auto-cast a flurry of small rocks in an 30 degrees arc movement to cover the enemy
	/// area from top to bottom. The small rocks do very little damage (0.5%) per hit, and there 20-40 rocks thrown 
	/// on each cast. If one of rocks hits the lone scout, it does enough damage to kill it. 
	/// </summary>
	void rockFlurrShooting()
	{	float radius=5; 
		
		float x=Mathf.Sin(startAngle)*radius+gameObject.transform.position.x;
		float y=gameObject.transform.position.y;
		float z= -Mathf.Abs(Mathf.Cos(startAngle)*radius)+gameObject.transform.position.z;
		
		Vector3 shootingPosition=new Vector3(x,y,z);
		
		
		Quaternion rotation=new Quaternion (gameObject.transform.rotation.x,
		                                    gameObject.transform.rotation.y,
		                                    gameObject.transform.rotation.z,1); 
		rockBehavior mov=projectile.GetComponent<rockBehavior>();
		mov.direction= new Vector3 (x-gameObject.transform.position.x,0,z-gameObject.transform.position.z);
		//projectile.RotateAround(gameObject.transform.position,new Vector3(0,0,1),startAngle);
		mov.gameObject.tag = this.gameObject.tag;

		Instantiate(projectile,shootingPosition,rotation);
		
		
		if (topToBottom)
			if (startAngle<180)
		{ startAngle+=1;
		}
		else topToBottom=false; 
		else if (startAngle>0)
			startAngle-=1;
		else topToBottom=true; 
	}
	
	/// <summary>
	/// Launches missiles. 
	/// has 3 charges, can be cast at different targets. Every consecutive charge that hits the same target 
	/// within 5 sec of the previous hit, deals 1% more damage. Each missile does 5% damage out of max enemy
    /// unit health.  The ability has 20 sec cool down.
	/// </summary>
	void missileLaunch(){
		Debug.Log ("launched");
	}

	/// <summary>
	/// Muds the splatter.
	/// targets enemy resource gathering fields. Slows the gathering process to 50% of normal speed for 5 sec. 
	/// It has 20 sec cool down. 
	/// </summary>
	void mudSplatter(){
		Debug.Log ("mudssssss");
	}

	void OnCollisionEnter(Collision col)
	{	
		if (col.gameObject.tag != this.tag) 
		{
			this.HP -= this.HP * 0.05f;
			Debug.Log (col.gameObject.name+" "+col.gameObject.tag+" in "+gameObject.tag);
			Destroy (col.gameObject);
		}

	}


	void UnitUpdate(){
		RocksMax = 60;
		RocksMin = 30;
	}


}
