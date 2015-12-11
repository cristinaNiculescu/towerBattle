using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MudDrop : NetworkBehaviour
{

    public Material resource;
    public Material mud;
    float projectileSpeed = 80f;
    public Transform target;
    float initialSpeed;
    GameObject resourceFieldTargeted;
    bool started;

    public float dur;
    public float speedRed;

    // Update is called once per frame
    void FixedUpdate()
    {
        this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, target.position, projectileSpeed * Time.deltaTime);
        if (this.gameObject.transform.position == target.position)
        {
            if (!started)
            {
                initialSpeed = target.gameObject.GetComponent<ResourceField>().speed;
                resourceFieldTargeted = target.gameObject;
                StartCoroutine(slowed(speedRed, dur));
                started = true;
            }
            else
                Destroy(gameObject, 25f);
        }
    }

    IEnumerator slowed(float speedReduction, float duration)
    {
        resourceFieldTargeted.gameObject.GetComponent<ResourceField>().speed *= speedReduction;
        resourceFieldTargeted.GetComponent<Renderer>().material = mud;
        yield return new WaitForSeconds(duration);
        resourceFieldTargeted.gameObject.GetComponent<ResourceField>().speed = initialSpeed;
        resourceFieldTargeted.GetComponent<Renderer>().material = resource;
        Destroy(gameObject);
    }
}
