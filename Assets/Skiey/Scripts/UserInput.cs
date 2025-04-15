using System;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    InputSystem_Actions _actions;
    private StoryManager _storyManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _storyManager = StoryManager.Instance;
        _actions = new InputSystem_Actions();
        _actions.Enable();
        _actions.UI.Submit.started += context => _storyManager.Next();
    }

    private void OnDestroy()
    {
        _actions.Dispose();
    }
}