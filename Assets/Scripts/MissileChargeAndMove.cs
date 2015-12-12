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

    void FixedUpdate()
    {
        if (target != Vector3.zero)
            this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, target, projectileSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Player 1")
        {
            print("Hitting the base 1");
            float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * missileDamagePercentage / 100;
            col.gameObject.GetComponent<UnitStructure>().TakeDamage(damageAmount);
            Destroy(gameObject);
        }
        if (col.transform.tag == "Player 2")
        {
            print("Hitting the base 2");
            float damageAmount = col.gameObject.GetComponent<UnitStructure>().HPMax * missileDamagePercentage / 100;
            col.gameObject.GetComponent<UnitStructure>().TakeDamage(damageAmount);
            Destroy(gameObject);
        }
    }
}
