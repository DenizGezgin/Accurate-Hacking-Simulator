using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public enum GameState
    {
        Menu,
        Walking,
        Hacking,
        WinLevel,
        LoseLevel
    }

    public GameState state;
    public static GameManager Instance;
    public static event Action<GameState> onGameStateChange;

    [SerializeField] private float escapeTime;
    [SerializeField] private int computersToHack = 1;
    private float remaningEscapeTime;
    
    public PlayerInputActions playerInputActions;
    [SerializeField] private GameUIHelper _ui;
    private bool isGameOver = false;
    private bool isAlarmOn = false;
    
    void Awake()
    {
        playerInputActions = new PlayerInputActions();

        Instance = this;

        remaningEscapeTime = escapeTime;

        playerInputActions.Player.Enable();
        playerInputActions.Menu.Disable();
        
        GuardBase.OnGuardHasSpottedPlayer += SetGameOver;
        playerInputActions.Menu.ResetScene.performed += ResetGame;
        Player.OnPlayerEnterGoal += CheckGameWon;
        Computer.OnComputerHacked += StartAlarm;
    }

    private void Start()
    {
        UpdateGameState(GameState.Walking);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;
        switch(newState)
        {
            case GameState.Menu:
                break;
            case GameState.Walking:
                HandleWalking();
                break;
            case GameState.Hacking:
                HandleHacking();
                break;
            case GameState.WinLevel:
                break;
            case GameState.LoseLevel:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        onGameStateChange?.Invoke(newState);
    }

    private void HandleHacking()
    {
        playerInputActions.Player.Disable();
    }
    
    private void HandleWalking()
    {
        playerInputActions.Player.Enable();
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
        if (isAlarmOn)
        {
            remaningEscapeTime -= Time.deltaTime;
        }
        _ui.SetEscapeTimerUI(remaningEscapeTime);

        if (remaningEscapeTime < 0)
        {
            SetGameOver();
        }
    }
    
    
    void CheckGameWon()
    {
        if(computersToHack == 0) SetGameWon();
    }
    
    void SetGameWon()
    {
        isGameOver = true;
        isAlarmOn = false;
        playerInputActions.Player.Disable();
        playerInputActions.Menu.Enable();
        _ui.ShowGameWinUI();
    }

    void SetGameOver()
    {
        isGameOver = true;
        isAlarmOn = false;
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

    private void StartAlarm()
    {
        computersToHack--;
        isAlarmOn = true;
    }
}
