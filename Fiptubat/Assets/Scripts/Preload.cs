using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Preloading to make builds smoother
/// </summary>
public class Preload : MonoBehaviour
{

    public int sceneIndex = 1;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(sceneIndex);
    }

}
