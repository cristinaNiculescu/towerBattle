using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Player_NetworkingSetup : NetworkBehaviour {

	// Use this for initialization
	void Start () 
    {
        //Enable all local Player Components
        if (isLocalPlayer)
        {
            GetComponent<Camera>().enabled = true;
            GetComponent<FlareLayer>().enabled = true;
            GetComponent<GUILayer>().enabled = true;
            GetComponent<AudioListener>().enabled = true;
            GetComponent<CameraController>().enabled = true;
        }
	}
}
