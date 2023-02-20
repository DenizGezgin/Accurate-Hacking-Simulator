using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    private Slider _slider;
    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _slider = gameObject.GetComponent<Slider>();
        _particleSystem = GameObject.Find("Progress Bar Particle").GetComponent<ParticleSystem>();
        _slider.gameObject.SetActive(false);
    }

    IEnumerator playParticle()
    {
        if (!_particleSystem.isPlaying)
        {
            _particleSystem.Play();
        }

        yield return new WaitForSeconds(0.2f);
        _particleSystem.Stop();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _particleSystem.Play();
    }

    public void IncrementByValue(float value)
    {
        _slider.value += value;
        StartCoroutine(playParticle());
    }
    
    public void DecrementByValue(float value)
    {
        _slider.value -= value;
    }
    
    public void HideSlider()
    {
        _slider.gameObject.SetActive(false);
    }
    
    public void ShowSlider()
    {
        _slider.gameObject.SetActive(true);
        _slider.value = 0;
    }
}
