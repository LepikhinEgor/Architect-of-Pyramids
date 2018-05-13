using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block : MonoBehaviour
{
    private Player player;


    private Builder parentBuilder;
    public Builder ParentBuilder
    {
        set { parentBuilder = value; }
        get { return parentBuilder; }
    }

    private float swingSpeed = 0.4F;
    public float SwingSpeed
    {
        set { swingSpeed = value; }
        get { return swingSpeed; }
    }

    private float swingAngle = 0;
    public float SwingAngle
    {
        set { swingAngle = value; }
        get { return swingAngle; }
    }

    private Camera cam1;
  
    public GameObject blockSprite;
    public GameObject blockSpriteActive;
    public GameObject blockSpriteActiveBack;

    public Rigidbody2D rigidBodyBlock;

    private int maxRotationAngle = 0;
    public int MaxRotationAngle
    {
        set { this.maxRotationAngle = value; }
        get { return this.maxRotationAngle; }
    }

    private bool isFly = true;
    public bool IsFly
    {
        set { this.isFly = value; }
        get { return this.isFly; }
    }

    private bool isSwing = false;
    public bool IsSwing
    {
        set { this.isSwing = value; }
        get { return this.isSwing; }
    }

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        blockSprite = GameObject.FindGameObjectWithTag("BlockSprite");
        blockSpriteActive = GameObject.FindGameObjectWithTag("BlockActiveSprite");
        blockSpriteActiveBack = GameObject.FindGameObjectWithTag("BlockActiveSpriteBack");

        
        rigidBodyBlock = GetComponent<Rigidbody2D>();
        cam1 = Camera.main;

        GameObject builder;
        builder = GameObject.FindGameObjectWithTag("FirstBuilder");
        ParentBuilder = builder.GetComponent<Builder>();
        builder = null;
        GetComponentInChildren<SpriteMask>().sprite = Resources.Load<Sprite>("Sprites/mask");
    }
    private void Start()
    {
        
        switch(Player.currentBlockMaterialNum)
        {
            case 0:
                blockSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/sapphireBlock");
                blockSpriteActive.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/sapphireBlockActive");
                blockSpriteActiveBack.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/sapphireBlockActiveBack");
                break;
            case 1:
                blockSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/rubyBlock");
                blockSpriteActive.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/rubyBlockActive");
                blockSpriteActiveBack.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/rubyBlockActiveBack");
                break;
            case 2:
                blockSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/emeraldBlock");
                blockSpriteActive.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/emeraldBlockActive");
                blockSpriteActiveBack.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/emeraldBlockActiveBack");
                break;
            case 3:
                blockSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/silverBlock");
                blockSpriteActive.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/silverBlockActive");
                blockSpriteActiveBack.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/silverBlockActiveBack");
                break;
            case 4:
                blockSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/goldenBlock");
                blockSpriteActive.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/goldenBlockActive");
                blockSpriteActiveBack.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/goldenBlockActiveBack");
                break;
            case 5:
                blockSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/topazBlock");
                blockSpriteActive.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/topazBlockActive");
                blockSpriteActiveBack.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/topazBlockActiveBack");
                break;
        }

        blockSpriteActive.SetActive(false);
        blockSpriteActiveBack.SetActive(false);
    }

    public void ThrowUp(float force)
    {
        rigidBodyBlock.velocity = Vector3.zero;
        rigidBodyBlock.AddForce(Vector3.up * force, ForceMode2D.Impulse);
        rigidBodyBlock.gravityScale = 2;
        isFly = true;
    }

    void Update()
    {
        if (player.IsWindy)
            rigidBodyBlock.AddForce(transform.right * 2F, ForceMode2D.Force);
        if (transform.position.y + 0.6F < ParentBuilder.transform.position.y && isFly)
        {
            //блок упал
            cam1.GetComponent<CameraController>().isFollow = false;
            Player.lives--;
            transform.rotation = ParentBuilder.transform.rotation;
            if (Player.lives > 0)
            {
                parentBuilder.isKeep = true;
                parentBuilder.CarryBlock();
                swingAngle = 0.2F;
                swingSpeed = swingAngle * 1.5F;
                maxRotationAngle = 40;
                cam1.GetComponent<CameraController>().isFollow = true;
                isFly = false;
            }
        }
        if (isFly)
        {
            Fly();
            MaxRotationAngle = 0;
        }
        if (maxRotationAngle != 0)
        {
            float swingCoef = 90F;
            Vector3 back = Vector3.back;
            back.z *= SwingSpeed*Time.deltaTime * swingCoef;
            Vector3 forward = Vector3.forward;
            forward.z *= SwingSpeed * Time.deltaTime * swingCoef;
            if (parentBuilder.isSwingRight)
            {
                if (transform.rotation.z > -swingAngle)
                    transform.Rotate(back);
                else
                    parentBuilder.isSwingRight = false;
            }
            else
            {
                if (transform.rotation.z < swingAngle)
                    transform.Rotate(forward);
                else
                    parentBuilder.isSwingRight = true;
            }
        }
    }

    void Fly()
    {
        Vector3 position = transform.position;

        int k;
        if (Math.Abs(transform.rotation.z) < 0.15F)
            k = 20;
        else
            k = 15;
        position.x += -transform.rotation.z * Time.deltaTime * k;//умножаем на 15 для более сильноо смещения вбок
        transform.position = position;
    }

    public void SetStrandartMaskPosition()
    {
        Vector3 maskPosition = transform.position;
        maskPosition.y += 0.4F; //хз почему 0.4 а не 0.18
        maskPosition.z -= 0.8F;
        GetComponentInChildren<SpriteMask>().transform.position = maskPosition;
    }
}