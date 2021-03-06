﻿using UnityEngine;
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

    public void build(Transform unit)
    {
        if (unit.GetComponent<UnitStructure>() != null && cs != null)
        {
            string name = gameObject.name.Substring(0, 9);
            unit.name = name;
            //int index = (int)(gameObject.name[gameObject.name.Length - 1]);
            int constructionCost = unit.GetComponent<UnitStructure>().costs[0];
            if (BaseManager.resources - constructionCost >= 0)
            {
                BaseManager.resources -= constructionCost;
                BaseManager.notEnough = "";
                Debug.Log("cs name is = " + cs.name);
                hpbar.SetActive(true);
                //BaseUnit.UnitsBuilt[index - 49] = unit;
                BaseUnit.reCheckShield();
                unit.transform.position = this.gameObject.transform.position;
                unit.transform.LookAt(GameObject.FindWithTag("Enemy").transform.position);
                GameObject theLocaPlayerObject = GameObject.FindWithTag("MainCamera");
                Debug.Log("theLocaPlayerObject " + theLocaPlayerObject.name);
                BuildUnit(unit.gameObject, theLocaPlayerObject);
                Unspawn(gameObject);
            }
            else BaseManager.notEnough = "not enough resources";
        }
        else
        {
            Debug.Log("T_T");
        }
    }

    public void SetupCanvas()
    {
        if (GameObject.Find("Canvas(Clone)") != null)
        {
            Debug.Log("Yay found the Canvas...[SERVER]...(Clone)");
            cs = GameObject.Find("Canvas(Clone)");//The Host will search for this....
            if (cs != null)
            {
                string name = base.gameObject.name.Substring(0, 9);
                //Debug.Log(name);// UnitSpot names
                panel = GameObject.Find("BuildPanelfor" + name);
                //GameObject player = GameObject.FindWithTag("MainCamera");
                panel.SetActive(false);
                hpbar = GameObject.Find("HealthBarfor" + name + "(Clone)");
                hpbar.SetActive(false);
                //GameObject temp = GameObject.Find("Base");
                GameObject temp = GameObject.Find("Base(Clone)");
                BaseUnit = temp.GetComponent<BaseManager>();
            }
        }
        else if (GameObject.Find("CanvasClient(Clone)") != null)
        {
            Debug.Log("Yay found the Canvas...[CLIENT]...(Clone)");
            cs = GameObject.Find("CanvasClient(Clone)");//The Host will search for this....
            if (cs != null)
            {
                string name = this.gameObject.name.Substring(0, 9);
                Debug.Log(name);// UnitSpot names
                panel = GameObject.Find("BuildPanelfor" + name);
                Debug.Log("panel name : " + panel.name);
                //GameObject player = GameObject.FindWithTag("MainCamera");
                panel.SetActive(false);
                hpbar = GameObject.Find("HealthBarfor" + name + "(Clone)");
                Debug.Log("hpbar name : " + hpbar.name);
                hpbar.SetActive(false);
                //GameObject temp = GameObject.Find("Base");
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
