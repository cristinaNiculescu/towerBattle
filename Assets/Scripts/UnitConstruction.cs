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
                //Debug.Log("Heya from player : " + clientPlayer2.GetComponent<NetworkBehaviour>().netId.Value);
                clientPlayer2.GetComponent<Player_NetworkingSetup>().unitSpotsSpawned.Add(this.gameObject);
            }
            else
            {
                GameObject clientPlayer1 = GameObject.Find("Player 1");//Player 1
                //Debug.Log("Heya from player : " + clientPlayer1.GetComponent<NetworkBehaviour>().netId.Value);
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
                RemoveListenersFromUnitSpot(player);//Remove all listeners before we spawn the new unit to replace this Spot.
                BuildUnit(unit.gameObject, player);//Build the unit.
                uint ID = unit.GetComponent<NetworkIdentity>().netId.Value;
                Debug.Log("The ID of the Unit : " + ID);
                GameObject[] instanceArray = GameObject.FindGameObjectsWithTag(unit.tag);
                for (int i = 0; i < instanceArray.Length; i++)
                {
                    if (instanceArray[i].GetInstanceID() == ID)
                        BaseUnit.UnitsBuilt[index - 49] = instanceArray[i];
                    BaseUnit.reCheckShield();
                }
                //Destroy(gameObject);
                Debug.Log(gameObject.name + ", gameObject to be UnSpawned");
                Unspawn(gameObject);
            }
            else BaseManager.notEnough = "not enough resources";
        }
    }

    /// <summary>
    /// Remove unused listeners related to the UnitSpot.
    /// </summary>
    /// <param name="player"></param>
    private void RemoveListenersFromUnitSpot(GameObject player)
    {
        if (player.name == "Player 1")
        {
            GameObject panel = GameObject.Find("BuildPanelfor" + gameObject.name.Substring(0, 9));
            Button[] btns = panel.GetComponentsInChildren<Button>();
            foreach (Button btn in btns)
            {
                btn.onClick.RemoveAllListeners();
            }
        }
        else if (player.name == "Player 7")
        {
            GameObject panel = GameObject.Find("BuildPanelfor2" + gameObject.name.Substring(0, 9));
            Button[] btns = panel.GetComponentsInChildren<Button>();
            foreach (Button btn in btns)
            {
                btn.onClick.RemoveAllListeners();
            }
        }
    }

    public void SetupCanvas()
    {
        if (GameObject.Find("CanvasClient(Clone)") != null)//Player 2
        {
            cs = GameObject.Find("CanvasClient(Clone)");
            if (cs != null)
            {
                string name = this.gameObject.name.Substring(0, 9);
                panel = GameObject.Find("BuildPanelfor2" + name);
                panel.SetActive(false);
                hpbar = GameObject.Find("HealthBarfor2" + name + "(Clone)");
                hpbar.SetActive(false);
                GameObject temp = GameObject.Find("Enemy_base(Clone)");
                BaseUnit = temp.GetComponent<BaseManager>();
            }
        }
        else if (GameObject.Find("Canvas(Clone)") != null)//Player 1
        {
            cs = GameObject.Find("Canvas(Clone)");
            if (cs != null)
            {
                string name = base.gameObject.name.Substring(0, 9);
                panel = GameObject.Find("BuildPanelfor" + name);
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
        //RpcSetInvisible(objID);
    }

    [Command]
    public void CmdBuildUnit(int unitIndex, GameObject player)
    {
        GameObject unitToBuild = NetworkManager.singleton.spawnPrefabs[unitIndex];
        GameObject go = GameObject.Instantiate(unitToBuild);
        go.transform.position = this.gameObject.transform.position;
        NetworkServer.SpawnWithClientAuthority(go, player);
        Debug.Log("Server: player to give auth = " + go.GetComponent<NetworkIdentity>().clientAuthorityOwner);
    }

    [ClientRpc]
    void RpcSetInvisible(NetworkInstanceId unitSpot)
    {
        ClientScene.FindLocalObject(unitSpot).GetComponent<MeshRenderer>().enabled = false;
    }
}
