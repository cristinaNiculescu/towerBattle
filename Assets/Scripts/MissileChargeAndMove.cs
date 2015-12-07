using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MissileChargeAndMove : NetworkBehaviour
{
    [SerializeField]
    public float missileDamagePercentage;
    [SerializeField]
    float projectileSpeed = 60f;
    [SyncVar]
    public Vector3 target = Vector3.zero;
    //[SyncVar]
    //Transform tranny;
    //Rigidbody missileRigidBody;

    void Start()
    {
        //if (hasAuthority)
        //{
        //tranny = transform;
        //missileRigidBody = GetComponent<Rigidbody>();
        //}
    }

    void FixedUpdate()
    {
        if (hasAuthority)
        {
            if (target != Vector3.zero)
                this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, target, projectileSpeed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (hasAuthority)
        {
            if (col.transform.tag == "Player 1")
            {
                print("Hitting the base 1");
                //col.gameObject.GetComponent<UnitStructure>().HP -= col.gameObject.GetComponent<UnitStructure>().HPMax * missileDamagePercentage / 100;
                float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * missileDamagePercentage / 100;
                CmdDealDamage(col.gameObject.GetComponent<NetworkIdentity>(), damageAmount);
                CmdDestroyMissile(gameObject);
            }
            if (col.transform.tag == "Player 2")
            {
                print("Hitting the base 2");
                //col.gameObject.GetComponent<UnitStructure>().HP -= col.gameObject.GetComponent<UnitStructure>().HPMax * missileDamagePercentage / 100;
                float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * missileDamagePercentage / 100;
                CmdDealDamage(col.gameObject.GetComponent<NetworkIdentity>(), damageAmount);
                CmdDestroyMissile(gameObject);
            }
        }
    }

    [Command]
    void CmdDealDamage(NetworkIdentity target, float damageAmount)
    {
        GameObject baseObj = NetworkServer.FindLocalObject(target.netId);
        baseObj.GetComponent<UnitStructure>().HP -= (damageAmount * 100);
    }

    [Command]
    void CmdDestroyMissile(GameObject missile)
    {
        NetworkServer.Destroy(missile);
    }
}
