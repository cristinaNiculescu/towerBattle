using UnityEngine;
using System.Collections;

public class rockBehavior : MonoBehaviour {

	float posX;
	float posZ;
	Renderer seen;
	public Vector3 direction;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += direction * 0.5f;	
		if( transform.position.x<=-1000f ||transform.position.x>=1000f ||
		   transform.position.z<=-1000f ||transform.position.z>=1000f) 
			Destroy (gameObject);

	}

	void OnCollisionEnter(Collision col)
	{	

		if (col.gameObject.tag != this.tag)
		{ Debug.Log("collided"+col.gameObject.name);
		/*	if(col.gameObject is AttackingUnit) 
			{ Debug.Log("atck");
				AttackingUnit tempUnit=col.gameObject.GetComponent<AttackingUnit>();
			tempUnit.HP-=tempUnit.HP*0.05f;
			Debug.Log ("collided with attackunit");
		}*/
		}
	}

	/*void OnBecameInvisible()
	{ 	//wish this would have worked properly - sometimes it gets stuck and the invisible is way way too far. 
		Destroy (gameObject);
	}*/


}
