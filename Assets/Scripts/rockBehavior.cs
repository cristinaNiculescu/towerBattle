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

    void FixedUpdate()
    {
        this.gameObject.transform.position += direction * 0.5f;
        if (transform.position.x <= -1000f || transform.position.x >= 1000f ||
            transform.position.z <= -1000f || transform.position.z >= 1000f)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (!col.gameObject.GetComponent<NetworkIdentity>().hasAuthority && col.transform.tag == "Player 1" || col.transform.tag == "attacking" || col.transform.tag == "defense" || col.transform.tag == "defense")
        {
            if (col.gameObject.GetComponent<BaseManager>().shieldPower > 0)
            {
                float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
                col.gameObject.GetComponent<BaseManager>().ShieldTakeDamage(damageAmount);
            }
            else
            {
                float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
                col.gameObject.GetComponent<UnitStructure>().TakeDamage(damageAmount);
            }
            Destroy(gameObject);
        }
        if (!col.gameObject.GetComponent<NetworkIdentity>().hasAuthority && col.transform.tag == "Player 2" || col.transform.tag == "attacking" || col.transform.tag == "defense" || col.transform.tag == "defense")
        {
            if (col.gameObject.GetComponent<BaseManager>().shieldPower > 0)
            {
                float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
                col.gameObject.GetComponent<BaseManager>().ShieldTakeDamage(damageAmount);
            }
            else
            {
                float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * 0.05f;
                col.gameObject.GetComponent<UnitStructure>().TakeDamage(damageAmount);
            }
            Destroy(gameObject);
        }
    }
}
