using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class BaseManager : NetworkBehaviour
{
	public int shieldDamageTaken = 0;
	public float shieldPower = 500;
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
        if (GetComponent<NetworkIdentity>().clientAuthorityOwner == null)
        {
            return;
        }
		structure.HP_Bar.value = structure.HP;
		if (this.tag != "Enemy")
		{
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

	void OnMouseUp()
	{
		if (this.tag != "Enemy")
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
