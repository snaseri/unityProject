using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FirstPersonController : MonoBehaviour
{
    PhotonView view;

    public bool CanMove { get; private set; } = true;

    private bool isSprinting => canSprint && Input.GetKey(sprintKey);

    private bool ShouldJump => Input.GetKey(jumpKey) && characterController.isGrounded;

    private bool ShouldCrouch =>
        Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;

    private bool isMoving;


    [Header("Functional Options")] 
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadbob = true;
    
    [Header("Controls")] 
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;  
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;  
    [Header("Movement Parameters")] 
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;


    [Header("Look Parameters")] 
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f; 
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f; 
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f; 
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [Header("Jumping Parameters")] 
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Crouch Parameters")] 
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0,0.5f,0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0,0,0);

    private bool isCrouching;
    private bool duringCrouchAnimation;
    
    [Header("Headbob Parameters")] 
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18;
    [SerializeField] private float sprintBobAmount = 0.1f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYpos = 0;
    private float timer;
    
    private Camera playerCamera;
    private CharacterController characterController;
    private Animator anime;
    private GameObject charModel;


    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    private bool isInAir => Mathf.Abs(moveDirection.y) > 0 && !characterController.isGrounded && Input.GetKeyDown(jumpKey);
    
// Start is called before the first frame update
    void Awake()
    {
        view = GetComponent<PhotonView>();
        playerCamera = GetComponentInChildren<Camera>();
            characterController = GetComponent<CharacterController>();

            // TODO Change way of getting this
            charModel = GameObject.Find("Player");

            anime = GetComponent<Animator>();
            defaultYpos = playerCamera.transform.localPosition.y;
            playerCamera.gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            if (view.IsMine)
            {
                {
                    HandleMovementInput();
                    HandleMouseLook();
                    if (canJump)
                        HandleJump();

                    if (canUseHeadbob)
                        HandleHeadbob();

                    if (canCrouch)
                        HandleCrouch();

                    ApplyFinalMovements();
                    HandleAnimations();
                }
            }
        }
    }
    
    
    private void HandleMovementInput()
    {
            currentInput = new Vector2(
                (isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"),
                (isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
            float moveDirectionY = moveDirection.y;
            moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) +
                            (transform.TransformDirection(Vector3.right) * currentInput.y);
            moveDirection.y = moveDirectionY;


            if (Mathf.Abs(currentInput.x) > 0f || Mathf.Abs(currentInput.y) > 0f)
                isMoving = true;
            else
                isMoving = false;

            //ANIMATIONS
            if (characterController.isGrounded && currentInput == Vector2.zero)
            {
                anime.SetBool("isRunning", false);
            }
            else if (isMoving && !isCrouching)
            {
                anime.SetBool("isRunning", true);
            }

            // CROUCH ANIMATION
            if (isMoving && isCrouching)
            {
                anime.SetBool("isCrouchWalk", true);
                anime.SetBool("isCrouchIdle", false);
            }
            else if (!isMoving && isCrouching)
            {
                anime.SetBool("isCrouchWalk", false);
                anime.SetBool("isCrouchIdle", true);
            }

    }
    
    private void HandleAnimations()
    {
        
        //Jumping
        if (isInAir)
        {
            anime.SetBool("isJumping", true);
        }
        else if (!isInAir && !characterController.isGrounded)
        {
            anime.SetBool("isJumping", true);  
        }
        else
        {
            anime.SetBool("isJumping", false);  
        }
        
        //Sprint
        if (isSprinting && !isCrouching)
        { 
            anime.SetBool("isSprinting", true);   
        }
        else
        {
            anime.SetBool("isSprinting", false);            
        }
        
    }

    private void HandleMouseLook()
    {
        if (view.IsMine)
        {
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
            rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
        }
    }

    private void HandleJump()
    {
        if (ShouldJump)
        {
            moveDirection.y = jumpForce;
        }
    }

    private void HandleCrouch()
    {
        if (ShouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }

    //LERP
    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            yield break;
        
        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        float currentModelHeight = charModel.transform.localPosition.y;
        float targetModelHeight = isCrouching ? -1f : 0f;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;


        while (timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed/timeToCrouch);
            charModel.transform.localPosition =
                new Vector3(0, Mathf.Lerp(currentModelHeight, targetModelHeight, timeElapsed / timeToCrouch), 0);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        
        characterController.height = targetHeight;
        characterController.center = targetCenter;
        charModel.transform.localPosition = new Vector3(0, targetModelHeight, 0);

        

        //ANIMATIONS
        isCrouching = !isCrouching;
        if (!isCrouching)
        {
            anime.SetBool("isCrouchWalk", false);
            anime.SetBool("isCrouchIdle", false);
        }


        duringCrouchAnimation = false;
    }

    private void HandleHeadbob()
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : isSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYpos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : isSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z);
        }
    }

    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
    }
    
}
