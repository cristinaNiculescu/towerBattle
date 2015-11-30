using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RockBehavior : NetworkBehaviour
{
    float posX;
    float posZ;
    Renderer seen;
    public Vector3 direction;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!localPlayerAuthority && !hasAuthority)
        {
            return;
        }
        transform.position += direction * 0.5f;
        if (transform.position.x <= -1000f || transform.position.x >= 1000f ||
            transform.position.z <= -1000f || transform.position.z >= 1000f)
        {
            //Destroy(gameObject, 1f);
            DestroyRock(gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Base1")
        {
            if (col.gameObject.GetComponent<UnitStructure>().BaseUnit.shieldPower > 0)
                col.gameObject.GetComponent<UnitStructure>().BaseUnit.shieldPower -= col.gameObject.GetComponent<UnitStructure>().HPMax *
                    0.05f;
            else
                col.gameObject.GetComponent<UnitStructure>().HP -= col.gameObject.GetComponent<UnitStructure>().HPMax *
                    0.05f;
            DestroyRock(gameObject);
        }
        if (col.transform.tag == "Base2")
        {
            if (col.gameObject.GetComponent<UnitStructure>().BaseUnit.shieldPower > 0)
                col.gameObject.GetComponent<UnitStructure>().BaseUnit.shieldPower -= col.gameObject.GetComponent<UnitStructure>().HPMax *
                    0.05f;
            else
                col.gameObject.GetComponent<UnitStructure>().HP -= col.gameObject.GetComponent<UnitStructure>().HPMax *
                    0.05f;
            DestroyRock(gameObject);
        }
    }

    [ClientCallback]
    void DestroyRock(GameObject rock)
    {
        CmdDestroyRock(rock);
    }

    [Command]
    void CmdDestroyRock(GameObject rock)
    {
        NetworkServer.Destroy(rock);
    }
}
