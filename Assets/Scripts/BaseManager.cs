using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class BaseManager : NetworkBehaviour
{
	public int shieldDamageTaken = 0;
	public float shieldPower = 500;
//	public Transform[] UnitsBuilt;
	public GameObject[] UnitsBuilt;
	float shieldMultiplier = 0;
	public static int resources = 200;
	float gatheringSpeed;
	UnitStructure structure;
	public GameObject InfoPanel;
	public Text[] infos;
	bool clicked;
	public static string notEnough;
	public GameObject imageWin;
	public GameObject imageLost;
	
	// Use this for initialization
	void Start()
	{
		
		structure = this.GetComponent<UnitStructure>();
		structure.HP = 2000;
		structure.HPMax = 2000;
		//UnitsBuilt = new Transform[5];
		UnitsBuilt = new GameObject[5];
		
		if (gameObject.tag == "Enemy")
			structure.healthBar = GameObject.Find("HealthBarforEnemyBase");
		else
			structure.healthBar = GameObject.Find("HealthBarforBase");
		structure.HP_Bar = structure.healthBar.GetComponent<Slider>();
		structure.HP_Bar.minValue = 0;
		structure.HP_Bar.maxValue = 2000;
		structure.HP_Bar.value = structure.HP;
		structure.BaseUnit = this;
		
		if (this.tag != "Enemy")
		{
			infos = new Text[7];
			InfoPanel = GameObject.Find("InfoPanel");
            //int i = 0;
			Traverse(InfoPanel, 0);
		}
		
		
	}
	
	void Traverse(GameObject obj, int i)
	{
		foreach (Transform child in obj.transform)
		{   
			infos [i] = child.GetComponent<Text>();
			Traverse(child.gameObject, i++);
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		
		structure.HP_Bar.value = structure.HP;
		if (this.tag != "Enemy")
		{
			//updateText();
//			Debug.Log(GameObject.FindWithTag("Enemy").GetComponent<UnitStructure>().BaseUnit.GetComponent<UnitStructure>().HP);
			if (structure.HP <= 0)
			{
				Lost();
			} 
			else if (GameObject.FindWithTag("Enemy").GetComponent<UnitStructure>().BaseUnit.GetComponent<UnitStructure>().HP <= 0)
			{
				Won();
			}
		}
	}
	
	public void reCheckShield()
	{
		shieldMultiplier = 0;
		for (int i=0; i<5; i++) 
			if (UnitsBuilt [i] != null)
		{
			shieldMultiplier++;
		}
		float temp = shieldMultiplier * 0.15f * 500;
		shieldPower = (int)temp + shieldPower;
	}
	
	/*void updateText()
	{   
		
		gatheringSpeed = 0;
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
						UnitStructure temp = UnitsBuilt [i - 1].GetComponent<UnitStructure>();
						Debug.Log(temp.tag);
						
						infos [i].text = "Unit" + i + ": " 
							+ temp.name + "\n" + "Health: " + temp.HP + "\n" + "Status: " + temp.statusUpdater;
						//Debug.Log(temp.statusUpdater);
						
						if (temp.GetComponent<SpecialUnit>())
						{
							gatheringSpeed += temp.GetComponent<SpecialUnit>().lastGathered;
							Debug.Log(temp.GetComponent<SpecialUnit>().lastGathered);
						}
					}
					
				}
				if (i == 6)
					infos [i].text = "Resources: " + resources + " " + notEnough + " " + "\n Gathering Speed: " + gatheringSpeed;
			}
		}
	}
	*/
	void OnMouseUp()
	{
		if (this.tag != "Enemy")
		//	updateText();
		clicked = true;
	}
	
	void Lost()
	{
		
		imageLost.SetActive(true);
	}
	
	void Won()
	{
		
		imageWin.SetActive(true);
	}
	
}
