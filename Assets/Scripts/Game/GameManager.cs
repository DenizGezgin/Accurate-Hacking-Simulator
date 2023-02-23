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
        LoseLevel,
        Dialog,
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
    public GameObject _goalObject;
    public GameObject _dialogBox;
    //private Renderer _goalRenderer;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();

        Instance = this;

        remaningEscapeTime = escapeTime;

        GuardBase.OnGuardHasSpottedPlayer += SetGameOver;
        playerInputActions.Menu.ResetScene.performed += ResetGame;
        Player.OnPlayerEnterGoal += CheckGameWon;
        Computer.OnComputerHacked += StartAlarm;
        
        playerInputActions.Menu.Disable();


        Debug.Log("Awake");
    }

    private void Start()
    {
        Renderer _goalRenderer = _goalObject.GetComponent<Renderer>();
        _goalRenderer.material.SetColor("_Color", Color.red);
        Debug.Log("Start");
        if (!SoundManager.Instance.GetCursedBoolAtIndex(SceneManager.GetActiveScene().buildIndex)) {
            _dialogBox.SetActive(true);
            SoundManager.Instance.SetCursedBoolToTrue(SceneManager.GetActiveScene().buildIndex);
            UpdateGameState(GameState.Dialog);
        }
        else {
            UpdateGameState(GameState.Walking);
        }
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
                //HandleWin();
                break;
            case GameState.LoseLevel:
                //HandleLose();
                break;
            case GameState.Dialog:
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
        playerInputActions.Menu.Disable();
    }

    void OnDestroy() {
        // Unsubscribe to the event
        GuardBase.OnGuardHasSpottedPlayer -= SetGameOver;
        Player.OnPlayerEnterGoal -= SetGameWon;
        Player.OnPlayerEnterGoal -= CheckGameWon;
        playerInputActions.Menu.ResetScene.performed -= ResetGame;
        Computer.OnComputerHacked -= StartAlarm;

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
        UpdateGameState(GameState.WinLevel);
        isGameOver = true;
        isAlarmOn = false;
        playerInputActions.Player.Disable();
        playerInputActions.Menu.Enable();
        _ui.ShowGameWinUI();
    }

    void SetGameOver()
    {
        if (state != GameState.WinLevel)
        {
            isGameOver = true;
            isAlarmOn = false;
            playerInputActions.Player.Disable();
            playerInputActions.Menu.Enable();
            _ui.ShowGameLoseUI();
            UpdateGameState(GameState.LoseLevel);
        }
        
    }
    
    private void ResetGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SoundManager.Instance.ChangeMusicToWalking();
            if (state == GameState.LoseLevel)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else if (state == GameState.WinLevel)
            {
                NextScene();
            }
        }
    }

    private void StartAlarm()
    {
        computersToHack--;
        if (computersToHack == 0)
        {
            _goalObject = GameObject.Find("Goal");
            Renderer _goalRenderer = _goalObject.GetComponent<Renderer>();
            _goalRenderer.material.SetColor("_Color", Color.green);
        }

        if (!isAlarmOn) {
            isAlarmOn = true;
            SoundManager.Instance.ChangeMusicToAlert();
        }
    }
    
    public void NextScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
