using UnityEngine;
using System.Collections;

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

	}
	
	// Update is called once per frame
	void Update () {
		

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
