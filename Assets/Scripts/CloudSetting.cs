using UnityEngine;
using System.Collections;

public class CloudSetting : MonoBehaviour {

	public Transform target;
	bool started;
	
	float projectileSpeed=80f;
	public float dur;
	public Vector3 position;
	public Vector3 size;

	// Use this for initialization
	void Start () {
		this.transform.localScale = this.size;
		//needs to know its the local player or not to set Renderer to active or false
	}
	
	// Update is called once per frame
	void Update () {

		if(this.gameObject.transform.position==position)
		{
			if (!started)
			{	
				started=true;				
			}
			//StartCoroutine(slowed());
			else Destroy(gameObject,dur);	
			
		}
	
	}
}
