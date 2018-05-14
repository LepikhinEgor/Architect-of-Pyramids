using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Platform : MonoBehaviour {

    GameObject piramid;

    [SerializeField]
    public int ID;

    BoxCollider2D platformCollider;
    [SerializeField]
    GameObject[] sides;
    GameObject blockBase;
    public GameObject backLight;
    public Sprite[] sprites;
    public Sprite[] spriteRes;

    //Количество соседних хороших граней
    [SerializeField]
    private int neighborEdgesCount;
    public int NeighborEdgesCount
    {
        set { this.neighborEdgesCount = value; }
        get { return this.neighborEdgesCount; }
    }

    //есть ли блок на платформе
    [SerializeField]
    private bool isBusy;//на платформе есть блок
    public bool IsBusy
    {
        set { this.isBusy = value; }
        get { return this.isBusy; }
    }

    //очки блока
    [SerializeField]
    private int score = 0;
    public int Score
    {
        set { this.score = value; }
        get { return this.score; }
    }

    //Номер материала блока(1-сапфир, 2 -изумруд, 3 - золото ...)
    [SerializeField]
    private int blockMaterialNum = 0;
    public int BlockMaterialNum
    {
        set { this.blockMaterialNum = value; }
        get { return this.blockMaterialNum; }
    }


    [SerializeField]
    private bool[] goodEdgePositions;
    public bool[] GoodEdgePositions
    {
        set { this.goodEdgePositions = value; }
        get { return this.goodEdgePositions; }
    }

    SpriteRenderer blockSprite = new SpriteRenderer();

    // Use this for initialization
    private void Awake()
    {

        piramid = GameObject.FindGameObjectWithTag("Piramid");

        sides = new GameObject[6];
        sprites = new Sprite[3];
        platformCollider = GetComponent<BoxCollider2D>();
        //goodEdgePositions = new bool[6];

        GameObject[] blockBases = GameObject.FindGameObjectsWithTag("BlockBase");
        foreach (GameObject obj in blockBases)
            if (obj.transform.parent.transform == transform)
            {
                blockBase = obj;
                break;
            }
        GameObject[] blocksBackLights = GameObject.FindGameObjectsWithTag("BlockBackLight");
        foreach (GameObject obj in blocksBackLights)
            if (obj.transform.parent.transform == transform)
            {
                backLight = obj;
                backLight.SetActive(false);
                break;
            }

        GameObject[][] buf = new GameObject[20][];
        buf[0] = GameObject.FindGameObjectsWithTag("RightSide");
        buf[1] = GameObject.FindGameObjectsWithTag("BottomRightSide");
        buf[2] = GameObject.FindGameObjectsWithTag("BottomLeftSide");
        buf[3] = GameObject.FindGameObjectsWithTag("LeftSide");
        buf[4] = GameObject.FindGameObjectsWithTag("TopLeftSide");
        buf[5] = GameObject.FindGameObjectsWithTag("TopRightSide");

        //поиск стороны блока из всех, которые нашлись по тэгу
        for (int i = 0; i < sides.Length; i++)
        {
            for (int j = 0; j < buf[i].Length; j++)
            {
                if (buf[i][j].transform.parent.transform == transform)
                {
                    sides[i] = buf[i][j];
                    break;
                }
            }
        }

        //возможно нужно менять
        for (int i = 0; i < goodEdgePositions.Length; i++)
            sides[i].SetActive(false);
    }

    void FillPlatformOldData()
    {
        isBusy = Piramid.platformsIsBusy[ID];
        blockMaterialNum = Piramid.platformsBlockMaterialNums[ID];
        goodEdgePositions = Piramid.platformsEdgePositions[ID];
        score = Piramid.platformsScore[ID];

        if(isBusy)
            InsertBlock();
        else
        {
            blockMaterialNum = 0;//здесь возможно запонит сапфиром
            goodEdgePositions  = new bool[6];
            score = 0;
        }
    }

    void Start () {
        if (!tag.Equals("Sample"))
            FillPlatformOldData();
    }

    // Update is called once per frame
    void Update () {
	}


    private void OnMouseDown()
    {
        GameObject sample = GameObject.FindGameObjectWithTag("Sample");

        if (sample && sample.GetComponent<Platform>().Score != 0 && neighborEdgesCount >= sample.GetComponent<Platform>().blockMaterialNum)
        {
            this.InsertBlock(sample);

            Player.isChoosingPlatform = false;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<Player>().ShowSelectBlockUI();

            piramid.GetComponent<Piramid>().GetPlatfomsInformation();
            piramid.GetComponent<Piramid>().RefreshTotalScore();
            piramid.GetComponent<Piramid>().RefreshNeighborEdgesCount();
            piramid.GetComponent<Piramid>().TurnHighLightsOFF();

            String str = EdgePositionsToString();
            Player.ReplacePlatformInXML(Player.currentPiramidID, ID, score, blockMaterialNum, str);
        }
        piramid.GetComponent<Piramid>().RefreshNeighborEdgesCount();
        Debug.Log("UPd");
    }

   
    //Вставка блока на платформу через параметры
    public void InsertBlock(){
        isBusy = true;

        switch (blockMaterialNum)
        {
            case 0:
                sprites[0] = Resources.Load<Sprite>("Sprites/sapphireBlock");
                sprites[1] = Resources.Load<Sprite>("Sprites/sapphireBlockRight");
                sprites[2] = Resources.Load<Sprite>("Sprites/sapphireBlockBottom");
                break;
            case 1:
                sprites[0] = Resources.Load<Sprite>("Sprites/rubyBlock");
                sprites[1] = Resources.Load<Sprite>("Sprites/rubyBlockRight");
                sprites[2] = Resources.Load<Sprite>("Sprites/rubyBlockBottom");
                break;
            case 2:
                sprites[0] = Resources.Load<Sprite>("Sprites/emeraldBlock");
                sprites[1] = Resources.Load<Sprite>("Sprites/emeraldBlockRight");
                sprites[2] = Resources.Load<Sprite>("Sprites/emeraldBlockBottom");
                break;
            case 3:
                sprites[0] = Resources.Load<Sprite>("Sprites/silverBlock");
                sprites[1] = Resources.Load<Sprite>("Sprites/silverBlockRight");
                sprites[2] = Resources.Load<Sprite>("Sprites/silverBlockBottom");
                break;
            case 4:
                sprites[0] = Resources.Load<Sprite>("Sprites/goldenBlock");
                sprites[1] = Resources.Load<Sprite>("Sprites/goldenBlockRight");
                sprites[2] = Resources.Load<Sprite>("Sprites/goldenBlockBottom");
                break;
            case 5:
                sprites[0] = Resources.Load<Sprite>("Sprites/topazBlock");
                sprites[1] = Resources.Load<Sprite>("Sprites/topazBlockRight");
                sprites[2] = Resources.Load<Sprite>("Sprites/topazBlockBottom");
                break;
        }

        blockBase.GetComponent<SpriteRenderer>().sprite = sprites[0];
        for (int i = 0; i < goodEdgePositions.Length; i++)
        {
            if (goodEdgePositions[i] && (i == 0 || i == 3))
            {
                this.goodEdgePositions[i] = goodEdgePositions[i];
                sides[i].SetActive(true);
                sides[i].GetComponent<SpriteRenderer>().sprite = sprites[1];
            }
            if (goodEdgePositions[i] && i != 0 && i != 3)
            {
                this.goodEdgePositions[i] = goodEdgePositions[i];
                sides[i].SetActive(true);
                sides[i].GetComponent<SpriteRenderer>().sprite = sprites[2];
            }
        }


    }
    //вствака блока на платфому через копирование
    public void InsertBlock(GameObject sample)
    {
        this.blockMaterialNum = sample.GetComponent<Platform>().blockMaterialNum;
        this.score = sample.GetComponent<Platform>().score;
        this.sprites = sample.GetComponent<Platform>().sprites;
        this.goodEdgePositions = sample.GetComponent<Platform>().GoodEdgePositions;

        for (int i = 0; i < 6; i++)
            sides[i].GetComponent<SpriteRenderer>().sprite = null;

        blockBase.GetComponent<SpriteRenderer>().sprite = sample.GetComponent<Platform>().sprites[0];

        //вставка спрайтов хороших граней в зависимости от массива goodEdgePositions
        for (int i = 0; i < goodEdgePositions.Length; i++)
        {
            if (goodEdgePositions[i] && (i == 0 || i == 3))
            {
                this.goodEdgePositions[i] = goodEdgePositions[i];
                sides[i].SetActive(true);
                sides[i].GetComponent<SpriteRenderer>().sprite = sprites[1];
            }
            if (goodEdgePositions[i] && i != 0 && i != 3)
            {
                
                this.goodEdgePositions[i] = goodEdgePositions[i];
                sides[i].SetActive(true);
                sides[i].GetComponent<SpriteRenderer>().sprite = sprites[2];
            }
        }
        isBusy = true;
    }


    //возвращает случайное распределение хороших граней в блоке, при заданном количестве 
    public void SetRandomGoodEdgePositions(int goodEdgeNum)
    {
        int randNum;

        for (int i = 0; i < goodEdgeNum;)
        {
            randNum = UnityEngine.Random.Range(0, goodEdgePositions.Length);
            if (!goodEdgePositions[randNum])
            {
                goodEdgePositions[randNum] = true;
                i++;
            }
        }
    }

    //возвращает количество хороших соседних граней
    public int FindNeighborEdgesCount()
    {
        float offsetX1 = 0, offsetX2 = 0;
        float offsetY = 0;
        switch(Player.currentPiramidID)
        {
            case 1:
                offsetX1 = 1.3F;
                offsetX2 = 0.6F;
                offsetY = 0.9F;
                break;
            case 2:
                offsetX1 = 1.1F;
                offsetX2 = 0.6F;
                offsetY = 0.8F;
                break;
            case 3:
                offsetX1 = 1F;
                offsetX2 = 0.45F;
                offsetY = 0.65F;
                break;
        }
        int neighborEdgesNum = 0;
        Vector3[] checkPos = new Vector3[6];
        checkPos[0] = checkPos[1] =checkPos[2]=checkPos[3]=checkPos[4]=checkPos[5] = transform.position;
        checkPos[0].x += offsetX1;
        checkPos[0].y += 0F;
        checkPos[1].x += offsetX2;
        checkPos[1].y -= offsetY;
        checkPos[2].x -= offsetX2;
        checkPos[2].y -= offsetY;
        checkPos[3].x -= offsetX1;
        checkPos[3].y += 0F;
        checkPos[4].x -= offsetX2;
        checkPos[4].y += offsetY;
        checkPos[5].x += offsetX2;
        checkPos[5].y += offsetY;
        Collider2D tmpCollider;
        for (int i = 0; i < checkPos.Length; i++)
        {
            tmpCollider = Physics2D.OverlapCircle(checkPos[i], 0.1F);
            //bool a = Piramid.platformsEdgePositions[tmpCollider.GetComponentInParent<Platform>().ID][(i + 3) % 6];
            //bool b = Piramid.platformsEdgePositions[0][0];
            if (tmpCollider)
                //через статические, так как не все успевают создаться платформы через Start
                if (Piramid.platformsEdgePositions[tmpCollider.GetComponentInParent<Platform>().ID][(i + 3) % 6])
                    neighborEdgesNum++;
                   
        }
        
        return neighborEdgesNum;
    }

    private String EdgePositionsToString()
    {
        String result = "";

        for (int i = 0; i < goodEdgePositions.Length; i++)
            result += goodEdgePositions[i]? "1":"0";

        return result;
    }
}
