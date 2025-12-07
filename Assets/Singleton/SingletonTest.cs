using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SingletonTest : MonoBehaviour
{
    [SerializeField]
    Button buttonNextScene;
    [SerializeField]
    string nextSceneName;

    // Start is called before the first frame update
    void Start()
    {
        buttonNextScene.onClick.AddListener(() =>
        {
            FadeManager.Instance.PlayFadeIn(1.5f, () =>
            {
                SceneManager.LoadScene(nextSceneName);
                // SceneManager.LoadSceneAsync(nextSceneName);
            });
        });

        if (FadeManager.Instance.IsFadeIn)
        {
            buttonNextScene.interactable = false;
            FadeManager.Instance.PlayFadeOut(1.5f, () =>
            {
                buttonNextScene.interactable = true;
            });
        }
        else
        {
            buttonNextScene.interactable = true;
        }
    }

}
