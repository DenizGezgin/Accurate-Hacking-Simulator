using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerInputActions playerInputActions;
    [SerializeField] private GameUIHelper _ui;
    private bool isGameOver;
    
    void Awake()
    {
        playerInputActions = new PlayerInputActions();
        isGameOver = false;
        Debug.Log("Game started!");
        playerInputActions.Player.Enable();
        playerInputActions.Menu.Disable();
        
        GuardBase.OnGuardHasSpottedPlayer += SetGameOver;
        playerInputActions.Menu.ResetScene.performed += ResetGame;
        Player.OnPlayerEnterGoal += SetGameWon;
    }
    
    void OnDestroy() {
        // Unsubscribe to the event
        GuardBase.OnGuardHasSpottedPlayer -= SetGameOver;
        Player.OnPlayerEnterGoal -= SetGameWon;
        playerInputActions.Menu.ResetScene.performed -= ResetGame;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    void SetGameWon()
    {
        isGameOver = true;
        playerInputActions.Player.Disable();
        playerInputActions.Menu.Enable();
        _ui.ShowGameWinUI();
    }

    void SetGameOver()
    {
        isGameOver = true;
        playerInputActions.Player.Disable();
        playerInputActions.Menu.Enable();
        _ui.ShowGameLoseUI();
    }
    
    private void ResetGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
}
