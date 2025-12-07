using System;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEvent : MonoBehaviour
{
    [SerializeField]
    GameObject root;
    [SerializeField]
    Text textTitle;
    [SerializeField]
    Text textMessage;
    [SerializeField]
    Button buttonYes;
    [SerializeField]
    Button buttonNo;

    public void Initialize(EventData eventData, Action<bool> onClick)
    {
        root.SetActive(true);
        textTitle.text = eventData.Title;
        textMessage.text = eventData.Message;

        buttonYes.onClick.AddListener(() =>
        {
            root.SetActive(false);
            onClick?.Invoke(true);
        });
        buttonNo.onClick.AddListener(() =>
        {
            root.SetActive(false);
            onClick?.Invoke(false);
        });
    }

}
