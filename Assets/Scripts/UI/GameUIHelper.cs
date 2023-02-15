using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIHelper : MonoBehaviour
{
    public GameObject gameLoseUI;
    public GameObject gameWinUI;

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
    
}
