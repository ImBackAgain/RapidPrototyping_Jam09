using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Scene Transition Properties
    [Header("Scene Transition Variables")]
    public string noviceSceneName;
    public string normalSceneName;
    public string expertSceneName;
    public string soundTestSceneName = "";
    //public string selectLevelSceneName = "";


    // Setup Properties
    [Header("Main Menu Objects")]
    [Space(10)]
    [Header("=== Setup Variables ===")]
    [Space(10)]
    public GameObject mainMenuParent;
    [Header("Primary menu")]
    public GameObject primaryMenuParent;
    public Button startGameButton;
    public Button instructionsButtton;
    public Button quitGameButton;
    [Header("Level selection menu")]
    public GameObject levelSelectParent;
    public Button noviceButtton, normalButtton, expertButtton, returnButtton;
    [Header("Extra stufff")]
    public Button soundTestButton;
    public Button creditsButton;
    public Image Background;
    public Image CreditsBG;
    [Header("Instructions objects")]
    public GameObject instrParent;
    public Button exitInstructionsButtton;
    [Header("Credits Objects")]
    public GameObject creditsParent;
    public ScrollRect creditsScrollRect;
    public Button exitCreditsButton;


    // Start()
    void Start()
    {
        // Error Logging
        if (soundTestSceneName.Length < 1)
            Debug.LogWarning("\t[ soundTestSceneName ] not set!");

        // Adding the button click listeners
        startGameButton.onClick.AddListener(OpenLevelSelect);
        instructionsButtton.onClick.AddListener(InstructionsFunc);
        quitGameButton.onClick.AddListener(QuitGameFunc);

        noviceButtton.onClick.AddListener(LoadNoviceScene);
        normalButtton.onClick.AddListener(LoadNormalScene);
        expertButtton.onClick.AddListener(LoadExpertScene);
        returnButtton.onClick.AddListener(CloseLevelSelect);

        soundTestButton.onClick.AddListener(SoundTestFunc);
        creditsButton.onClick.AddListener(CreditsFunc);

        exitInstructionsButtton.onClick.AddListener(ExitInstructionsCalllback);
        exitCreditsButton.onClick.AddListener(ExitCreditsFunc);

        // Showing the main menu
        MainMenuVisibility(true);
        CreditsVisibility(false);
        InstructionVisibility(false);
        CloseLevelSelect();
    }

    // MainMenuVisibility()
    private void MainMenuVisibility(bool visible)
    {
        mainMenuParent.SetActive(visible);
        //titleText.gameObject.SetActive(visible);
        startGameButton.gameObject.SetActive(visible);
        soundTestButton.gameObject.SetActive(visible);
        instructionsButtton.gameObject.SetActive(visible);
        creditsButton.gameObject.SetActive(visible);
        quitGameButton.gameObject.SetActive(visible);
        Background.enabled = visible;
        CreditsBG.enabled = !visible;
    }

    void OpenLevelSelect()
    {
        primaryMenuParent.SetActive(false);
        levelSelectParent.SetActive(true);
    }

    void LoadNoviceScene()
    {
        SceneManager.LoadScene(noviceSceneName);
    }

    void LoadNormalScene()
    {
        SceneManager.LoadScene(normalSceneName);
    }

    void LoadExpertScene()
    {
        SceneManager.LoadScene(expertSceneName);
    }

    void CloseLevelSelect()
    {
        primaryMenuParent.SetActive(true);
        levelSelectParent.SetActive(false);
    }

    void InstructionVisibility(bool visible)
    {
        instrParent.SetActive(visible);
    }

    // CreditsVisibility()
    private void CreditsVisibility(bool visible)
    {
        creditsParent.SetActive(visible);
        creditsScrollRect.gameObject.SetActive(visible);
        exitCreditsButton.gameObject.SetActive(visible);
    }

    void InstructionsFunc()
    {
        InstructionVisibility(true);
        MainMenuVisibility(false);
    }

    // QuitFunc()
    private void QuitGameFunc()
    {
        Debug.Log("\t[ QuitFunc() ] called!");
        Application.Quit();
    }

    // SoundTestFunc()
    private void SoundTestFunc()
    {
        SceneManager.LoadScene(soundTestSceneName);
    }

    // CreditsFunc()
    private void CreditsFunc()
    {
        MainMenuVisibility(false);
        CreditsVisibility(true);
    }

    void ExitInstructionsCalllback()
        //"Function" is so generic though
    {
        InstructionVisibility(false);
        MainMenuVisibility(true);
    }

    // ExitCreditsFunc()
    private void ExitCreditsFunc()
    {
        CreditsVisibility(false);
        MainMenuVisibility(true);
    }
}
