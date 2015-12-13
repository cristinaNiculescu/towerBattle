using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BigRockBehavior : NetworkBehaviour
{
    [SyncVar]
    public Vector3 target;
    bool started;
    [SerializeField]
    float projectileSpeed = 80f;
    [SerializeField]
    public float dur;
    [SerializeField]
    public float damagePercentage;

    // Update is called once per frame
    void FixedUpdate()
    {
        this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, target, projectileSpeed * Time.deltaTime);
        //if (this.gameObject.transform.position == target)
        //{
        //if (!started)
        //{
        //    if (target.gameObject.GetComponent<UnitStructure>().BaseUnit.shieldPower > 0)
        //    {
        //        //target.gameObject.GetComponent<UnitStructure>().BaseUnit.shieldPower -= target.gameObject.GetComponent<UnitStructure>().HPMax * damagePercentage;
        //        float damage = target.gameObject.GetComponent<UnitStructure>().HPMax * damagePercentage;
        //    }
        //    else
        //    {
        //        target.gameObject.GetComponent<UnitStructure>().HP -= target.gameObject.GetComponent<UnitStructure>().HPMax * damagePercentage;
        //    }
        //    target.gameObject.GetComponent<UnitStructure>().isDisoriented = true;
        //    target.gameObject.GetComponent<UnitStructure>().disorientDur = dur;
        //    StartCoroutine(target.gameObject.GetComponent<UnitStructure>().dizzy(dur));
        //    started = true;
        //}
        //else
        //    //Destroy(gameObject);
        //    Destroy(gameObject);
        //}
    }

    void OnCollisionEnter(Collision col)
    {
        if (!col.gameObject.GetComponent<NetworkIdentity>().hasAuthority && col.transform.tag == "Player 1" || col.transform.tag == "attacking" || col.transform.tag == "defense" || col.transform.tag == "defense")
        {
            if (!started)
            {
                print("Basemanager ShieldPower ? " + col.gameObject.GetComponent<BaseManager>().shieldPower);
                if (col.gameObject.GetComponent<BaseManager>().shieldPower > 0)
                {
                    //target.gameObject.GetComponent<UnitStructure>().BaseUnit.shieldPower -= target.gameObject.GetComponent<UnitStructure>().HPMax * damagePercentage;
                    float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * damagePercentage;
                    col.gameObject.GetComponent<BaseManager>().ShieldTakeDamage(damageAmount);
                }
                else
                {
                    //col.gameObject.GetComponent<UnitStructure>().HP -= col.gameObject.GetComponent<UnitStructure>().HPMax * damagePercentage;
                    float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * damagePercentage;
                    col.gameObject.GetComponent<UnitStructure>().TakeDamage(damageAmount);
                }
                col.gameObject.GetComponent<UnitStructure>().isDisoriented = true;
                col.gameObject.GetComponent<UnitStructure>().disorientDur = dur;
                StartCoroutine(col.gameObject.GetComponent<UnitStructure>().dizzy(dur));
                started = true;
                Destroy(gameObject, 1f);
            }
            else
                Destroy(gameObject);
        }
        if (!col.gameObject.GetComponent<NetworkIdentity>().hasAuthority && col.transform.tag == "Player 2" || col.transform.tag == "attacking" || col.transform.tag == "defense" || col.transform.tag == "defense")
        {
            if (!started)
            {
                print("Basemanager ShieldPower ? " + col.gameObject.GetComponent<BaseManager>().shieldPower);
                if (col.gameObject.GetComponent<BaseManager>().shieldPower > 0)
                {
                    //target.gameObject.GetComponent<UnitStructure>().BaseUnit.shieldPower -= target.gameObject.GetComponent<UnitStructure>().HPMax * damagePercentage;
                    float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * damagePercentage;
                    col.gameObject.GetComponent<BaseManager>().ShieldTakeDamage(damageAmount);
                }
                else
                {
                    //col.gameObject.GetComponent<UnitStructure>().HP -= col.gameObject.GetComponent<UnitStructure>().HPMax * damagePercentage;
                    float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * damagePercentage;
                    col.gameObject.GetComponent<UnitStructure>().TakeDamage(damageAmount);
                }
                col.gameObject.GetComponent<UnitStructure>().isDisoriented = true;
                col.gameObject.GetComponent<UnitStructure>().disorientDur = dur;
                StartCoroutine(col.gameObject.GetComponent<UnitStructure>().dizzy(dur));
                started = true;
                Destroy(gameObject, 1f);
            }
            else
                Destroy(gameObject);
        }
    }
}
