using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    [SyncVar]
    public int players = 0;

    // Use this for initialization
    void Start()
    {

    }
}
