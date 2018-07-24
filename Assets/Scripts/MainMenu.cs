using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    UnityEngine.Object newGameWindowPrefab;
    GameObject menuCanvas;
    GameObject newGameButton;
    GameObject continueGameButton;
    GameObject HTPButton;
    GameObject autorsButton;
    GameObject soundSlider;
    GameObject musicSlider;
	// Use this for initialization
	void Start () {
        newGameWindowPrefab = Resources.Load("Prefabs/NewGameWindow");
        menuCanvas = GameObject.FindGameObjectWithTag("Menu");
        newGameButton = menuCanvas.transform.Find("NewGameButton").gameObject;
        continueGameButton = menuCanvas.transform.Find("ContinueGameButton").gameObject;
        soundSlider = menuCanvas.transform.Find("SoundSlider").gameObject;
        musicSlider = menuCanvas.transform.Find("MusicSlider").gameObject;
        soundSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("SoundVolume");
        musicSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicVolume");

        if (!PlayerPrefs.HasKey("FirstLaunch"))
        {
            soundSlider.GetComponent<Slider>().value = 0.5F;
            musicSlider.GetComponent<Slider>().value = 0.5F;
        }
        newGameButton.GetComponent<Button>().onClick.AddListener(CreateQuestionWindow);
        continueGameButton.GetComponent<Button>().onClick.AddListener(Player.LoadLastPiramidScene);

    }

    void CreateQuestionWindow()
    {
        if (!PlayerPrefs.HasKey("FirstLaunch"))
        {
            PlayerPrefs.SetFloat("FirstLaunch",1);
            Player.LoadNewGame();

        }
        else
            Instantiate(newGameWindowPrefab, transform.parent);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
