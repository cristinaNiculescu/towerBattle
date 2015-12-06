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
    public Vector3 target;
    [SerializeField]
    Transform tranny;

    void Start()
    {
        if (hasAuthority)
            tranny = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
            this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, target, projectileSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision col)
    {
        if (hasAuthority)
        {
            if (col.transform.tag == "Player 1")
            {
                print("Hitting the base 1");
                col.gameObject.GetComponent<UnitStructure>().HP -= col.gameObject.GetComponent<UnitStructure>().HPMax *
                    missileDamagePercentage / 100;
                DestroyMissile(gameObject);
            }
            if (col.transform.tag == "Player 2")
            {
                print("Hitting the base 2");
                col.gameObject.GetComponent<UnitStructure>().HP -= col.gameObject.GetComponent<UnitStructure>().HPMax *
                    missileDamagePercentage / 100;
                DestroyMissile(gameObject);
            }
        }
    }

    void DestroyMissile(GameObject missile)
    {
        NetworkServer.Destroy(missile);
    }
}
