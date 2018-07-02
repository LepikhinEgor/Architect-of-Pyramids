using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleMotion : MonoBehaviour {

    GameObject scoreUI;
    bool isAnimated;
    float speed;
    float scale;
    Vector3 targetScale;
    private bool isMoving;

    public int scoreDifferent;
    // Use this for initialization

    private void Awake()
    {
    }

    public void SetTarget(Vector3 target)
    {
        targetScale = new Vector3(scale, scale, 1); 
        target.z = -10F;
        transform.position = target;
        isMoving = true;
    }
    void Start () {
        scoreUI = GameObject.FindGameObjectWithTag("UIScore");
        scale = 1;
        speed = 3;
        switch (Player.currentPiramidID)
        {
            case 1:
                scale = 0.99F;
                speed = 2.6F;
                break;
            case 2:
                scale = 0.89F;
                speed = 2.8F;
                break;
            case 3:
                scale = 0.6F;
                speed = 3F;
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (isMoving)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, speed * Time.deltaTime);

            if (transform.localScale.x <= scale + 0.04F && !isAnimated)
            {
                GameObject.FindGameObjectWithTag("OK").GetComponent<AudioSource>().Play();
                isAnimated = true;
                GameObject smokeAnim = (GameObject)Instantiate(Player.smokePrefab, Player.selectedPlatform.transform);
                Vector3 smokePosition = smokeAnim.transform.position;
                smokePosition.x = Player.selectedPlatform.transform.position.x;
                smokePosition.y = Player.selectedPlatform.transform.position.y;
                smokeAnim.transform.position = smokePosition;
                GameObject canvas = GameObject.FindGameObjectWithTag("MainCanvas");
                GameObject scoreDiff = (GameObject)Instantiate(Player.scoreDiffPrefab, canvas.transform);
                Vector3 textPos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
                scoreDiff.transform.position =textPos;


                string text = "";
                if (scoreDifferent < 0)
                {
                    text = "-" + System.Math.Abs(scoreDifferent).ToString();
                    scoreDiff.GetComponent<Text>().color = Color.red;
                }
                if (scoreDifferent > 0)
                {
                    text = "+" + scoreDifferent.ToString();
                    scoreDiff.GetComponent<Text>().color = Color.green;
                }
                if (scoreDifferent == 0)
                    text = "0";
                scoreDiff.GetComponent<Text>().text = text;

                Vector3 targetTextPos = scoreDiff.transform.position;
                targetTextPos.y += Screen.height / 16;
                scoreDiff.GetComponent<ScoreDiffMotion>().SetTargetPos(targetTextPos);

            }
            if (transform.localScale.x <= scale + 0.005F)
            {
                transform.gameObject.SetActive(false);
            }
        }
	}
}
