using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ChoiceData : ScriptableObject
{
    public List<Choice> choices = new List<Choice>();

    [System.Serializable]
    public class Choice
    {
        public int choiceID;
        public string text;
    }
}
