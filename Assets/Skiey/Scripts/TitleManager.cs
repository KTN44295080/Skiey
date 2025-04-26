using System.Security.Cryptography.X509Certificates;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] texts;

    [SerializeField] private Image titleLogo;
    [SerializeField] private Image iconLogo;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private GameObject creditPanel;
    [SerializeField] private Sprite[] backgroundSprites;

    [SerializeField] private Color[] colors;

    [SerializeField] private Vector3 toScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int clearFlag = PlayerPrefs.GetInt("ClearFlag", 0);
        backgroundImage.sprite = backgroundSprites[clearFlag];
        titleLogo.color = clearFlag == 3 || clearFlag == 2 ? colors[1] : colors[0];
        backgroundImage.color = clearFlag == 2 ? Color.clear : Color.white;
        iconLogo.enabled = clearFlag > 0 && clearFlag != 3;
        continueButton.enabled = clearFlag != 3;

        texts = FindObjectsOfType<TextMeshProUGUI>();
        creditPanel.SetActive(false);
        LMotion.Create(titleLogo.rectTransform.localPosition, Vector3.up * 240, 2).WithEase(Ease.InQuint)
            .BindToLocalPosition(titleLogo.rectTransform);
        LMotion.Create(0f, 1f, 2).WithOnComplete(() =>
        {
            iconLogo.color = clearFlag >= 2 ? colors[1] : colors[0];
            foreach (TextMeshProUGUI text in texts)
                text.color = clearFlag >= 2 ? colors[1] : colors[0];
            if(clearFlag == 3)
                continueButton.targetGraphic.color *= .6f;

            foreach (TextMeshProUGUI text in texts)
                LMotion.Create(0f, 1f, 1).BindToColorA(text);
            if(clearFlag > 0)
                LMotion.Create(0f, 1f, 1).BindToColorA(iconLogo);
        }).BindToColorA(backgroundImage);

        LMotion.Create(titleLogo.rectTransform.localScale, Vector3.one, 2).WithEase(Ease.InQuint).WithOnComplete(
            () =>
            {
            }).BindToLocalScale(titleLogo.rectTransform);
    }

    public void OnStartGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.DeleteKey("StoryIndex");
        PlayerPrefs.DeleteKey("TextIndex");
        SceneManager.LoadScene("Main");
    }

    public void OnLoadGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnOnption(bool isOn)
    {
        
    }

    public void OnCredit(bool isOn)
    {
        creditPanel.SetActive(isOn);
        uiPanel.SetActive(!isOn);
    } 
    
    public void OnExitGame()
    {
        Application.Quit();
    }
}
