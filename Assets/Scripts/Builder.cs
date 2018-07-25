﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Builder : MonoBehaviour
{
    Vector3 leftCorner;
    Vector3 rightCorner;
    Vector3 checkerPosRight;
    Vector3 checkerPosLeft;
    public Animator animator;
    //[SerializeField]
    public float throwForse;
    Wind wind;
    [SerializeField]
    public bool isSwingRight;

    [SerializeField]
    private float speed;
    [SerializeField]
    private Vector3 direction;
    private Block block;
    public bool isKeep = false;

    private SpriteRenderer sprite;
    private float catchSkill = 0.35F;

    private Collider2D tmpCollider; //для поиска коллайдеров

    //переменные для контроля положения блока ровно над строителем
    private float deltaPosX, previousPosX;

    private void Awake()
    {
        block = Player.block;
        animator = transform.Find("Sprite").GetComponent<Animator>();
        //block = GameObject.FindGameObjectWithTag("Block").GetComponent<Block>();
        speed = 3.5F;
        sprite = transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>();
        previousPosX = transform.position.x;
    }
    private void Start()
    {
        SetDirection();
        block = Player.block;
    }

    void SetDirection()
    {
        //устанавливает направление движения по флагу isSwingRight
        Vector3 localPos = transform.Find("Sprite").localPosition;
        if (isSwingRight)
        {
            direction.x = 1;
            sprite.flipX = false;
            localPos.x *= 1;
            transform.Find("Sprite").localPosition = localPos;
        }
        else
        {
            direction.x = -1;
            sprite.flipX = true;
            localPos.x *= -1;
            transform.Find("Sprite").localPosition = localPos;
        }
    }

    private void Update()
    {
        if (block != null && block.IsFly)
        {
            SwitchAnimationByDistance();
        }

        if (!isKeep)
            CatchBlock(); // проверка, нет ли блока над строителем

        if (isKeep)
        {
            CarryBlock();
            Run();
        }

        if (!tag.Equals("LastBuilder") && isKeep && (Input.GetButtonDown("Jump") || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            animator.SetBool("IsMoving", false);
            isKeep = false;
            block.rigidBodyBlock.velocity = Vector3.zero;
            block.ThrowUp(throwForse);
            block.rigidBodyBlock.gravityScale = 2;

            if (wind != null)
                wind.isActiveWind = true;
        }
    }

    void SwitchAnimationByDistance()
    {
        //включает анимацию при приближении к блоку
        // чтобы строители не имели одинаковую фазу анимации

        float offset = transform.position.y - block.transform.position.y;
        if (offset < 8 && offset > 3 || System.Math.Abs(offset) < 1.2F)
        {
            animator.SetBool("IsAnimation", true);
        }
        else
        {
            animator.SetBool("IsAnimation", false);
        }
    }

    private void Run()
    {
        //движение строителя
        //работает когда блок у него в руках

        //проверка позиций справа и слева на наличие коллайдеров
        checkerPosRight = transform.position;
        checkerPosRight.x += 0.3F;
        checkerPosLeft = transform.position;
        checkerPosLeft.x -= 0.3F;

        //справа
        tmpCollider = Physics2D.OverlapCircle(checkerPosRight, 0.3F);
        if (tmpCollider)
        {
            if (!tmpCollider.GetComponent<Block>() && direction.x > 0)
            {
                direction.x = -direction.x;
                sprite.flipX = true;
                Vector3 mirrorXPos = transform.Find("Sprite").localPosition;
                mirrorXPos.x *= -1;
                transform.Find("Sprite").localPosition = mirrorXPos;
            }
        }

        //слева
        tmpCollider = Physics2D.OverlapCircle(checkerPosLeft, 0.3F);
        if (tmpCollider)
            if (!tmpCollider.GetComponent<Block>() && direction.x < 0)
            {
                direction.x = -direction.x;
                sprite.flipX = false;
                Vector3 mirrorXPos = transform.Find("Sprite").localPosition;
                mirrorXPos.x *= -1;
                transform.Find("Sprite").localPosition = mirrorXPos;
            }
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);

        deltaPosX = System.Math.Abs(transform.position.x - previousPosX);
        previousPosX = transform.position.x;
    }


    public void CatchBlock()
    { 
        //проверка на наличие блока сверху строителя
        leftCorner = transform.position;
        leftCorner.x -= catchSkill;
        rightCorner = transform.position;
        rightCorner.x += catchSkill;
        rightCorner.y += 0.2F;
        Collider2D findedCollider = Physics2D.OverlapArea(leftCorner, rightCorner);
        if (findedCollider != null && findedCollider.GetComponent<Block>())
        {
            GameObject canvas = GameObject.FindGameObjectWithTag("MainCanvas");
            Block newBlock = findedCollider.GetComponent<Block>();
            if (newBlock.rigidBodyBlock.velocity.y <= 0)
            {
                block.transform.localRotation = Quaternion.AngleAxis(0, Vector3.up);
                isKeep = true;
                int coef = 1;

                block = newBlock;

                if (!(Player.perfectTimer.Timer > 0) || block.ParentBuilder == this)
                    block.catchSound.Play();
                if (block.ParentBuilder != this)
                {
                    if (System.Math.Abs(newBlock.transform.position.x - transform.position.x) < catchSkill / 1.8)
                    {
                        Debug.Log("Add time for Perfect Timer");
                        Player.perfectTimer.Timer = 7;
                        block.SetStrandartMaskPosition();
                        block.blockSpriteActive.SetActive(true);
                        block.blockSpriteActiveBack.SetActive(true);
                        block.transform.rotation = transform.rotation;
                        coef = 2;
                    }

                    if (Player.perfectTimer.Timer > 0)
                    {
                        Player.PerfectCoef++;
                        PlayPerfectCatchSound();
                        CreatePopUpPerfectCoef();
                    }
                    Player.score += coef * Player.PerfectCoef;
                    RefreshScoreLine();
                }
                block.ParentBuilder = this;

                Debug.Log("Is Catched");
                block.IsFly = false;
                block.rigidBodyBlock.velocity = Vector3.zero;
                block.MaxRotationAngle = 40;
                block.IsSwing = true;

                if (Player.perfectTimer.Timer > 0)
                {
                    float swingSpeedCoef = 2F;
                    block.SwingAngle = System.Math.Abs(newBlock.transform.position.x - transform.position.x) * 0.2F;
                    block.SwingSpeed = block.SwingAngle * swingSpeedCoef;
                }
                else
                {
                    block.SwingAngle = System.Math.Abs(newBlock.transform.position.x - transform.position.x) * 0.4F;
                    block.SwingSpeed = block.SwingAngle;
                }

                if (tag.Equals("LastBuilder"))
                {
                    LastBuilderAction();
                }

                if (tag.Equals("WindStart"))
                {
                    if (canvas.transform.Find("EffectName(Clone)") != null)
                        Destroy(canvas.transform.Find("EffectName(Clone)").gameObject);
                    Instantiate(Player.windPrefab, Camera.main.transform);
                    wind = block.gameObject.AddComponent<Wind>();

                    GameObject effName = (GameObject)Instantiate(Player.effectNamePrefab, canvas.transform);
                    effName.GetComponent<Text>().text = "WIND";
                }
                if (!tag.Equals("LastBuilder"))
                    animator.SetBool("IsMoving", true);

                wind = block.GetComponent<Wind>();
                if (wind != null)
                    wind.isActiveWind = false;

                if (tag.Equals("WindEnd"))
                {
                    Destroy(canvas.transform.Find("EffectName(Clone)").gameObject);
                    Debug.Log("Destroy Wind");
                    Destroy(wind);
                    ParticleSystem parSys = Camera.main.transform.Find("Wind(Clone)").gameObject.GetComponent<ParticleSystem>();
                    var newEmmision = parSys.emission;
                    newEmmision.rateOverTime = 0;
                    Destroy(Camera.main.transform.Find("Wind(Clone)").gameObject, 10);
                }

                if (tag.Equals("FogStart"))
                {
                    if (canvas.transform.Find("EffectName(Clone)")!= null)
                        Destroy(canvas.transform.Find("EffectName(Clone)").gameObject);
                    FogController fogContr = Camera.main.gameObject.AddComponent<FogController>();
                    fogContr.block = block;

                    GameObject effName = (GameObject)Instantiate(Player.effectNamePrefab, canvas.transform);
                    effName.GetComponent<Text>().text = "FOG";
                }

                if (tag.Equals("FogEnd"))
                {
                    Destroy(canvas.transform.Find("EffectName(Clone)").gameObject);
                    FogController fogContr = Camera.main.gameObject.GetComponent<FogController>();
                    fogContr.isDissipating = true;
                }
            }

        }

    }

    void LastBuilderAction()
    {
        speed = 0;
        block.catchSound.clip = block.lastCatchSound;
        block.catchSound.Play();

        block.isTimerInc = false;

        GetBonuses();
        Player.ShowResultWindow();
        GameObject.FindGameObjectWithTag("ResultWindow").GetComponent<ResultsWindow>().timer = block.timer;
        Destroy(transform.Find("Sprite").gameObject.GetComponent<Animator>());
    }

    void GetBonuses()
    {
        Player.score = (int)(Player.score * (1 + Player.lives * 0.05));

        if (block.timer < (Player.currentBlockMaterialNum + 1) * 6 * 3)
            Player.score = (int)(Player.score * 1.05);

        if (Player.currentBlockMaterialNum == 5 || Player.currentBlockMaterialNum == 8 || Player.currentBlockMaterialNum == 2)
            Player.score = (int)(Player.score * 1.5);
    }

    void RefreshScoreLine()
    {
        //обновляет заполнение линии очков
        RectTransform scoreLine = GameObject.FindGameObjectWithTag("ScoreLine").GetComponent<RectTransform>();

        float val;
        if (Player.currentBlockMaterialNum == 5 || Player.currentBlockMaterialNum == 8 || Player.currentBlockMaterialNum == 2)
            val = (float)(Player.score * 1.5F) / ((float)(Player.currentMaxScore) / 2F);
        else
            val = (float)(Player.score) / ((float)(Player.currentMaxScore) / 2F);

        if (val > 1)
            val = 1;

        float lineLenght = Screen.width * (scoreLine.anchorMax.x - scoreLine.anchorMin.x);

        Vector3 rightLineCorner = scoreLine.GetComponent<RectTransform>().offsetMax;
        rightLineCorner.x = 0 - lineLenght + lineLenght * (val);
        scoreLine.GetComponent<RectTransform>().offsetMax = rightLineCorner;
    }

    void SetIllumRadius()
    {
        //устанавливает радиус рассеивания тумана вокруг игрока

        if (Player.perfectTimer.Timer > 0)
            Player.illumRadius = 0.4F;
        else
            Player.illumRadius = 0.3F;
    }

    void PlayPerfectCatchSound()
    {
        //Задает частоту звука приземления и проигрывает его
        int floorsNum = 6;
        switch (Player.currentBlockMaterialNum)
        {
            case 0: floorsNum = 6; break;
            case 1: floorsNum = 12; break;
            case 2: floorsNum = 18; break;
            case 3: floorsNum = 24; break;
            case 4: floorsNum = 30; break;
            case 5: floorsNum = 30; break;
            case 6: floorsNum = 36; break;
            case 7: floorsNum = 42; break;
            case 8: floorsNum = 42; break;
        }

        int note = floorsNum / 12;
        if (floorsNum < 10)
            note = 1;
        note = (Player.PerfectCoef - 1) / note;

        float bellPitch = (float)System.Math.Pow(2, note / 12.0);
        if (bellPitch > 2)
            bellPitch = 2;
        block.perfectCatchSound.pitch = bellPitch;

        block.perfectCatchSound.Play();
    }

    void CreatePopUpPerfectCoef()
    {
        //создает всплывающую надпись с коэфициентом умножения очков 
        GameObject canvas = GameObject.FindGameObjectWithTag("MainCanvas");
        Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        Vector3 textPos = RectTransformUtility.WorldToScreenPoint(cam, transform.position);
        string text = "x" + (Player.PerfectCoef).ToString();

        PerfectCoef perfCoef = ((GameObject)Instantiate(Player.perfectCoefPrefab, canvas.transform)).GetComponent<PerfectCoef>();
        perfCoef.SetPosition(textPos);
        perfCoef.SetText(text);
    }

    public void CarryBlock()
    {

        Vector3 newBlockPos = transform.position;
        //float offset = System.Math.Abs(newBlockPos.x - transform.position.x);
        newBlockPos.y += 0.7F;
        newBlockPos.z += 1;
        if (direction.x > 0)
            newBlockPos.x += deltaPosX;
        else
            newBlockPos.x -= deltaPosX;
        block.transform.position = newBlockPos;
        block.rigidBodyBlock.gravityScale = 0;

        SetIllumRadius();
    }
}