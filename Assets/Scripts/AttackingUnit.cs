using UnityEngine;
using System.Collections;

public class AttackingUnit : MonoBehaviour {
	/*
	-the attacking unit has: - 250 hp;
	- 30x30 size;
	- requires timer
	- can be upgraded 2 times
	 */

	public int HP=250;
	public int diameter=30;
	//public int length;
	//public int width;
	public string type="attack";
	public bool isInConstruction=false;
	public int upgrades=0;
	public Transform UnitFace;
	public int[] costs= new int[5];
	public int damageAbility1=0;
	public int damageAbility2=0;
	int RocksMin;
	int RocksMax;
	bool started=false;
	// Use this for initialization
	void Start () {
		RocksMin = 20;
		RocksMax = 40;
		Debug.Log(RocksMax+" " +RocksMin);
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (Time.realtimeSinceStartup);
		if ((Time.realtimeSinceStartup >= 20) && !started) 
		{	started=true;
			Debug.Log("about to autocast");
			StartCoroutine (rockFlurr ());
		}
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

	/// <summary>
	/// Rocks the flurr.
	/// every 20 sec, the unit will auto-cast a flurry of small rocks in an 30 degrees arc movement to cover the enemy
	/// area from top to bottom. The small rocks do very little damage (0.5%) per hit, and there 20-40 rocks thrown 
	/// on each cast. If one of rocks hits the lone scout, it does enough damage to kill it. 
	/// </summary>

	IEnumerator rockFlurr(){

		int noRocks = Random.Range (RocksMin, RocksMax);
		float delayBetweenRockThrows = (float)noRocks / 20f;
		float shootPeriod = 20f;
		while (shootPeriod-delayBetweenRockThrows>=0) 
		{
			yield return new WaitForSeconds(delayBetweenRockThrows);
			shoot();
			Debug.Log ("shooting for "+shootPeriod+" more secs");
		}
		yield return new WaitForSeconds(20);
		StartCoroutine (rockFlurr());
	}

	void shoot(){

		Vector3 shootingPosition=new Vector3(gameObject.transform.position.x,
		                                     gameObject.transform.position.y,
		                                     gameObject.transform.position.z) ;
		Debug.Log(shootingPosition);
	}
	//Throw

	/// <summary>
	/// Launches missiles. 
	/// has 3 charges, can be cast at different targets. Every consecutive charge that hits the same target 
	/// within 5 sec of the previous hit, deals 1% more damage. Each missile does 5% damage out of max enemy
    /// unit health.  The ability has 20 sec cool down.
	/// </summary>
	void missileLaunch(){
	}

	/// <summary>
	/// Muds the splatter.
	/// targets enemy resource gathering fields. Slows the gathering process to 50% of normal speed for 5 sec. 
	/// It has 20 sec cool down. 
	/// </summary>
	void mudSplatter(){
	}


	void UnitUpdate(){
		RocksMax = 60;
		RocksMin = 30;
	}
}
