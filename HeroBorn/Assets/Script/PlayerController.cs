using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject bulletPrefab;

    public float BulletSpeed = 100f;



    public float mouseSensitivity = 100f;

    public float xRotation = 0f;
    public float yRotation = 0f;
    private InputAction playerActions;
    private Rigidbody rb;
    private float moveSpeed = 5f;

    private bool isJump = false;
    private bool isGrounded = false;

    private bool isShooting = false;

    //Vector2 move = new Vector2();

    private Vector2 inputVector;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
        Debug.LogFormat("Move Input: {0}", inputVector);

        
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            isJump = true;
        }

    }

    void OnShot(InputValue value)
    {
        if (value.isPressed && !isShooting)
        {
            isShooting = true;
            Debug.Log("Shooting!");

            GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.forward, Quaternion.identity);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.linearVelocity = transform.forward * BulletSpeed;

            //Destroy(bullet, 2f); // Destroy the bullet after 2 seconds
            isShooting = false;
        }
    }
    void OnMouseLook(InputValue value)
    {
        Vector2 mouse = value.Get<Vector2>();
        float mouseX = mouse.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouse.y * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, 0, 0);

        yRotation += mouseX;

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    void Awake ()
    {
        playerActions = new InputAction();
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = transform.right * inputVector.x + transform.forward * inputVector.y;
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);

        if (isJump && isGrounded)
        {
            Debug.Log("Jump!");
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            isJump = false;

        }
    
    } 



    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log("Player Hit by Enemy!");
            GMBehavior.instance.playerHp -= 10;
        }
        else if (collision.gameObject.CompareTag("HpPickup"))
        {
            //Debug.Log("Item Collected!");
            Destroy(collision.gameObject);
        }
    } 
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
