using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MissileChargeAndMove : NetworkBehaviour
{
    #region NetworkVars
    [SyncVar]
    Vector3 syncPos;
    [SerializeField]
    Transform myTransform;
    //[SerializeField]
    //float lerpRate = 15f;
    Vector3 lastPosition;
    float threshold = 0.5f;
    #endregion

    [SerializeField]
    public float missileDamagePercentage;
    [SerializeField]
    public Vector3 direction;
    // Use this for initialization
    [SerializeField]
    float projectileSpeed = 60f;
    [SyncVar]
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, target.position, projectileSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision col)
    {	//Debug.Log (col.transform.tag);
        if (col.transform.tag == "Enemy")
        {
            col.gameObject.GetComponent<UnitStructure>().HP -= col.gameObject.GetComponent<UnitStructure>().HPMax *
                missileDamagePercentage / 100;
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        TransmitPosition();
    }

    /// <summary>
    /// Lerp position of the other player to smooth their movements.
    /// </summary>
    void LerpPosition()
    {
        if (!isLocalPlayer || !hasAuthority)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, syncPos, projectileSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Command to tell the server about our position.
    /// </summary>
    /// <param name="position"></param>
    [Command]
    void CmdProvidePositionToServer(Vector3 position)
    {
        syncPos = position;
    }

    /// <summary>
    /// Transmit our position to Clients only.
    /// </summary>
    [ClientCallback]
    void TransmitPosition()
    {
        this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, target.position, projectileSpeed * Time.deltaTime);
        //So based on the distance since we last moved and our current distance, if we havn't moved more than the threshold don't send our position to other clients.
        if (localPlayerAuthority && Vector3.Distance(myTransform.position, lastPosition) > threshold)
        {
            CmdProvidePositionToServer(transform.position);
            lastPosition = myTransform.position;
        }
    }
}
