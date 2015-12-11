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
        if (col.transform.tag == "Base1")
        //if (col.transform.tag == "Player 1")
        {
            if (col.gameObject.GetComponent<UnitStructure>().BaseUnit.shieldPower > 0)
            {
                float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
                col.gameObject.GetComponent<UnitStructure>().CmdShieldTakeDamage(damageAmount);
                //col.gameObject.GetComponent<UnitStructure>().BaseUnit.shieldPower -= col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
            }
            else
            {
                float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
                col.gameObject.GetComponent<UnitStructure>().CmdTakeDamage(damageAmount);
                //col.gameObject.GetComponent<UnitStructure>().HP -= col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
            }
            //DestroyRock(gameObject);
            Destroy(gameObject);
        }
        if (col.transform.tag == "Base2")
        //if (col.transform.tag == "Player 2")
        {
            if (col.gameObject.GetComponent<UnitStructure>().BaseUnit.shieldPower > 0)
            {
                float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
                col.gameObject.GetComponent<UnitStructure>().CmdShieldTakeDamage(damageAmount);
                //col.gameObject.GetComponent<UnitStructure>().BaseUnit.shieldPower -= col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
            }
            else
            {
                float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
                col.gameObject.GetComponent<UnitStructure>().CmdTakeDamage(damageAmount);
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
