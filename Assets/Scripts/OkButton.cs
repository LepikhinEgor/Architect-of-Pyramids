using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OkButton : MonoBehaviour {
    public GameObject blockSelection;
    Button btn;

    private void Awake()
    {
        blockSelection = GameObject.FindGameObjectWithTag("BlockSelection");

        btn = GetComponent<Button>();
        //btn.onClick.AddListener(OkButtonAction);
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OkButtonAction()
    {
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
                    SceneManager.LoadScene("SilverFloors1");
                else
                            if (blockSelection.GetComponent<BlockSelection>().BlockMaterialNum == 3)
                    SceneManager.LoadScene("Aquamarine Floors1");
                else
                                if (blockSelection.GetComponent<BlockSelection>().BlockMaterialNum == 5)
                    SceneManager.LoadScene("TopazFloors1");
                //else
                // SceneManager.LoadScene("mainScene");
            }
        }
        if (Player.isChoosingPlatform)
        {
            GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
            GameObject selectedPlatform = new GameObject();

            foreach (GameObject pl in platforms)
            {
                if (pl.GetComponent<Platform>().ID == Player.selectedPlatfomID)
                    selectedPlatform = pl;
            }
            Platform platform = selectedPlatform.GetComponent<Platform>();
            GameObject sample = GameObject.FindGameObjectWithTag("Sample");
            GameObject piramid = GameObject.FindGameObjectWithTag("Piramid");
            if (Player.selectedPlatfomID != -1 && sample && sample.GetComponent<Platform>().Score != 0 && selectedPlatform.GetComponent<Platform>().NeighborEdgesCount >= sample.GetComponent<Platform>().BlockMaterialNum)
            {
                sample.GetComponent<SampleMotion>().scoreDifferent = sample.GetComponent<Platform>().Score - Player.selectedPlatform.Score;
                Debug.Log(Player.selectedPlatfomID);
                platform.InsertBlock(sample);

                Player.isChoosingPlatform = false;
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                player.GetComponent<Player>().ShowSelectBlockUI();

                piramid.GetComponent<Piramid>().GetPlatfomsInformation();
                piramid.GetComponent<Piramid>().RefreshTotalScore();
                piramid.GetComponent<Piramid>().RefreshNeighborEdgesCount();

                string str = selectedPlatform.GetComponent<Platform>().EdgePositionsToString();
                piramid.GetComponent<Piramid>().RefreshNeighborEdgesCount();
                Player.RefreshBlocksLock();
                GameObject scoreUI = GameObject.FindGameObjectWithTag("UIScore");
                scoreUI.GetComponent<Text>().text = (piramid.GetComponent<Piramid>().totalScore).ToString();
                Destroy(selectedPlatform.transform.Find("YellowLightPrefab(Clone)").gameObject);
                selectedPlatform.transform.Find("Select").gameObject.SetActive(true);

                piramid.GetComponent<Piramid>().TurnHighLightsOFF();
                Player.selectedPlatfomID = -1;
                Player.ReplacePlatformInXML(Player.currentPiramidID, platform.ID, platform.Score, platform.BlockMaterialNum, str);
                sample.GetComponent<SampleMotion>().SetTarget(selectedPlatform.transform.position);
                GameObject canvas = GameObject.FindGameObjectWithTag("MainCanvas");
                Destroy(canvas.transform.Find("SampleScorePrefab(Clone)").gameObject);
                Destroy(canvas.transform.Find("BlockScorePrefab(Clone)").gameObject);

                Player.score = 0;
                piramid.GetComponent<Piramid>().RefreshPiramidScoreLine();

                Destroy(Player.pointer);
            }
            Debug.Log("UPd");
        }
    }

    //private void OnMouseDown()
    //{ 

    //    if (tag.Equals("OK") && !Player.isChoosingPlatform)
    //    {
    //        Player.currentBlockMaterialNum = blockSelection.GetComponent<BlockSelection>().BlockMaterialNum;
    //        Player.score = 0;
    //        Player.PerfectCoef = 1;

    //        GameObject piramid = GameObject.FindGameObjectWithTag("Piramid");
    //        piramid.GetComponent<Piramid>().GetPlatfomsInformation();

    //        Player.currentPiramidID = piramid.GetComponent<Piramid>().ID;
    //        Player.lives = 3;

    //        if (blockSelection.GetComponent<BlockSelection>().BlockMaterialNum == 0)
    //            SceneManager.LoadScene("SaphireFloors1");
    //        else
    //        {
    //            if (blockSelection.GetComponent<BlockSelection>().BlockMaterialNum == 1)
    //                SceneManager.LoadScene("RubyFloors1");
    //            else
    //                if (blockSelection.GetComponent<BlockSelection>().BlockMaterialNum == 2)
    //                SceneManager.LoadScene("EmeraldFloors1");
    //            else
    //                    if (blockSelection.GetComponent<BlockSelection>().BlockMaterialNum == 4)
    //                SceneManager.LoadScene("GoldenFloors1");
    //            else
    //                        if (blockSelection.GetComponent<BlockSelection>().BlockMaterialNum == 3)
    //                            SceneManager.LoadScene("SilverFloors1");
    //            else
    //                            if (blockSelection.GetComponent<BlockSelection>().BlockMaterialNum == 5)
    //                                SceneManager.LoadScene("TopazFloors1");
    //            //else
    //               // SceneManager.LoadScene("mainScene");
    //        }
    //    }
    //    if (Player.isChoosingPlatform)
    //    {
    //        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
    //        GameObject selectedPlatform = new GameObject();

    //        foreach(GameObject pl in platforms)
    //        {
    //            if (pl.GetComponent<Platform>().ID == Player.selectedPlatfomID)
    //                selectedPlatform = pl;
    //        }
    //        Platform platform = selectedPlatform.GetComponent<Platform>();
    //        GameObject sample = GameObject.FindGameObjectWithTag("Sample");
    //        GameObject piramid = GameObject.FindGameObjectWithTag("Piramid");
    //        if (Player.selectedPlatfomID != -1 &&sample && sample.GetComponent<Platform>().Score != 0 && selectedPlatform.GetComponent<Platform>().NeighborEdgesCount >= sample.GetComponent<Platform>().BlockMaterialNum)
    //        {
    //            sample.GetComponent<SampleMotion>().scoreDifferent = sample.GetComponent<Platform>().Score - Player.selectedPlatform.Score;
    //            Debug.Log(Player.selectedPlatfomID);
    //            platform.InsertBlock(sample);

    //            Player.isChoosingPlatform = false;
    //            GameObject player = GameObject.FindGameObjectWithTag("Player");
    //            player.GetComponent<Player>().ShowSelectBlockUI();

    //            piramid.GetComponent<Piramid>().GetPlatfomsInformation();
    //            piramid.GetComponent<Piramid>().RefreshTotalScore();
    //            piramid.GetComponent<Piramid>().RefreshNeighborEdgesCount();

    //            string str = selectedPlatform.GetComponent<Platform>().EdgePositionsToString();
    //            piramid.GetComponent<Piramid>().RefreshNeighborEdgesCount();
    //            Player.RefreshBlocksLock();
    //            GameObject scoreUI = GameObject.FindGameObjectWithTag("UIScore");
    //            scoreUI.GetComponent<Text>().text = (piramid.GetComponent<Piramid>().totalScore).ToString();
    //            Destroy(selectedPlatform.transform.Find("YellowLightPrefab(Clone)").gameObject);
    //            selectedPlatform.transform.Find("Select").gameObject.SetActive(true);

    //            piramid.GetComponent<Piramid>().TurnHighLightsOFF();
    //            Player.selectedPlatfomID = -1;
    //            Player.ReplacePlatformInXML(Player.currentPiramidID, platform.ID, platform.Score, platform.BlockMaterialNum, str);
    //            sample.GetComponent<SampleMotion>().SetTarget(selectedPlatform.transform.position);
    //            GameObject canvas = GameObject.FindGameObjectWithTag("MainCanvas");
    //            Destroy(canvas.transform.Find("SampleScorePrefab(Clone)").gameObject);
    //            Destroy(canvas.transform.Find("BlockScorePrefab(Clone)").gameObject);

    //            piramid.GetComponent<Piramid>().RefreshPiramidScoreLine();
    //        }
    //        Debug.Log("UPd");
    //    }
    //}
}
