using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BlockSelection : MonoBehaviour {

    public GameObject[] blockColors;
    public GameObject piramid;

    GameObject canvas;
    Text blockMaterialText;
    UnityEngine.Object prohibitWindowPrefab;
    GameObject prohibitWindow;
    GameObject okButton;

    //сообщения для каждого блока, показываюшие сколько ещё нужно очков
    string[] prohibitMessages;

    //переменные для прокрутки списка блоков
    bool isScrolling;
    bool isDamping = false;

    int dampingFrameNum;
    int currentDampingFrame;

    float mouseYPos;
    float blockDeltaPos = 0;
    float lastMouseXPos;
    float currentMouseXpos;
    float deltaXPos;

    [SerializeField]
    private int blockMaterialNum;
    public int BlockMaterialNum
    {
        set { this.blockMaterialNum = value; }
        get { return this.blockMaterialNum; }
    }


    private void Awake()
    {
        Player.blockSelection = this.gameObject;
        isScrolling = false;
        prohibitWindowPrefab = Resources.Load("Prefabs/ProhibitWindowPrefab");
        dampingFrameNum = 10;
        currentDampingFrame = dampingFrameNum;
        blockMaterialNum = 0;
        prohibitMessages = new string[9];
    }
    void Start () {

        canvas = GameObject.FindGameObjectWithTag("MainCanvas");
        blockMaterialText = canvas.transform.Find("BlockMaterial").GetComponent<Text>();

        piramid = GameObject.FindGameObjectWithTag("Piramid");
        SetBlocksPosition();
        RefreshBlocksLock();
        SetNearestBlockColor();
    }

    void SetBlocksPosition()
    {
        blockColors = GameObject.FindGameObjectsWithTag("BlockColor");

        foreach (GameObject blockColor in blockColors)
        {
            Vector3 blockColorPos = blockColor.transform.position;
            //ставим текущий блок в центре и увеличиваем в размере
            blockColorPos.x -= Player.currentBlockMaterialNum * 1.4F;
            blockColor.transform.position = blockColorPos;
            if (Math.Abs(blockColor.transform.position.x) < 0.2F)
            {
                Vector3 tmp = blockColor.transform.localScale;
                tmp.x = 2.3F;
                tmp.y = 2.3F;
                blockColor.transform.localScale = tmp;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            lastMouseXPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            mouseYPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
            if (mouseYPos > transform.position.y - 0.9 && mouseYPos < transform.position.y + 0.9 && !Player.isChoosingPlatform)
            {
                isScrolling = true;
                if (!Player.currPiramidIsLock && prohibitWindow != null)
                    Destroy(prohibitWindow);
            }
        }

        if (Input.GetMouseButton(0) && isScrolling)
        {
            MoveBlocks();
        }
        
        if (Input.GetMouseButtonUp(0) && isScrolling)
        {
            isScrolling = false;
            isDamping = true;
            blockDeltaPos = deltaXPos;

            if (Math.Abs(deltaXPos) < 0.1F)
                dampingFrameNum = currentDampingFrame = 4;
            if (Math.Abs(deltaXPos) > 0.1F && Math.Abs(deltaXPos) < 0.5F)
                dampingFrameNum = currentDampingFrame = 7;
            if (Math.Abs(deltaXPos) > 0.5F)
                dampingFrameNum = currentDampingFrame = 10;
        }
        if (isDamping)
        {
            DampingScroll();
        }
    }

    void MoveBlocks()
    {
        //вызывается при удерживании пальца на списке блоков
        //перемещает блоки в зависимости от движения пальца
        currentMouseXpos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        deltaXPos = currentMouseXpos - lastMouseXPos;

        foreach (GameObject blockColor in blockColors)
        {
            Vector3 tmp = blockColor.transform.position;
            tmp.x += deltaXPos;
            blockColor.transform.position = tmp;

            tmp = blockColor.transform.localScale;
            float blockScale;
            if (Math.Abs(blockColor.transform.position.x) < 0.7F)
            {
                if (Math.Abs(blockColor.transform.position.x) < 0.15F)
                {
                    tmp.x = 2.3F;
                    tmp.y = 2.3F;
                    blockColor.transform.localScale = tmp;
                }
                else
                {
                    blockScale = 1.8F + 0.65F - Math.Abs(blockColor.transform.position.x);// здесь получше бы алгоритм
                    tmp.x = blockScale;
                    tmp.y = blockScale;
                    blockColor.transform.localScale = tmp;
                }
            }
            else
            {
                tmp.x = 1.8F;
                tmp.y = 1.8F;
                blockColor.transform.localScale = tmp;
            }

        }

        lastMouseXPos = currentMouseXpos;
    }

    public void DampingScroll()
    {
        //Вызывается при отпускании пальца от экрана
        //Делает плавное затухание прокрутки списка блоков
        Vector3 blockColorPos;

        float firstBlockColorPosX = 0;
        float lastBlockColorPosX  = 8.4F;

        foreach (GameObject blockColor in blockColors)
        {
            if (blockColor.GetComponent<BloсkSprite>().ID == 0)
                firstBlockColorPosX = blockColor.transform.position.x;
            if (blockColor.GetComponent<BloсkSprite>().ID == 8)
                lastBlockColorPosX = blockColor.transform.position.x;
        }

        bool isEndOfDamping = (firstBlockColorPosX >= 0 && blockDeltaPos > 0) || (lastBlockColorPosX <= 0 && blockDeltaPos < 0);
        if (!isEndOfDamping)
            foreach (GameObject blockColor in blockColors)
            {
                blockColorPos = blockColor.transform.position;
                blockColorPos.x += blockDeltaPos;
                blockColor.transform.position = blockColorPos;
            }
        else
        {
            isDamping = false;
            currentDampingFrame = dampingFrameNum;
            SetNearestBlockColor();
        }

        currentDampingFrame--;
        blockDeltaPos -= blockDeltaPos / dampingFrameNum;
        if (currentDampingFrame == 0)
        {
            isDamping = false;
            currentDampingFrame = dampingFrameNum;
            SetNearestBlockColor();
        }
    }

    public void SetNearestBlockColor()
    {
        //Делает самый близкий блок к центру текущим, увеличивает его в размере
        float minDeltaX = 100;// ближайший к центру блок

        for (int i = 0; i < blockColors.Length; i++)
            if (Math.Abs(blockColors[i].transform.position.x) < Math.Abs(minDeltaX))
            {
                minDeltaX = blockColors[i].transform.position.x;
            }

        foreach (GameObject blockColor in blockColors)
        {
            Vector3 tmp = blockColor.transform.position;
            tmp.x -= minDeltaX;
            blockColor.transform.position = tmp;
            if (Math.Abs(blockColor.transform.position.x) < 0.5F)
            {
                if (blockColor.GetComponent<BloсkSprite>().isUnlocked)
                { 
                    blockMaterialNum = blockColor.GetComponent<BloсkSprite>().ID;
                    Player.currentBlockMaterialNum = blockColor.GetComponent<BloсkSprite>().ID;
                }
                else
                {
                    piramid.GetComponent<Piramid>().TurnHighLightsOFF();
                    BlockMaterialNum = 100;
                    int id = blockColor.GetComponent<BloсkSprite>().ID;
                    CreateProhibitWindow(id);
                    Debug.Log(prohibitMessages[id]);
                }

                string blockMaterial = "";
                switch (blockColor.GetComponent<BloсkSprite>().ID)
                {
                    case 0: blockMaterial = "Sapphire block";break;
                    case 1: blockMaterial = "Ruby block"; break;
                    case 2: blockMaterial = "Emerald block"; break;
                    case 3: blockMaterial = "Aquamarine block"; break;
                    case 4: blockMaterial = "Silver block";break;
                    case 5: blockMaterial = "Golden block";break;
                    case 6: blockMaterial = "Onyx block"; break;
                    case 7: blockMaterial = "Pearl block"; break;
                    case 8: blockMaterial = "Amethyst block";break;
                    default:
                        blockMaterial = "Unknown block";
                        Debug.Log("Cannot identify block color");
                        break;
                }
                blockMaterialText.text = blockMaterial;

                piramid.GetComponent<Piramid>().HighlightBlocks(blockMaterialNum);

                blockColor.transform.localScale = new Vector3(2.2F,2.2F, 1);
            }
            else
            {
                blockColor.transform.localScale = new Vector3(1.8F, 1.8F, 1);
            }
        }
    }

    void CreateProhibitWindow(int messageID)
    {
        //создает окно, указывающее сколько очков нехватает для блока
        prohibitWindow = GameObject.FindGameObjectWithTag("Window");
        if (prohibitWindow == null && !Player.currPiramidIsLock)
        {
            Debug.Log("Create prohibit window, when lock block selected");
            canvas = GameObject.FindGameObjectWithTag("MainCanvas");
            prohibitWindow = (GameObject)Instantiate(prohibitWindowPrefab, canvas.transform);
            prohibitWindow.GetComponent<ProhibitWindow>().SetText(prohibitMessages[messageID]);
        }
        else
            Debug.Log("Cannot create prohibit window, when lock block selected");
    }

    public void RefreshBlocksLock()
    {
        //Вызывается при изменении очков в пирамиде
        // Обновляет информацию о блоках, указывающую на их доступность

        int piramidID = Player.currentPiramidID;
        piramidID--;

        foreach (GameObject blockColor in blockColors)
        {
            int blockID = blockColor.GetComponent<BloсkSprite>().ID;
            if (piramidID == 0)
            {
                switch (blockID)
                {
                    case 0:
                        blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                        blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        break;
                    case 1:
                        if (piramid.GetComponent<Piramid>().totalScore > Player.needScoreCount[piramidID][1])
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 2:
                        if (piramid.GetComponent<Piramid>().totalScore > Player.needScoreCount[piramidID][2])
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                }

                prohibitMessages[0] = "First block locked, it's error, please tell us about this in Google Play";
                for (int i = 1; i < 9; i++)
                {
                    if (i < 3)
                    {
                        string needMoreScore = (Player.needScoreCount[piramidID][i] - Player.Pir1TotalScore).ToString();
                        prohibitMessages[i] = "To open this block you need " + needMoreScore + " more points";
                    }
                    
                    if (i >= 3)
                    {
                        prohibitMessages[i] = "This block will be unlocked during the construction of the second pyramid";
                    }
                    if (i >= 6)
                    {
                        prohibitMessages[i] = "This block will be unlocked during the construction of the third pyramid";
                    }
                }
            }
            if (piramidID == 1)
            {
                switch (blockID)
                {
                    case 0:
                        blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                        blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        break;
                    case 1:
                        if (piramid.GetComponent<Piramid>().totalScore > Player.needScoreCount[piramidID][1])
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 2:
                        if (piramid.GetComponent<Piramid>().totalScore > Player.needScoreCount[piramidID][2])
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 3:
                        if (piramid.GetComponent<Piramid>().totalScore > Player.needScoreCount[piramidID][3])
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 4:
                        if (piramid.GetComponent<Piramid>().totalScore > Player.needScoreCount[piramidID][4])
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 5:
                        if (piramid.GetComponent<Piramid>().totalScore > Player.needScoreCount[piramidID][5])
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                }
                prohibitMessages[0] = "First block locked, it's error, please tell us about this in Google Play";
                for (int i = 1; i < 9; i++)
                {
                    if (i < 6)
                    {
                        string needMoreScore = (Player.needScoreCount[piramidID][i] - Player.Pir2TotalScore).ToString();
                        prohibitMessages[i] = "To open this block you need " + needMoreScore + " more points";
                    }

                    if (i >= 6)
                    {
                        prohibitMessages[i] = "This block will be unlocked during the construction of the third pyramid";
                    }
                }
            }
            if (piramidID == 2)
            {
                Debug.Log("Third");
                switch (blockID)
                {
                    case 0:
                        blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                        blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        break;
                    case 1:
                        if (piramid.GetComponent<Piramid>().totalScore > Player.needScoreCount[piramidID][1])
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 2:
                        if (piramid.GetComponent<Piramid>().totalScore > Player.needScoreCount[piramidID][2])
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 3:
                        if (piramid.GetComponent<Piramid>().totalScore > Player.needScoreCount[piramidID][3])
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 4:
                        if (piramid.GetComponent<Piramid>().totalScore > Player.needScoreCount[piramidID][4])
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 5:
                        if (piramid.GetComponent<Piramid>().totalScore > Player.needScoreCount[piramidID][5])
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 6:
                        if (piramid.GetComponent<Piramid>().totalScore > Player.needScoreCount[piramidID][6])
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 7:
                        if (piramid.GetComponent<Piramid>().totalScore > Player.needScoreCount[piramidID][7])
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 8:
                        if (piramid.GetComponent<Piramid>().totalScore > Player.needScoreCount[piramidID][8])
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                }
                prohibitMessages[0] = "First block locked, it's error, please tell us about this in Google Play";
                for (int i = 1; i < 9; i++)
                {
                    string needMoreScore = (Player.needScoreCount[piramidID][i] - Player.Pir3TotalScore).ToString();
                    prohibitMessages[i] = "To open this block you need " + needMoreScore + " more points";
                }
            }
        }
    }
}