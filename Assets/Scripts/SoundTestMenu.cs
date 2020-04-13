using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundTestMenu : MonoBehaviour
{
    [Header("Scene Transition Variables")]
    public string mainMenuSceneName = "";

    // Start()
    void Start()
    {
        
    }

    // This is called by the menu option within the sound test scene
    public void ReturnFromSoundTestFunc()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
