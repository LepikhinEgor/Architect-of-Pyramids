using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSound : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (Player.menuSound == null)
        {
            GetComponent<AudioSource>().pitch /= (float)System.Math.Sqrt(System.Math.Pow(2, 2.0/12.0));
            Player.menuSound = this.gameObject;
            GetComponent<AudioSource>().volume = Player.musicVolume;
            DontDestroyOnLoad(this);
        }
        else
        {
            Debug.Log("Dont needed");
            Destroy(this.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
