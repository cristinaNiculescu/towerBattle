using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MudDrop : NetworkBehaviour
{
    public Material resource;
    public Material mud;
    float projectileSpeed = 80f;
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
        //if (this.gameObject.transform.position == target.position)
        //{
        //    if (!started)
        //    {
        //        initialSpeed = target.gameObject.GetComponent<ResourceField>().speed;
        //        resourceFieldTargeted = target.gameObject;
        //        StartCoroutine(slowed(speedRed, dur));
        //        started = true;
        //    }
        //    else
        //        Destroy(gameObject, 25f);
        //}
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Player 1" && col.gameObject.name.StartsWith("resourceField"))
        {
            print("Yeah! Hit that motherfucker Player 1!");
            if (!started)
            {
                //initialSpeed = col.gameObject.GetComponent<ResourceField>().speed;
                //resourceFieldTargeted = col.gameObject;
                //StartCoroutine(slowed(speedRed, dur));
                col.gameObject.GetComponent<ResourceField>().SlowDownResource(speedRed, dur);
                started = true;
                Destroy(gameObject, 25f);
            }
            else
                Destroy(gameObject, 25f);
        }
        if (col.transform.tag == "Player 2" && col.gameObject.name.StartsWith("resourceField"))
        {
            print("Yeah! Hit that motherfucker Player 2!");
            if (!started)
            {
                //initialSpeed = col.gameObject.GetComponent<ResourceField>().speed;
                //resourceFieldTargeted = col.gameObject;
                //StartCoroutine(slowed(speedRed, dur));
                col.gameObject.GetComponent<ResourceField>().SlowDownResource(speedRed, dur);
                started = true;
                Destroy(gameObject, 25f);
            }
            else
                Destroy(gameObject, 25f);
        }
    }

    //IEnumerator slowed(float speedReduction, float duration)
    //{
    //    resourceFieldTargeted.gameObject.GetComponent<ResourceField>().speed *= speedReduction;
    //    //resourceFieldTargeted.GetComponent<Renderer>().material = mud;
    //    resourceFieldTargeted.GetComponent<ResourceField>().ChangeMat("mud");
    //    yield return new WaitForSeconds(duration);
    //    resourceFieldTargeted.gameObject.GetComponent<ResourceField>().speed = initialSpeed;
    //    //resourceFieldTargeted.GetComponent<Renderer>().material = resource;
    //    resourceFieldTargeted.GetComponent<ResourceField>().ChangeMat("resource");
    //    Destroy(gameObject);
    //}
}
