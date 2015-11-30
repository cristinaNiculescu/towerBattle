using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class UnitConstruction : NetworkBehaviour
{
    public GameObject cs;
    bool activeMarker = false;
    Vector3 tempPosition;
    GameObject panel;
    GameObject hpbar;
    bool canBeClicked;
    BaseManager BaseUnit;

    void Start()
    {
        AssignUnitSpotsToPlayer();
    }

    /// <summary>
    /// Assign UnitSpots to the rightful player.
    /// </summary>
    private void AssignUnitSpotsToPlayer()
    {
        if (localPlayerAuthority && hasAuthority)
        {
            if (GameObject.Find("Player 7") != null)
            {
                GameObject clientPlayer2 = GameObject.Find("Player 7");//Player 2
                Debug.Log("Heya from player : " + clientPlayer2.GetComponent<NetworkBehaviour>().netId.Value);
                clientPlayer2.GetComponent<Player_NetworkingSetup>().unitSpotsSpawned.Add(this.gameObject);
            }
            else
            {
                GameObject clientPlayer1 = GameObject.Find("Player 1");//Player 1
                Debug.Log("Heya from player : " + clientPlayer1.GetComponent<NetworkBehaviour>().netId.Value);
                clientPlayer1.GetComponent<Player_NetworkingSetup>().unitSpotsSpawned.Add(this.gameObject);
            }
        }
    }

    void OnMouseEnter()
    {
        if (localPlayerAuthority && hasAuthority)
            canBeClicked = true;
    }
    void OnMouseExit()
    {
        if (localPlayerAuthority && hasAuthority)
            canBeClicked = false;
    }

    void OnMouseUp()
    {
        if (localPlayerAuthority && hasAuthority)
        {
            if (canBeClicked)
            {
                panel.SetActive(activeMarker);
                activeMarker = !activeMarker;
            }
        }
    }

    public void build(GameObject unit, GameObject player)
    {
        if (localPlayerAuthority && hasAuthority)
        {
            unit.name = gameObject.name.Substring(0, 9);
            int index = (int)(gameObject.name[gameObject.name.Length - 1]);
            int constructionCost = unit.GetComponent<UnitStructure>().costs[0];
            if (BaseManager.resources - constructionCost >= 0)
            {
                BaseManager.resources -= constructionCost;
                BaseManager.notEnough = "";
                hpbar.SetActive(true);
                BuildUnit(unit.gameObject, player);//Build the unit.
                //int ID = Instantiate(unit, gameObject.transform.position, Quaternion.identity).GetInstanceID();
                unit.transform.position = this.gameObject.transform.position;
                uint ID = unit.GetComponent<NetworkIdentity>().netId.Value;
                //GameObject[] instanceArray = GameObject.FindGameObjectsWithTag(unit.tag);
                GameObject[] instanceArray = player.GetComponent<Player_NetworkingSetup>().unitSpotsSpawned.ToArray();
                for (int i = 0; i < instanceArray.Length; i++)
                {
                    //if (instanceArray[i].GetInstanceID() == ID)
                    if (instanceArray[i].GetComponent<NetworkIdentity>().netId.Value == ID)
                        BaseUnit.UnitsBuilt[index - 49] = instanceArray[i];
                    BaseUnit.reCheckShield();
                }
                //Destroy(gameObject);
                Unspawn(gameObject);
            }
            else BaseManager.notEnough = "not enough resources";
        }
    }

    public void SetupCanvas()
    {
        if (GameObject.Find("CanvasClient(Clone)") != null)//Player 2
        {
            Debug.Log("Yay found the Canvas...[PLAYER 2]...(Clone)");
            cs = GameObject.Find("CanvasClient(Clone)");
            if (cs != null)
            {
                string name = this.gameObject.name.Substring(0, 9);
                panel = GameObject.Find("BuildPanelfor2" + name);
                print("Player 2 : " + panel != null);
                panel.SetActive(false);
                hpbar = GameObject.Find("HealthBarfor2" + name + "(Clone)");
                hpbar.SetActive(false);
                GameObject temp = GameObject.Find("Enemy_base(Clone)");
                BaseUnit = temp.GetComponent<BaseManager>();
            }
        }
        else if (GameObject.Find("Canvas(Clone)") != null)//Player 1
        {
            Debug.Log("Yay found the Canvas...[PLAYER 1]...(Clone)");
            cs = GameObject.Find("Canvas(Clone)");
            if (cs != null)
            {
                string name = base.gameObject.name.Substring(0, 9);
                panel = GameObject.Find("BuildPanelfor" + name);
                print(panel != null);
                panel.SetActive(false);
                hpbar = GameObject.Find("HealthBarfor" + name + "(Clone)");
                hpbar.SetActive(false);
                GameObject temp = GameObject.Find("Base(Clone)");
                BaseUnit = temp.GetComponent<BaseManager>();
            }
        }
        else
        {
            Debug.Log("T_T.... no canvas!");
        }
    }

    [ClientCallback]
    void Unspawn(GameObject obj)
    {
        NetworkInstanceId unitId = obj.GetComponent<NetworkIdentity>().netId;
        CmdUnspawnUnit(unitId);
    }

    [ClientCallback]
    void BuildUnit(GameObject theUnitToBuild, GameObject player)
    {
        int theUnitToBuildIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(theUnitToBuild);
        CmdBuildUnit(theUnitToBuildIndex, player);
    }

    [Command]
    public void CmdUnspawnUnit(NetworkInstanceId objID)
    {
        GameObject unspawnedObj = NetworkServer.FindLocalObject(objID);
        NetworkServer.UnSpawn(unspawnedObj);
        //NetworkServer.Destroy(unspawnedObj);
        //For the server we don't want to see it, but it will stil exists, because we need the reference to the old object due to buttons listeners.
        //unspawnedObj.GetComponent<MeshRenderer>().enabled = false;
    }

    [Command]
    public void CmdBuildUnit(int unitIndex, GameObject player)
    {
        GameObject unitToBuild = NetworkManager.singleton.spawnPrefabs[unitIndex];
        GameObject go = GameObject.Instantiate(unitToBuild);
        NetworkServer.SpawnWithClientAuthority(go, player);
        Debug.Log("Server: player to give auth = " + go.GetComponent<NetworkIdentity>().clientAuthorityOwner);
    }
}
