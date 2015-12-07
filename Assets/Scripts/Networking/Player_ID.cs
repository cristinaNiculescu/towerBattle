using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_ID : NetworkBehaviour
{
    [SyncVar]
    public string playerUniqueIdentity;
    //NetworkInstanceId playerNetID;
    Transform myTransform;
    GameManager gameManager;

    void Awake()
    {
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (myTransform.name == "" || myTransform.name == "Player(Clone)")
        {
            SetIdentity();
        }
    }

    public override void OnStartLocalPlayer()
    {
        GetNetIdentity();
        SetIdentity();
    }

    [ClientCallback]
    void GetNetIdentity()
    {
        //playerNetID = GetComponent<NetworkIdentity>().netId;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        CmdGameManagerAddPlayer(gameManager.GetComponent<NetworkIdentity>());
        CmdTellServerMyIdentity(MakeUniqueIdentity(gameManager.players));
    }

    void SetIdentity()
    {
        if (!isLocalPlayer)
        {
            myTransform.name = playerUniqueIdentity;
        }
        else
        {
            myTransform.name = MakeUniqueIdentity(gameManager.players);
        }
    }

    string MakeUniqueIdentity(int numberOfPlayers)
    {
        //string uniqueIdentity = "Player " + playerNetID.ToString();
        string uniqueIdentity = "Player " + (numberOfPlayers+1);
        return uniqueIdentity;
    }

    [Command]
    void CmdTellServerMyIdentity(string identity)
    {
        playerUniqueIdentity = identity;
    }

    [Command]
    void CmdGameManagerAddPlayer(NetworkIdentity gameManagerID)
    {
        NetworkServer.FindLocalObject(gameManagerID.netId).GetComponent<GameManager>().players++;
    }
}
