using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelButton : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

    }
    private void OnMouseDown()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");


        if (Player.isChoosingPlatform)
        { 
            Player.isChoosingPlatform = false;
            player.GetComponent<Player>().ShowSelectBlockUI();
        }
    }
}
