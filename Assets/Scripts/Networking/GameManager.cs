﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    [SyncVar]
    public int players = 0;
    GameObject player1_Base;
    GameObject player2_Base;
    [SerializeField]
    float player1_HP;
    [SerializeField]
    float player2_HP;
    bool hasChecked = false;

    // Use this for initialization
    void Start()
    {

    }

    void Update()
    {
        if (hasAuthority)
        {
            SetReferencesToBases();
            CheckBaseStateHP();
        }
    }

    /// <summary>
    /// Check the state of each base, and see whether or not someone has reached zero health.
    /// </summary>
    private void CheckBaseStateHP()
    {
        if (hasChecked)
        {
            if (player1_HP <= 0)//Player 1 Lost.
            {
                print("Player 1 lsot");
                //player1_Base.GetComponent<BaseManager>().Lost();
                //player2_Base.GetComponent<BaseManager>().Won();
                //Time.timeScale = 0;
                //RpcSetVictorAndLoser(player1_Base.GetComponent<NetworkIdentity>().playerControllerId);
            }
            else if (player2_HP <= 0)//Player 2 Lost.
            {
                print("Player 2 lsot");
                //player1_Base.GetComponent<BaseManager>().Won();
                //player2_Base.GetComponent<BaseManager>().Lost();
                //Time.timeScale = 0;
                //RpcSetVictorAndLoser(player2_Base.GetComponent<NetworkIdentity>().playerControllerId);
            }
        }
    }

    /// <summary>
    /// IF both abses has spawned, setup the refernces.
    /// </summary>
    private void SetReferencesToBases()
    {
        if (GameObject.Find("Base(Clone)") != null && GameObject.Find("Enemy_base(Clone)") != null && players > 1 && !hasChecked)
        {
            hasChecked = true;
            print("2 players has joined!");
            player1_Base = GameObject.Find("Base(Clone)");//Player 1 Base;
            player2_Base = GameObject.Find("Enemy_base(Clone)");//Player 2 Base;
            player1_HP = player1_Base.GetComponent<UnitStructure>().HP;
            player2_HP = player2_Base.GetComponent<UnitStructure>().HP;
        }
    }

    //[ClientRpc]
    //public void RpcSetVictorAndLoser(short whoLostPlayerControllerId)
    //{
    //    Debug.Log("Who lost? : " + whoLostPlayerControllerId);
    //}
}
