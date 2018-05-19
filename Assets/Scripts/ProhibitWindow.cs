using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class  ProhibitWindow: MonoBehaviour {

    // Use this for initialization
    Text message;

    private void Awake()
    {
        message = GetComponentInChildren<Text>();
    }

    void Start () {
		
	}
	
    public void SetText(string text)
    {
        message.text = text;
    }
	// Update is called once per frame
	void Update () {
		
	}
}
