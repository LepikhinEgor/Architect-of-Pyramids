using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour {
    public bool isActiveWind = false;
    Rigidbody2D rigidBody;
	// Use this for initialization
	void Start () {
        rigidBody = transform.gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
       if (isActiveWind)
           rigidBody.AddForce(transform.right * 2F, ForceMode2D.Force);
    }
}
