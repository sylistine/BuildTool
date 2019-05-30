using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Application = Djn.Application;

public class TestLoader : MonoBehaviour
{
    private void Awake() {
        Application.Init();
    }

    private void Start() {
        var buildData = Application.BuildData;
        if (buildData.LevelDatas.Length > 0)
            SceneManager.LoadScene(buildData.LevelDatas[0].MainScene);
    }
}
