using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    private void OnMouseDown()
    {
        Piramid.isFirst = true;
        Player.currentBlockMaterialNum = 0;
        SceneManager.LoadScene("Piramids");
    }

    // Update is called once per frame
    void Update () {
		
	}
}
