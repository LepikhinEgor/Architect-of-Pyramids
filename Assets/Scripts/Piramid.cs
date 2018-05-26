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
        foreach (GameObject pl in platforms)
        {
            if (pl.GetComponent<Platform>().NeighborEdgesCount >= blockMaterialNum)
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