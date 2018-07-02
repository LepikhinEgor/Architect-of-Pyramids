using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    GameObject menuCanvas;
    GameObject newGameButton;
    GameObject continueGameButton;
    GameObject HTPButton;
    GameObject autorsButton;
    GameObject soundSlider;
    GameObject musicSlider;
	// Use this for initialization
	void Start () {
        menuCanvas = GameObject.FindGameObjectWithTag("Menu");
        newGameButton = menuCanvas.transform.Find("NewGameButton").gameObject;
        continueGameButton = menuCanvas.transform.Find("ContinueGameButton").gameObject;
        soundSlider = menuCanvas.transform.Find("SoundSlider").gameObject;
        musicSlider = menuCanvas.transform.Find("MusicSlider").gameObject;


        newGameButton.GetComponent<Button>().onClick.AddListener(Player.LoadNewGame);
        continueGameButton.GetComponent<Button>().onClick.AddListener(Player.LoadLastPiramidScene);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
