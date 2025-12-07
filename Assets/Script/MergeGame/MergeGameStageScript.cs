using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MergeGameStageScript : MonoBehaviour
{
    [SerializeField]
    private GridLayoutGroup gridLayoutGroup;

    // マージアイテムをぶら下げる親（= GridImage の RectTransform）
    public RectTransform ItemsRoot =>
        gridLayoutGroup != null
            ? gridLayoutGroup.GetComponent<RectTransform>()
            : null;

    private void Start()
    {
        StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {
        yield return new WaitForEndOfFrame();
        gridLayoutGroup.enabled = false;
    }
}
