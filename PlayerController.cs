using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Base setup")]
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    public float tiltAmount = 5f;
    public float tiltSpeed = 5f;
    private float currentTiltAngleX = 0f;
    private float currentTiltAngleZ = 0f;

    public float normalFoV = 60f;
    public float runningFoV = 75f;
    public float fovLerpSpeed = 5f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    private Vector3 currentVelocity = Vector3.zero;
    public float smoothTime = 0.1f;

    [HideInInspector]
    public bool canMove = true;

    public Camera playerCamera;
    public Transform camHolder;

    public bool StopPlayer = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Kamera ve mouse ayarlarý
        if (playerCamera != null)
        {
            playerCamera.enabled = true;
            playerCamera.fieldOfView = normalFoV;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (StopPlayer) return;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0);

        if (playerCamera != null)
        {
            float targetFoV = isRunning ? runningFoV : normalFoV;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFoV, Time.deltaTime * fovLerpSpeed);
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float targetSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float targetSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;

        Vector3 targetMoveDirection = (forward * targetSpeedX) + (right * targetSpeedY);
        moveDirection = Vector3.SmoothDamp(moveDirection, targetMoveDirection, ref currentVelocity, smoothTime);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove && playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            camHolder.localRotation = Quaternion.Euler(rotationX, 0, 0);

            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

            float targetTiltX = Input.GetAxis("Vertical") * tiltAmount;
            float targetTiltZ = Input.GetAxis("Horizontal") * -tiltAmount;
            currentTiltAngleX = Mathf.Lerp(currentTiltAngleX, targetTiltX, Time.deltaTime * tiltSpeed);
            currentTiltAngleZ = Mathf.Lerp(currentTiltAngleZ, targetTiltZ, Time.deltaTime * tiltSpeed);

            camHolder.localRotation = Quaternion.Euler(rotationX + currentTiltAngleX, 0, currentTiltAngleZ);
        }
    }
}
