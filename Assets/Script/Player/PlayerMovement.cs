using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float aimSpeed = 2.5f;
    [SerializeField] private float jumpForce = 5f;

    PlayerController player;
    PlayerStamina stamina;

    private Rigidbody rb;
    private Vector2 inputVector;
    private bool isJump = false;
    private bool isGrounded = false;

    // 외부 상태 참조
    private bool isSprinting;
    private bool isAiming;
    private bool playerCanRun;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<PlayerController>();
        stamina = GetComponent<PlayerStamina>();
    }

    public void SetInput(Vector2 input) => inputVector = input;
    public void SetSprinting(bool value) => isSprinting = value;
    public void SetAiming(bool value) => isAiming = value;
    public void SetJump(bool value) => isJump = value;
    public void SetPlayerCanRun(bool value) => playerCanRun = value;

    void FixedUpdate()
    {
        Move();
        Jump();
    }

    private void Move()
    {
        if (rb == null) return;

        float currentSpeed = isAiming ? aimSpeed : (isSprinting && playerCanRun) ? sprintSpeed : moveSpeed;
        Vector3 moveDirection = transform.right * inputVector.x + transform.forward * inputVector.y;
        rb.linearVelocity = new Vector3(moveDirection.x * currentSpeed, rb.linearVelocity.y, moveDirection.z * currentSpeed);
    }



    private void Jump()
    {
        // if (rb == null) return;
        // if (isJump && isGrounded)
        // {
        //     rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        //     isJump = false;
        // }

        if (isJump && isGrounded && player.currentStamina >= stamina.jumpCost && playerCanRun)
        {
            Debug.Log("Jump!");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            player.currentStamina -= stamina.jumpCost;

            if (player.currentStamina <= 0f)
            {
                player.currentStamina = 0f;
                playerCanRun = false; // 지침 상태(UI 빨간색 변경)
                isSprinting = false;  // 달리기도 멈춤
            }

            isJump = false;
        }

        else if (isJump)
        {
            isJump = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) { isGrounded = true; }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) { isGrounded = false; }
    }
}