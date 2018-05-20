using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Builder : MonoBehaviour
{
    //[SerializeField]
    private float throwForse;

    [SerializeField]
    public bool isSwingRight;

    [SerializeField]
    private float speed;
    [SerializeField]
    private Vector3 direction;
    private Block block;
    public bool isKeep = false;

    private SpriteRenderer sprite;
    private Player player;
    private float catchSkill = 0.35F;

    private Collider2D tmpCollider; //для поиска коллайдеров

    private float deltaPosX, previousPosX;

    private void Awake()
    {
        //block = GameObject.FindGameObjectWithTag("Block").GetComponent<Block>();
        throwForse = 11.7F;
        speed = 3.5F;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        previousPosX = transform.position.x;
    }

    private void Update()
    {
        //Debug.Log(Player.perfectTimer.Timer);
        if (!isKeep)
            CatchBlock(); // проверка, нет ли блока над строителем
        if (isKeep)
        {

            CarryBlock();
            Run();
        }
        if (isKeep && (Input.GetButtonDown("Jump") || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)))
        {
            isKeep = false;
            block.rigidBodyBlock.velocity = Vector3.zero;
            block.ThrowUp(throwForse);
            block.rigidBodyBlock.gravityScale = 2;
        }
    }
    private void Run()
    {
        Vector3 checkerPosRight = transform.position;
        checkerPosRight.x += 0.5F;
        checkerPosRight.y -= 0.5F;
        Vector3 checkerPosLeft = transform.position;
        checkerPosLeft.x -= 0.5F;
        checkerPosLeft.y -= 0.5F;
        tmpCollider = Physics2D.OverlapCircle(checkerPosRight, 0.3F);
        if (tmpCollider)
        {
            if (!tmpCollider.GetComponent<Block>() && direction.x > 0)
                direction.x = -direction.x;
        }
        tmpCollider = Physics2D.OverlapCircle(checkerPosLeft, 0.3F);
        if (tmpCollider)
            if (!tmpCollider.GetComponent<Block>() && direction.x < 0)
                direction.x = -direction.x;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);

        deltaPosX = System.Math.Abs(transform.position.x - previousPosX);
        previousPosX = transform.position.x;
        //sprite.flipX = direction.x > 0; //поворот при движении в другую сторону
    }


    public void CatchBlock()
    { 
        Vector3 leftCorner = transform.position;
        leftCorner.x -= catchSkill;
        Vector3 rightCorner = transform.position;
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

                if (block.ParentBuilder != this)
                {
                    player.LevelNum++;
                    if (Player.perfectTimer.Timer > 0)
                    {
                        Player.PerfectCoef++;
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
                    GameObject scoreLine = GameObject.FindGameObjectWithTag("ScoreLine");
                    float val = (float)(Player.score) / ((float)(Player.currentMaxScore)/2F);
                    scoreLine.GetComponent<Slider>().value = val;
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
        newBlockPos.y += 0.5F;
        newBlockPos.z += 1;
        if (direction.x > 0)
            newBlockPos.x += deltaPosX;
        else
            newBlockPos.x -= deltaPosX;
        block.transform.position = newBlockPos;
        block.rigidBodyBlock.gravityScale = 0;
    }
}