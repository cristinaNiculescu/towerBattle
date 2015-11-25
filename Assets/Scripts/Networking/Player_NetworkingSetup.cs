using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Events;

public class Player_NetworkingSetup : NetworkBehaviour
{
    public List<GameObject> unitSpots = new List<GameObject>();

    bool hasChecked = false;
    public List<Transform> prefabUnits = new List<Transform>();
    public Button[] btns;
    public GameObject[] unitSpotsSpawned;

    public override void OnStartLocalPlayer()
    {
        if (base.isLocalPlayer)
        {
            if (this.isClient)
            {
                Debug.Log("Client " + base.netId);
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        //Enable all local Player Components
        if (isLocalPlayer)
        {
            GetComponent<Camera>().enabled = true;
            GetComponent<FlareLayer>().enabled = true;
            GetComponent<GUILayer>().enabled = true;
            GetComponent<AudioListener>().enabled = true;
            //GetComponent<CameraController>().enabled = true;
            for (int i = 0; i < unitSpots.Count; i++)
            {
                SpawnUnitSpots(unitSpots[i]);
            }
            int k = 0;
            unitSpotsSpawned = GameObject.FindGameObjectsWithTag("UnitSpots");
            foreach (GameObject unitSpot in unitSpotsSpawned)
            {
                GameObject panel = GameObject.Find("BuildPanelforUnitSpot" + (k + 1));//Plus one because the numbering of panels start from '1' and ends with '5'.
                Debug.Log("Panel found = " + panel.name);
                btns = panel.GetComponentsInChildren<Button>();
                int j = 0;
                foreach (Button btn in btns)
                {
                    // btns[j].onClick.AddListener( delegate {unitSpotsSpawned[i].GetComponent<UnitConstruction>().build(unitSpotsSpawned[i].transform);});
                    //btn.onClick.AddListener(delegate { build(unitSpot, j); });
                    AddListener(btn, unitSpot, j);
                    j++;
                }
                k++;
            }
        }
    }

    void AddListener(Button b, GameObject obj, int value)
    {
        b.onClick.AddListener(() => build(obj, value));
    }

    public void build(GameObject obj, int value)
    {
        Debug.Log("Wazzaup!......." + prefabUnits.Count);
        obj.GetComponent<UnitConstruction>().build(prefabUnits[value]);
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;
    }

    [ClientCallback]
    public void SpawnUnitSpots(GameObject unitSpot)
    {
        var go = (GameObject)Instantiate(unitSpot);
        CmdSpawnUnitSpots(go);
    }

    [Command]
    public void CmdSpawnUnitSpots(GameObject unitSpot)
    {
        //var go = (GameObject)Instantiate(unitSpot);
        //NetworkServer.Spawn(unitSpot);
        NetworkServer.SpawnWithClientAuthority(unitSpot, base.connectionToClient);
    }

    //[ClientCallback]
    //public void AssignAuthToMe()
    //{
    //    CmdAssignAuthority();
    //    Debug.Log("NetworkManager.singleton.client.connection " + NetworkManager.singleton.client.connection);
    //    Debug.Log("base.connectionToClient " + base.connectionToClient);
    //    hasChecked = true;
    //}

    //[Command]
    //public void CmdAssignAuthority()
    //{
    //    Dictionary<NetworkInstanceId, NetworkIdentity> serverobjects = NetworkServer.objects;
    //    foreach (var key in serverobjects.Keys)
    //    {
    //        string authBefore = (this.hasAuthority) ? "Mine" : "Theirs";
    //        Debug.Log("Before " + authBefore + ", Value Name = " + serverobjects[key].name + ", NetworkIdentity: " + serverobjects[key].GetComponent<NetworkIdentity>().netId);
    //        serverobjects[key].GetComponent<NetworkIdentity>().AssignClientAuthority(NetworkManager.singleton.client.connection);
    //        string authAfter = (this.hasAuthority) ? "Mine" : "Theirs";
    //        Debug.Log("After " + authAfter + ", Value Name = " + serverobjects[key].name + ", NetworkIdentity: " + serverobjects[key].GetComponent<NetworkIdentity>().netId);
    //    }
    //}
}
