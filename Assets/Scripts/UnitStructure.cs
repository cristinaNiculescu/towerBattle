using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitStructure : MonoBehaviour {

	public float HP;
	public float HPMax;
	public bool isInConstruction=false;
	public int upgrades=0;
	public GameObject healthBar;
	public Slider HP_Bar;
	public BaseManager BaseUnit;

	public GameObject panel;
	bool canBeClicked;
	bool activeMarker=false;
	string tempName;
	GameObject image;
	public Color colorUnit;

	// Use this for initialization
	void Start () {
	
	//	HP = HPMax;
	}

	public IEnumerator waitConstruction(float duration, Color color){
		float progress = 0;
		float smoothness=1f;
		float increment = smoothness /duration;
		while (progress<1) 
		{
			progress+=increment;
			gameObject.GetComponent<Renderer>().material.color = Color.Lerp (Color.white, color, progress);
			
			yield return new WaitForSeconds(smoothness);
		}
		//yield return new WaitForSeconds(20f);
		
		isInConstruction = false;
		//Debug.Log ("built"+Time.realtimeSinceStartup);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
