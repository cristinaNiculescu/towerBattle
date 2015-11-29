using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Events;

public class Player_NetworkingSetup : NetworkBehaviour
{
    public List<GameObject> unitSpots = new List<GameObject>();
    public List<Transform> prefabUnits = new List<Transform>();
    public GameObject canvas;
    public GameObject clientCanvas;
    public GameObject playerBase;
    public GameObject enemyBase;
    public Button[] btns;
    public List<GameObject> unitSpotsSpawned;
    bool hasChecked = false;

    public override void OnStartLocalPlayer()
    {
        GetComponent<Camera>().enabled = true;
        GetComponent<FlareLayer>().enabled = true;
        GetComponent<GUILayer>().enabled = true;
        GetComponent<AudioListener>().enabled = true;
        //GetComponent<CameraController>().enabled = true;
    }

    void Start()
    {
        if (base.netId.Value == 1)
        {
            Instantiate(canvas);
            Instantiate(playerBase);
        }
        else if (base.netId.Value == 7)
        {
            Instantiate(clientCanvas);
            Instantiate(enemyBase);
        }
        for (int i = 0; i < unitSpots.Count; i++)
        {
            SpawnUnitSpots(unitSpots[i], this.gameObject);
        }
        //int k = 1;//Begin from '1' because the numbering of panels start from '1' and ends with '5'.
        //foreach (GameObject unitSpot in unitSpotsSpawned)
        //{
        //    GameObject panel = GameObject.Find("BuildPanelforUnitSpot" + (k));
        //    btns = panel.GetComponentsInChildren<Button>();
        //    int j = 0;
        //    foreach (Button btn in btns)
        //    {
        //        AddListener(btn, unitSpot, j);
        //        j++;
        //    }
        //    k++;
        //    unitSpot.GetComponent<UnitConstruction>().SetupCanvas();
        //}
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;
        if (unitSpotsSpawned.Count == 5 && !hasChecked)
        {
            int k = 1;//Begin from '1' because the numbering of panels start from '1' and ends with '5'.
            foreach (GameObject unitSpot in unitSpotsSpawned)
            {
                GameObject panel = GameObject.Find("BuildPanelforUnitSpot" + (k));
                btns = panel.GetComponentsInChildren<Button>();
                int j = 0;
                foreach (Button btn in btns)
                {
                    AddListener(btn, unitSpot, j);
                    j++;
                }
                k++;
                unitSpot.GetComponent<UnitConstruction>().SetupCanvas();
            }
            hasChecked = true;
        }
    }

    void AddListener(Button b, GameObject obj, int value)
    {
        b.onClick.AddListener(() => build(obj, value));
    }

    public void build(GameObject obj, int value)
    {
        obj.GetComponent<UnitConstruction>().build(prefabUnits[value].gameObject);
    }

    [ClientCallback]
    public void SpawnUnitSpots(GameObject unitSpot, GameObject player)
    //public void SpawnUnitSpots(GameObject unitSpot, NetworkConnection player)
    {
        int prefabIndex = NetworkManager.singleton.spawnPrefabs.IndexOf(unitSpot);
        CmdSpawnUnitSpots(prefabIndex, player);
    }

    [Command]
    public void CmdSpawnUnitSpots(int spawnIndex, GameObject thePlayer)
    {
        GameObject unitSpawned = NetworkManager.singleton.spawnPrefabs[spawnIndex];
        GameObject go = null;
        //if (base.connectionToClient.connectionId == -1)//The Host must not spawn anything... 
        if (base.connectionToClient.connectionId == 1)//The First Client to enter the game
        {
            go = (GameObject)Instantiate(unitSpawned);
            go.transform.position = new Vector3(go.transform.position.x, go.transform.position.y, go.transform.position.z);
            //NetworkServer.SpawnWithClientAuthority(go, thePlayer);
            NetworkServer.SpawnWithClientAuthority(go, base.connectionToClient);
            Debug.Log("Server go auth? " + go.GetComponent<NetworkIdentity>().clientAuthorityOwner);
        }
        else if (base.connectionToClient.connectionId == 2)//The Second Client to enter the game
        {
            go = (GameObject)Instantiate(unitSpawned);
            go.transform.position = new Vector3(-go.transform.position.x, go.transform.position.y, -go.transform.position.z);
            //NetworkServer.SpawnWithClientAuthority(go, thePlayer);
            NetworkServer.SpawnWithClientAuthority(go, base.connectionToClient);
            Debug.Log("Client go auth? " + go.GetComponent<NetworkIdentity>().clientAuthorityOwner);
        }
    }
}
