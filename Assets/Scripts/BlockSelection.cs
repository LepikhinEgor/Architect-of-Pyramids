using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class BlockSelection : MonoBehaviour {

    private static float lastMouseXPos;
    private float currentMouseXpos;
    private float deltaXPos;
    private GameObject[] blockColors;

    private GameObject piramid;

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

    Sprite[] sprites;
    // Use this for initialization
    private void Awake()
    {
        //для первой пирамиды
        blockMaterialCount = 8;
        blockMaterialNum = 0;
        sprites = new Sprite[blockMaterialCount];
        sprites[0] = Resources.Load<Sprite>("Sprites/sapphireBlock");
        sprites[1] = Resources.Load<Sprite>("Sprites/rubyBlock");
        sprites[2] = Resources.Load<Sprite>("Sprites/emeraldBlock");
        sprites[3] = Resources.Load<Sprite>("Sprites/silverBlock");
        sprites[4] = Resources.Load<Sprite>("Sprites/goldenBlock");
        sprites[5] = Resources.Load<Sprite>("Sprites/topazBlock");
        sprites[6] = Resources.Load<Sprite>("Sprites/amethystBlock");

    }
    void Start () {
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
            piramid = GameObject.FindGameObjectWithTag("Piramid");
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
                    blockMaterialNum = blockColor.GetComponent<BloсkSprite>().ID;
                    piramid.GetComponent<Piramid>().HighlightBlocks(blockMaterialNum);
                    Vector3 blockColorScale = blockColor.transform.localScale;
                    blockColorScale.x = 2.2F;
                    blockColorScale.y = 2.2F;
                    blockColor.transform.localScale = blockColorScale;
                }
                else
                {
                    //if (blockColor.transform.position.x < 0)
                    //{
                    //    Vector3 blockColorPos = blockColor.transform.position;
                    //    blockColorPos.x -= 0.2F;
                    //    blockColor.transform.position = blockColorPos;
                    //}
                    //else
                    //{
                    //    Vector3 blockColorPos = blockColor.transform.position;
                    //    blockColorPos.x += 0.2F;
                    //    blockColor.transform.position = blockColorPos;
                    //}

                    Vector3 blockColorScale = blockColor.transform.localScale;
                    blockColorScale.x = 1.8F;
                    blockColorScale.y = 1.8F;
                    blockColor.transform.localScale = blockColorScale;
                }
            }


            //lastMouseXPos = currentMouseXpos;
        }


    }
    //public void ChangeBlock(bool toNextBlock)
    //{
    //    if (toNextBlock)
    //        if (!(blockMaterialNum == blockMaterialCount - 1))
    //        {
    //            blockMaterialNum++;
    //            GetComponentInParent<SpriteRenderer>().sprite = sprites[blockMaterialNum];
    //            Debug.Log(blockMaterialNum);
    //        }
    //    if (!toNextBlock)
    //        if (!(blockMaterialNum == 0))
    //        {
    //            blockMaterialNum--;
    //            GetComponentInParent<SpriteRenderer>().sprite = sprites[blockMaterialNum];
    //            Debug.Log(blockMaterialNum);
    //        }
    //}
}
