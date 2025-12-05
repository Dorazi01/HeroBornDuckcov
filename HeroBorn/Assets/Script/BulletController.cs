using UnityEngine;

public class BulletController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 2f); // Destroy the bullet after 2 seconds
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
