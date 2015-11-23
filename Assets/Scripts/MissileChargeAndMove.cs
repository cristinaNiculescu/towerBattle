using UnityEngine;
using System.Collections;

public class MissileChargeAndMove : MonoBehaviour {

	public float missileDamagePercentage;
	public Vector3 direction;
	// Use this for initialization
	float projectileSpeed=60f;
	public Transform target;

	void Start () {
		//Debug.Log ("% damage:" + missileDamagePercentage);
	}
	
	// Update is called once per frame
	void Update () {

		this.gameObject.transform.position =  Vector3.MoveTowards(this.gameObject.transform.position, 
		                                                          target.position, projectileSpeed * Time.deltaTime);
	}

	void OnCollisionEnter(Collision col)
	{	//Debug.Log (col.transform.tag);
		if (col.transform.tag == "Enemy") {
			col.gameObject.GetComponent<UnitStructure> ().HP -= col.gameObject.GetComponent<UnitStructure> ().HPMax *
				missileDamagePercentage / 100;
			Destroy(gameObject);	
		}
	}

}
