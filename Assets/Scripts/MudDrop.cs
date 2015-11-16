using UnityEngine;
using System.Collections;

public class MudDrop : MonoBehaviour {

	public Material resource;
	public Material mud;
	float projectileSpeed=80f;
	public Transform target;
	float initialSpeed;
	GameObject resourceFieldTargeted;
	// Use this for initialization
	void Start () {
		Debug.Log ("mud thrown");
	}
	
	// Update is called once per frame
	void Update () {
		this.gameObject.transform.position =  Vector3.MoveTowards(this.gameObject.transform.position, 
		                                                          target.position, projectileSpeed * Time.deltaTime);
	}

	void OnCollisionEnter(Collision col)
	{	//Debug.Log (col.transform.tag);
		if (col.transform.tag == "enemy_Resource") {
			initialSpeed=col.gameObject.GetComponent<ResourceField>().speed;
			resourceFieldTargeted=col.gameObject;

			StartCoroutine(slowed());
			resourceFieldTargeted.gameObject.GetComponent<ResourceField>().speed=initialSpeed;
			resourceFieldTargeted.GetComponent<Renderer> ().material =resource;

			Destroy(gameObject);	
		}
	}

	IEnumerator slowed()
	{	
		resourceFieldTargeted.gameObject.GetComponent<ResourceField>().speed/=2;
		resourceFieldTargeted.GetComponent<Renderer> ().material = mud;
		yield return new WaitForSeconds(40f);

	}
}
