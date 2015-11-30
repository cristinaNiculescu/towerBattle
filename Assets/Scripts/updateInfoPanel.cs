using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class updateInfoPanel : MonoBehaviour
{
    public GameObject InfoPanel;
    public Text[] infos;
    bool clicked;
    public GameObject baseF;
    BaseManager friendlyBase;
    float gatheringSpeed;
    public GameObject[] UnitsBuilt;
    bool needsRefresh = false;

    // Use this for initialization
    void Start()
    {
        if (this.tag != "Enemy")
        {
            infos = new Text[7];
            InfoPanel = GameObject.Find("InfoPanel");
            int i = 0;
            Traverse(InfoPanel, 0);
            UnitsBuilt = new GameObject[5];
        }
        else if (this.tag == "Enemy")
        {
            infos = new Text[7];
            InfoPanel = GameObject.Find("InfoPanel2");
            int i = 0;
            Traverse(InfoPanel, 0);
            UnitsBuilt = new GameObject[5];
        }
        friendlyBase = baseF.GetComponent<BaseManager>();
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
        if ((friendlyBase.shieldPower > 500 || needsRefresh) && (this.tag != "Enemy"))
        {
            {
                needsRefresh = true;
                for (int i = 0; i < friendlyBase.UnitsBuilt.Length; i++)
                {
                    if (friendlyBase.UnitsBuilt[i])
                    {
                        UnitsBuilt[i] = friendlyBase.UnitsBuilt[i];
                    }
                }
                updateText();
            }
        }
        else if ((friendlyBase.shieldPower > 500 || needsRefresh) && (this.tag == "Enemy"))
        {
            {
                needsRefresh = true;
                for (int i = 0; i < friendlyBase.UnitsBuilt.Length; i++)
                {
                    if (friendlyBase.UnitsBuilt[i])
                    {
                        UnitsBuilt[i] = friendlyBase.UnitsBuilt[i];
                    }
                }
                updateText();
            }
        }
    }

    void updateText()
    {
        gatheringSpeed = 0;
        string message = "empty Status";
        for (int i = 0; i <= 6; i++)
        {
            if (infos[i])
            {
                if (i == 0)
                    infos[i].text = "Status: Alive & Ready \n" + "Health: " + friendlyBase.GetComponent<UnitStructure>().HP + "\n Shield: " + friendlyBase.shieldPower;
                if (i >= 1 && i <= 5)
                {
                    infos[i].fontSize = 8;
                    if (UnitsBuilt[i - 1])
                    {
                        UnitStructure temp = UnitsBuilt[i - 1].GetComponent<UnitStructure>();
                        switch (temp.tag)
                        {
                            case "attacking": AttackingUnit statusAtCheckObj = temp.GetComponent<AttackingUnit>();
                                message = statusAtCheckObj.status();
                                break;
                            case "defense": DefensiveUnit statusDefCheckObj = temp.GetComponent<DefensiveUnit>();
                                message = statusDefCheckObj.status();
                                break;
                            case "special": SpecialUnit statusSPCheckObj = temp.GetComponent<SpecialUnit>();
                                message = statusSPCheckObj.status();
                                break;
                        }
                        infos[i].text = "Unit" + i + ": "
                            + temp.name + "\n" + "Health: " + temp.HP + "\n" + "Status: " + message;
                        if (temp.GetComponent<SpecialUnit>())
                        {
                            gatheringSpeed += temp.GetComponent<SpecialUnit>().lastGathered;
                        }
                    }
                }
                if (i == 6)
                    infos[i].text = "Resources: " + BaseManager.resources + " " + BaseManager.notEnough + " " + "\n Gathering Speed: " + gatheringSpeed;
            }
        }
    }
}

