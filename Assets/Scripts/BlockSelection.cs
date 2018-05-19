using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class BlockSelection : MonoBehaviour {

    private UnityEngine.Object prohibitWindowPrefab;
    GameObject prohibitWindow;

    string[] prohibitMessages;

    float blockDeltaPos = 0;
    private bool isDamping = false;
    int dampingFrameNum;
    int currentDampingFrame;
    private static float lastMouseXPos;
    private float currentMouseXpos;
    private float deltaXPos;
    public GameObject[] blockColors;

    public GameObject piramid;

    [SerializeField]
    private int blockMaterialCount;
    public int BlockMaterialCount
    {
        set { this.blockMaterialCount = value; }
        get { return this.blockMaterialCount; }
    }

    [SerializeField]
    private int blockMaterialNum;
    public int BlockMaterialNum
    {
        set { this.blockMaterialNum = value; }
        get { return this.blockMaterialNum; }
    }

    // Use this for initialization
    private void Awake()
    {
        prohibitWindowPrefab = Resources.Load("Prefabs/ProhibitWindowPrefab");
        dampingFrameNum = 10;
        currentDampingFrame = dampingFrameNum;
        blockMaterialNum = 0;
        prohibitMessages = new string[9];
    }
    void Start () {
        Debug.Log("StratBlSel");
        piramid = GameObject.FindGameObjectWithTag("Piramid");

        blockColors = GameObject.FindGameObjectsWithTag("BlockColor");
        foreach (GameObject blockColor in blockColors)
        {
            Vector3 blockColorPos = blockColor.transform.position;
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
        RefreshBlocksLock();
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            Destroy(prohibitWindow);
            lastMouseXPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x; ;
        }

        if (Input.GetMouseButton(0))
        {
            currentMouseXpos =  Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
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
                    else{
                        blockScale = 1.8F + 0.65F - Math.Abs(blockColor.transform.position.x);// здесь получше бы алгоритм
                        tmp.x = blockScale;
                        tmp.y = blockScale;
                        blockColor.transform.localScale = tmp; }
                }

            }

            lastMouseXPos = currentMouseXpos;
        }

        if (Input.GetMouseButtonUp(0))
        {
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
    public void DampingScroll()
    {
        Vector3 blockColorPos;

        float firstBlockColorPosX = 0;
        float lastBlockColorPosX  = 8.4F;

        foreach (GameObject blockColor in blockColors)
        {
            if (blockColor.GetComponent<BloсkSprite>().ID == 0)
                firstBlockColorPosX = blockColor.transform.position.x;
            if (blockColor.GetComponent<BloсkSprite>().ID == 6)
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
        float minDeltaX = 100;
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
                    blockMaterialNum = blockColor.GetComponent<BloсkSprite>().ID;
                else
                {
                    BlockMaterialNum = 100;
                    GameObject canvas = GameObject.FindGameObjectWithTag("MainCanvas");
                    Instantiate(prohibitWindowPrefab, canvas.transform);
                    prohibitWindow = GameObject.FindGameObjectWithTag("Window");
                    int id = blockColor.GetComponent<BloсkSprite>().ID;
                    prohibitWindow.GetComponent<ProhibitWindow>().SetText(prohibitMessages[id]);
                }
                piramid.GetComponent<Piramid>().HighlightBlocks(blockMaterialNum);
                Vector3 blockColorScale = blockColor.transform.localScale;
                blockColorScale.x = 2.2F;
                blockColorScale.y = 2.2F;
                blockColor.transform.localScale = blockColorScale;
            }
            else
            {
                Vector3 blockColorScale = blockColor.transform.localScale;
                blockColorScale.x = 1.8F;
                blockColorScale.y = 1.8F;
                blockColor.transform.localScale = blockColorScale;
            }
        }
    }
    public void RefreshBlocksLock()
    {
        foreach (GameObject blockColor in blockColors)
        {
            int PirId = piramid.GetComponent<Piramid>().ID;
            if (piramid.GetComponent<Piramid>().ID == 1)
            {
                switch (blockColor.GetComponent<BloсkSprite>().ID)
                {
                    case 0:
                        blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                        blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        prohibitMessages[0] = "ERRRRRROORRRRRRRRR AAAAAAAAAAAA";
                        break;
                    case 1:
                        if (piramid.GetComponent<Piramid>().totalScore > 200)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        prohibitMessages[1] = "To open this block you need " + (200 -Player.Pir1TotalScore).ToString()
                            + " more points";
                        break;
                    case 2:
                        Debug.Log(blockColor.GetComponent<BloсkSprite>().ID);
                        if (piramid.GetComponent<Piramid>().totalScore > 600)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        prohibitMessages[2] = "To open this block you need " + (600 - Player.Pir1TotalScore).ToString()
                            + " more points";
                        break;
                }
                prohibitMessages[3] = "This block will be unlocked during the construction of the second pyramid";
                prohibitMessages[4] = "This block will be unlocked during the construction of the second pyramid";
                prohibitMessages[5] = "This block will be unlocked during the construction of the third pyramid";
                prohibitMessages[6] = "This block will be unlocked during the construction of the third pyramid";
            }
            if (piramid.GetComponent<Piramid>().ID == 2)
            {
                switch (blockColor.GetComponent<BloсkSprite>().ID)
                {
                    case 0:
                        blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                        blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        prohibitMessages[0] = "ERRRRRROORRRRRRRRR AAAAAAAAAAAA";
                        break;
                    case 1:
                        if (piramid.GetComponent<Piramid>().totalScore > 250)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        prohibitMessages[1] = "To open this block you need " + (250 - Player.Pir1TotalScore).ToString()
                            + " more points";
                        break;
                    case 2:
                        if (piramid.GetComponent<Piramid>().totalScore > 1000)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        prohibitMessages[2] = "To open this block you need " + (1000 - Player.Pir1TotalScore).ToString()
                            + " more points";
                        break;
                    case 3:
                        if (piramid.GetComponent<Piramid>().totalScore > 2400)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        prohibitMessages[3] = "To open this block you need " + (2400 - Player.Pir1TotalScore).ToString()
                            + " more points";
                        break;
                    case 4:
                        if (piramid.GetComponent<Piramid>().totalScore > 2400)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        prohibitMessages[4] = "To open this block you need " + (2400 - Player.Pir1TotalScore).ToString()
                            + " more points";
                        break;
                }
                prohibitMessages[5] = "This block will be unlocked during the construction of the third pyramid";
                prohibitMessages[6] = "This block will be unlocked during the construction of the third pyramid";
            }
            if (piramid.GetComponent<Piramid>().ID == 3)
            {
                switch (blockColor.GetComponent<BloсkSprite>().ID)
                {
                    case 0:
                        blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                        blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        prohibitMessages[0] = "ERRRRRROORRRRRRRRR AAAAAAAAAAAA";
                        break;
                    case 1:
                        if (piramid.GetComponent<Piramid>().totalScore > 300)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        prohibitMessages[1] = "To open this block you need " + (300 - Player.Pir1TotalScore).ToString()
                            + " more points";
                        break;
                    case 2:
                        if (piramid.GetComponent<Piramid>().totalScore > 1500)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        prohibitMessages[2] = "To open this block you need " + (1500 - Player.Pir1TotalScore).ToString()
                            + " more points";
                        break;
                    case 3:
                        if (piramid.GetComponent<Piramid>().totalScore > 4000)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        prohibitMessages[3] = "To open this block you need " + (4000 - Player.Pir1TotalScore).ToString()
                            + " more points";
                        break;
                    case 4:
                        if (piramid.GetComponent<Piramid>().totalScore > 4000)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        prohibitMessages[4] = "To open this block you need " + (4000 - Player.Pir1TotalScore).ToString()
                            + " more points";
                        break;
                    case 5:
                        if (piramid.GetComponent<Piramid>().totalScore > 6000)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        prohibitMessages[5] = "To open this block you need " + (6000 - Player.Pir1TotalScore).ToString()
                            + " more points";
                        break;
                    case 6:
                        if (piramid.GetComponent<Piramid>().totalScore > 10000)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        prohibitMessages[6] = "To open this block you need " + (10000 - Player.Pir1TotalScore).ToString()
                            + " more points";
                        break;
                }
            }
        }
    }
}