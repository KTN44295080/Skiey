using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private StoryData[] storyDatas;
    [SerializeField] private ChoiceData[] choiceDatas;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private GameObject logPanel;
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private GameObject choicePanel;
    private TextMeshProUGUI[] choicesTexts;
    public static StoryManager Instance { get; private set; }
    public int storyIndex { get; private set; }
    public int textIndex { get; private set; }
    private int stockIndex;
    private string storyLog;
    private bool isReading;
    
    private MotionHandle motionHandle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        storyText.text = "";
        
        choicesTexts = choicePanel.GetComponentsInChildren<TextMeshProUGUI>();
        SetStroryData(storyIndex, textIndex);
    }

    void SetStroryData(int _storyIndex, int _textIndex)
    {
        Story element = storyDatas[_storyIndex].stories[_textIndex];
        backgroundImage.sprite = element.background ? element.background : backgroundImage.sprite;
        characterImage.sprite = element.character ? element.character : characterImage.sprite;
        characterImage.color = element.isHighlight ? Color.white : Color.gray;
        float duration = (float)element.text.Length / 20;
        StartCoroutine(ReadingText(element.text));
        //motionHandle = LMotion.String.Create512Bytes("", element.text, duration).WithOnComplete(() => isReading = false).BindToText(storyText);
        characterName.text = element.name;
        
        storyLog += element.name + "\n";
        storyLog += element.text + "\n \n";
    }

    IEnumerator ReadingText(string text)
    {
        var wait = new WaitForSeconds(0.05f);
        isReading = true;
        foreach (char c in text)
        {
            if(!isReading) break;
            storyText.text += c;
            if (c == '、' || c == '，') yield return new WaitForSeconds(0.13f);
            else if (c == '。' || c == '．' || c == '？') yield return new WaitForSeconds(0.25f);
            if(c == '\n') yield return new WaitForSeconds(0.3f);
            else yield return wait;
        }
        isReading = false;
    }

    public void Next()
    {
        if (isReading)
        {
            storyText.text = storyDatas[storyIndex].stories[textIndex].text;
            isReading = false;
            return;
        }

        textIndex++;
        storyText.text = "";
        characterName.text = "";
        if (textIndex >= storyDatas[storyIndex].stories.Count)
        {
            textIndex = 0;
            storyIndex++;
            if (storyIndex >= storyDatas.Length)
            {
                SceneManager.LoadScene("Title");
                return;
            }

            if (storyDatas[storyIndex - 1].isChoice)
            {
                SetChoiceText(storyDatas[storyIndex].chapter);
                return;
            }
        }
        SetStroryData(storyIndex, textIndex);
    }

    public void VisibleLog(bool isVisible)
    {
        logPanel.SetActive(isVisible);
        logText.text = storyLog;
    }

    public void SetChoiceText(int index)
    {
        choicePanel.SetActive(true);
        for (int i = 0; i < choiceDatas[index].choices.Count; i++)
        {
            choicesTexts[i].text = choiceDatas[index].choices[i].text;
        }
    }

    public void OnChoice(int index)
    {
        choicePanel.SetActive(false);
        storyIndex += index;
        stockIndex = index;
        
        SetStroryData(storyIndex, textIndex);
    }
}
