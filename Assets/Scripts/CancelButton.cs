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
            if (Player.selectedPlatfomID != -1)
            {
                GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
                GameObject selectedPlatform = new GameObject();

                foreach (GameObject pl in platforms)
                {
                    if (pl.GetComponent<Platform>().ID == Player.selectedPlatfomID)
                        selectedPlatform = pl;
                }

                Destroy(selectedPlatform.transform.Find("YellowLightPrefab(Clone)").gameObject);
                selectedPlatform.transform.Find("Select").gameObject.SetActive(true);
            }
            Player.selectedPlatfomID = -1;
            player.GetComponent<Player>().ShowSelectBlockUI();
        }
    }
}
