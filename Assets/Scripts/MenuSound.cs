using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSound : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (Player.menuSound == null)
        {
            Player.menuSound = this.gameObject;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
