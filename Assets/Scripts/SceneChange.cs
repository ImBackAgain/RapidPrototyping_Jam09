using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [Header("Scene Transition Variables")]
    public string destinationSceneName = "";

    // Start()
    void Start()
    {
        
    }

    // This is called by the menu option within the sound test scene
    public void SwitchScenes()
    {
        SceneManager.LoadScene(destinationSceneName);
    }
}
