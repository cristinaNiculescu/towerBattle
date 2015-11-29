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

    // Update is called once per frame
    void Update()
    {
        if (!localPlayerAuthority && hasAuthority)
            return;
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
                //Debug.Log(activeMarker);
            }
        }
    }

    public void build(GameObject unit)
    {
        unit.name = gameObject.name;
        int index = (int)(gameObject.name[gameObject.name.Length - 1]);

        int constructionCost = unit.GetComponent<UnitStructure>().costs[0];

        if (BaseManager.resources - constructionCost >= 0)
        {

            BaseManager.resources -= constructionCost;
            BaseManager.notEnough = "";
            hpbar.SetActive(true);



            unit.transform.LookAt(GameObject.FindWithTag("Enemy").transform.position);
            int ID = Instantiate(unit, gameObject.transform.position, Quaternion.identity).GetInstanceID();

            GameObject[] instanceArray = GameObject.FindGameObjectsWithTag(unit.tag);
            for (int i = 0; i < instanceArray.Length; i++)
            {
                if (instanceArray[i].GetInstanceID() == ID)
                    BaseUnit.UnitsBuilt[index - 49] = instanceArray[i];
                BaseUnit.reCheckShield();
            }
            Destroy(gameObject);
        }
        else BaseManager.notEnough = "not enough resources";
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
        //NetworkServer.UnSpawn(unspawnedObj);
        NetworkServer.Destroy(unspawnedObj);
        //For the server we don't want to see it, but it will stil exists, because we need the reference to the old object due to buttons listeners.
        unspawnedObj.GetComponent<MeshRenderer>().enabled = false;
    }

    [Command]
    public void CmdBuildUnit(int unitIndex, GameObject player)
    {
        GameObject unitToBuild = NetworkManager.singleton.spawnPrefabs[unitIndex];
        GameObject go = GameObject.Instantiate(unitToBuild);
        NetworkServer.SpawnWithClientAuthority(go, player);
    }
}
