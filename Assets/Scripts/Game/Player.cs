using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public static event System.Action OnPlayerEnterGoal;
    //public static event System.Action OnInteractionRange;
    
    private PlayerInputActions playerInputActions;
    [SerializeField] private float movementSpeed = 7;
    [SerializeField] private float turningSpeed;
    [SerializeField] private float smoothMoveTime = .1f;
    
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform interactionLocation;
    [SerializeField] private float interactionRadius;
    [SerializeField] private TMPro.TextMeshProUGUI interactionText;
    private Quaternion textInitRotation;

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

        playerInputActions.Player.Interact.performed += HandleInteract;
        
        textInitRotation = interactionText.transform.rotation;
    }

    private void OnDestroy() {
        playerInputActions.Player.Interact.performed -= HandleInteract;

    }

    void LateUpdate()
    {
        interactionText.transform.rotation = textInitRotation;
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        Collider[] colliders =  Physics.OverlapSphere(transform.position, 5f, interactableLayer);
        if (colliders.Length != 0) {
            Interactable interactable = colliders[0].GetComponent<Interactable>();
            if (interactable != null) {
                //interactable.Interact();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Movement Stuff
        inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        Vector3 inputDirection = new Vector3(inputVector.x, 0, inputVector.y);
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputDirection.magnitude, ref smoothMoveVelocity, smoothMoveTime);
        
        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turningSpeed * inputVector.magnitude); //Dont turn when input is 0,0
        //transform.eulerAngles = Vector3.up * angle;
        
        velocity = inputDirection * (movementSpeed * smoothInputMagnitude);
        //transform.Translate(velocity * Time.deltaTime, Space.World); // Change by move amount 
        
        //Interaction stuff
        bool isNearInteraction = false;
        Collider[] colliders =  Physics.OverlapSphere(interactionLocation.position, interactionRadius, interactableLayer);
        if (colliders.Length != 0) {
            Interactable interactable = colliders[0].GetComponent<Interactable>();
            if (interactable != null) {
                isNearInteraction = true;
                interactionText.text = interactable.GetDescription();
                if (playerInputActions.Player.Interact.WasPerformedThisFrame()) {
                    interactable.Interact();
                }
            }
            
        }

        if (!isNearInteraction) interactionText.text = "";


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
