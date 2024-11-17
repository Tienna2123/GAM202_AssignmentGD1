using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Parameters
    private float moveSpeed = 5f;
    private float jumpForce = 5f;
    private Vector3 moveDirection;
    private Rigidbody rb;

    // State flags
    private bool isGrounded;
    private bool isAiming;
    private bool isCrouching;
    private bool isRolling;
    private bool isDead;
    private bool isUsingItem;
    private bool isShooting;
    private bool isReloading;
    private bool isSprinting;

    // Components
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        isGrounded = true;
    }

    void Update()
    {
        if (isDead) return;

        HandleInput();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        HandleMovement();
    }

    void HandleInput()
    {
        // Basic movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        // Crouch
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCrouch();
        }

        // Roll
        if (Input.GetKeyDown(KeyCode.R) && isGrounded && !isRolling)
        {
            StartRoll();
        }

        // Aim
        if (Input.GetMouseButton(1))
        {
            StartAiming();
        }
        else
        {
            StopAiming();
        }

        // Shoot
        if (Input.GetMouseButtonDown(0) && !isReloading)
        {
            Shoot();
        }

        // Use Item
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseItem();
        }

        // Sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            StartSprint();
        }
        else
        {
            StopSprint();
        }
    }

    void HandleMovement()
    {
        float currentSpeed = isSprinting ? moveSpeed * 1.5f : moveSpeed;
        currentSpeed = isCrouching ? currentSpeed * 0.5f : currentSpeed;

        Vector3 movement = moveDirection * currentSpeed;
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);

        // Rotate character to face movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        animator?.SetTrigger("Jump");
    }

    void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        animator?.SetBool("Crouch", isCrouching);
    }

    void StartRoll()
    {
        isRolling = true;
        animator?.SetTrigger("Roll");
        Invoke("StopRoll", 0.5f); // Adjust roll duration as needed
    }

    void StopRoll()
    {
        isRolling = false;
    }

    void StartAiming()
    {
        isAiming = true;
        animator?.SetBool("Aiming", true);
    }

    void StopAiming()
    {
        isAiming = false;
        animator?.SetBool("Aiming", false);
    }

    void Shoot()
    {
        if (isAiming)
        {
            isShooting = true;
            animator?.SetTrigger("Shoot");
            // Implement shooting logic here
            Invoke("StopShooting", 0.1f);
        }
    }

    void StopShooting()
    {
        isShooting = false;
    }

    void UseItem()
    {
        isUsingItem = true;
        animator?.SetTrigger("Use");
        // Implement item use logic here
        Invoke("StopUsingItem", 0.5f);
    }

    void StopUsingItem()
    {
        isUsingItem = false;
    }

    void StartSprint()
    {
        isSprinting = true;
        animator?.SetBool("Sprint", true);
    }

    void StopSprint()
    {
        isSprinting = false;
        animator?.SetBool("Sprint", false);
    }

    void UpdateAnimations()
    {
        animator?.SetFloat("Speed", moveDirection.magnitude);
        animator?.SetBool("OnGround", isGrounded);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if character hits the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    public void Die()
    {
        isDead = true;
        animator?.SetTrigger("Death");
        // Implement death logic here
    }
}
