using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsWindow : ProhibitWindow {

    GameObject okButton;
    GameObject retryButton;
    // Use this for initialization

    private void Awake()
    {
        okButton = transform.Find("okButton").gameObject;
        retryButton = transform.Find("retryButton").gameObject;
    }
    void Start () {
        okButton.GetComponent<Button>().onClick.AddListener(Player.LoadPiramidScene);
        retryButton.GetComponent<Button>().onClick.AddListener(Player.ReloadFloors);
        switch(Player.lives)
        {
            case 0:
                transform.Find("Lives").transform.Find("Image").gameObject.SetActive(false);
                transform.Find("Lives").transform.Find("Image (1)").gameObject.SetActive(false);
                transform.Find("Lives").transform.Find("Image (2)").gameObject.SetActive(false);
                break;
            case 1:
                Debug.Log(transform.Find("Lives").transform);
                transform.Find("Lives").transform.Find("Image (1)").gameObject.SetActive(false);
                transform.Find("Lives").transform.Find("Image (2)").gameObject.SetActive(false);
                break;
            case 2:
                transform.Find("Lives").transform.Find("Image (2)").gameObject.SetActive(false);
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
