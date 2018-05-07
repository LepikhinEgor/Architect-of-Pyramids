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
    private bool isKeep = false;

    private SpriteRenderer sprite;
    private Player player;
    private float catchSkill = 0.35F;

    private Collider2D tmpCollider; //для поиска коллайдеров

    private void Awake()
    {

        throwForse = 11.7F;
        speed = 4.0F;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        sprite = GetComponentInChildren<SpriteRenderer>();

    }

    private void Update()
    {
        if (!isKeep)
            CatchBlock(); // проверка, нет ли блока над строителем
        if (isKeep)
        {

            CarryBlock();
            Run();
        }
        if (Input.GetButtonDown("Jump") && isKeep)
        {
            block.rigidBodyBlock.velocity = Vector3.zero;
            block.ThrowUp(throwForse);
            isKeep = false;
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
                int coef = 1;
                if (tag.Equals("LastBuilder"))
                {
                    Player.isChoosingPlatform = true;
                    SceneManager.LoadScene("Piramid1");
                }

                block = newBlock;
                if (System.Math.Abs(newBlock.transform.position.x - transform.position.x) < catchSkill / 2 && block.ParentBuilder != this)
                {
                    Debug.Log("Add time for Perfect Timer");
                    player.perfectTimer.Timer = 7;
                    block.SetStrandartMaskPosition();
                    block.blockSpriteActive.SetActive(true);
                    block.blockSpriteActiveBack.SetActive(true);
                    block.transform.rotation = transform.rotation;
                    coef = 2;
                }

                if (block.ParentBuilder != this)
                {
                    player.LevelNum++;
                    if (player.perfectTimer.Timer > 0)
                        player.PerfectCoef++;
                    Player.score += coef * player.PerfectCoef;
                    GameObject scoreLine = GameObject.FindGameObjectWithTag("ScoreLine");
                    float val = (float)(Player.score) / ((float)(Player.currentMaxScore)/2F);
                    Debug.Log(val);
                    scoreLine.GetComponent<Slider>().value = val;
                }
                block.ParentBuilder = this;

                Debug.Log("Is Catched");
                isKeep = true;
                block.IsFly = false;
                block.rigidBodyBlock.velocity = Vector3.zero;
                block.MaxRotationAngle = 45;
                block.IsSwing = true;

                if (player.perfectTimer.Timer > 0)
                {
                    block.SwingAngle = System.Math.Abs(newBlock.transform.position.x - transform.position.x) * 0.2F;
                    //Player.score += 2 * player.PerfectCoef;
                }
                else
                {
                    block.SwingAngle = System.Math.Abs(newBlock.transform.position.x - transform.position.x) * 0.5F;
                    //Player.score += 1;
                }
                block.SwingSpeed = block.SwingAngle * 2.5F;
                Debug.Log(Player.score);
            }

        }

    }
    public void CarryBlock()
    {
        Vector3 blockPos = transform.position;
        blockPos.y += 0.5F;
        blockPos.z += 1;
        if (direction.x > 0)
            blockPos.x += 0.06F;
        else
            blockPos.x -= 0.06F;
        block.transform.position = blockPos;
        block.rigidBodyBlock.gravityScale = 0;
    }
}