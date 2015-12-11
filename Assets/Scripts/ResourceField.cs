using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ResourceField : NetworkBehaviour {

    [SyncVar]
	public float speed;

	// Use this for initialization
	void Start () {
		speed = 2;
	}
}
