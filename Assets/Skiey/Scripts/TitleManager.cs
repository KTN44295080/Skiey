using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void OnStartGame()
    {
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
    
    public void OnExitGame()
    {
        Application.Quit();
    }
}
