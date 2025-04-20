using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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
    [SerializeField] private Image fadePanel;
    public static StoryManager Instance { get; private set; }
    private int StoryIndex
    {
        get => storyIndex;
        set
        {
            storyIndex = value;
            PlayerPrefs.SetInt("StoryIndex", value);
        }
    }
    
    private int TextIndex
    {
        get => textIndex;
        set
        {
            textIndex = value;
            PlayerPrefs.SetInt("TextIndex", value);
        }
    }

    private int storyIndex;
    private int textIndex;
    private int stockIndex;
    private string storyLog;
    private bool isReading;
    private bool isLoading;
    
    private MotionHandle motionHandle;
    
    private Color keepAlpha;

    private MotionHandle characterMotion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        storyText.text = "";
        choicesTexts = choicePanel.GetComponentsInChildren<TextMeshProUGUI>();
        StoryIndex = PlayerPrefs.GetInt("StoryIndex", 0);
        TextIndex = PlayerPrefs.GetInt("TextIndex", 0);
        SetStroryData(StoryIndex, TextIndex);

        keepAlpha = Color.white;
    }

    void SetStroryData(int _storyIndex, int _textIndex)
    {
        Story element = storyDatas[_storyIndex].stories[_textIndex];
        float result = element.character ? 1f : 0f;
        if (characterImage.sprite != element.character && (!characterImage.sprite || !element.character))
            characterMotion = LMotion.Create(1 - result, result, 0.8f).WithOnComplete(() =>
            {
                characterImage.sprite = element.character;
            }).BindToColorA(characterImage);
        if (element.character)
            characterImage.sprite = element.character;
        

        keepAlpha.a = characterImage.color.a;
        characterImage.color = element.isHighlight ? Color.white: Color.gray;
        characterImage.color *= keepAlpha;
        backgroundImage.color = element.name == "" ? Color.white * 0.9f : Color.white;
        backgroundImage.color = element.background ? backgroundImage.color : Color.black;
        backgroundImage.sprite = element.background;
        StartCoroutine(ReadingText(element.text));
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
            if (c == '、' || c == '，') yield return new WaitForSeconds(0.14f);
            else if (c == '。' || c == '．' || c == '？') yield return new WaitForSeconds(0.25f);
            if(c == '\n') yield return new WaitForSeconds(0.25f);
            else yield return wait;
        }
        isReading = false;
    }

    public void Next()
    {
        if (isLoading) return;
        if (isReading)
        {
            if (characterMotion.IsActive())
                characterMotion.Complete();
            storyText.text = storyDatas[storyIndex].stories[textIndex].text;
            isReading = false;
            return;
        }

        TextIndex++;
        storyText.text = "";
        characterName.text = "";
        if (TextIndex >= storyDatas[StoryIndex].stories.Count)
        {
            if (storyDatas[StoryIndex].isChoice)
                SetChoiceText(storyDatas[StoryIndex + 1].chapter);
            else
                LMotion.Create(0f, 1f, storyDatas[StoryIndex].fadeTime)
                    .WithOnComplete(() => OnNextScene(StoryIndex, TextIndex, storyDatas[StoryIndex].fadeTime)).BindToColorA(fadePanel);
            TextIndex = 0;
            StoryIndex++;
            isLoading = true;
            if (storyIndex >= storyDatas.Length)
                SceneManager.LoadScene("Title");
            return;
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
        StoryIndex += index;
        stockIndex = index;
        
        SetStroryData(storyIndex, textIndex);
    }

    void OnNextScene(int _storyIndex, int _textIndex, float fadeTime)
    {
        backgroundImage.sprite = storyDatas[_storyIndex].stories[_textIndex].background;
        characterImage.color = Color.clear;
        characterImage.sprite = null;
        LMotion.Create(1f, 0f, fadeTime).WithDelay(fadeTime).WithOnComplete(() =>
        {
            SetStroryData(storyIndex, textIndex); 
            isLoading = false;
        }).BindToColorA(fadePanel);
    }


}
