using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource _musicSource, _sfxSource;
    [SerializeField] private AudioClip _alertMusic, _walkingMusic;

    public bool[] cursedBools;
    
    private void Awake()
    {
        if (Instance == null){
            Instance = this;
            cursedBools = new bool[5];
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(AudioClip clip) {
        _sfxSource.PlayOneShot(clip);
    }
    
    public void ChangeMusicToAlert() {
        _musicSource.clip = _alertMusic;
        _musicSource.Play();
    }
    
    public void ChangeMusicToWalking() {
        _musicSource.clip = _walkingMusic;
        _musicSource.Play();
    }

    public void SetCursedBoolToTrue(int index) {
        cursedBools[index] = true;
    }

    public bool GetCursedBoolAtIndex(int index) {
        return cursedBools[index];
    }
}
