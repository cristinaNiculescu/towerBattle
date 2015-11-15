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
	
	void FixedUpdate () 
    {
        TransmitRotation();
        LerpRotation();
	}

    void LerpRotation()
    {
        if(!isLocalPlayer)
            playerCamTransform.rotation = Quaternion.Lerp(playerCamTransform.rotation, syncPlayerCamRotation, Time.deltaTime * lerpRate);
    }

    [Command]
    void CmdProvideRotationToServer(Quaternion playerCamRotation)
    {
        syncPlayerCamRotation = playerCamRotation;
    }

    [ClientCallback]
    void TransmitRotation()
    {
        if (isLocalPlayer)
            CmdProvideRotationToServer(playerCamTransform.rotation);
    }
}
