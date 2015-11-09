using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class AutoClearRT : MonoBehaviour
{
    public bool noClearAfterStart = false;
    // Use this for initialization
    void Start()
    {
        GetComponent<Camera>().clearFlags = CameraClearFlags.Color;
    }

    void OnPostRender()
    {
        if (!noClearAfterStart)
        {
            GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
        }
    }
}
