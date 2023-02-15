using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static event System.Action OnPlayerEnterGoal;
    
    private PlayerInputActions playerInputActions;
    [SerializeField] private float movementSpeed = 7;
    [SerializeField] private float turningSpeed;
    [SerializeField] private float smoothMoveTime = .1f;

    private float angle;
    private float smoothInputMagnitude;
    private float smoothMoveVelocity;
    private Vector3 velocity;
    private Vector2 inputVector;
    private float rotateAdd; // For incrementing the rotation

    private Rigidbody _rigidbody;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();
        playerInputActions = gameManager.playerInputActions;
    }

    // Update is called once per frame
    void Update()
    {
        inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        Vector3 inputDirection = new Vector3(inputVector.x, 0, inputVector.y);
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputDirection.magnitude, ref smoothMoveVelocity, smoothMoveTime);
        
        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turningSpeed * inputVector.magnitude); //Dont turn when input is 0,0
        //transform.eulerAngles = Vector3.up * angle;
        
        velocity = inputDirection * (movementSpeed * smoothInputMagnitude);
        //transform.Translate(velocity * Time.deltaTime, Space.World); // Change by move amount 

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Goal") {
            if (OnPlayerEnterGoal != null) {
                OnPlayerEnterGoal();
            }
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.MoveRotation(Quaternion.Euler(Vector3.up*angle));
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputDirection), 0.15f);
        _rigidbody.MovePosition(_rigidbody.position + velocity*Time.deltaTime);
    }

}
