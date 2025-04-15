using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StoryData : ScriptableObject
{
    public List<Story> stories = new List<Story>();
}

[System.Serializable]
public class Story
{
    public Sprite background;
    public Sprite character;
    public string name;
    public bool isHighlight;
    [TextArea] public string text;
}
