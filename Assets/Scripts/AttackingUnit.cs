using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class AttackingUnit : NetworkBehaviour
{
    UnitStructure structure;

    public Transform projectile;
    Vector3 shootingPosition;
    int RocksMin;
    int RocksMax;
    bool started = false;
    bool topToBottom = true;
    float startAngle;
    bool canBeClicked;
    bool activeMarker = false;
    string tempName;
    public string message = " ";
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
    IEnumerator co;
    bool throwingRocks = false;

    // Use this for initialization
    void Start()
    {
        if (localPlayerAuthority && hasAuthority)
        {
            structure = this.GetComponent<UnitStructure>();
            structure.HP = 250;
            structure.HPMax = 250;
            structure.colorUnit = gameObject.GetComponent<Renderer>().material.color;
            structure.isInConstruction = true;
            StartCoroutine(structure.waitConstruction(20f, structure.colorUnit)); //needs to be 20;
            GameObject temp = null;
            //if (GameObject.Find("Player 7").GetComponent<NetworkIdentity>().connectionToClient == GetComponent<NetworkIdentity>().clientAuthorityOwner)
            if (GetComponent<NetworkIdentity>().clientAuthorityOwner == GameObject.Find("Player 7").GetComponent<NetworkIdentity>().clientAuthorityOwner)
            {
                Debug.Log("Player 2 has auth for go: " + gameObject.name);
                structure.healthBar = GameObject.Find("HealthBarfor2" + gameObject.name);
                temp = GameObject.Find("Enemy_base(Clone)");
                tempName = gameObject.name.Substring(0, 9);
                structure.panel = GameObject.Find("BuildPanelfor2" + tempName);
            }
            //else if (GameObject.Find("Player 1").GetComponent<NetworkIdentity>().connectionToClient == GetComponent<NetworkIdentity>().clientAuthorityOwner)
            else if (GetComponent<NetworkIdentity>().clientAuthorityOwner == GameObject.Find("Player 1").GetComponent<NetworkIdentity>().clientAuthorityOwner)
            {
                Debug.Log("Player 1 has auth for go: " + gameObject.name);
                structure.healthBar = GameObject.Find("HealthBarfor" + gameObject.name);
                temp = GameObject.Find("Base(Clone)");
                tempName = gameObject.name.Substring(0, 9);
                structure.panel = GameObject.Find("BuildPanelfor" + tempName);
            }
            Debug.Log("Temp base = " + temp.name);
            //structure.healthBar = GameObject.Find("HealthBarfor" + gameObject.name);
            structure.HP_Bar = structure.healthBar.GetComponent<Slider>();
            structure.HP_Bar.minValue = 0;
            structure.HP_Bar.maxValue = structure.HPMax;
            structure.name = "Attacking Unit";
            //GameObject temp = GameObject.Find("Base(Clone)");
            structure.BaseUnit = temp.GetComponent<BaseManager>();
            //tempName = gameObject.name.Substring(0, 9);
            //structure.panel = GameObject.Find("BuildPanelfor" + tempName);
            changePanel();
            structure.panel.SetActive(activeMarker);
            targets = new Transform[3];
            RocksMin = 20;
            RocksMax = 40;
            startAngle = gameObject.transform.rotation.z;
            co = rockFlurr();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (GetComponent<NetworkIdentity>().clientAuthorityOwner == null)
        if (!localPlayerAuthority && !hasAuthority)
        {
            return;
        }
        if (!structure.isInConstruction && !structure.isUnderRepair && !structure.isDisoriented)
        {
            if (!started)
            {
                started = true;
                //co = rockFlurr ();
                StartCoroutine(co);
            }
            if (structure.HP <= 0f)
            {
                Destroy(gameObject, 0.1f);
                structure.BaseUnit.reCheckShield();
            }
            structure.HP_Bar.value = structure.HP;
            if (triggeredMissileLaunch && Input.GetMouseButtonUp(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 10000f) && (missileCurrentCharges < 3))
                {
                    if (hit.transform.tag == "Enemy")
                    {
                        target = hit.transform;
                        targets[missileCurrentCharges] = target;
                        MissileChargeAndMove damage = missile.GetComponent<MissileChargeAndMove>();
                        damage.target = target;
                        bool sameTarget = true;
                        for (int i = 0; i < missileCurrentCharges; i++)
                        {
                            if (targets[i].name != target.name)
                                sameTarget = false;
                        }
                        if (sameTarget && Time.realtimeSinceStartup - timeAtMissileLaunch <= 5.0f)
                        {
                            damage.missileDamagePercentage = 5 + missileCurrentCharges * upgradeMultiplier;
                        }
                        else
                        {
                            damage.missileDamagePercentage = 5;
                        }
                        missileCurrentCharges++;
                        timeAtMissileLaunch = Time.realtimeSinceStartup;
                        //Instantiate(missile, gameObject.transform.position, Quaternion.identity);
                        SpawnMissile(missile.gameObject);
                        missile.LookAt(target.position);
                        triggeredMissileLaunch = false;
                        if (missileCurrentCharges == 3)
                        {
                            missileAbilityAvailable = false;
                            StartCoroutine(co);
                            StartCoroutine(rechargeMissile(missileCooldown));
                        }
                    }
                }
            }
            if (mudTriggered && Input.GetMouseButtonUp(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 10000f))
                {
                    if (hit.transform.tag == "enemy_Resource")
                    {
                        MudDrop droplet = mud.GetComponent<MudDrop>();
                        droplet.target = hit.transform;
                        droplet.speedRed = mudImpactSpeed;
                        droplet.dur = mudImpactDuration;
                        //Instantiate(mud, gameObject.transform.position, Quaternion.identity);
                        SpawnMud(mud.gameObject);
                        mud.LookAt(hit.transform.position);
                        StartCoroutine(gatherMud());
                    }
                }
            }
        }
        else
            if (started)
                if (structure.isInConstruction || structure.isUnderRepair || structure.isDisoriented)
                {
                    StopCoroutine(co);
                    started = false;
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
        if (GameObject.Find("Player 7").GetComponent<NetworkIdentity>().connectionToClient == GetComponent<NetworkIdentity>().clientAuthorityOwner)
        {
            GameObject tempOBj = GameObject.Find("BuildPanelfor2" + tempName + "/Text");
            Text panelTitle = tempOBj.GetComponent<Text>();
            panelTitle.text = "Abilities";
            tempOBj = GameObject.Find("BuildPanelfor2" + tempName + "/BuildDef");
            Button btn = tempOBj.GetComponent<Button>();
            Text btnText = btn.GetComponentInChildren<Text>();
            btnText.text = "Missiles";
            btn.onClick.AddListener(() => missileLaunch());
            tempOBj = GameObject.Find("BuildPanelfor2" + tempName + "/BuildAtck");
            tempOBj.SetActive(false);
            tempOBj = GameObject.Find("BuildPanelfor2" + tempName + "/BuildSpec");
            Button btn2 = tempOBj.GetComponent<Button>();
            Text btn2text = btn2.GetComponentInChildren<Text>();
            btn2text.text = "Throw Mud";
            btn2.onClick.AddListener(() => mudSplatter());
        }
        else if (GameObject.Find("Player 1").GetComponent<NetworkIdentity>().connectionToClient == GetComponent<NetworkIdentity>().clientAuthorityOwner)
        {
            GameObject tempOBj = GameObject.Find("BuildPanelfor" + tempName + "/Text");
            Text panelTitle = tempOBj.GetComponent<Text>();
            panelTitle.text = "Abilities";
            tempOBj = GameObject.Find("BuildPanelfor" + tempName + "/BuildDef");
            Button btn = tempOBj.GetComponent<Button>();
            Text btnText = btn.GetComponentInChildren<Text>();
            btnText.text = "Missiles";
            btn.onClick.AddListener(() => missileLaunch());
            tempOBj = GameObject.Find("BuildPanelfor" + tempName + "/BuildAtck");
            tempOBj.SetActive(false);
            tempOBj = GameObject.Find("BuildPanelfor" + tempName + "/BuildSpec");
            Button btn2 = tempOBj.GetComponent<Button>();
            Text btn2text = btn2.GetComponentInChildren<Text>();
            btn2text.text = "Throw Mud";
            btn2.onClick.AddListener(() => mudSplatter());
        }
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
        throwingRocks = true;
        yield return new WaitForSeconds(20);
        int noRocks = Random.Range(RocksMin, RocksMax);
        float delayBetweenRockThrows = 20f / (float)noRocks;
        float shootPeriod = 20f;
        while (shootPeriod - delayBetweenRockThrows >= 0)
        {
            yield return new WaitForSeconds(delayBetweenRockThrows);
            rockFlurrShooting();
            shootPeriod = shootPeriod - delayBetweenRockThrows;
        }
        StartCoroutine(co);
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
        shootingPosition = new Vector3(x, y, z);
        Quaternion rotation = new Quaternion(gameObject.transform.rotation.x, gameObject.transform.rotation.y, gameObject.transform.rotation.z, 1);
        RockBehavior mov = projectile.GetComponent<RockBehavior>();
        mov.direction = new Vector3(x - gameObject.transform.position.x, 0f, z - gameObject.transform.position.z);
        mov.gameObject.tag = this.gameObject.tag;
        //Instantiate(projectile, shootingPosition, rotation);
        SpawnRock(projectile.gameObject);
        projectile.position = shootingPosition;
        if (topToBottom)
            if (startAngle < 180)
            {
                startAngle += 1;
            }
            else
                topToBottom = false;
        else if (startAngle > 0)
            startAngle -= 1;
        else
            topToBottom = true;
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
            if (BaseManager.resources - structure.costs[1] >= 0)
            {
                BaseManager.resources -= structure.costs[1];
                BaseManager.notEnough = "";
                triggeredMissileLaunch = true;
                StopCoroutine(co);
                activeMarker = false;
                structure.panel.SetActive(activeMarker);
            }
            else
                BaseManager.notEnough = "not enough resources";
        }
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
        if (mudReady)
        {
            if (BaseManager.resources - structure.costs[2] >= 0)
            {
                BaseManager.resources -= structure.costs[2];
                BaseManager.notEnough = "";
                mudTriggered = true;
                mudReady = false;
            }
            else
                BaseManager.notEnough = "not enough resources";
        }
    }

    IEnumerator gatherMud()
    {
        mudTriggered = false;
        yield return new WaitForSeconds(40);
        mudReady = true;
    }

    public void upgrade(float upgradeDuration)
    {
        structure.upgrades++;
        if (structure.upgrades == 1)
        {
            if (BaseManager.resources - structure.costs[3] >= 0)
            {
                BaseManager.resources -= structure.costs[3];
                BaseManager.notEnough = "";
                StartCoroutine(structure.waitConstruction(upgradeDuration, structure.colorUnit)); //needs to be 20;
                upgradeMultiplier = 2;
                mudImpactSpeed = 0.6f;
                mudImpactDuration = 30f;
            }
            else
                BaseManager.notEnough = "not enough resources";
        }
        if (structure.upgrades == 2)
        {
            if (BaseManager.resources - structure.costs[4] >= 0)
            {
                BaseManager.resources -= structure.costs[4];
                BaseManager.notEnough = "";
                StartCoroutine(structure.waitConstruction(upgradeDuration, structure.colorUnit));
                missileCooldown = 15f;
                RocksMax = 60;
                RocksMin = 40;
            }
            else
                BaseManager.notEnough = "not enough resources";
        }
    }

    public string status()
    {
        message = " ";
        if (structure)
        {
            if (structure.isInConstruction)
            {
                message = "building";
                return message;
            }
            else if (structure.isUnderRepair)
            {
                if (structure.upgrades == 0)
                {
                    message = "repairing";
                    return message;
                }
                else
                {
                    message = "upgrading";
                    return message;
                }
            }
            else
            {
                if (!mudReady)
                    message += "gathering mud;";
                else
                    message += " mud ready;";

                if (!missileAbilityAvailable)
                    message += " preparing missiles;";
                else
                    message += " missiles ready;";

                return message;
            }
        }
        else
            return message;
    }

    [ClientCallback]
    void SpawnMissile(GameObject missile)
    {
        int missileIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(missile);
        GameObject player = GameObject.FindWithTag("MainCamera");//The localplayer is the only one with camera enabled.
        CmdSpawnMissile(missileIndex, player);
    }

    [Command]
    void CmdSpawnMissile(int missileIndex, GameObject player)
    {
        GameObject missileToLaunch = NetworkManager.singleton.spawnPrefabs[missileIndex];
        GameObject go = GameObject.Instantiate(missileToLaunch);
        go.transform.position = this.gameObject.transform.position;
        NetworkServer.SpawnWithClientAuthority(go, player);
    }

    [ClientCallback]
    void SpawnMud(GameObject mud)
    {
        int mudIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(mud);
        GameObject player = GameObject.FindWithTag("MainCamera");//The localplayer is the only one with camera enabled.
        CmdSpawnMud(mudIndex, player);
    }

    [Command]
    void CmdSpawnMud(int mudIndex, GameObject player)
    {
        GameObject mud = NetworkManager.singleton.spawnPrefabs[mudIndex];
        GameObject go = GameObject.Instantiate(mud);
        go.transform.position = this.gameObject.transform.position;
        NetworkServer.SpawnWithClientAuthority(go, player);
    }

    [ClientCallback]
    void SpawnRock(GameObject rock)
    {
        int rockIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(rock);
        GameObject player = GameObject.FindWithTag("MainCamera");//The localplayer is the only one with camera enabled.
        CmdSpawnRock(rockIndex, player);
    }

    [Command]
    void CmdSpawnRock(int rockIndex, GameObject player)
    {
        GameObject rock = NetworkManager.singleton.spawnPrefabs[rockIndex];
        GameObject go = GameObject.Instantiate(rock);
        go.transform.position = shootingPosition;
        NetworkServer.SpawnWithClientAuthority(go, player);
    }
}
