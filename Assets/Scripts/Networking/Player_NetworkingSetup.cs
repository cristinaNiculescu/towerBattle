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
    public GameObject canvas;
    public GameObject playerBase;
    public GameObject enemyBase;
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
        Debug.Log("playerControllerID : " + playerControllerId);

        //Enable all local Player Components
        if (isLocalPlayer)
        {
            GetComponent<Camera>().enabled = true;
            GetComponent<FlareLayer>().enabled = true;
            GetComponent<GUILayer>().enabled = true;
            GetComponent<AudioListener>().enabled = true;
            Instantiate(canvas);
            Instantiate(playerBase);
            Instantiate(enemyBase);
            //GetComponent<CameraController>().enabled = true;

            for (int i = 0; i < unitSpots.Count; i++)
            {
                SpawnUnitSpots(unitSpots[i], gameObject);
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

    void Update()
    {
        if (!isLocalPlayer)
            return;
    }

    void AddListener(Button b, GameObject obj, int value)
    {
        b.onClick.AddListener(() => build(obj, value));
    }

    public void build(GameObject obj, int value)
    {
        obj.GetComponent<UnitConstruction>().build(prefabUnits[value]);
    }

    [ClientCallback]
    //public void SpawnUnitSpots(GameObject unitSpot, GameObject player)
    public void SpawnUnitSpots(GameObject unitSpot, GameObject player)
    {
        int prefabIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(unitSpot);
        //Debug.Log("We are about to Spawn = " + unitSpot);
        foreach (GameObject obj in NetworkManager.singleton.spawnPrefabs)
        {
            Debug.Log("OBJ = " + obj.name);
        }
        CmdSpawnUnitSpots(prefabIndex, player);
    }

    [Command]
    //public void CmdSpawnUnitSpots(GameObject spawnUnitSpot, GameObject thePlayer)
    public void CmdSpawnUnitSpots(int spawnIndex, GameObject thePlayer)
    {
        GameObject unitSpawned = NetworkManager.singleton.spawnPrefabs[spawnIndex];
        //Debug.Log("spawnUnitSpot = " + unitSpawned + ", thePlayer = " + thePlayer);
        //var go = (GameObject)Instantiate(spawnUnitSpot);
        var go = (GameObject)Instantiate(unitSpawned);
        NetworkServer.SpawnWithClientAuthority(go, thePlayer);//Works! Assigns Auhtority to thePlayer GameObject.
        //NetworkServer.Spawn(go);
        Debug.Log("go auth? " + go.GetComponent<NetworkIdentity>().clientAuthorityOwner);
    }
}
