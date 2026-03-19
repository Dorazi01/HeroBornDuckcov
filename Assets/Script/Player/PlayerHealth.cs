using UnityEngine;
using System.Collections;
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private Coroutine damageCoroutine;
    private bool isTouchingEnemy;

    void Start() => currentHealth = maxHealth;

    public float GetHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;

    public void TakeDamage(int amount)
    {
        GMBehavior.instance.playerHp -= amount;
        if (GMBehavior.instance.playerHp <= 0)
        {
            Debug.Log("Player is dead!");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GMBehavior.instance.isGameOver = true;
            Time.timeScale = 0f;
            // Handle player death (e.g., restart level, show game over screen)
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!isTouchingEnemy)
            {
                isTouchingEnemy = true;
                damageCoroutine = StartCoroutine(DamageRoutine());
            }
        }
        else if (collision.gameObject.CompareTag("HpPickup"))
        {
            currentHealth = maxHealth;
            Destroy(collision.gameObject);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isTouchingEnemy = false;
            if (damageCoroutine != null) StopCoroutine(damageCoroutine);
        }
    }

    IEnumerator DamageRoutine()
    {
        while (isTouchingEnemy)
        {
            TakeDamage(20);
            yield return new WaitForSeconds(1.0f);
        }
    }

    public int GetCurrentHealth()
    {
        return GMBehavior.instance.playerHp;
    }
}