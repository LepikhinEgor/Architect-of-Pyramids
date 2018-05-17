using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PiramidButton : MonoBehaviour {

	// Use this for initialization
	void Start () {

    }


    private void OnMouseDown()
    {
        if (tag.Equals("SmallPiramid"))
            SceneManager.LoadScene("Piramid1");
        if (tag.Equals("MediumPiramid") && Player.Pir1TotalScore > 1000)
            SceneManager.LoadScene("Piramid2");
        if (tag.Equals("LargePiramid") && Player.Pir2TotalScore > 4000)
            SceneManager.LoadScene("Piramid3");

    }
    // Update is called once per frame
    void Update () {
		
	}
}
