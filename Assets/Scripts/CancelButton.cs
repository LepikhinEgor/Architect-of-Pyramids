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
            GetComponent<AudioSource>().Play();
            Player.isChoosingPlatform = false;
            if (Player.selectedPlatfomID != -1)
            {

                Destroy(Player.selectedPlatform.transform.Find("YellowLightPrefab(Clone)").gameObject);
                Player.selectedPlatform.transform.Find("Select").gameObject.SetActive(true);
            }
            GameObject sample = GameObject.FindGameObjectWithTag("Sample");
            sample.SetActive(false);
            Player.selectedPlatfomID = -1;
            Player.score = 0;
            GameObject canvas = GameObject.FindGameObjectWithTag("MainCanvas");
            Destroy(canvas.transform.Find("SampleScorePrefab(Clone)").gameObject);
            if (Player.selectedPlatform!= null)
                Destroy(canvas.transform.Find("BlockScorePrefab(Clone)").gameObject);
            player.GetComponent<Player>().ShowSelectBlockUI();
        }
    }
}
