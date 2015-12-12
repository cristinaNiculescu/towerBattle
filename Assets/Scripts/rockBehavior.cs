using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RockBehavior : NetworkBehaviour
{
    float posX;
    float posZ;
    Renderer seen;
    [SyncVar]
    public Vector3 direction;

    // Update is called once per frame
    //void Update()
    void FixedUpdate()
    {
        this.gameObject.transform.position += direction * 0.5f;
        if (transform.position.x <= -1000f || transform.position.x >= 1000f ||
            transform.position.z <= -1000f || transform.position.z >= 1000f)
        {
            //DestroyRock(gameObject);
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        //if (col.transform.tag == "Base1")
        if (col.gameObject.name.StartsWith("Base"))
        {
            print("Basemanager ShieldPower ? " + col.gameObject.GetComponent<BaseManager>().shieldPower);
            if (col.gameObject.GetComponent<BaseManager>().shieldPower > 0)
            {
                float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
                col.gameObject.GetComponent<BaseManager>().ShieldTakeDamage(damageAmount);
                //col.gameObject.GetComponent<UnitStructure>().BaseUnit.shieldPower -= col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
            }
            else
            {
                float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
                col.gameObject.GetComponent<UnitStructure>().TakeDamage(damageAmount);
                //col.gameObject.GetComponent<UnitStructure>().HP -= col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
            }
            //DestroyRock(gameObject);
            Destroy(gameObject);
        }
        //if (col.transform.tag == "Base2")
        if (col.gameObject.name.StartsWith("Enemy_base"))
        {
            print("Basemanager ShieldPower ? " + col.gameObject.GetComponent<BaseManager>().shieldPower);
            if (col.gameObject.GetComponent<BaseManager>().shieldPower > 0)
            {
                float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
                col.gameObject.GetComponent<BaseManager>().ShieldTakeDamage(damageAmount);
                //col.gameObject.GetComponent<UnitStructure>().BaseUnit.shieldPower -= col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
            }
            else
            {
                float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
                col.gameObject.GetComponent<UnitStructure>().TakeDamage(damageAmount);
                //col.gameObject.GetComponent<UnitStructure>().HP -= col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
            }
            //DestroyRock(gameObject);
            Destroy(gameObject);
        }
    }

    //[ClientCallback]
    //void DestroyRock(GameObject rock)
    //{
    //    CmdDestroyRock(rock);
    //}

    //[Command]
    //void CmdDestroyRock(GameObject rock)
    //{
    //    NetworkServer.Destroy(rock);
    //}
}
