using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Piramid : MonoBehaviour
{

    GameObject[] platforms;

    [SerializeField]
    public int ID;
    [SerializeField]
    public int totalScore = 0;

    public static bool isFirst = true;
    public static bool[][] platformsEdgePositions;
    public static int[] platformsBlockMaterialNums;
    public static int[] platformsScore;

    //[SerializeField]
    public static bool[] platformsIsBusy;
    [SerializeField]
    public bool[] platformsIsBusy2;
    // Use this for initialization

    private void Awake()
    {
        Player.selectedPlatfomID = -1;
        Player.currentPiramidID = ID;
        Debug.Log("PiramidAwake");
        totalScore = Player.LoadPiramidTotalScoreFromXML(ID);
        platforms = GameObject.FindGameObjectsWithTag("Platform");
        if (Player.isFirst)//чтобы не пересоздавались при загрузке этой же сцены дважды
        {
            Debug.Log("Init Piramid");
            platformsEdgePositions = new bool[platforms.Length][];
            for (int i = 0; i < platformsEdgePositions.Length; i++)
            {
                platformsEdgePositions[i] = new bool[6];
            }
            platformsBlockMaterialNums = new int[platforms.Length];
            platformsScore = new int[platforms.Length];
            platformsIsBusy = new bool[platforms.Length];

            Player.isFirst = false;
            isFirst = false;
        }
    }
    void Start()
    {
        FillPlatformsFromXML();
        GetPlatfomsInformation();
        RefreshNeighborEdgesCount();
        RefreshPiramidScoreLine();
        HighlightBlocks(Player.currentBlockMaterialNum);

        PlayerPrefs.SetInt("Piramid", ID);
    }

    public void RefreshPiramidScoreLine()
    {
        Debug.Log("FFF");
        RectTransform scoreLine = GameObject.FindGameObjectWithTag("ScoreLine").GetComponent<RectTransform>();
        float val = 0;
        switch (Player.currentPiramidID)
        {
            case 1: val = (float)(Player.Pir1TotalScore) / (Player.Pir1TotalScoreMax); break;
            case 2: val = (float)(Player.Pir2TotalScore) / (Player.Pir2TotalScoreMax); break;
            case 3: val = (float)(Player.Pir3TotalScore) / (Player.Pir3TotalScoreMax); break;
        }
        if (val > 1)
            val = 1;
        float lineLenght = Screen.width * (scoreLine.anchorMax.x - scoreLine.anchorMin.x);
        Vector3 rightLineCorner = scoreLine.GetComponent<RectTransform>().offsetMax;
        rightLineCorner.x = 0 - lineLenght + lineLenght * (val);
        scoreLine.GetComponent<RectTransform>().offsetMax = rightLineCorner;
    }
    public void RefreshNeighborEdgesCount()
    {
        foreach (GameObject pl in platforms)
        {
            pl.GetComponent<Platform>().NeighborEdgesCount = pl.GetComponent<Platform>().FindNeighborEdgesCount();
        }
    }

    public void GetPlatfomsInformation()
    {
        int curTotalScore = 0;
        foreach (GameObject pl in platforms)
        {
            platformsEdgePositions[pl.GetComponent<Platform>().ID] = pl.GetComponent<Platform>().GoodEdgePositions;
            platformsBlockMaterialNums[pl.GetComponent<Platform>().ID] = pl.GetComponent<Platform>().BlockMaterialNum;
            platformsScore[pl.GetComponent<Platform>().ID] = pl.GetComponent<Platform>().Score;
            curTotalScore += pl.GetComponent<Platform>().Score;
            totalScore = curTotalScore;
            switch (Player.currentPiramidID)
            {
                case 1: Player.Pir1TotalScore = totalScore;break;
                case 2: Player.Pir2TotalScore = totalScore; break;
                case 3: Player.Pir3TotalScore = totalScore; break;
            }
            platformsIsBusy[pl.GetComponent<Platform>().ID] = pl.GetComponent<Platform>().IsBusy;
        }
    }

    public void HighlightBlocks(int blockMaterialNum)
    {
        int neighboresNeedsCount = 0;

        switch (blockMaterialNum)
        {
            case 0:
                neighboresNeedsCount = 0; break;
            case 1:
                neighboresNeedsCount = 1; break;
            case 2:
                neighboresNeedsCount = 2; break;
            case 3:
                neighboresNeedsCount = 3; break;
            case 4:
                neighboresNeedsCount = 4; break;
            case 5:
                neighboresNeedsCount = 4; break;
            case 6:
                neighboresNeedsCount = 5; break;
            case 7:
                neighboresNeedsCount = 6; break;
            case 8:
                neighboresNeedsCount = 6; break;
            default:
                neighboresNeedsCount = 100;break;
        }

        foreach (GameObject pl in platforms)
        {
            if (pl.GetComponent<Platform>().NeighborEdgesCount >= neighboresNeedsCount)
                pl.GetComponent<Platform>().backLight.SetActive(true);
            else
                pl.GetComponent<Platform>().backLight.SetActive(false);
        }
    }
    public void TurnHighLightsOFF()
    {
        foreach (GameObject pl in platforms)
            pl.GetComponent<Platform>().backLight.SetActive(false);
    }

    public void RefreshTotalScore()
    {
        totalScore = 0;
        foreach (GameObject pl in platforms)
            totalScore += pl.GetComponent<Platform>().Score;
    }

    public void FillPlatformsFromXML()
    {
        PlatformInformation[] platformsInfromation = Player.LoadDataFromXML(ID);

        for (int i = 0; i < platformsInfromation.Length; i++)
        {
            foreach (GameObject pl in platforms)
            {
                if (pl.GetComponent<Platform>().ID == platformsInfromation[i].ID)
                {
                    pl.GetComponent<Platform>().Score = platformsInfromation[i].Score;
                    pl.GetComponent<Platform>().BlockMaterialNum = platformsInfromation[i].BlockMaterialNum;
                    pl.GetComponent<Platform>().GoodEdgePositions = platformsInfromation[i].GoodEdgePositions;
                    if (pl.GetComponent<Platform>().Score > 0)
                        pl.GetComponent<Platform>().InsertBlock();
                }
            }
            //Platform pl = platforms[platformsInfromation[i].ID].GetComponent<Platform>();

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}