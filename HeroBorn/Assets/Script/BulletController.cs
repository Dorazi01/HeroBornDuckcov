using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Effects")]
    public GameObject impactEffectPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 2f); // Destroy the bullet after 2 seconds
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (impactEffectPrefab != null)
        {
            ContactPoint contact = collision.contacts[0];
            
            Vector3 spawnPos = contact.point + (contact.normal * 0.1f);

            GameObject effect = Instantiate(impactEffectPrefab, spawnPos, Quaternion.LookRotation(contact.normal));
            Destroy(effect, 1f);; 
        }

        // 3. 총알 파괴
        Destroy(gameObject);
    }
    

}
