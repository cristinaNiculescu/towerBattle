using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class DefensiveUnit : NetworkBehaviour
{
    //public Transform UnitFace;
    UnitStructure structure;

    bool started = false;
    bool canBeClicked;
    bool activeMarker = false;
    string tempName;

    RaycastHit hit;
    Ray ray;
    Transform target;

    public Transform cliff;
    public float disorientDuration;
    bool rockTriggered = false;
    bool rockReady = true;
    float rockCooldown;
    float rockPercentageDamage = 20f;

    public Transform cloud;
    public float cloudCharges;
    bool cloudTriggered = false;
    bool cloudReady = true;
    float cloudCooldown;
    float cloudDuration = 10f;
    int cloudRadius = 60;


    // Use this for initialization
    void Start()
    {
        structure = this.GetComponent<UnitStructure>();
        structure.HP = 500;
        structure.HPMax = 500;
        attributeCosts();
        //Debug.Log (structure.costs [0]);
        structure.colorUnit = gameObject.GetComponent<Renderer>().material.color;
        structure.isInConstruction = true;
        StartCoroutine(structure.waitConstruction(20f, structure.colorUnit));
        //BaseManager.resources -= structure.costs [0];

        structure.healthBar = GameObject.Find("HealthBarfor" + gameObject.name);
        structure.HP_Bar = structure.healthBar.GetComponent<Slider>();
        structure.HP_Bar.minValue = 0;
        structure.HP_Bar.maxValue = structure.HPMax;
        structure.HP_Bar.value = structure.HP;

        structure.name = "Defensive Unit";
        //GameObject temp = GameObject.Find("Base");
        GameObject temp = GameObject.Find("Base(Clone)");
        structure.BaseUnit = temp.GetComponent<BaseManager>();

        tempName = gameObject.name.Substring(0, 9);
        //Debug.Log(tempName);
        structure.panel = GameObject.Find("BuildPanelfor" + tempName);
        changePanel();
        structure.panel.SetActive(activeMarker);

        disorientDuration = 10f;
        rockCooldown = 60;
    }


    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }
        if (!structure.isInConstruction && !structure.isUnderRepair && !structure.isDisoriented)
        {

            if (structure.HP <= 0f)
            {
                Destroy(gameObject, 0.1f);
                structure.BaseUnit.reCheckShield();
            }

            structure.HP_Bar.value = structure.HP;
        }
        if (rockTriggered && Input.GetMouseButtonUp(0))
        {
            //Debug.Log("after if");
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Debug.Log(Physics.Raycast (ray, out hit, 10000f)+" "+ray);
            if (Physics.Raycast(ray, out hit, 10000f))
            {
                //	Debug.Log (hit.transform.tag);
                if (hit.transform.tag == "Enemy")
                {
                    BigRockBehavior cliffBit = cliff.GetComponent<BigRockBehavior>();
                    cliffBit.target = hit.transform;
                    cliffBit.dur = disorientDuration;
                    cliffBit.damagePercentage = rockPercentageDamage;
                    Instantiate(cliff, gameObject.transform.position, Quaternion.identity);
                    cliff.LookAt(hit.transform.position);
                    rockReady = false;
                    rockTriggered = false;
                    StartCoroutine(throwCliff());
                }
            }
        }

        if (cloudTriggered && Input.GetMouseButtonUp(0))
        {
            //Debug.Log("after if");
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Debug.Log(Physics.Raycast (ray, out hit, 10000f)+" "+ray);
            if (Physics.Raycast(ray, out hit, 10000f) && cloudCharges <= 1)
            {
                //	Debug.Log (hit.transform.tag);
                if (hit.transform.tag != "Enemy" && hit.transform.tag != "Untagged")
                {
                    CloudSetting puff = cloud.GetComponent<CloudSetting>();
                    puff.target = hit.transform;
                    puff.dur = cloudDuration;
                    puff.size = new Vector3((float)cloudRadius, 1f, (float)cloudRadius);
                    puff.position = new Vector3(hit.transform.position.x, 10f, hit.transform.position.z);
                    Debug.Log(puff.position + " " + hit.transform.position);
                    Instantiate(cloud, puff.position, Quaternion.identity);
                    cloudCharges++;
                    cloudTriggered = false;
                    if (cloudCharges == 2)
                    {
                        cloudReady = false;
                        cloudCharges = 0;
                        StartCoroutine(settleCloud());
                    }
                }
            }
        }

    }


    void OnMouseEnter()
    {
        if (!structure.isInConstruction && !structure.isDisoriented && !structure.isUnderRepair && !UnitStructure.TeamLookingForTarget)
            canBeClicked = true;
    }
    void OnMouseExit()
    {
        canBeClicked = false;
    }

    void OnMouseUp()
    {
        if (canBeClicked)
        {
            structure.panel.SetActive(activeMarker);
            activeMarker = !activeMarker;

        }
    }

    void changePanel()
    {

        GameObject tempOBj = GameObject.Find("BuildPanelfor" + tempName + "/Text");
        Text panelTitle = tempOBj.GetComponent<Text>();
        panelTitle.text = "Abilities";

        tempOBj = GameObject.Find("BuildPanelfor" + tempName + "/BuildDef");
        Button btn = tempOBj.GetComponent<Button>();
        Text btnText = btn.GetComponentInChildren<Text>();
        btnText.text = "Set Cloud";
        btn.onClick.AddListener(() => setCloud());

        tempOBj = GameObject.Find("BuildPanelfor" + tempName + "/BuildAtck");
        tempOBj.SetActive(false);

        tempOBj = GameObject.Find("BuildPanelfor" + tempName + "/BuildSpec");
        Button btn2 = tempOBj.GetComponent<Button>();
        Text btn2text = btn2.GetComponentInChildren<Text>();
        btn2text.text = "Throw Rock";
        btn2.onClick.AddListener(() => throwRock());
    }

    /// <summary>
    /// Sets the cloud.
    /// one cloud lasts 10 sec, the ability has 2 charge and 30 sec cool-down. 
    /// The clouds form like little tornado's and can be used to mask portions (60 units radius) 
    /// of the map above friendly units. The cast sets the center of the cloud.  
    /// This will prevent enemy units from targeting them.
    /// </summary>
    void setCloud()
    {
        if (cloudReady)
        {
            if (BaseManager.resources - structure.costs[1] >= 0)
            {
                BaseManager.resources -= structure.costs[1];
                BaseManager.notEnough = "";
                activeMarker = false;
                structure.panel.SetActive(activeMarker);
                cloudTriggered = true;
                Debug.Log("cloud is above");
            }
            else BaseManager.notEnough = "not enough resources";
        }
    }
    IEnumerator settleCloud()
    {
        yield return new WaitForSeconds(cloudCooldown);
        cloudReady = true;

    }

    /// <summary>
    /// Throws the rock.
    /// the rock is thrown at a certain target and makes a little bit of mess :  
    /// 20% damage of max health of the enemy unit it hits and creates a cloud of 
    /// dust around the same that lasts for 10 sec.  
    /// The cloud temporarily incapacitates the enemy unit. 
    /// The ability has 1 min cool down
    /// </summary>
    void throwRock()
    {
        if (rockReady)
        {
            if (BaseManager.resources - structure.costs[2] >= 0)
            {
                BaseManager.resources -= structure.costs[2];
                BaseManager.notEnough = "";
                activeMarker = false;
                structure.panel.SetActive(activeMarker);
                rockTriggered = true;
                Debug.Log("threw rock");
            }
            else BaseManager.notEnough = "not enough resources";
        }

    }

    IEnumerator throwCliff()
    {
        yield return new WaitForSeconds(rockCooldown);
        rockReady = true;

    }

    void attributeCosts()
    {
        structure.costs[0] = 50; //to build 
        structure.costs[1] = 5; // to set 1 cloud
        structure.costs[2] = 50; // to throw the cliff
        structure.costs[3] = 125; // to upgrade step 1
        structure.costs[4] = 250; // to upgrade step 2
    }

    public void upgrade(float upgradeDuration)
    {
        Debug.Log("upgrading defensive, step" + structure.upgrades);
        structure.upgrades++;
        if (structure.upgrades == 1)
        {
            if (BaseManager.resources - structure.costs[3] >= 0)
            {
                BaseManager.resources -= structure.costs[3];
                BaseManager.notEnough = "";
                StartCoroutine(structure.waitConstruction(upgradeDuration, structure.colorUnit)); //needs to be 30 for upgrades;
                cloudDuration = 13f;
                rockPercentageDamage = 30f;

            }
            else BaseManager.notEnough = "not enough resources";
        }
        if (structure.upgrades == 2)
        {
            if (BaseManager.resources - structure.costs[4] >= 0)
            {
                BaseManager.resources -= structure.costs[4];
                BaseManager.notEnough = " ";
                StartCoroutine(structure.waitConstruction(upgradeDuration, structure.colorUnit));
                cloudDuration = 15f;
                cloudRadius = 70;
                rockPercentageDamage = 40f;
            }
            else BaseManager.notEnough = "not enough resources";
        }
    }


    public string status()
    {
        return "status";
    }

}
