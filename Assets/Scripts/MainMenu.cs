using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Scene Transition Properties
    [Header("Scene Transition Variables")]
    public string startGameSceneName = "";
    public string soundTestSceneName = "";

    // Setup Properties
    [Header("Main Menu Objects")]
    [Space(10)]
    [Header("=== Setup Variables ===")]
    [Space(10)]
    public GameObject mainMenuParent;
    public Text titleText;
    public Button startGameButton;
    public Button soundTestButton;
    public Button instructionsButtton;
    public Button quitGameButton;
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
        if (startGameSceneName.Length < 1)
            Debug.LogWarning("\t[ startGameSceneName ] not set!");
        if (soundTestSceneName.Length < 1)
            Debug.LogWarning("\t[ soundTestSceneName ] not set!");

        // Adding the button click listeners
        startGameButton.onClick.AddListener(StartGameFunc);
        soundTestButton.onClick.AddListener(SoundTestFunc);
        instructionsButtton.onClick.AddListener(InstructionsFunc);
        creditsButton.onClick.AddListener(CreditsFunc);
        quitGameButton.onClick.AddListener(QuitGameFunc);

        exitInstructionsButtton.onClick.AddListener(ExitInstructionsCalllback);
        exitCreditsButton.onClick.AddListener(ExitCreditsFunc);

        // Showing the main menu
        MainMenuVisibility(true);
        CreditsVisibility(false);
        InstructionVisibility(false);
    }

    // MainMenuVisibility()
    private void MainMenuVisibility(bool visible)
    {
        mainMenuParent.SetActive(visible);
        titleText.gameObject.SetActive(visible);
        startGameButton.gameObject.SetActive(visible);
        soundTestButton.gameObject.SetActive(visible);
        instructionsButtton.gameObject.SetActive(visible);
        creditsButton.gameObject.SetActive(visible);
        quitGameButton.gameObject.SetActive(visible);
        Background.enabled = visible;
        CreditsBG.enabled = !visible;
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

    // StartGameFunc()
    private void StartGameFunc()
    {
        SceneManager.LoadScene(startGameSceneName);
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

    // LoadGameFunc()
    private void LoadGameFunc(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
