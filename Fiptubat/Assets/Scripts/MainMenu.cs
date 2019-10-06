using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Attached to the camera in the main menu.
/// <summary>
public class MainMenu : MonoBehaviour
{
    public string sceneName;

    public void StartTheGame() {
        SceneManager.LoadScene(sceneName);
    }
}
