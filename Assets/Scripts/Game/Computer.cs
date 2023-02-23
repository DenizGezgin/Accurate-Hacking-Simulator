using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class Computer : Interactable
{
    public static event System.Action OnComputerHacked;
    
    public bool isHacked = false;
    [SerializeField] Slider _slider;
    [SerializeField] private AudioClip clickSound;
    private SliderScript _sliderScript;

    private void Awake()
    {
        _sliderScript = _slider.GetComponent<SliderScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override string GetDescription()
    {
           if(isHacked) return "Hacked!";
           return "Press E to Hack";
    }

    IEnumerator DoHacking()
    {
        _sliderScript.ShowSlider();
        int keyCounter = 0;
        string[] lastInputs = new string[5];;
        int inputIndex = 0;
        while (keyCounter < 15)
        {
            InputSystem.onAnyButtonPress
                .CallOnce(ctrl =>
                {
                    //if (ctrl.displayName == "Esc")
                    //{
                        //_sliderScript.HideSlider();
                        //.DecrementByValue(keyCounter);
                        //GameManager.Instance.UpdateGameState(GameManager.GameState.Walking);
                        //return;
                    //}
                    bool inputExists = false;
                    for (int i = 0; i < inputIndex; i++) {
                        if (lastInputs[i] == ctrl.displayName) {
                            inputExists = true;
                            break;
                        }
                    }
                    if(!inputExists)
                    {
                        SoundManager.Instance.PlaySound(clickSound);
                        lastInputs[inputIndex] = ctrl.displayName;
                        inputIndex = (inputIndex + 1) % 5;
                        _sliderScript.IncrementByValue(1);
                        keyCounter++;
                    }
                });
            yield return new WaitForSeconds(0.1f);
        }
        isHacked = true;
        _sliderScript.HideSlider();
        GameManager.Instance.UpdateGameState(GameManager.GameState.Walking);
        if (OnComputerHacked != null) OnComputerHacked();
    }
    public override void Interact()
    {
        if (!isHacked)
        {
            GameManager.Instance.UpdateGameState(GameManager.GameState.Hacking);
            StartCoroutine(DoHacking());
        }
    }
}
