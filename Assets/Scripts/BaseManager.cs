using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BaseManager : MonoBehaviour {

	public int shieldDamageTaken=0;
	public float shieldPower=500;
	public Transform[] UnitsBuilt;
	float shieldMultiplier=0;
	public static int resources=200;
	UnitStructure structure;
	public GameObject InfoPanel;
	public Text[] infos;
	bool clicked;
	// Use this for initialization
	void Start () {

			structure = this.GetComponent<UnitStructure> ();
			structure.HP = 2000;
			structure.HPMax = 2000;
			UnitsBuilt = new Transform[5];

			if (gameObject.tag == "Enemy")
				structure.healthBar = GameObject.Find ("HealthBarforEnemyBase");
			else
				structure.healthBar = GameObject.Find ("HealthBarforBase");
			structure.HP_Bar = structure.healthBar.GetComponent<Slider> ();
			structure.HP_Bar.minValue = 0;
			structure.HP_Bar.maxValue = 2000;
			structure.HP_Bar.value = structure.HP;
			structure.BaseUnit = this;

			if (this.tag != "Enemy") 
			{
			infos = new Text[7];
			InfoPanel = GameObject.Find ("InfoPanel");
			int i = 0;

			Traverse (InfoPanel,0);
			//Debug.Log (infos + " " + infos.Length+","+i);
		/*	for(int j=0;j<infos.Length;j++)
				if(infos [j]!=null)
					Debug.Log(infos [j].name);*/

		}
	}

	void Traverse(GameObject obj, int i)
	{
		foreach (Transform child in obj.transform)
		{	
			infos[i]=child.GetComponent<Text>();
			Traverse(child.gameObject, i++);
		}
	}
	// Update is called once per frame
	void Update () {
		
		structure.HP_Bar.value = structure.HP;

		if (clicked && (this.tag!="Enemy"))
			updateText ();

	}

	public void reCheckShield(){
		shieldMultiplier = 0;
		for (int i=0; i<5; i++) 
			if (UnitsBuilt [i] != null)
		{	shieldMultiplier++;
		}
		float temp = shieldMultiplier * 0.15f * 500;
		shieldPower = (int)temp+shieldPower;
	}

	void updateText(){	
		string unitType=" ";
		//string status = "status";


		for (int i=0; i<=6; i++) 
		{ 	
			if (infos [i]) 
			{
				if (i == 0)
					infos [i].text = "Status: Alive & Ready \n" + "Health: " + structure.HP + "\n Shield: " + shieldPower;
				if (i >= 1 && i <= 5) 
				{
					if (UnitsBuilt [i - 1]) 
					{
						infos [i].text = "Unit" + i + ": " 
							+ UnitsBuilt [i - 1].GetComponent<UnitStructure> ().name + "\n"
							+ "Health: " + UnitsBuilt [i - 1].GetComponent<UnitStructure> ().HP + "\n"
								+ "Status: " + UnitsBuilt [i - 1].GetComponent<UnitStructure> ().status;
						Debug.Log(UnitsBuilt [i - 1].GetComponent<UnitStructure> ().status);
					}
				}
				if (i==6)
					infos [i].text = "Resources: " + resources + "\n Gathering Speed: ";
			}
		}
	}


	void OnMouseUp(){
		if (this.tag!="Enemy")
			updateText ();
		clicked = true;
	}

	void Lost()
	{

	}

	void Won(){

	}

}
