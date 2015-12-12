using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CloudSetting : NetworkBehaviour
{
    bool started;

    //float projectileSpeed=80f;
    [SyncVar]
    public float dur;
    [SyncVar]
    public Vector3 position;
    [SyncVar]
    public Vector3 size;

    // Use this for initialization
    void Start()
    {
        this.transform.localScale = this.size;
        //needs to know its the local player or not to set Renderer to active or false
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.transform.position == position)
        {
            if (!started)
            {
                started = true;
            }
            //StartCoroutine(slowed());
            else Destroy(gameObject, dur);
        }
    }
}
