using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsWindow : ProhibitWindow {
    public float timer;
    GameObject livesNum;
    GameObject timerSprite;
    GameObject bonus1;
    GameObject bonus2;
    GameObject bonus3;

    RectTransform scoreLine;
    GameObject okButton;
    GameObject retryButton;
    // Use this for initialization


    void SetData()
    {
        livesNum.GetComponent<Text>().text = (Player.lives).ToString();
        bonus1.GetComponent<Text>().text ="+ " + (Player.lives * 5).ToString() + "%";
        if (timer > (Player.currentBlockMaterialNum+1)*6*3)
        {
            timerSprite.GetComponent<Image>().color = Color.red;
            bonus2.GetComponent<Text>().text = "+ 0%";
            bonus2.GetComponent<Text>().color = Color.red;
        }
        else
        {
            bonus2.GetComponent<Text>().text = "+ 5%";
        }

        if (Player.currentBlockMaterialNum == 5 || Player.currentBlockMaterialNum == 8 || Player.currentBlockMaterialNum == 2)
            bonus3.GetComponent<Text>().text = "+ 50%";
        else
        {
            transform.Find("Premium").gameObject.SetActive(false);
            bonus3.SetActive(false);
        }

        scoreLine = GameObject.FindGameObjectWithTag("ScoreLine").GetComponent<RectTransform>();
        float val = (float)(Player.score) / ((float)(Player.currentMaxScore) / 2F);
        if (val > 1)
            val = 1;
        float lineLenght = Screen.width * (scoreLine.anchorMax.x - scoreLine.anchorMin.x);
        Vector3 rightLineCorner = scoreLine.GetComponent<RectTransform>().offsetMax;
        rightLineCorner.x = 0 - lineLenght + lineLenght * (val);
        scoreLine.GetComponent<RectTransform>().offsetMax = rightLineCorner;
        okButton.GetComponent<Button>().onClick.AddListener(Player.LoadPiramidScene);
        retryButton.GetComponent<Button>().onClick.AddListener(Player.ReloadFloors);
    }

    private void Awake()
    {
        okButton = transform.Find("okButton").gameObject;
        retryButton = transform.Find("retryButton").gameObject;

        livesNum = transform.Find("LivesNum").gameObject;
        timerSprite = transform.Find("TimerSprite").gameObject;
        bonus1 = transform.Find("Bonus1").gameObject;
        bonus2 = transform.Find("Bonus2").gameObject;
        bonus3 = transform.Find("Bonus3").gameObject;
    }
    void Start () {
        SetData();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
