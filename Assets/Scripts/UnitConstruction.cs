using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class UnitConstruction : NetworkBehaviour
{
    public NetworkConnection myPlayerConnection = null;
    public GameObject cs;
    bool activeMarker = false;
    Vector3 tempPosition;
    GameObject panel;
    GameObject hpbar;
    bool canBeClicked;
    BaseManager BaseUnit;

    // Use this for initialization
    void Start()
    {
        cs = GameObject.Find("Canvas");
        if (cs != null)
        {
            string name = gameObject.name.Substring(0, 9);
            Debug.Log(name);
            panel = GameObject.Find("BuildPanelfor" + name);
            panel.SetActive(false);
            hpbar = GameObject.Find("HealthBarfor" + name + "(Clone)");
            hpbar.SetActive(false);
            GameObject temp = GameObject.Find("Base");
            BaseUnit = temp.GetComponent<BaseManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseEnter()
    {
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
            panel.SetActive(activeMarker);
            activeMarker = !activeMarker;
            //Debug.Log (activeMarker);
        }
    }

    public void build(Transform unit)
    {
        Debug.Log("I was called to arms!");
        if (unit.GetComponent<UnitStructure>() != null)
        {
            Debug.Log("localPlayerAuthority = " + localPlayerAuthority + ", hasAuthority = " + hasAuthority);
            string name = gameObject.name.Substring(0, 9);
            unit.name = name;
            int index = (int)(gameObject.name[gameObject.name.Length - 1]);
            int constructionCost = unit.GetComponent<UnitStructure>().costs[0];
            if (BaseManager.resources - constructionCost >= 0)
            {
                BaseManager.resources -= constructionCost;
                BaseManager.notEnough = "";
                hpbar.SetActive(true);
                //BaseUnit.UnitsBuilt[index - 49] = unit;

                BaseUnit.reCheckShield();
                unit.transform.LookAt(GameObject.FindWithTag("Enemy").transform.position);
                unit.transform.position = gameObject.transform.position;
                SetupConnectionToClient(unit.gameObject);
                CmdBuildUnit(unit.gameObject);
                CmdDestroydUnit(gameObject);
            }
            else BaseManager.notEnough = "not enough resources";
        }
        else
        {
            Debug.Log("T_T");
        }
    }

    /// <summary>
    /// Setup a connection to client currently playing.
    /// Assign the connection to the variable myPlayerConnection.
    /// </summary>
    private void SetupConnectionToClient(GameObject unit)
    {
        List<PlayerController> playerControllers = NetworkManager.singleton.client.connection.playerControllers;
        foreach (PlayerController playerController in playerControllers)
        {
            Debug.Log("PlayerController unetView = " + playerController.unetView);
            myPlayerConnection = playerController.unetView.clientAuthorityOwner;
            if (myPlayerConnection == null)
            {
                myPlayerConnection = NetworkManager.singleton.client.connection;
                Debug.Log("myPlayerConnection was null = " + myPlayerConnection);
                GameObject obj = GameObject.FindWithTag("MainCamera");
                Debug.Log("Camera name = " + obj.name);
            }
            Debug.Log("myPlayerConnection = " + myPlayerConnection);
        }
    }

    [Command]
    public void CmdBuildUnit(GameObject unit)
    {
        GameObject obj = Instantiate(unit, unit.transform.position, Quaternion.identity) as GameObject;
        Debug.Log("OBJ - clientAuthorityOwner before = " + obj.GetComponent<NetworkIdentity>().clientAuthorityOwner);
        Debug.Log("localPlayerAuth before = " + obj.GetComponent<NetworkIdentity>().localPlayerAuthority);
        //if (base.isLocalPlayer && myPlayerConnection != null)
        //{
        //    obj.GetComponent<NetworkIdentity>().RemoveClientAuthority(myPlayerConnection);
        //    Debug.Log("LALAASDASDasD");
        //}
        NetworkServer.SpawnWithClientAuthority(obj, myPlayerConnection);
        Debug.Log("OBJ - clientAuthorityOwner after = " + obj.GetComponent<NetworkIdentity>().clientAuthorityOwner);
        Debug.Log("OBJ - localPlayerAuth after = " + obj.GetComponent<NetworkIdentity>().localPlayerAuthority);
    }

    [Command]
    public void CmdDestroydUnit(GameObject obj)
    {
        NetworkServer.Destroy(obj);
        Debug.Log("DESTROY! = " + obj.name);
    }
}
