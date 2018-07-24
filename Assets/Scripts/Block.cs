using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Block : MonoBehaviour
{
    //ссылки на объекты трещин блока
    private GameObject firstDamage;
    private GameObject secondDamage;
    private GameObject thirdDamage;

    //таймер продолжает работу
    public bool isTimerInc;
    public float timer;
    public GameObject blockMask;
#region AudioVariables
    public AudioClip catchClip;
    public AudioClip perfectCatchClip;
    public AudioClip lastCatchSound;
    public AudioClip lastBadCatchSound;
    public AudioClip crackClip;
    public AudioSource catchSound;
    public AudioSource perfectCatchSound;
    public AudioSource crackSound;
#endregion

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
  
    //ссылки на спрайты блока
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
        InitialSprites();
        InitialSounds();

        isTimerInc = true;
        timer = 0;

        Player.block = this;
        
        rigidBodyBlock = GetComponent<Rigidbody2D>();
        cam1 = Camera.main;

        ParentBuilder = GameObject.FindGameObjectWithTag("FirstBuilder").GetComponent<Builder>();
    }

    void InitialSprites()
    {
        firstDamage = transform.Find("Defects").Find("firstDefect").gameObject;
        secondDamage = transform.Find("Defects").Find("secondDefect").gameObject;
        thirdDamage = transform.Find("Defects").Find("thirdDefect").gameObject;
        firstDamage.SetActive(false);
        secondDamage.SetActive(false);
        thirdDamage.SetActive(false);

        blockSprite = GameObject.FindGameObjectWithTag("BlockSprite");
        blockSpriteActive = GameObject.FindGameObjectWithTag("BlockActiveSprite");
        blockSpriteActiveBack = GameObject.FindGameObjectWithTag("BlockActiveSpriteBack");

        GetComponentInChildren<SpriteMask>().sprite = Resources.Load<Sprite>("Sprites/mask");
    }

    void InitialSounds()
    {
        crackClip = Resources.Load<AudioClip>("Sounds/Crack");
        catchClip = Resources.Load<AudioClip>("Sounds/catchSound");
        perfectCatchClip = Resources.Load<AudioClip>("Sounds/perfectCatchSound");
        lastCatchSound = Resources.Load<AudioClip>("Sounds/LastCatch");
        lastBadCatchSound = Resources.Load<AudioClip>("Sounds/LastBadCatch");

        crackSound = gameObject.AddComponent<AudioSource>();
        crackSound.volume = Player.soundsVolume;
        crackSound.clip = crackClip;

        perfectCatchSound = gameObject.AddComponent<AudioSource>();
        perfectCatchSound.volume = Player.soundsVolume;
        perfectCatchSound.clip = perfectCatchClip;

        catchSound = gameObject.AddComponent<AudioSource>();
        catchSound.volume = Player.soundsVolume;
        catchSound.clip = catchClip;
    }
    private void Start()
    {
        blockMask = GameObject.FindGameObjectWithTag("BlockMask");
        SetBlockSpritesColor();
    }

    void SetBlockSpritesColor()
    {
        string mainSpritePath;
        string activeSpritePath;
        string fillingSpritePath;
        switch (Player.currentBlockMaterialNum)
        {
            case 0:
                mainSpritePath = "Sprites/sapphireBlock";
                activeSpritePath = "Sprites/sapphireBlockActive";
                fillingSpritePath = "Sprites/sapphireBlockActiveBack";
                break;
            case 1:
                mainSpritePath = "Sprites/rubyBlock";
                activeSpritePath = "Sprites/rubyBlockActive";
                fillingSpritePath = "Sprites/rubyBlockActiveBack";
                break;
            case 2:
                mainSpritePath = "Sprites/emeraldBlock";
                activeSpritePath = "Sprites/emeraldBlockActive";
                fillingSpritePath = "Sprites/emeraldBlockActiveBack";
                break;
            case 3:
                mainSpritePath = "Sprites/aquamarineBlock";
                activeSpritePath = "Sprites/aquamarineBlockActive";
                fillingSpritePath = "Sprites/aquamarineBlockActiveBack";
                break;
            case 4:
                mainSpritePath = "Sprites/silverBlock";
                activeSpritePath = "Sprites/silverBlockActive";
                fillingSpritePath = "Sprites/silverBlockActiveBack";
                break;
            case 5:
                mainSpritePath = "Sprites/goldenBlock";
                activeSpritePath = "Sprites/goldenBlockActive";
                fillingSpritePath = "Sprites/goldenBlockActiveBack";
                break;
            case 6:
                mainSpritePath = "Sprites/topazBlock";
                activeSpritePath = "Sprites/topazBlockActive";
                fillingSpritePath = "Sprites/topazBlockActiveBack";
                break;
            case 7:
                mainSpritePath = "Sprites/pearlyBlock";
                activeSpritePath = "Sprites/pearlyBlockActive";
                fillingSpritePath = "Sprites/pearlyBlockActiveBack";
                break;
            case 8:
                mainSpritePath = "Sprites/amethystBlock";
                activeSpritePath = "Sprites/amethystBlockActive";
                fillingSpritePath = "Sprites/amethystBlockActiveBack";
                break;
            default:
                mainSpritePath = "Sprites/sapphireBlock";
                activeSpritePath = "Sprites/sapphireBlockActive";
                fillingSpritePath = "Sprites/sapphireBlockActiveBack";
                Debug.Log("Initialize of block sprite was failure");
                break;
        }

        blockSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(mainSpritePath);
        blockSpriteActive.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(activeSpritePath);
        blockSpriteActiveBack.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(fillingSpritePath);

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
            Player.lives--;
            parentBuilder.animator.SetBool("IsMoving", true);
            transform.rotation = ParentBuilder.transform.rotation;

            if (Player.lives >= 0)
                Drop();
            else
                Crash();

            Player.ankhLivesUI.text = Player.lives.ToString();
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

    void Drop()
    {
        //вызывается когда блок уронили, но ещё остались жизни
        //отнимает жизнь, возвращает блок уронившему строителю, задает небольшое покачиваение
        switch (Player.lives)
        {
            case 2: firstDamage.SetActive(true); break;
            case 1: secondDamage.SetActive(true); break;
            case 0: thirdDamage.SetActive(true); break;
        }

        crackSound.Play();

        //возвращение блока уронившему строителю
        parentBuilder.isKeep = true;
        parentBuilder.CarryBlock();
        rigidBodyBlock.velocity = Vector3.zero;

        //небольшое покачивание после падения
        swingAngle = 0.2F;
        swingSpeed = swingAngle * 1.5F;
        maxRotationAngle = 40;
        isFly = false;
    }

    void Crash()
    {
        // вызывается при последнем падении блока(когда не осталось жизней, lives = -1)
        // делает замирание камеры, вызывает окно с результатами, скрывает блок
        // обнуляет все результаты сцены

        Debug.Log("Block has crached");
        cam1.GetComponent<CameraController>().isFollow = false;

        perfectCatchSound.clip = lastBadCatchSound;
        perfectCatchSound.Play();

        isTimerInc = false;
        isFly = false;

        //обнуление результатов
        Player.lives = 0;
        Player.score = 0;
        Player.ShowResultWindow();
        GameObject.FindGameObjectWithTag("ResultWindow").GetComponent<ResultsWindow>().timer = 10000;

        parentBuilder.animator.SetBool("IsMoving", false);

        //зависание блока в вохдухе
        rigidBodyBlock.gravityScale = 0;
        rigidBodyBlock.velocity = Vector3.zero;

        //скрывание спрайтов
        transform.Find("blockSprite").gameObject.SetActive(false);
        transform.Find("Defects").gameObject.SetActive(false);
        if (transform.Find("blockActiveSprite") != null)
            transform.Find("blockActiveSprite").gameObject.SetActive(false);
        if (transform.Find("blockActiveBackSprite") != null)
            transform.Find("blockActiveBackSprite").gameObject.SetActive(false);
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