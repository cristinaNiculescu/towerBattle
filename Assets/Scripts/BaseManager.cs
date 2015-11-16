using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BaseManager : MonoBehaviour {

	public int shieldDamageTaken=0;
	public int shieldPower=500;
	public Transform[] UnitsBuilt;
	float shieldMultiplier=0;
	public int resources=200;
	UnitStructure structure;
	// Use this for initialization
	void Start () {
		structure = this.GetComponent<UnitStructure> ();
		structure.HP = 2000;
		structure.HPMax = 2000;
		UnitsBuilt = new Transform[5];

		if (gameObject.tag == "Enemy")
			structure.healthBar = GameObject.Find ("HealthBarforEnemyBase");
		else structure.healthBar = GameObject.Find ("HealthBarforBase");
		structure.HP_Bar = structure.healthBar.GetComponent<Slider> ();
		structure.HP_Bar.minValue = 0;
		structure.HP_Bar.maxValue = 2000;
		structure.HP_Bar.value = structure.HP;
	}
	
	// Update is called once per frame
	void Update () {

		structure.HP_Bar.value = structure.HP;

	}

	public void reCheckShield(){
		shieldMultiplier = 0;
		for (int i=0; i<5; i++) 
			if (UnitsBuilt [i] != null)
		{	shieldMultiplier++;
			//Debug.Log (i + " " + UnitsBuilt [i].name);
		}
		float temp = shieldMultiplier * 0.15f * 500;
		shieldPower = (int)temp+shieldPower;
	}

	void Lost()
	{

	}

	void Won(){

	}
}
