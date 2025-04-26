using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StoryData : ScriptableObject
{
    public bool fake;
    public int chapter;
    public float fadeTime = 1;
    public bool isChoice;
    public AudioClip bgm;
    public float volume;
    public List<Story> stories = new List<Story>();
}

[System.Serializable]
public class Story
{
    public Color backgroundColor;
    public Sprite background;
    public Sprite character;
    public AudioClip sound;
    public string name;
    public bool isHighlight;
    public bool canChange;
    public bool stopMusic;
    [TextArea] public string text;
}
