using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player_GetAuthority : NetworkBehaviour
{
    //bool hasBeenAssignedAuhtority = false;

    //void Update()
    //{
    //    if(isLocalPlayer && !hasBeenAssignedAuhtority)
    //    {
    //        hasBeenAssignedAuhtority = true;
    //        CmdAssignAuthorities();
    //    }
    //}

    //[Command]
    //void CmdAssignAuthorities()
    //{
    //    GameObject[] identities = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
    //    foreach (GameObject obj in identities)
    //    {
    //        if (obj.GetComponent<NetworkIdentity>() != null)
    //        {
    //            obj.GetComponent<NetworkIdentity>().AssignClientAuthority(base.connectionToClient);
    //            Debug.Log("obj has it! = " + obj.name);
    //        }
    //    }
    //}
}
