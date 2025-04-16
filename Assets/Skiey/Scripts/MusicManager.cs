using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip nonVocalClip;
    [SerializeField] private AudioClip vocalClip;

    public bool OnVocal
    {
        get => onVocal;
        set
        {
            if(onVocal == value) return;
            onVocal = value;
            float time = _audioSource.time;
            _audioSource.clip = value ? vocalClip : nonVocalClip;
            _audioSource.time = time;
            _audioSource.Play();
        }
    }
    [SerializeField] private bool onVocal;
    private bool deltaOnVocal;
    private AudioSource _audioSource;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TryGetComponent(out _audioSource);
        _audioSource.clip = OnVocal ? vocalClip : nonVocalClip;
        _audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(onVocal == deltaOnVocal) return;
        float time = _audioSource.time;
        _audioSource.clip = OnVocal ? vocalClip : nonVocalClip;
        _audioSource.time = time;
        _audioSource.Play();
        deltaOnVocal = OnVocal;
    }
}
