using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneButton : MonoBehaviour
{
    [SerializeField]
    private string SceneName;

    public void SwitchScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
