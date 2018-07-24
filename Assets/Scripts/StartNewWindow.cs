using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartNewWindow : MonoBehaviour {
    GameObject ok,cancel;
	// Use this for initialization
	void Start () {
        ok = transform.Find("StartNew").gameObject;
        cancel = transform.Find("Cancel").gameObject;

        ok.GetComponent<Button>().onClick.AddListener(Player.LoadNewGame);
        cancel.GetComponent<Button>().onClick.AddListener(CloseWindow);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void CloseWindow()
    {
        Destroy(this.gameObject);
    
    }
}
