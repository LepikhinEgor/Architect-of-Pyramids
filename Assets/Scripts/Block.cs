using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Block : MonoBehaviour
{
    public bool isTimerInc;
    public float timer;
    public GameObject blockMask;
    public AudioClip catchClip;
    public AudioClip perfectCatchClip;
    public AudioSource catchSound;
    public AudioSource perfectCatchSound;

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
        isTimerInc = true;
        timer = 0;
        Player.block = this;
        blockSprite = GameObject.FindGameObjectWithTag("BlockSprite");
        blockSpriteActive = GameObject.FindGameObjectWithTag("BlockActiveSprite");
        blockSpriteActiveBack = GameObject.FindGameObjectWithTag("BlockActiveSpriteBack");
        
        rigidBodyBlock = GetComponent<Rigidbody2D>();
        cam1 = Camera.main;

        ParentBuilder = GameObject.FindGameObjectWithTag("FirstBuilder").GetComponent<Builder>();
        GetComponentInChildren<SpriteMask>().sprite = Resources.Load<Sprite>("Sprites/mask");

        catchClip = Resources.Load<AudioClip>("Sounds/catchSound");
        perfectCatchClip = Resources.Load<AudioClip>("Sounds/perfectCatchSound");
        perfectCatchSound = gameObject.AddComponent<AudioSource>();
        perfectCatchSound.clip = perfectCatchClip;
        catchSound = gameObject.AddComponent<AudioSource>();
        catchSound.clip = catchClip;
    }
    private void Start()
    {
        blockMask = GameObject.FindGameObjectWithTag("BlockMask");
        switch (Player.currentBlockMaterialNum)
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
        if (isTimerInc)
            timer += Time.deltaTime;
        if (isFly && transform.position.y + 1F < ParentBuilder.transform.position.y)
        {
            //блок упал
            cam1.GetComponent<CameraController>().isFollow = false;
            Player.lives--;
            parentBuilder.animator.SetBool("IsMoving", true);
            transform.rotation = ParentBuilder.transform.rotation;
            Player.ankhLivesUI.text = Player.lives.ToString();
            if (Player.lives > 0)
            {
                rigidBodyBlock.velocity = Vector3.zero;
                parentBuilder.isKeep = true;
                parentBuilder.CarryBlock();
                swingAngle = 0.2F;
                swingSpeed = swingAngle * 1.5F;
                maxRotationAngle = 40;
                cam1.GetComponent<CameraController>().isFollow = true;
                isFly = false;
            }
            else
            {
                isTimerInc = false;
                isFly = false;
                Debug.Log(Player.lives);
                Player.score = 0;
                Player.ShowResultWindow();
                GameObject.FindGameObjectWithTag("ResultWindow").GetComponent<ResultsWindow>().timer = 10000;
                parentBuilder.animator.SetBool("IsMoving", false);
                Destroy(this.gameObject, 0.05F);
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
                {
                    transform.Rotate(back);
                }
                else
                    parentBuilder.isSwingRight = false;
            }
            else
            {
                if (transform.rotation.z < swingAngle)
                {
                    transform.Rotate(forward);
                }
                else
                    parentBuilder.isSwingRight = true;
            }
            blockMask.transform.rotation = blockSprite.transform.rotation;
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