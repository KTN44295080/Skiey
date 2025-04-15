using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private StoryData[] storyDatas;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private GameObject logPanel;
    [SerializeField] private TextMeshProUGUI logText;
    public static StoryManager Instance { get; private set; }
    public int storyIndex { get; private set; }
    public int textIndex { get; private set; }
    private string storyLog;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        SetStroryData(storyIndex, textIndex);
    }

    void SetStroryData(int _storyIndex, int _textIndex)
    {
        Story element = storyDatas[_storyIndex].stories[_textIndex];
        backgroundImage.sprite = element.background;
        characterImage.sprite = element.character;
        characterImage.color = element.isHighlight ? Color.white : Color.gray;
        float duration = (float)element.text.Length / 20;
        LMotion.String.Create128Bytes("", element.text, duration).BindToText(storyText);
        characterName.text = element.name;
        
        storyLog += element.name + "\n";
        storyLog += element.text + "\n \n";
    }

    public void Next()
    {
        textIndex++;
        storyText.text = "";
        characterName.text = "";

        if (textIndex >= storyDatas[storyIndex].stories.Count)
        {
            textIndex = 0;
            storyIndex++;
        }

        SetStroryData(storyIndex, textIndex);
    }

    public void VisibleLog(bool isVisible)
    {
        logPanel.SetActive(isVisible);
        logText.text = storyLog;
    }
}
