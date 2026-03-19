using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float sprintCost = 20f;
    [SerializeField] internal float jumpCost = 20f;
    // [SerializeField] private float exhaustRecoveryDelay = 0.5f;
    PlayerController player;

    private bool playerCanRun = true;
    private bool isSprinting;
    private bool isMoving;

    public float GetCurrentStamina() => player.currentStamina;
    public float GetMaxStamina() => player.maxStamina;
    public bool CanSprint() => playerCanRun;

    void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    void Start() => player.currentStamina = player.maxStamina;

    void FixedUpdate() => UpdateStamina();

    public void SetSprinting(bool value) => isSprinting = value;
    public void SetMoving(bool isMoving) => this.isMoving = isMoving;

    private void UpdateStamina()
    {
        if (isSprinting && playerCanRun && isMoving && !player.isAiming)
        {
            player.currentStamina -= sprintCost * Time.fixedDeltaTime;
            if (player.currentStamina < 0) player.currentStamina = 0;
            if (player.currentStamina <= 0) playerCanRun = false;
        }
        else
        {
            player.currentStamina += 30f * Time.deltaTime;
            if (player.currentStamina > player.maxStamina) player.currentStamina = player.maxStamina;
            if (!playerCanRun && player.currentStamina >= player.maxStamina) playerCanRun = true;
        }
    }

    public bool TryConsumeStamina(float amount)
    {
        if (player.currentStamina >= amount)
        {
            player.currentStamina -= amount;
            return true;
        }
        return false;
    }
}