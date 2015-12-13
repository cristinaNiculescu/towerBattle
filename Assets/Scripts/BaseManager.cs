using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class BaseManager : NetworkBehaviour
{
    public int shieldDamageTaken = 0;
    [SyncVar]
    public float shieldPower = 500;
    public GameObject[] UnitsBuilt = new GameObject[5];
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

    [SyncVar]
    public bool lost = false;
    [SyncVar]
    public bool won = false;

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
            {
                //structure.healthBar = GameObject.Find("HealthBarforEnemyBase");
                GameObject canvas = GameObject.Find("CanvasClient(Clone)");
                foreach (Transform trans in canvas.transform)
                {
                    if (trans.gameObject.name == "ImageWin")
                    {
                        imageWin = trans.gameObject;
                    }
                    if (trans.gameObject.name == "ImageLose")
                    {
                        imageLost = trans.gameObject;
                    }
                    if (trans.gameObject.name == "HealthBarforEnemyBase")
                    {
                        structure.healthBar = trans.gameObject;
                    }
                }
                gameObject.GetComponent<Renderer>().material.color = new Color(0.0f, 0.9f, 0.12f, 1f);
            }
            else
            {
                //structure.healthBar = GameObject.Find("HealthBarforBase");
                GameObject canvas = GameObject.Find("Canvas(Clone)");
                foreach (Transform trans in canvas.transform)
                {
                    if (trans.gameObject.name == "ImageWin")
                    {
                        imageWin = trans.gameObject;
                    }
                    if (trans.gameObject.name == "ImageLose")
                    {
                        imageLost = trans.gameObject;
                    }
                    if (trans.gameObject.name == "HealthBarforBase")
                    {
                        structure.healthBar = trans.gameObject;
                    }
                }
                gameObject.GetComponent<Renderer>().material.color = new Color(0.0f, 0.9f, 0.12f, 1f);
            }
            structure.HP_Bar = structure.healthBar.GetComponent<Slider>();
            structure.HP_Bar.minValue = 0;
            structure.HP_Bar.maxValue = 2000;
            structure.HP_Bar.value = structure.HP;
            structure.BaseUnit = this;

            //if (this.name == "Base(Clone)")//Player 1
            if (this.gameObject.tag == "Player 1")//Player 1
            {
                infos = new Text[7];
                InfoPanel = GameObject.Find("InfoPanel");
                Traverse(InfoPanel, 0);
            }
            //if (this.name == "Enemy_base(Clone)")//Player 2
            if (this.gameObject.tag == "Player 2")//Player 2
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
        if (!hasAuthority)
        {
            return;
        }
        structure.HP_Bar.value = structure.HP;
        StateOfGame();
    }

    public void reCheckShield()
    {
        shieldMultiplier = 0;
        for (int i = 0; i < 5; i++)
        {
            if (UnitsBuilt[i] != null)
            {
                shieldMultiplier++;
            }
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

    void StateOfGame()
    {
        if (won)
        {
            print("I WON! YAAAY!");
            imageWin.SetActive(true);
            won = false;
            Time.timeScale = 0;
        }
        if (lost)
        {
            print("OH NOOOOS I LOST!! DAMMIT!");
            imageLost.SetActive(true);
            lost = false;
            Time.timeScale = 0;
        }
    }

    public void ShieldTakeDamage(float amount)
    {
        if (!isServer)
            return;
        //BaseUnit.shieldPower -= amount;
        shieldPower -= amount;
    }
}
