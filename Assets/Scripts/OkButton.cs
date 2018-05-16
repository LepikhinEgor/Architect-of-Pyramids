using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OkButton : MonoBehaviour {

    public GameObject nextBlockBtn;
    public GameObject previousBlockBtn;
    public GameObject blockSelection;

    private void Awake()
    {
        blockSelection = GameObject.FindGameObjectWithTag("BlockSelection");
        nextBlockBtn = GameObject.FindGameObjectWithTag("NextBlock");
        previousBlockBtn = GameObject.FindGameObjectWithTag("PreviousBlock");
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        GameObject sample = GameObject.FindGameObjectWithTag("Sample");

        if (tag.Equals("OK") && !Player.isChoosingPlatform)
        {
            Player.currentBlockMaterialNum = blockSelection.GetComponent<BlockSelection>().BlockMaterialNum;
            Player.score = 0;
            Player.PerfectCoef = 1;

            GameObject piramid = GameObject.FindGameObjectWithTag("Piramid");
            piramid.GetComponent<Piramid>().GetPlatfomsInformation();

            Player.currentPiramidID = piramid.GetComponent<Piramid>().ID;
            Player.lives = 3;

            if (blockSelection.GetComponent<BlockSelection>().BlockMaterialNum == 0)
                SceneManager.LoadScene("SaphireFloors1");
            else
            {
                if (blockSelection.GetComponent<BlockSelection>().BlockMaterialNum == 1)
                    SceneManager.LoadScene("RubyFloors1");
                else
                    if (blockSelection.GetComponent<BlockSelection>().BlockMaterialNum == 2)
                    SceneManager.LoadScene("EmeraldFloors1");
                else
                        if (blockSelection.GetComponent<BlockSelection>().BlockMaterialNum == 4)
                    SceneManager.LoadScene("GoldenFloors1");
                else
                            if (blockSelection.GetComponent<BlockSelection>().BlockMaterialNum == 3)
                                SceneManager.LoadScene("SilverFloors1");
                else
                                if (blockSelection.GetComponent<BlockSelection>().BlockMaterialNum == 5)
                                    SceneManager.LoadScene("TopazFloors1");
                //else
                   // SceneManager.LoadScene("mainScene");
            }
        }

    }
}
