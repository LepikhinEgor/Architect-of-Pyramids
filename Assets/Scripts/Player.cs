using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Xml;
using System.IO;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static bool isLoadingScene = false;
    public static int selectedPlatfomID = -1;
    public static UnityEngine.Object perfectCoefPrefab;
    public static UnityEngine.Object resultWindowPrefab;
    public static UnityEngine.Object blockScorePrefab;
    public static UnityEngine.Object sampleScorePrefab;

    public static bool currPiramidIsLock = false;
    private UnityEngine.Object prohibitWindowPrefab;
    GameObject prohibitWindow;

    static bool isMoving;
    Vector3 target, camPos;
    bool isMovingToNextPiramid = false;
    bool isMovingToPrevPiramid = false;
    float lastXPos, currentXPos;
    Camera cam;
    float deltaXPos;
    bool isScrolling;
    public static int currentPiramidID;
    public static int currentMaxScore;
    public static bool isFirst = true;
    GameObject sample;
    GameObject backButton;
    static GameObject blockSelection;
    GameObject okButton;
    GameObject cancelButton;

    GameObject piramid;

    public static int Pir1TotalScore = 0;
    public static int Pir2TotalScore = 0;
    public static int Pir3TotalScore = 0;

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
        Pir1TotalScore = LoadPiramidTotalScoreFromXML(1);
        Pir2TotalScore = LoadPiramidTotalScoreFromXML(2);
        Pir3TotalScore = LoadPiramidTotalScoreFromXML(3);
    }
    private void Start()
    {
        perfectCoefPrefab = Resources.Load("Prefabs/PerfectCoefPrefab");
        resultWindowPrefab = Resources.Load("Prefabs/ResultsWindowPrefab"); 
        blockScorePrefab = Resources.Load("Prefabs/BlockScorePrefab");
        sampleScorePrefab = Resources.Load("Prefabs/SampleScorePrefab");
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
            RectTransform scoreLine = GameObject.FindGameObjectWithTag("ScoreLine").GetComponent<RectTransform>();

            float lineLenght = Screen.width*(scoreLine.anchorMax.x - scoreLine.anchorMin.x)*0.8F;
            Vector3 rightLineCorner = scoreLine.GetComponent<RectTransform>().offsetMax;
            rightLineCorner.x -= lineLenght;
            scoreLine.GetComponent<RectTransform>().offsetMax = rightLineCorner;
        }
        

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Piramid1") ||
            SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Piramid2") ||
            SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Piramid3"))
        {
            
            prohibitWindowPrefab = Resources.Load("Prefabs/ProhibitWindowPrefab");

            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            GameObject scoreUI = GameObject.FindGameObjectWithTag("UIScore");
            scoreUI.GetComponent<Text>().text = (LoadPiramidTotalScoreFromXML(currentPiramidID)).ToString();

            backButton = GameObject.FindGameObjectWithTag("Back");
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

            GameObject canvas = GameObject.FindGameObjectWithTag("MainCanvas");
            if (currentPiramidID == 2 && Player.Pir1TotalScore < 1000)
            {
                Debug.Log("Window prohibit build 2 pyramid");
                currPiramidIsLock = true;
                Instantiate(prohibitWindowPrefab, canvas.transform);
                prohibitWindow = GameObject.FindGameObjectWithTag("Window");
                string message = "To unlock this pyramid you need " + (1000 - Pir1TotalScore).ToString() + " more points in first pyramid";
                prohibitWindow.GetComponent<ProhibitWindow>().SetText(message);
                okButton.SetActive(false);
            }
            else
                currPiramidIsLock = false;
            Debug.Log(currentPiramidID);
            if (currentPiramidID == 3 && Player.Pir2TotalScore < 100)
            {
                Debug.Log("Window prohibit build 3 pyramid");
                currPiramidIsLock = true;
                Instantiate(prohibitWindowPrefab, canvas.transform);
                prohibitWindow = GameObject.FindGameObjectWithTag("Window");
                string message = "To unlock this pyramid you need " + (4000 - Pir2TotalScore).ToString() + " more points in second pyramid";
                prohibitWindow.GetComponent<ProhibitWindow>().SetText(message);
                okButton.SetActive(false);
            }
            else
                currPiramidIsLock = false;
            if (isChoosingPlatform)
            {
                Instantiate(sampleScorePrefab, canvas.transform);
                canvas.transform.Find("SampleScorePrefab(Clone)").gameObject.GetComponent<Text>().text = Player.score.ToString();
            }
            Debug.Log("start");
        }
        CalcMaxScore();
    }
    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Piramid1") ||
            SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Piramid2") ||
            SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Piramid3"))
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastXPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                float mouseYPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
                //deltaXPos = lastXPos - cam.transform.position.x;
                deltaXPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
                if (mouseYPos > -1 && mouseYPos < 5 && !isChoosingPlatform)
                    isScrolling = true;
                prohibitWindow = GameObject.FindGameObjectWithTag("Window");
            }

            if (Input.GetMouseButton(0) && isScrolling)
            {
                if (!Player.currPiramidIsLock && prohibitWindow != null)
                    Destroy(prohibitWindow);
                Vector3 camPos = cam.transform.position;
                float offsetX = cam.ScreenToWorldPoint(Input.mousePosition).x - deltaXPos;

                if (prohibitWindow != null)
                    Destroy(prohibitWindow);

                if (currentPiramidID == 1)
                {
                    if (offsetX <= 0 || cam.transform.position.x >= 0)
                    {
                        if (camPos.x - offsetX < 0)
                            camPos.x = 0;
                        else
                            camPos.x -= offsetX;
                        cam.transform.position = camPos;
                    }
                    else
                    {
                        camPos.x = 0;
                        cam.transform.position = camPos;
                    }
                }

                if (currentPiramidID == 2)
                {
                    camPos.x -= offsetX;
                    cam.transform.position = camPos;
                }

                if (currentPiramidID == 3)
                {
                    if (offsetX >= 0 || cam.transform.position.x <= 0)
                    {
                        if (camPos.x - offsetX >= 0)
                            camPos.x = 0;
                        else
                            camPos.x -= offsetX;
                        cam.transform.position = camPos;
                    }
                    else
                    {
                        camPos.x = 0;
                        cam.transform.position = camPos;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && isScrolling)
            {
                target = cam.transform.position;
                target.z = -20;
                // target.x = 6;
                if (currentPiramidID == 1)
                    if (cam.transform.position.x > 3)
                    {
                        target.x = 6;
                        isMovingToNextPiramid = true;
                    }
                if (currentPiramidID == 2)
                {
                    if (cam.transform.position.x > 4)
                    {
                        target.x = 7.3F;
                        isMovingToNextPiramid = true;
                    }
                    if (cam.transform.position.x < -3)
                    {
                        target.x = -6F;
                        isMovingToPrevPiramid = true;
                    }
                }
                if (currentPiramidID == 3)
                    if (cam.transform.position.x < -4)
                    {
                        target.x = -7.3F;
                        isMovingToPrevPiramid = true;
                    }
                target.z = -20;
                isScrolling = false;
                isMoving = true;
            }

        }

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
        if (Player.currentPiramidID == 1 && isMoving)
        {
            if (isMovingToNextPiramid)
            {
                prohibitWindow = GameObject.FindGameObjectWithTag("Window");

                CameraTo(cam.transform.position, target, 10);
                if (Math.Abs(cam.transform.position.x - target.x) < 0.5F && prohibitWindow == null)
                { 
                    SceneManager.LoadSceneAsync("Piramid2");
                    isFirst = true;
                    isMovingToNextPiramid = false;
                    isMoving = false;
                }
            }
            else
            {
                target.x = 0;
                CameraTo(cam.transform.position, target, 10);
                if (cam.transform.position == target)
                    isMoving = false;
            }
        }

        if (Player.currentPiramidID == 2 && isMoving)
        {
            if (!isMovingToPrevPiramid && !isMovingToNextPiramid)
            {
                target.x = 0;
                CameraTo(cam.transform.position, target, 10);
                if (cam.transform.position == target)
                    isMoving = false;
            }

            if (isMovingToPrevPiramid)
            {
                CameraTo(cam.transform.position, target, 10);
                if (Math.Abs(cam.transform.position.x - target.x) < 0.5F)
                {
                    SceneManager.LoadSceneAsync("Piramid1");
                    isFirst = true;
                    isMovingToNextPiramid = false;
                    isMoving = false;
                }
            }

            if (isMovingToNextPiramid)
            {
                target.x = 7.3F;
                Debug.Log(cam.transform.position.x);
                CameraTo(cam.transform.position, target, 10);
                if (Math.Abs(cam.transform.position.x - target.x) < 0.5F)
                {
                    SceneManager.LoadSceneAsync("Piramid3");
                    isFirst = true;
                    isMovingToNextPiramid = false;
                    isMoving = false;
                }
            }
        }
        if (Player.currentPiramidID == 3 && isMoving)
        {
            if (isMovingToPrevPiramid)
            {
                CameraTo(cam.transform.position, target, 10);
                if (Math.Abs(cam.transform.position.x - target.x) < 0.5F && prohibitWindow == null)
                {
                    SceneManager.LoadSceneAsync("Piramid2");
                    isFirst = true;
                    isMovingToNextPiramid = false;
                    isMoving = false;
                }
            }
            else
            {
                target.x = 0;
                CameraTo(cam.transform.position, target, 10);
                if (cam.transform.position == target)
                    isMoving = false;
            }
        }
    }

    private static void CameraTo(Vector3 oldPos, Vector3 targetPos, float speed)
    {
        float newSpeed = speed;
        targetPos.z = -20F;
        if (Math.Abs(targetPos.x - Camera.main.transform.position.x) < 1F && targetPos.x == 0)
        {
            blockSelection.GetComponent<BlockSelection>().SetNearestBlockColor();
            newSpeed = 30;
        }
        Camera.main.transform.position = Vector3.Lerp(oldPos, targetPos, newSpeed * Time.deltaTime);
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
    }

    public int CalcGENbyScore()
    {
        int goodEdgeNum = 6;
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
        //информация для blockSelection, так как при скрытии UI при запуске он не успевает сам инициилизироваться
        blockSelection.GetComponent<BlockSelection>().blockColors =  GameObject.FindGameObjectsWithTag("BlockColor");
        blockSelection.GetComponent<BlockSelection>().piramid = GameObject.FindGameObjectWithTag("Piramid");
        blockSelection.SetActive(false);
        okButton.SetActive(true);
        cancelButton.SetActive(true);
        sample.SetActive(true);
    }
    public void ShowSelectBlockUI()
    {
        blockSelection.SetActive(true);
        okButton.SetActive(true);
        cancelButton.SetActive(false);
        //sample.SetActive(false);
    }

    public static void RefreshBlocksLock()
    {
        blockSelection.GetComponent<BlockSelection>().RefreshBlocksLock();
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

    public static int LoadPiramidTotalScoreFromXML(int piramidID)
    {
        XmlDocument xmlSave = new XmlDocument();

        String tmp = "Piramid";
        tmp += piramidID.ToString();

        int totalScore = 0;
        if (File.Exists(Application.persistentDataPath + "/Save/Save.xml"))
        {
            xmlSave.Load(Application.persistentDataPath + "/Save/Save.xml");

            XmlNode piramid = xmlSave.GetElementsByTagName(tmp)[0];

            foreach (XmlNode platform in piramid.ChildNodes)
            {
                totalScore += Convert.ToInt32(platform.Attributes[1].Value);
            }
        }

        return totalScore;
    }

    public static void ReloadFloors()
    {
        Player.score = 0;
        Player.PerfectCoef = 0;
        Player.perfectTimer.Timer = 0;
        Player.lives = 3;

        string sceneName = "";
        switch(currentBlockMaterialNum)
        {
            case 0: sceneName = "SaphireFloors1";break;
            case 1: sceneName = "RubyFloors1"; break;
            case 2: sceneName = "EmeraldFloors1"; break;
            case 3: sceneName = "SilverFloors1"; break;
            case 4: sceneName = "GoldenFloors1"; break;
            case 5: sceneName = "TopazFloors1"; break;
            case 6: sceneName = "AmetistFloors1"; break;
        }
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadPiramidScene()
    {
        if (score == 0)
            isChoosingPlatform = false;
        else
            isChoosingPlatform = true;

        switch (currentPiramidID)
        {
            case 1: SceneManager.LoadSceneAsync("Piramid1");break;
            case 2: SceneManager.LoadSceneAsync("Piramid2"); break;
            case 3: SceneManager.LoadSceneAsync("Piramid3"); break;
        }
    }

    public static void ShowResultWindow()
    {
        GameObject canvas = GameObject.FindGameObjectWithTag("MainCanvas");
        Instantiate(Player.resultWindowPrefab, canvas.transform);
        GameObject resultWindow = GameObject.FindGameObjectWithTag("ResultWindow");

        string message = "Score: " + (Player.score).ToString() + "/" + (Player.currentMaxScore).ToString();

        resultWindow.GetComponent<ProhibitWindow>().SetText(message);
    }
}
