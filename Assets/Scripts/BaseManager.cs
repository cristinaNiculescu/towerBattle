using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class BaseManager : NetworkBehaviour
{
    [SyncVar]
    public int shieldDamageTaken = 0;
    [SyncVar]
    public float shieldPower = 500;
    public GameObject[] UnitsBuilt = new GameObject[5];
    [SyncVar]
    float shieldMultiplier = 0;
    public static int resources = 200;
    [SyncVar]
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
        if (hasAuthority)
        {
            structure = this.GetComponent<UnitStructure>();
            structure.HP = 2000;
            structure.HPMax = 2000;
            UnitsBuilt = new GameObject[5];

            if (gameObject.tag == "Player 2")//Player 2
                structure.healthBar = GameObject.Find("HealthBarforEnemyBase");
            else
                structure.healthBar = GameObject.Find("HealthBarforBase");
            structure.HP_Bar = structure.healthBar.GetComponent<Slider>();
            structure.HP_Bar.minValue = 0;
            structure.HP_Bar.maxValue = 2000;
            structure.HP_Bar.value = structure.HP;
            structure.BaseUnit = this;

            if (this.name == "Base(Clone)")//Player 1
            {
                infos = new Text[7];
                InfoPanel = GameObject.Find("InfoPanel");
                Traverse(InfoPanel, 0);
            }
            if (this.name == "Enemy_base(Clone)")//Player 2
            {
                infos = new Text[7];
                InfoPanel = GameObject.Find("InfoPanel2");
                Traverse(InfoPanel, 0);
            }
        }
    }

    void Traverse(GameObject obj, int i)
    {
        foreach (Transform child in obj.transform)
        {
            infos[i] = child.GetComponent<Text>();
            Traverse(child.gameObject, i++);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!localPlayerAuthority && !hasAuthority)
        {
            return;
        }
        structure.HP_Bar.value = structure.HP;
        if (this.name == "Base(Clone)")
        {
            if (structure.HP <= 0)
            {
                Lost();
                Time.timeScale = 0;
            }
            if (GameObject.Find("Enemy_base(Clone)") != null)
            {
                if (GameObject.Find("Enemy_base(Clone)").GetComponent<UnitStructure>().BaseUnit.GetComponent<UnitStructure>().HP <= 0)
                {
                    Won();
                    Time.timeScale = 0;
                }
            }
        }
        if (this.name == "Enemy_base(Clone)")
        {
            if (structure.HP <= 0)
            {
                Lost();
                Time.timeScale = 0;
            }
            if (GameObject.Find("Base(Clone)") != null)
            {
                if (GameObject.Find("Base(Clone)").GetComponent<UnitStructure>().BaseUnit.GetComponent<UnitStructure>().HP <= 0)
                {
                    Won();
                    Time.timeScale = 0;
                }
            }
        }
    }

    public void reCheckShield()
    {
        shieldMultiplier = 0;
        for (int i = 0; i < 5; i++)
            if (UnitsBuilt[i] != null)
            {
                shieldMultiplier++;
            }
        float temp = shieldMultiplier * 0.15f * 500;
        shieldPower = (int)temp + shieldPower;
    }

    void OnMouseUp()
    {
        if (this.name == "Base(Clone)")
            clicked = true;
        if (this.name == "Enemy_base(Clone)")
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
