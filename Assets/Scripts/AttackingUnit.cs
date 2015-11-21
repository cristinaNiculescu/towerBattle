using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class AttackingUnit : NetworkBehaviour
{

    UnitStructure structure;
    public int[] costs = new int[5];
    public int damageAbility1 = 0;
    public int damageAbility2 = 0;
    public Transform projectile;
    int RocksMin;
    int RocksMax;
    bool started = false;
    bool topToBottom = true;
    float startAngle;

    bool canBeClicked;
    bool activeMarker = false;
    string tempName;

    public Texture2D cursorCrosshair;

    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public Transform missile;
    bool triggeredMissileLaunch = false;
    RaycastHit hit;
    Ray ray;
    Transform target;
    int missileCurrentCharges = 0;
    bool missileAbilityAvailable = true;
    Transform[] targets;
    float timeAtMissileLaunch;

    bool mudReady = true;
    bool mudTriggered = false;
    public Transform mud;
    float mudImpactSpeed = 0.5f;
    float mudImpactDuration = 20f;

    int upgradeMultiplier = 1;
    float missileCooldown = 20f;

    // Use this for initialization
    void Start()
    {
        structure = this.GetComponent<UnitStructure>();
        structure.HP = 250;
        structure.HPMax = 250;
        attributeCosts();

        structure.colorUnit = gameObject.GetComponent<Renderer>().material.color;
        structure.isInConstruction = true;
        StartCoroutine(structure.waitConstruction(20f, structure.colorUnit)); //needs to be 20;

        structure.healthBar = GameObject.Find("HealthBarfor" + gameObject.name);
        structure.HP_Bar = structure.healthBar.GetComponent<Slider>();
        structure.HP_Bar.minValue = 0;
        structure.HP_Bar.maxValue = structure.HPMax;

        structure.name = "Attacking Unit";
        GameObject temp = GameObject.Find("Base");
        structure.BaseUnit = temp.GetComponent<BaseManager>();

        tempName = gameObject.name.Substring(0, 9);
        //	Debug.Log(tempName);
        structure.panel = GameObject.Find("BuildPanelfor" + tempName);
        changePanel();
        structure.panel.SetActive(activeMarker);

        targets = new Transform[3];
        RocksMin = 20;
        RocksMax = 40;
        startAngle = gameObject.transform.rotation.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (!structure.isInConstruction && !structure.isUnderRepair)
        {
            if (!started)
            {
                started = true;
                StartCoroutine(rockFlurr());
            }
            if (structure.HP <= 0f)
            {
                Destroy(gameObject, 0.1f);
                structure.BaseUnit.reCheckShield();
            }

            //Debug.Log (gameObject.tag + " " + HP);
            structure.HP_Bar.value = structure.HP;

            if (triggeredMissileLaunch && Input.GetMouseButtonUp(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 10000f) && (missileCurrentCharges < 3))
                {
                    //Debug.Log (hit.transform.tag);
                    if (hit.transform.tag == "Enemy")
                    {
                        //	Debug.Log (hit.transform);
                        target = hit.transform;
                        targets[missileCurrentCharges] = target;
                        //	Debug.Log (hit.transform+""+target+""+targets[missileCurrentCharges]);
                        MissileChargeAndMove damage = missile.GetComponent<MissileChargeAndMove>();
                        damage.target = target;

                        //Debug.Log("currentCharges:"+missileCurrentCharges);

                        bool sameTarget = true;
                        for (int i = 0; i < missileCurrentCharges; i++)
                        {
                            if (targets[i].name != target.name)
                                sameTarget = false;
                            //Debug.Log(targets[i].name+""+target.name+""+sameTarget);
                        }

                        if (sameTarget && Time.realtimeSinceStartup - timeAtMissileLaunch <= 5.0f)
                        {
                            //Debug.Log(Time.realtimeSinceStartup-timeAtMissileLaunch);
                            damage.missileDamagePercentage = 5 + missileCurrentCharges * upgradeMultiplier;
                        }
                        else
                        {
                            damage.missileDamagePercentage = 5;
                        }
                        missileCurrentCharges++;
                        timeAtMissileLaunch = Time.realtimeSinceStartup;
                        Instantiate(missile, gameObject.transform.position, Quaternion.identity);
                        missile.LookAt(target.position);
                        triggeredMissileLaunch = false;
                        if (missileCurrentCharges == 3)
                        {
                            missileAbilityAvailable = false;
                            StartCoroutine(rockFlurr());
                            StartCoroutine(rechargeMissile(missileCooldown));

                        }
                        else
                            Debug.Log("couldn't shoot");
                    }

                }
            }


            if (mudTriggered && Input.GetMouseButtonUp(0))
            {
                //Debug.Log("after if");
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //Debug.Log(Physics.Raycast (ray, out hit, 10000f)+" "+ray);
                if (Physics.Raycast(ray, out hit, 10000f))
                {
                    //	Debug.Log (hit.transform.tag);
                    if (hit.transform.tag == "enemy_Resource")
                    {
                        MudDrop droplet = mud.GetComponent<MudDrop>();
                        droplet.target = hit.transform;
                        droplet.speedRed = mudImpactSpeed;
                        droplet.dur = mudImpactDuration;
                        Instantiate(mud, gameObject.transform.position, Quaternion.identity);
                        mud.LookAt(hit.transform.position);
                        StartCoroutine(gatherMud());
                    }
                }
            }

        }
        structure.status = status();
        Debug.Log(status() + " " + structure.status);
    }

    void OnMouseEnter()
    {
        if (!structure.isUnderRepair && !UnitStructure.TeamLookingForTarget)
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
            //Debug.Log (activeMarker);
        }
    }

    void changePanel()
    {
        //Debug.Log ("BuildPanelfor" + tempName + "/buildAtck");
        GameObject tempOBj = GameObject.Find("BuildPanelfor" + tempName + "/Text");
        Text panelTitle = tempOBj.GetComponent<Text>();
        panelTitle.text = "Abilities";

        tempOBj = GameObject.Find("BuildPanelfor" + tempName + "/BuildDef");
        Button btn = tempOBj.GetComponent<Button>();
        Text btnText = btn.GetComponentInChildren<Text>();
        btnText.text = "Missiles";
        btn.onClick.AddListener(() => missileLaunch());

        tempOBj = GameObject.Find("BuildPanelfor" + tempName + "/buildAtck");
        tempOBj.SetActive(false);

        tempOBj = GameObject.Find("BuildPanelfor" + tempName + "/BuildSpec");
        Button btn2 = tempOBj.GetComponent<Button>();
        Text btn2text = btn2.GetComponentInChildren<Text>();
        btn2text.text = "Throw Mud";
        btn2.onClick.AddListener(() => mudSplatter());
    }
    /// <summary>
    /// Attributes the costs.
    /// 0 - to build: 40 resources;
    /// 1 - to cast ability 2:  15 resources/charge;
    /// 2- to cast ability 3:  35 resources;
    /// 3- to upgrade - step 1: 100 resources;
    /// 4- to upgrade step 2: 225 resources;
    /// </summary>
    void attributeCosts()
    {
        structure.costs[0] = 40; // build
        structure.costs[1] = 15; // cast rockFlurr
        structure.costs[2] = 35; // cast mudSplatter
        structure.costs[3] = 100; //upgrade step 1
        structure.costs[4] = 225; //upgrade step 2
    }


    IEnumerator rockFlurr()
    {
        yield return new WaitForSeconds(20);
        int noRocks = Random.Range(RocksMin, RocksMax);
        float delayBetweenRockThrows = 20f / (float)noRocks;
        //Debug.Log ("shoots every "+delayBetweenRockThrows);
        float shootPeriod = 20f;
        while (shootPeriod - delayBetweenRockThrows >= 0)
        {
            yield return new WaitForSeconds(delayBetweenRockThrows);
            rockFlurrShooting();
            shootPeriod = shootPeriod - delayBetweenRockThrows;

        }


        StartCoroutine(rockFlurr());
    }
    /// <summary>
    /// Rocks the flurr.
    /// every 20 sec, the unit will auto-cast a flurry of small rocks in an 30 degrees arc movement to cover the enemy
    /// area from top to bottom. The small rocks do very little damage (0.5%) per hit, and there 20-40 rocks thrown 
    /// on each cast. If one of rocks hits the lone scout, it does enough damage to kill it. 
    /// </summary>
    void rockFlurrShooting()
    {
        float radius = 5;

        float x = -Mathf.Sin(startAngle) * radius + gameObject.transform.position.x;
        float y = 0f;
        float z = Mathf.Abs(Mathf.Cos(startAngle) * radius) + gameObject.transform.position.z;

        Vector3 shootingPosition = new Vector3(x, y, z);


        Quaternion rotation = new Quaternion(gameObject.transform.rotation.x,
                                            gameObject.transform.rotation.y,
                                            gameObject.transform.rotation.z, 1);
        RockBehavior mov = projectile.GetComponent<RockBehavior>();
        mov.direction = new Vector3(x - gameObject.transform.position.x, 0f, z - gameObject.transform.position.z);
        //projectile.RotateAround(gameObject.transform.position,new Vector3(0,0,1),startAngle);
        mov.gameObject.tag = this.gameObject.tag;
        Instantiate(projectile, shootingPosition, rotation);


        if (topToBottom)
            if (startAngle < 180)
            {
                startAngle += 1;
            }
            else topToBottom = false;
        else if (startAngle > 0)
            startAngle -= 1;
        else topToBottom = true;
    }

    /// <summary>
    /// Launches missiles. 
    /// has 3 charges, can be cast at different targets. Every consecutive charge that hits the same target 
    /// within 5 sec of the previous hit, deals 1% more damage. Each missile does 5% damage out of max enemy
    /// unit health.  The ability has 20 sec cool down.
    /// </summary>
    void missileLaunch()
    {
        if (missileAbilityAvailable)
        {
            triggeredMissileLaunch = true;
            BaseManager.resources -= structure.costs[1];
            StopCoroutine(rockFlurr());
            activeMarker = false;
            structure.panel.SetActive(activeMarker);
        }
        else Debug.Log("missiles not ready yet");
    }
    IEnumerator rechargeMissile(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        missileAbilityAvailable = true;
        for (int i = 0; i <= 2; i++)
            targets[i] = null;
        missileCurrentCharges = 0;
    }

    /// <summary>
    /// Muds the splatter.
    /// targets enemy resource gathering fields. Slows the gathering process to 50% of normal speed for 20 sec. 
    /// It has 40 sec cool down. 
    /// </summary>
    void mudSplatter()
    {
        activeMarker = false;
        structure.panel.SetActive(activeMarker);
        //Debug.Log ("mudssssss"+mudTriggered+mudReady);
        if (mudReady)
        {
            mudTriggered = true;
            mudReady = false;
            BaseManager.resources -= structure.costs[2];
        }
        else
            Debug.Log("still gathering mud");
    }

    IEnumerator gatherMud()
    {
        mudTriggered = false;
        yield return new WaitForSeconds(40);
        mudReady = true;
    }

    void OnCollisionEnter(Collision col)
    {

    }

    public void upgrade()
    {
        structure.upgrades++;
        Debug.Log("upgrading attacking, step" + structure.upgrades);
        if (structure.upgrades == 1)
        {
            StartCoroutine(structure.waitConstruction(30f, structure.colorUnit)); //needs to be 20;
            upgradeMultiplier = 2;
            mudImpactSpeed = 0.6f;
            mudImpactDuration = 30f;
        }
        if (structure.upgrades == 2)
        {
            StartCoroutine(structure.waitConstruction(30f, structure.colorUnit));
            missileCooldown = 15f;
            RocksMax = 60;
            RocksMin = 40;
            BaseManager.resources -= structure.costs[5];
        }
    }

    public string status()
    {
        string message = " ";
        if (structure.isUnderRepair)
            message = "repairing";
        if (structure.isInConstruction)
            if (structure.upgrades == 0)
                message = "building";
            else
                message = "upgrading";
        if (!mudReady) message += " gathering mud;";
        else message += " mud ready;";
        if (!missileAbilityAvailable) message += " preparing missiles;";
        else message += " missiles ready;";
        return message;

    }


}
