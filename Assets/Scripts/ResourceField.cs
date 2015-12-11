using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ResourceField : NetworkBehaviour
{
    public Material resource;
    public Material mud;
    [SyncVar]
    public float speed;
    public float initialSpeed;

    // Use this for initialization
    void Start()
    {
        speed = 2;
        initialSpeed = speed;
    }


    /// <summary>
    /// Changes the material accordingly.
    /// </summary>
    /// <param name="material"></param>
    public void ChangeMat(string material)
    {
        if (material == "mud")
        {
            GetComponent<Renderer>().material = mud;
        }
        if (material == "resource")
        {
            GetComponent<Renderer>().material = resource;
        }
    }

    public void SlowDownResource(float speedReduction, float duration)
    {
        StartCoroutine(Slowed(speedReduction, duration));
    }

    IEnumerator Slowed(float speedReduction, float duration)
    {
        speed *= speedReduction;
        ChangeMat("mud");
        yield return new WaitForSeconds(duration);
        speed = initialSpeed;
        ChangeMat("resource");
    }
}
