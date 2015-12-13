using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthSlider : MonoBehaviour
{
    Slider healthSlider;
    UnitStructure unitStructure;
    public GameObject unitSpot = null;

    // Use this for initialization
    void Start()
    {
        healthSlider = this.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (unitStructure == null)
        {
            if (unitSpot != null)
            {
                unitStructure = unitSpot.GetComponent<UnitStructure>();
                healthSlider.maxValue = unitStructure.HPMax;
                Debug.Log("unitStructure : " + unitStructure.tag + ", US-HPMAX : " + unitStructure.HPMax);
            }
            if (this.transform.parent.gameObject.tag == "Player 1" && unitSpot == null)
            {
                unitStructure = GameObject.Find("Enemy_base(Clone)").GetComponent<UnitStructure>();
                healthSlider.maxValue = unitStructure.HPMax;
            }
            if (this.transform.parent.gameObject.tag == "Player 2" && unitSpot == null)
            {
                unitStructure = GameObject.Find("Base(Clone)").GetComponent<UnitStructure>();
                healthSlider.maxValue = unitStructure.HPMax;
            }
            return;
        }
        healthSlider.value = unitStructure.HP;
    }
}
