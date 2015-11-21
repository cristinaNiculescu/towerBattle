using UnityEngine;
using System.Collections;

public class BigRockBehavior : MonoBehaviour {

	public Transform target;
	bool started;

	float projectileSpeed=80f;
	public float dur;
	public float damagePercentage;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		this.gameObject.transform.position = Vector3.MoveTowards (this.gameObject.transform.position, 
		                                                          target.position, projectileSpeed * Time.deltaTime);
		if(this.gameObject.transform.position==target.position)
		{
			if (!started)
			{	if (target.gameObject.GetComponent<UnitStructure>().BaseUnit.shieldPower>0)
					target.gameObject.GetComponent<UnitStructure> ().BaseUnit.shieldPower -= 
					target.gameObject.GetComponent<UnitStructure> ().HPMax *damagePercentage;
				else
					target.gameObject.GetComponent<UnitStructure> ().HP -= 
						target.gameObject.GetComponent<UnitStructure> ().HPMax *damagePercentage;
				target.gameObject.GetComponent<UnitStructure> ().isDisoriented = true;
				target.gameObject.GetComponent<UnitStructure> ().disorientDur=dur;
				StartCoroutine(target.gameObject.GetComponent<UnitStructure> ().dizzy(dur));
				started=true;				
			}
			//StartCoroutine(slowed());
			else Destroy(gameObject);	
			
		}
	}
	
}
