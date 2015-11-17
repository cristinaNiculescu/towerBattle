using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_ID : NetworkBehaviour {

    [SyncVar]
    public string playerUniqueIdentity;
    NetworkInstanceId playerNetID;
    Transform myTransform;

	void Awake () {
        myTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
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
        playerNetID = GetComponent<NetworkIdentity>().netId;
        CmdTellServerMyIdentity(MakeUniqueIdentity());
    }

    void SetIdentity()
    {
        if (!isLocalPlayer)
        {
            myTransform.name = playerUniqueIdentity;
        }
        else
        {
            myTransform.name = MakeUniqueIdentity();
        }
    }

    string MakeUniqueIdentity()
    {
        string uniqueIdentity = "Player " + playerNetID.ToString();
        return uniqueIdentity;
    }

    [Command]
    void CmdTellServerMyIdentity(string identity)
    {
        playerUniqueIdentity = identity;
    }
}
