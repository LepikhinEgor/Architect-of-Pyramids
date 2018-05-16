using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class BlockSelection : MonoBehaviour {

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
        blockMaterialNum = 0;
    }
    void Start () {
        Debug.Log("StratBlSel");
        piramid = GameObject.FindGameObjectWithTag("Piramid");

        blockColors = GameObject.FindGameObjectsWithTag("BlockColor");
        foreach (GameObject blockColor in blockColors)
        {
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
                        BlockMaterialNum = 100;
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
                        break;
                    case 1:
                        if (piramid.GetComponent<Piramid>().totalScore > 100)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 2:
                        Debug.Log(blockColor.GetComponent<BloсkSprite>().ID);
                        if (piramid.GetComponent<Piramid>().totalScore > 300)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                }
            }
            if (piramid.GetComponent<Piramid>().ID == 2)
            {
                switch (blockColor.GetComponent<BloсkSprite>().ID)
                {
                    case 0:
                        blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                        blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        break;
                    case 1:
                        if (piramid.GetComponent<Piramid>().totalScore > 200)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 2:
                        if (piramid.GetComponent<Piramid>().totalScore > 350)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 3:
                        if (piramid.GetComponent<Piramid>().totalScore > 500)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 4:
                        if (piramid.GetComponent<Piramid>().totalScore > 700)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                }
            }
            if (piramid.GetComponent<Piramid>().ID == 3)
            {
                switch (blockColor.GetComponent<BloсkSprite>().ID)
                {
                    case 0:
                        blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                        blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                        break;
                    case 1:
                        if (piramid.GetComponent<Piramid>().totalScore > 200)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 2:
                        if (piramid.GetComponent<Piramid>().totalScore > 350)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 3:
                        if (piramid.GetComponent<Piramid>().totalScore > 500)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 4:
                        if (piramid.GetComponent<Piramid>().totalScore > 700)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 5:
                        if (piramid.GetComponent<Piramid>().totalScore > 1000)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                    case 6:
                        if (piramid.GetComponent<Piramid>().totalScore > 1500)
                        {
                            blockColor.transform.Find("LockSprite").gameObject.SetActive(false);
                            blockColor.GetComponent<BloсkSprite>().isUnlocked = true;
                        }
                        break;
                }
            }
        }
    }
}
