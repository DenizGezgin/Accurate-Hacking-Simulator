using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Dialog : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _textComponenet;
    [SerializeField] private AudioClip _scrollSound;


    public string[] lines;

    [SerializeField] private float _textSpeed;
    private int _index;
    
    // Start is called before the first frame update
    void Start()
    {
        _textComponenet.text = String.Empty;
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (_textComponenet.text == lines[_index]) {
                NextLine();
            }
            else {
                StopAllCoroutines();
                _textComponenet.text = lines[_index];
            }
        }
    }

    void StartDialogue() {
        _index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine() {
        foreach (char c in lines[_index].ToCharArray()) {
            _textComponenet.text += c;
            SoundManager.Instance.PlaySound(_scrollSound);
            yield return new WaitForSeconds(_textSpeed);
        }
    }

    void NextLine() {
        if (_index < lines.Length - 1) {
            _index++;
            _textComponenet.text = String.Empty;
            StartCoroutine(TypeLine());
        }
        else {
            GameManager.Instance.UpdateGameState(GameManager.GameState.Walking);
            gameObject.SetActive(false);
        }
    }
}
