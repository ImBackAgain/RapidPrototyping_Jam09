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
    public Button creditsButton;
    public Button quitGameButton;
    public Image Background;
    public Image CreditsBG;
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
        creditsButton.onClick.AddListener(CreditsFunc);
        quitGameButton.onClick.AddListener(QuitGameFunc);
        exitCreditsButton.onClick.AddListener(ExitCreditsFunc);

        // Showing the main menu
        MainMenuVisibility(true);
        CreditsVisibility(false);
    }

    // MainMenuVisibility()
    private void MainMenuVisibility(bool visible)
    {
        mainMenuParent.SetActive(visible);
        titleText.gameObject.SetActive(visible);
        startGameButton.gameObject.SetActive(visible);
        soundTestButton.gameObject.SetActive(visible);
        creditsButton.gameObject.SetActive(visible);
        quitGameButton.gameObject.SetActive(visible);
        Background.enabled = visible;
        CreditsBG.enabled = !visible;
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

    // QuitFunc()
    private void QuitGameFunc()
    {
        Debug.Log("\t[ QuitFunc() ] called!");
        Application.Quit();
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
