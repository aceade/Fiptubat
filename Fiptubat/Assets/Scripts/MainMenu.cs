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

    public Image infoPanel, controlsPanel, optionsPanel;
    public Slider musicVolumeControl, speechVolumeControl, effectsVolumeControl, cameraSpeedControl;

    public void StartTheGame() {
        SceneManager.LoadScene(sceneName);
    }

    public void ShowInfoPanel() {
        controlsPanel.gameObject.SetActive(false);
        infoPanel.gameObject.SetActive(true);
        optionsPanel.gameObject.SetActive(false);
    }

    public void ShowControlsPanel() {
        controlsPanel.gameObject.SetActive(true);
        infoPanel.gameObject.SetActive(false);
        optionsPanel.gameObject.SetActive(false);
    }

    public void ShowOptionsPanel() {
        controlsPanel.gameObject.SetActive(false);
        infoPanel.gameObject.SetActive(false);
        optionsPanel.gameObject.SetActive(true);
    }

    public void SavePreferences() {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeControl.value);
        PlayerPrefs.SetFloat("EffectsVolume", effectsVolumeControl.value);
        PlayerPrefs.SetFloat("VoiceVolume", speechVolumeControl.value);
        PlayerPrefs.SetFloat("CameraSpeed", cameraSpeedControl.value);
    }
}
