using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthSlider : MonoBehaviour
{
    Slider healthSlider;
    UnitStructure unitStructure;

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
            if (this.transform.parent.gameObject.tag == "Player 1")
            {
                unitStructure = GameObject.Find("Enemy_base(Clone)").GetComponent<UnitStructure>();
                healthSlider.maxValue = unitStructure.HPMax;
            }
            if (this.transform.parent.gameObject.tag == "Player 2")
            {
                unitStructure = GameObject.Find("Base(Clone)").GetComponent<UnitStructure>();
                healthSlider.maxValue = unitStructure.HPMax;
            }
            return;
        }
        healthSlider.value = unitStructure.HP;
    }
}
