using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private StoryData[] storyData;
    [SerializeField] private StoryData[] storyData2;
    [SerializeField] private StoryData[] storyData_Yuki;
    [SerializeField] private StoryData[] storyData_Epilogue;
    [SerializeField] private ChoiceData[] choiceDatas;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private GameObject logPanel;
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private GameObject choicePanel;
    private TextMeshProUGUI[] choicesTexts;
    [SerializeField] private Image fadePanel;
    [SerializeField] private GameObject changeButton;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private GameObject shareButton;
    [SerializeField] private Image[] uis;
    private StoryData[] currentStoryData;

    [SerializeField] private Color redColor;
    public static StoryManager Instance { get; private set; }
    private int StoryIndex
    {
        get => storyIndex;
        set
        {
            storyIndex = value;
            if (isHajime)
                PlayerPrefs.SetInt("StoryIndex", value);
        }
    }
    
    private int TextIndex
    {
        get => textIndex;
        set
        {
            textIndex = value;
            if (isHajime)
                PlayerPrefs.SetInt("TextIndex", value);
        }
    }

    private int storyIndex;
    private int textIndex;
    private int stockIndex;
    private string storyLog;
    private bool isReading;
    private bool isLoading;
    private bool isHajime = true;
    private bool waitEnd;
    private Image nameImage;
    
    private Color keepAlpha;

    private MotionHandle characterMotion;
    private MotionHandle backgroundMotion;
    private MotionHandle fadeMotion;
    
    private AudioSource audioSource;

    private int clearFlag;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        nameImage = characterName.transform.parent.GetComponent<Image>();
        TryGetComponent(out audioSource);
        currentStoryData = PlayerPrefs.GetInt("Judge") > 0 ? storyData2 : storyData;
        storyText.text = "";
        choicesTexts = choicePanel.GetComponentsInChildren<TextMeshProUGUI>();
        StoryIndex = PlayerPrefs.GetInt("StoryIndex", 0);
        TextIndex = PlayerPrefs.GetInt("TextIndex", 0);
        clearFlag = PlayerPrefs.GetInt("ClearFlag", 0);
        SetStroryData(StoryIndex, TextIndex);
        keepAlpha = Color.white;
        
        skipButton.SetActive(PlayerPrefs.GetInt("ClearFlag", 0) > 0);
    }

    void SetStroryData(int _storyIndex, int _textIndex)
    {
        Story element = currentStoryData[_storyIndex].stories[_textIndex];
        
        float result = element.character ? 1f : 0f;
        if (characterImage.sprite != element.character && (!characterImage.sprite || !element.character))
            characterMotion = LMotion.Create(1 - result, result, 0.8f).WithOnComplete(() =>
            {
                characterImage.sprite = element.character;
            }).BindToColorA(characterImage);
        if (element.character)
            characterImage.sprite = element.character;
        if(isHajime)
            changeButton.SetActive(element.canChange && clearFlag >= 1);
        keepAlpha.a = characterImage.color.a;
        characterImage.color = element.isHighlight ? Color.white: Color.gray;
        characterImage.color *= keepAlpha;
        backgroundImage.color = element.name == "" ? Color.white * 0.9f : Color.white;
        backgroundImage.color = element.background ? backgroundImage.color : Color.black;
        result = element.background ? 1f : 0f;
        if (backgroundImage.sprite != element.background && element.backgroundColor != Color.white && (!backgroundImage.sprite || !element.background))
            characterMotion = LMotion.Create(1 - result, result, 0.8f).WithOnComplete(() =>
            {
                backgroundImage.sprite = element.background;
            }).BindToColorA(backgroundImage);
        if (element.character)
            characterImage.sprite = element.character;
        if (element.stopMusic)
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
            else
                audioSource.UnPause();
        }

        backgroundImage.sprite = element.background;
        backgroundImage.color = element.backgroundColor == Color.white ? Color.white : backgroundImage.color;
        StopCoroutine(ReadingText(""));
        StartCoroutine(ReadingText(element.text));
        characterName.text = element.name;
        nameImage.enabled = element.name != "";
        
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
        if (waitEnd && !fadeMotion.IsActive())
        {
            if (StoryIndex == 0)
            {
                shareButton.SetActive(false);
                fadeMotion = LMotion.Create(0f, 1f, 2)
                    .WithOnComplete(() =>
                    {
                        foreach (Image ui in uis)
                        {
                            ui.color = redColor;
                        }
                        OnNextScene(StoryIndex, TextIndex, 1);
                        waitEnd = false;
                    })
                    .BindToColorA(fadePanel);
            }
            else
            {
                fadeMotion = LMotion.Create(0f, 1f, 2)
                    .WithOnComplete(() => SceneManager.LoadScene("Title"))
                    .BindToColorA(fadePanel);
            }
            return;
        }

        if (isLoading) return;
        if (isReading)
        {
            if (characterMotion.IsActive())
                characterMotion.Complete();
            storyText.text = currentStoryData[storyIndex].stories[textIndex].text;
            isReading = false;
            return;
        }

        TextIndex++;
        storyText.text = "";
        characterName.text = "";
        if (TextIndex >= currentStoryData[StoryIndex].stories.Count)
        {
            if (currentStoryData[StoryIndex].isChoice)
                SetChoiceText(currentStoryData[StoryIndex + 1].chapter);
            else
            {
                fadeMotion = LMotion.Create(0f, 1f, currentStoryData[StoryIndex].fadeTime)
                    .WithOnComplete(() => OnNextScene(StoryIndex, TextIndex, currentStoryData[StoryIndex].fadeTime))
                    .BindToColorA(fadePanel);
            }

            TextIndex = 0;
            StoryIndex++;
            isLoading = true;
            if (StoryIndex >= currentStoryData.Length)
            {
                //shareButton.SetActive(true);
                waitEnd = true;
                if (fadeMotion.IsActive())
                    fadeMotion.Cancel();
                if (PlayerPrefs.GetInt("ClearFlag", 0) == 0)
                {
                    PlayerPrefs.SetInt("ClearFlag", 1);
                    endText.text = "NORMAL END / 藍の色";
                }
                else if(PlayerPrefs.GetInt("ClearFlag", 0) == 1 && !isHajime)
                {
                    PlayerPrefs.SetInt("ClearFlag", 2);
                    endText.text = "ANOTHER END / 哀の色";
                }
                else if(StoryIndex > 12)
                {
                    PlayerPrefs.SetInt("ClearFlag", 3);
                    endText.text = "TRUE END / 愛の色";
                    StoryIndex = 0;
                    TextIndex = 0;
                    currentStoryData = storyData_Epilogue;
                } else if (clearFlag == 3)
                {
                    LMotion.Create(0f, 1f, 2)
                        .WithOnComplete(() => SceneManager.LoadScene("Title"))
                        .BindToColorA(fadePanel);
                }
                PlayerPrefs.DeleteKey("StoryIndex");
                PlayerPrefs.DeleteKey("TextIndex");
            }
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
        if(PlayerPrefs.GetInt("ClearFlag", 0) < 2)
            choicesTexts[1].transform.parent.gameObject.SetActive(false);
        for (int i = 0; i < choiceDatas[index].choices.Count; i++)
        {
            choicesTexts[i].text = choiceDatas[index].choices[i].text;
        }
    }

    public void OnChoice(int index)
    {
        choicePanel.SetActive(false);
        PlayerPrefs.SetInt("Judge", index);

        if (index == 1)
            currentStoryData = storyData2;

        OnNextScene(StoryIndex, TextIndex, 1);
    }

    void OnNextScene(int _storyIndex, int _textIndex, float fadeTime)
    {
        if (audioSource.clip != currentStoryData[_storyIndex].bgm)
        {
            audioSource.clip = currentStoryData[_storyIndex].bgm;
            audioSource.Play();
        }
        audioSource.volume = currentStoryData[_storyIndex].volume;
        changeButton.SetActive(false);
        endText.text = "";
        storyText.text = "";
        characterName.text = "";
        backgroundImage.sprite = currentStoryData[_storyIndex].stories[_textIndex].background;
        characterImage.color = Color.clear;
        characterImage.sprite = null;
        LMotion.Create(1f, 0f, fadeTime).WithDelay(fadeTime).WithOnComplete(() =>
        {
            SetStroryData(_storyIndex, _textIndex); 
            isLoading = false;
        }).BindToColorA(fadePanel);
    }

    public void OnShare()
    {
        naichilab.UnityRoomTweet.Tweet ("skiey", endText.text + "を見た。", "アイノイロ");
    }
    
    public void OnChange()
    {
        if(!isHajime) return;

        changeButton.SetActive(false);
        isHajime = !isHajime;
        TextIndex = 0;
        StoryIndex = currentStoryData[StoryIndex].chapter;
        fadeMotion = LMotion.Create(0f, 1f, 1)
            .WithOnComplete(() =>
            {
                foreach (Image ui in uis)
                    ui.color = redColor;    
                OnNextScene(StoryIndex, TextIndex, 1);
            })
            .BindToColorA(fadePanel);
        currentStoryData = isHajime ? storyData : storyData_Yuki;
        //SetStroryData(StoryIndex, TextIndex);
    }

    public void OnSkip()
    {
        if (backgroundMotion.IsActive())
            backgroundMotion.Complete();
        if(characterMotion.IsActive())
            characterMotion.Complete();
        TextIndex = currentStoryData[StoryIndex].stories.Count - 1;
        Next();
    }
}
