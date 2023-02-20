using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUIHelper : MonoBehaviour
{
    public GameObject gameLoseUI;
    public GameObject gameWinUI;

    [SerializeField] private GameObject _hackingUI;
    
    public TextMeshProUGUI escapeTimerUI;

    private void Awake()
    {
        GameManager.onGameStateChange += GameManagerOnGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChange -= GameManagerOnGameStateChange;
    }

    private void GameManagerOnGameStateChange(GameManager.GameState state)
    {
        if (state == GameManager.GameState.Hacking)
        {
            _hackingUI.SetActive(true);
        }
        else if (state == GameManager.GameState.Walking)
        {
            _hackingUI.SetActive(false);
        }
    }

    public void ShowGameLoseUI() {
        gameWinUI.SetActive(false);
        gameLoseUI.SetActive(true);
    }
    public void ShowGameWinUI() {
        gameWinUI.SetActive(true);
        gameLoseUI.SetActive(false);
    }
    public void ClearUI() {
        gameWinUI.SetActive(false);
        gameLoseUI.SetActive(false);
    }
    
    public void SetEscapeTimerUI(float time)
    {
        escapeTimerUI.text = time.ToString("F2");
    }
    
}
