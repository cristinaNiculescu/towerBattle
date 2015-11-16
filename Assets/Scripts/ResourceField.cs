using UnityEngine;
using System.Collections;

public class ResourceField : MonoBehaviour {

	public float speed;

	// Use this for initialization
	void Start () {
		gameObject.tag = "enemy_Resource";
		//Debug.Log (gameObject.tag);
		speed = 2;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
