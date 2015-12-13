using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class UnitStructure : NetworkBehaviour
{
    [SyncVar]
    public float HP;
    [SyncVar]
    public float HPMax;
    public bool isInConstruction = false;
    public int upgrades = 0;
    public GameObject healthBar;
    public Slider HP_Bar;
    public BaseManager BaseUnit;
    public string theName;
    public GameObject panel;
    bool canBeClicked;
    string tempName;
    GameObject image;
    public Color colorUnit;
    public bool isUnderRepair;
    public bool isDisoriented;
    public float disorientDur;

    private string status;

    public string statusUpdater
    {
        get { return status; }
        set { status = value; }
    }

    public int[] costs = new int[5];
    public static bool TeamLookingForTarget;

    // Use this for initialization
    void Start()
    {
        healthBar.GetComponent<HealthSlider>().unitSpot = this.gameObject;
        TeamLookingForTarget = false;
    }

    public IEnumerator waitConstruction(float duration, Color color)
    {
        float progress = 0;
        float smoothness = 1f;
        float increment = smoothness / duration;
        while (progress < 1)
        {
            progress += increment;
            gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, color, progress);
            yield return new WaitForSeconds(smoothness);
        }
        isInConstruction = false;
    }

    public IEnumerator dizzy(float duration)
    {
        //Debug.Log("is disoriented");
        yield return new WaitForSeconds(duration);
        isDisoriented = false;
    }

    //[Command]
    public void TakeDamage(float amount)
    {
        if (!isServer)
            return;
        HP -= amount;
    }
}
