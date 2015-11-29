using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MudDrop : NetworkBehaviour {

	public Material resource;
	public Material mud;
	float projectileSpeed=80f;
	public Transform target;
	float initialSpeed;
	GameObject resourceFieldTargeted;
	bool started;

	public float dur;
	public float speedRed;

	// Use this for initialization
	void Start () {
//		Debug.Log ("mud thrown");
	}
	
	// Update is called once per frame
	void Update () {
		this.gameObject.transform.position =  Vector3.MoveTowards(this.gameObject.transform.position, 
		                                                          target.position, projectileSpeed * Time.deltaTime);

		if(this.gameObject.transform.position==target.position)
		{
			if (!started)
			{	initialSpeed=target.gameObject.GetComponent<ResourceField>().speed;
				resourceFieldTargeted=target.gameObject;
				StartCoroutine(slowed (speedRed,dur));
				started=true;
//				Debug.Log(initialSpeed);

			}
			//StartCoroutine(slowed());
			 else Destroy(gameObject,25f);	
	
		}
	}

	//the collision seems to work and not work weirdly on this one. created a work-around 
	/*void OnCollisionEnter(Collision col)
	{	Debug.Log (col.transform.tag+"Collided");
		if (col.transform.tag == "enemy_Resource") {
			//Debug.Log(col.transform.tag)
			initialSpeed=col.gameObject.GetComponent<ResourceField>().speed;
			resourceFieldTargeted=col.gameObject;

			//StartCoroutine(slowed());
			resourceFieldTargeted.gameObject.GetComponent<ResourceField>().speed=initialSpeed;
			resourceFieldTargeted.GetComponent<Renderer> ().material =resource;

			Destroy(gameObject);	
		}
		Debug.Log (col.transform.tag);
		if(col.transform.tag=="enemy_Resource")
		{
			//if (!started)
			//{	
				initialSpeed=target.gameObject.GetComponent<ResourceField>().speed;
				resourceFieldTargeted=target.gameObject;
				StartCoroutine(slowed ());
				started=true;
				Debug.Log(initialSpeed);
				
			//}
			//StartCoroutine(slowed());
			//else Destroy(gameObject,25f);	
			
		}
	}*/

	IEnumerator slowed(float speedReduction, float duration)
	{	//Debug.Log ("started cor");
		resourceFieldTargeted.gameObject.GetComponent<ResourceField>().speed*=speedReduction;
		resourceFieldTargeted.GetComponent<Renderer> ().material = mud;
		yield return new WaitForSeconds(duration);
		resourceFieldTargeted.gameObject.GetComponent<ResourceField>().speed=initialSpeed;
		//Debug.Log("changedMaterialback");
		resourceFieldTargeted.GetComponent<Renderer> ().material =resource;
		Destroy(gameObject);
	}
}
