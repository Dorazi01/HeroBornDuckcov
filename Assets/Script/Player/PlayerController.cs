using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private PlayerShooter shooter;
    [SerializeField] private PlayerStamina stamina;
    [SerializeField] private PlayerHealth health;
    [SerializeField] private Transform firePoint;
    [SerializeField] private LineRenderer laserLine;

    InputAction playerInput;

    public float targetDistance;
    private Vector2 inputVector;
    internal bool isAiming;

    public bool isReloading;
    public float reloadProgress;

    public int maxAmmo = 30;
    public int currentAmmo;

    public bool playerCanRun;

    public float currentStamina;
    public float maxStamina;

    void Awake()
    {
        playerInput = new InputAction();
    }


    void Update()
    {
        // 거리 계산 (UI용)
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out var hit, 1000f))
            targetDistance = Vector3.Distance(transform.position, hit.point);
    }

    // ===== 입력 이벤트 (InputActions에서 호출) =====
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            inputVector = context.ReadValue<Vector2>();
        else if (context.phase == InputActionPhase.Canceled)
            inputVector = Vector2.zero;

        movement.SetInput(inputVector);
        stamina.SetMoving(inputVector.magnitude > 0.01f);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && stamina.TryConsumeStamina(20f))
            movement.SetJump(true);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        bool sprinting = context.phase == InputActionPhase.Performed;
        movement.SetSprinting(sprinting);
        stamina.SetSprinting(sprinting);
        shooter.SetSprinting(sprinting);
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        isAiming = context.phase == InputActionPhase.Performed;
        playerCamera.SetAiming(isAiming);
        movement.SetAiming(isAiming);
        shooter.SetAiming(isAiming);
    }

    public void OnShot(InputAction.CallbackContext context)
    {
        bool firing = context.phase == InputActionPhase.Performed;
        shooter.SetFireHeld(firing);
        playerCamera.SetFiring(firing);
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && !shooter.GetIsReloading())
            StartCoroutine(shooter.ReloadCoroutine());
    }

    public void OnMouseLook(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            playerCamera.HandleMouseLook(context.ReadValue<Vector2>());
    }
}