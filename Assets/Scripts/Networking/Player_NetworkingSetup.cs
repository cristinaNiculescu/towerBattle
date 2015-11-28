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
    public GameObject clientCanvas;
    public GameObject playerBase;
    public GameObject enemyBase;
    public Button[] btns;
    public GameObject[] unitSpotsSpawned;
    public uint netIDValue = 0;

    public override void OnStartLocalPlayer()
    {
        if (base.isLocalPlayer)
        {
            if (this.isClient)
            {
                Debug.Log("Client net ID " + base.netId.Value);
                netIDValue = base.netId.Value;
            }
        }
    }

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
            if (base.netId.Value == 1)
            {
                Instantiate(canvas);
            }
            else
                Instantiate(clientCanvas);

            Instantiate(playerBase);
            Instantiate(enemyBase);
            //SpawnPlayerSetup(canvas, playerBase, enemyBase, gameObject);
            for (int i = 0; i < unitSpots.Count; i++)
            {
                SpawnUnitSpots(unitSpots[i], this.gameObject);
            }
            int k = 0;
            unitSpotsSpawned = GameObject.FindGameObjectsWithTag("UnitSpots");
            foreach (GameObject unitSpot in unitSpotsSpawned)
            {
                GameObject panel = GameObject.Find("BuildPanelforUnitSpot" + (k + 1));//Plus one because the numbering of panels start from '1' and ends with '5'.
                //Debug.Log("Panel found = " + panel.name);
                btns = panel.GetComponentsInChildren<Button>();
                int j = 0;
                foreach (Button btn in btns)
                {
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
    public void SpawnUnitSpots(GameObject unitSpot, GameObject player)
    {
        int prefabIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(unitSpot);
        //Debug.Log("We are about to Spawn = " + unitSpot);
        CmdSpawnUnitSpots(prefabIndex, player);
    }

    //[ClientCallback]
    //public void SpawnPlayerSetup(GameObject prefab1, GameObject prefab2, GameObject prefab3, GameObject player)
    //{
    //    int prefabIndex1 = NetworkManager.singleton.spawnPrefabs.IndexOf(prefab1);
    //    int prefabIndex2 = NetworkManager.singleton.spawnPrefabs.IndexOf(prefab2);
    //    int prefabIndex3 = NetworkManager.singleton.spawnPrefabs.IndexOf(prefab3);
    //    CmdSpawnPlayerSetup(prefabIndex1, player);//Canvas
    //    CmdSpawnPlayerSetup(prefabIndex2, player);//PlayerBase
    //    CmdSpawnPlayerSetup(prefabIndex3, player);//EnemyBase
    //}

    [Command]
    //public void CmdSpawnUnitSpots(GameObject spawnUnitSpot, GameObject thePlayer)
    public void CmdSpawnUnitSpots(int spawnIndex, GameObject thePlayer)
    {
        GameObject unitSpawned = NetworkManager.singleton.spawnPrefabs[spawnIndex];
        GameObject go = null;
        if (base.connectionToClient.connectionId == -1)//The Host must not spawn anything... 
        {
            go = (GameObject)Instantiate(unitSpawned);
            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z);
            NetworkServer.SpawnWithClientAuthority(go, thePlayer);//Works! Assigns Auhtority to thePlayer GameObject.
            //Debug.Log("Server go auth? " + go.GetComponent<NetworkIdentity>().clientAuthorityOwner);
        }
        else
        {
            go = (GameObject)Instantiate(unitSpawned);
            go.transform.position = new Vector3(-go.transform.position.x, go.transform.position.y, -go.transform.position.z);
            NetworkServer.SpawnWithClientAuthority(go, thePlayer);//Works! Assigns Auhtority to thePlayer GameObject.
            //Debug.Log("Client go auth? " + go.GetComponent<NetworkIdentity>().clientAuthorityOwner);
        }
    }
}
