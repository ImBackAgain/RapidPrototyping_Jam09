using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectLevel : MonoBehaviour
{
    // Scene Transition Properties
    public string startGameSceneName = "";
    public string skipTutorialSceneName = "";
    public string skipToModifierSceneName = "";
    public string mainMenuSceneName = "";

    // Setup Properties
    public Button startGameButton;
    public Button skipTutorialButton;
    public Button skipToModifierButton;
    public Button backToMainButton;


    // Start is called before the first frame update
    void Start()
    {
        // Error Logging
        if (startGameSceneName.Length < 1)
            Debug.LogWarning("\t[ startGameSceneName ] not set!");
        if (mainMenuSceneName.Length < 1)
            Debug.LogWarning("\t[ mainMenuSceneName ] not set!");


        // Adding the button click listeners
        startGameButton.onClick.AddListener(StartGameFunc);
        skipTutorialButton.onClick.AddListener(SkipTutorialFunc);
        skipToModifierButton.onClick.AddListener(SkipToModifierFunc);
        backToMainButton.onClick.AddListener(BackToMainFunc);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void BackToMainFunc()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // StartGameFunc()
    private void StartGameFunc()
    {
        SceneManager.LoadScene(startGameSceneName);
    }
    // SkipTutorialFunc()
    private void SkipTutorialFunc()
    {
        SceneManager.LoadScene(skipTutorialSceneName);
    }
    // ModifierLevelFunc()
    private void SkipToModifierFunc()
    {
        SceneManager.LoadScene(skipToModifierSceneName);
    }


}
