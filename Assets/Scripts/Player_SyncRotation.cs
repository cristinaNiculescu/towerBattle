using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_SyncRotation : NetworkBehaviour 
{
    [SyncVar]
    Quaternion syncPlayerCamRotation;

    [SerializeField]
    Transform playerCamTransform;
    [SerializeField]
    float lerpRate = 15f;

    Quaternion lastPlayerCamRotation;
    float threshold = 5.0f;

    void Update()
    {
        LerpRotation();
    }

	void FixedUpdate () 
    {
        TransmitRotation();
	}

    void LerpRotation()
    {
        if(!isLocalPlayer)
            playerCamTransform.rotation = Quaternion.Lerp(playerCamTransform.rotation, syncPlayerCamRotation, Time.deltaTime * lerpRate);
    }

    /// <summary>
    /// Command to tell the server about our rotation.
    /// </summary>
    /// <param name="playerCamRotation"></param>
    [Command]
    void CmdProvideRotationToServer(Quaternion playerCamRotation)
    {
        syncPlayerCamRotation = playerCamRotation;
    }

    /// <summary>
    /// Transmit our rotation to Clients only.
    /// </summary>
    [ClientCallback]
    void TransmitRotation()
    {
        if (isLocalPlayer)
        {
            //So based on the angle since we last rotated and our current rotation, if we havn't rotated more than the threshold don't send our rotation to other clients.
            if (Quaternion.Angle(playerCamTransform.rotation, lastPlayerCamRotation) > threshold)
            {
                CmdProvideRotationToServer(playerCamTransform.rotation);
                lastPlayerCamRotation = playerCamTransform.rotation;
            }
        }
    }
}
