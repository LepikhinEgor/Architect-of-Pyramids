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
    private float throwForse;
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

    private float deltaPosX, previousPosX;

    private void Awake()
    {
        block = Player.block;
        animator = transform.Find("Sprite").GetComponent<Animator>();
        //block = GameObject.FindGameObjectWithTag("Block").GetComponent<Block>();
        throwForse = 11.7F;
        speed = 3.5F;
        sprite = GetComponentInChildren<SpriteRenderer>();
        previousPosX = transform.position.x;
    }
    private void Start()
    {
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
        block = Player.block;
    }

    private void Update()
    {
        if (block.IsFly)
        {
            if (System.Math.Abs(block.transform.position.y - transform.position.y) < 5)
            {
                animator.SetBool("IsAnimation", true);
            }
            else
            {
                animator.SetBool("IsAnimation", false);
            }
        }
        //Debug.Log(Player.perfectTimer.Timer);
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
    private void Run()
    {
        checkerPosRight = transform.position;
        checkerPosRight.x += 0.5F;
        checkerPosRight.y -= 0.5F;
        checkerPosLeft = transform.position;
        checkerPosLeft.x -= 0.5F;
        checkerPosLeft.y -= 0.5F;
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
        leftCorner = transform.position;
        leftCorner.x -= catchSkill;
        rightCorner = transform.position;
        rightCorner.x += catchSkill;
        Collider2D findedCollider = Physics2D.OverlapArea(leftCorner, rightCorner);
        if (findedCollider != null && findedCollider.GetComponent<Block>())
        {
            Block newBlock = findedCollider.GetComponent<Block>();
            if (newBlock.rigidBodyBlock.velocity.y <= 0)
            {
                isKeep = true;
                int coef = 1;

                block = newBlock;
                if (System.Math.Abs(newBlock.transform.position.x - transform.position.x) < catchSkill / 1.8 && block.ParentBuilder != this)
                {
                    Debug.Log("Add time for Perfect Timer");
                    Player.perfectTimer.Timer = 7;
                    block.SetStrandartMaskPosition();
                    block.blockSpriteActive.SetActive(true);
                    block.blockSpriteActiveBack.SetActive(true);
                    block.transform.rotation = transform.rotation;
                    coef = 2;
                }

                if (!(Player.perfectTimer.Timer > 0))
                    block.catchSound.Play();
                if (block.ParentBuilder != this)
                {
                    if (Player.perfectTimer.Timer > 0)
                    {
                        Player.PerfectCoef++;
                        float bellPitch = (float)System.Math.Pow(2, (Player.PerfectCoef -1) / 12.0);
                        if (bellPitch > 2)
                            bellPitch = 2;

                        block.perfectCatchSound.pitch = bellPitch;
                        block.perfectCatchSound.Play();
                        GameObject canvas = GameObject.FindGameObjectWithTag("MainCanvas");
                        Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                        Vector3 textPos = RectTransformUtility.WorldToScreenPoint(cam, transform.position);
                        Instantiate(Player.perfectCoefPrefab, canvas.transform);
                        GameObject perfCoef = GameObject.FindGameObjectWithTag("PerfectCoef");
                        perfCoef.GetComponent<PerfectCoef>().SetPosition(textPos);

                        string text = "x" + (Player.PerfectCoef).ToString();
                        perfCoef.GetComponent<PerfectCoef>().SetText(text);
                    }
                    Player.score += coef * Player.PerfectCoef;
                    RectTransform scoreLine = GameObject.FindGameObjectWithTag("ScoreLine").GetComponent<RectTransform>();
                    float val = (float)(Player.score) / ((float)(Player.currentMaxScore)/2F);
                    if (val > 1)
                        val = 1;
                    float lineLenght = Screen.width * (scoreLine.anchorMax.x - scoreLine.anchorMin.x);
                    Vector3 rightLineCorner = scoreLine.GetComponent<RectTransform>().offsetMax;
                    rightLineCorner.x = 0 - lineLenght + lineLenght*(val);
                    scoreLine.GetComponent<RectTransform>().offsetMax = rightLineCorner;
                }
                block.ParentBuilder = this;

                Debug.Log("Is Catched");
                string blockOffset = "Block offset to builder by " + (System.Math.Abs(newBlock.transform.position.x - transform.position.x)).ToString();
                Debug.Log(blockOffset);
                block.IsFly = false;
                block.rigidBodyBlock.velocity = Vector3.zero;
                block.MaxRotationAngle = 40;
                block.IsSwing = true;

                PerfectTimer pt = GameObject.FindGameObjectWithTag("PerfectTimer").GetComponent<PerfectTimer>();
                if (pt.Timer > 0)
                {
                    block.SwingAngle = System.Math.Abs(newBlock.transform.position.x - transform.position.x) * 0.2F;
                    //Player.score += 2 * player.PerfectCoef;
                }
                else
                {
                    block.SwingAngle = System.Math.Abs(newBlock.transform.position.x - transform.position.x) * 0.4F;
                    //Player.score += 1;
                }

                if (tag.Equals("LastBuilder"))
                {
                    Player.score += (int)(Player.score * Player.lives * 0.05);
                    Player.ShowResultWindow();
                }

                if (tag.Equals("WindStart"))
                {
                    wind = block.gameObject.AddComponent<Wind>();
                }
                wind = block.GetComponent<Wind>();

                animator.SetBool("IsMoving", true);

                if (wind != null)
                    wind.isActiveWind = false;

                if (tag.Equals("WindEnd"))
                {
                    Debug.Log("Destroy Wind");
                    Destroy(wind);
                }

                float swingSpeedCoef = 2F;
                if (pt.Timer > 0)
                    block.SwingSpeed = block.SwingAngle* swingSpeedCoef;
                else
                    block.SwingSpeed = block.SwingAngle;
                string score = "Score: " + (Player.score).ToString(); 
                Debug.Log(Player.score);
            }

        }

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
    }
}