using UnityEngine;
using System.Collections;

public class RockBehavior : MonoBehaviour {

	float posX;
	float posZ;
	Renderer seen;
	public Vector3 direction;

	// Use this for initialization
	void Start () {
		//gameObject.GetComponent<Rigidbody>().velocity =new Vector3(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		//direction.x = direction.x * 0.5f;
		//direction.y = direction.y * 0f;
		//direction.z = direction.z * 0.5f;

		transform.position += direction*0.5f;	
		if (transform.position.x <= -1000f || transform.position.x >= 1000f ||
			transform.position.z <= -1000f || transform.position.z >= 1000f) {
			Destroy (gameObject, 1f);
			//Debug.Log("Destroyed");
			}


	}

	void OnCollisionEnter(Collision col)
	{	

		if (col.transform.tag == "Enemy") {
			if (col.gameObject.GetComponent<UnitStructure>().BaseUnit.shieldPower>0)
				col.gameObject.GetComponent<UnitStructure> ().BaseUnit.shieldPower -= col.gameObject.GetComponent<UnitStructure> ().HPMax *
					0.05f;
			else
			col.gameObject.GetComponent<UnitStructure> ().HP -= col.gameObject.GetComponent<UnitStructure> ().HPMax *
				0.05f;
			Destroy(gameObject);	
		}

	}

	/*void OnBecameInvisible()
	{ 	//wish this would have worked properly - sometimes it gets stuck and the invisible is way way too far. 
		Destroy (gameObject);
	}*/


}
