using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MudDrop : NetworkBehaviour
{
    public Material resource;
    public Material mud;
    float projectileSpeed = 80f;
    [SyncVar]
    public Vector3 target;
    float initialSpeed;
    GameObject resourceFieldTargeted;
    bool started;

    public float dur;
    public float speedRed;

    // Update is called once per frame
    void FixedUpdate()
    {
        this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, target, projectileSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Base1_Resources" && col.gameObject.name.StartsWith("resourceField"))
        {
            if (!started)
            {
                col.gameObject.GetComponent<ResourceField>().SlowDownResource(speedRed, dur);
                started = true;
                Destroy(gameObject, 25f);
            }
            else
                Destroy(gameObject, 25f);
        }
        if (col.transform.tag == "Base2_Resources" && col.gameObject.name.StartsWith("resourceField"))
        {
            if (!started)
            {
                col.gameObject.GetComponent<ResourceField>().SlowDownResource(speedRed, dur);
                started = true;
                Destroy(gameObject, 25f);
            }
            else
                Destroy(gameObject, 25f);
        }
    }
}
