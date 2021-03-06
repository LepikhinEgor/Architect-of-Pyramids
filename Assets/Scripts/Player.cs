﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Xml;
using System.IO;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public static int currentMaxScore;
    public static bool isFirst = true;
    GameObject sample;
    GameObject nextBlockBtn;
    GameObject previousBlockBtn;
    GameObject blockSelection;
    GameObject okButton;
    GameObject cancelButton;

    GameObject piramid;


    [SerializeField]
    public static bool isChoosingPlatform = false;


    [SerializeField]
    public static int currentBlockMaterialNum = 0;

    [SerializeField]
    public static int lives = 3;

    [SerializeField]
    private int levelNum = 0;
    public int LevelNum
    {
        set { levelNum = value; }
        get { return levelNum; }
    }

    [SerializeField]
    public static int score = 0;

    [SerializeField]
    public static int PerfectCoef = 1;

    [SerializeField]
    private bool isWindy = false;
    public bool IsWindy
    {
        set { isWindy = value; }
        get { return isWindy; }
    }

    public static PerfectTimer perfectTimer;
    private Block block;

    private float lightSpeed = 0.2F;
    private bool isLight = false;
    private void Awake()
    {
    }
    private void Start()
    {
        if (!File.Exists(Application.persistentDataPath + "/Save/Save.xml"))
        {
            Debug.Log("FFFF");
            CreateXML(); }

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("mainScene")
            || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("SaphireFloors1")
            || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("EmeraldFloors1")
            || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GoldenFloors1")
            || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("rubyFloors1")
            || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("SilverFloors1")
            || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("TopazFloors1"))
        {

            block = FindObjectOfType<Block>();
            perfectTimer = GameObject.FindGameObjectWithTag("PerfectTimer").GetComponent<PerfectTimer>();
            GameObject scoreLine = GameObject.FindGameObjectWithTag("ScoreLine");
            scoreLine.GetComponent<Slider>().value = 0;
        }
        

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Piramid1") ||
            SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Piramid2"))
        {

            blockSelection = GameObject.FindGameObjectWithTag("BlockSelection");
            okButton = GameObject.FindGameObjectWithTag("OK");
            cancelButton = GameObject.FindGameObjectWithTag("Cancel");

            sample = GameObject.FindGameObjectWithTag("Sample");
            piramid = GameObject.FindGameObjectWithTag("Piramid");

            sample.SetActive(true);
            if (Player.score != 0)
            {
                sample.GetComponent<Platform>().Score = Player.score;
                sample.GetComponent<Platform>().BlockMaterialNum = Player.currentBlockMaterialNum;
                int goodEdgesCount = CalcGENbyScore();
                sample.GetComponent<Platform>().SetRandomGoodEdgePositions(goodEdgesCount);
                sample.GetComponent<Platform>().InsertBlock();
                piramid.GetComponent<Piramid>().RefreshNeighborEdgesCount();
                //piramid.GetComponent<Piramid>().HighlightBlocks(sample.GetComponent<Platform>().BlockMaterialNum);

            }

            piramid.GetComponent<Piramid>().TurnHighLightsOFF();
            piramid.GetComponent<Piramid>().HighlightBlocks(sample.GetComponent<Platform>().BlockMaterialNum);
            ShowSelectBlockUI();
            if (isChoosingPlatform)
                HideSelectBlockUI();
            Debug.Log("start");
        }
        CalcMaxScore();
    }
    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("mainScene")
            || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("SaphireFloors1")
            || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("EmeraldFloors1")
            || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("GoldenFloors1")
            || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("rubyFloors1")
            || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("SilverFloors1")
            || SceneManager.GetActiveScene() == SceneManager.GetSceneByName("TopazFloors1"))
        {
            if (isWindy)
                block.rigidBodyBlock.AddForce(transform.right * 2F, ForceMode2D.Force);
            if (perfectTimer)
            {
                if (perfectTimer.Timer > 0)
                {
                    isLight = true;
                    Vector3 maskPosition = block.GetComponentInChildren<SpriteMask>().transform.position;
                    maskPosition.y -= Time.deltaTime * lightSpeed;
                    maskPosition.x = block.transform.position.x;
                    block.GetComponentInChildren<SpriteMask>().transform.position = maskPosition;
                }
                else
                {
                    if (isLight)
                    {
                        Debug.Log("HEY");
                        isLight = false;
                        block.SetStrandartMaskPosition();

                        PerfectCoef = 1;
                        block.blockSpriteActive.SetActive(false);
                        block.blockSpriteActiveBack.SetActive(false);
                    }
                }
            }
        }
    }

    public void CalcMaxScore()
    {
        int floorsNum = (currentBlockMaterialNum + 1) * 6; //считаются все, кроме первого
        int maxScore = 0;

        for (int i = 2; i <= (floorsNum + 1); i++) //начинается сразу с x2, заканчивается x(n+1)
            maxScore += i;
        maxScore *= 2;
        maxScore = maxScore + (int)(maxScore * 3 * 0.05);
        currentMaxScore = maxScore;
        Debug.Log(maxScore);
    }

    public int CalcGENbyScore()
    {

        int goodEdgeNum = 6;

        score = (int)(score + score * lives * 0.05);
        Debug.Log(score);
        for (int i = 1; i <= 6; i++)
        {
            if (currentMaxScore / 12 * i > score)
            {
                goodEdgeNum = i - 1;
                break;
            }
        }

        return goodEdgeNum;
    }
    public void HideSelectBlockUI()
    {
        blockSelection.SetActive(false);
        okButton.SetActive(false);
        cancelButton.SetActive(true);
        sample.SetActive(true);
    }
    public void ShowSelectBlockUI()
    {
        blockSelection.SetActive(true);
        okButton.SetActive(true);
        cancelButton.SetActive(false);
        sample.SetActive(false);
    }

    public static void ReplacePlatformInXML(int piramidID, int ID, int score, int blockMaterialNum, String edgePositions)
    {

        isFirst = false;
        XmlDocument xmlSave = new XmlDocument();

        if (File.Exists(Application.persistentDataPath + "/Save/Save.xml"))
            xmlSave.Load(Application.persistentDataPath + "/Save/Save.xml");
        else
            Debug.Log("Cannot read file, when replace platform");

        XmlNode currentNode;
        XmlNodeList nodes = xmlSave.GetElementsByTagName("Platform");
        foreach (XmlNode node in nodes)
        {
            String tmp = "Piramid";
            tmp += piramidID.ToString();
            if (node.Attributes[0].Value.Equals(ID.ToString()) && node.ParentNode.Name.Equals(tmp))
            {
                currentNode = node;
                node.Attributes[1].InnerText = score.ToString();
                node.Attributes[2].InnerText = blockMaterialNum.ToString();
                node.Attributes[3].InnerText = edgePositions;
            }
        }

        xmlSave.Save(Application.persistentDataPath + "/Save/Save.xml");

    }

    public static void CreateXML()
    {
        XmlDocument xmlSave = new XmlDocument();
        XmlNode informationNode = xmlSave.CreateElement("Information");
        xmlSave.AppendChild(informationNode);

        XmlNode newNode;
        XmlNode piramid;
        XmlAttribute newAttribute;

        piramid = xmlSave.CreateElement("Piramid1");
        informationNode.AppendChild(piramid);
        piramid = xmlSave.CreateElement("Piramid2");
        informationNode.AppendChild(piramid);
        piramid = xmlSave.CreateElement("Piramid3");
        informationNode.AppendChild(piramid);
        for (int i = 1; i <= 3; i++)
        {
            string piramidName = "Piramid" + i.ToString();
            piramid = xmlSave.GetElementsByTagName(piramidName)[0];
            int platformsNum = 0;
            switch (i)
            {
                case 1: platformsNum = 6;break;
                case 2: platformsNum = 10; break;
                case 3: platformsNum = 21; break;
            }
            for (int j = 0; j < platformsNum; j++)
            {
                newNode = xmlSave.CreateElement("Platform");
                newAttribute = xmlSave.CreateAttribute("ID");
                newAttribute.Value = j.ToString();
                newNode.Attributes.Append(newAttribute);

                newAttribute = xmlSave.CreateAttribute("Score");
                newAttribute.Value = "0";
                newNode.Attributes.Append(newAttribute);

                newAttribute = xmlSave.CreateAttribute("BlockMaterialNum");
                newAttribute.Value = "0";
                newNode.Attributes.Append(newAttribute);

                newAttribute = xmlSave.CreateAttribute("EdgePositions");
                newAttribute.Value = "000000";
                newNode.Attributes.Append(newAttribute);

                piramid.AppendChild(newNode);
            }
        }
        System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Save");
        xmlSave.Save(Application.persistentDataPath + "/Save/Save.xml");
    }

    public static PlatformInformation[] LoadDataFromXML(int piramidID)
    {
        int size = 0;
        switch (piramidID)
        {
            case 1: size = 6;break;
            case 2: size = 10; break;
            case 3: size = 21; break;
        }

        PlatformInformation[] platformsInformation = new PlatformInformation[size]; ;
        for (int i = 0; i < platformsInformation.Length; i++)
        {
            platformsInformation[i] = new PlatformInformation();
        }
        XmlDocument xmlSave = new XmlDocument();

        String tmp = "Piramid";
        tmp += piramidID.ToString();

        if (File.Exists(Application.persistentDataPath + "/Save/Save.xml"))
        {
            xmlSave.Load(Application.persistentDataPath + "/Save/Save.xml");

            XmlNode piramid = xmlSave.GetElementsByTagName(tmp)[0];

            foreach (XmlNode platform in piramid.ChildNodes)
            {
                int curID = Convert.ToInt32(platform.Attributes[0].Value);

                platformsInformation[curID].ID = curID;
                platformsInformation[curID].Score = Convert.ToInt32(platform.Attributes[1].Value);
                platformsInformation[curID].BlockMaterialNum = Convert.ToInt32(platform.Attributes[2].Value);

                char[] postitionsString = platform.Attributes[3].Value.ToCharArray();
                bool[] newGoodEdgePositions = new bool[6];
                for (int i = 0; i < newGoodEdgePositions.Length; i++)
                {
                    if (postitionsString[i].Equals('1'))
                        newGoodEdgePositions[i] = true;
                    if (postitionsString[i].Equals('0'))
                        newGoodEdgePositions[i] = false;
                }

                platformsInformation[curID].GoodEdgePositions = newGoodEdgePositions;
            }
        }

        return platformsInformation;
    }
}
