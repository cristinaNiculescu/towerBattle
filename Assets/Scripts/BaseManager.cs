using UnityEngine;
using System.Collections;

public class BaseManager : MonoBehaviour {

	public int shieldDamageTaken=0;
	public int shieldPower=500;
	public Transform[] UnitsBuilt;
	int shieldMultiplier=0;
	public int resources=200;

	// Use this for initialization
	void Start () {
	
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
			Debug.Log (i + " " + UnitsBuilt [i].name);
		}
	}

	void Lost()
	{

	}

	void Won(){

	}
}
