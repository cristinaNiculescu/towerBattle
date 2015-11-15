using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_SyncPosition : NetworkBehaviour 
{
    //SyncVar is basically telling all other clients to update this value.
    [SyncVar]
    Vector3 syncPos;

    [SerializeField]
    Transform myTransform;
    [SerializeField]
    float lerpRate = 15f;

	void FixedUpdate () 
    {
        TransmitPosition();
        LerpPosition();
	}

    /// <summary>
    /// Lerp position of the other player to smooth their movements.
    /// </summary>
    void LerpPosition()
    {
        if (!isLocalPlayer)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
        }
    }

    /// <summary>
    /// Command to tell the server about our position.
    /// </summary>
    /// <param name="position"></param>
    [Command]
    void CmdProvidePositionToServer(Vector3 position)
    {
        syncPos = position;
    }

    /// <summary>
    /// Transmit our position to Clients only.
    /// </summary>
    [ClientCallback]
    void TransmitPosition()
    {
        if(isLocalPlayer)
            CmdProvidePositionToServer(transform.position);
    }
}
