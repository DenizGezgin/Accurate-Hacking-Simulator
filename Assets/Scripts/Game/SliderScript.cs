using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    private Slider _slider;

    private void Awake()
    {
        _slider = gameObject.GetComponent<Slider>();
        _slider.gameObject.SetActive(false);
    }

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void IncrementByValue(float value)
    {
        _slider.value += value;
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
