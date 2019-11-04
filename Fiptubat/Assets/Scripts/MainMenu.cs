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

    public Image infoPanel, controlsPanel;

    public void StartTheGame() {
        SceneManager.LoadScene(sceneName);
    }

    public void ShowInfoPanel() {
        controlsPanel.gameObject.SetActive(false);
        infoPanel.gameObject.SetActive(true);
    }

    public void ShowControlsPanel() {
        controlsPanel.gameObject.SetActive(true);
        infoPanel.gameObject.SetActive(false);
    }
}
