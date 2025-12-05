using UnityEditor.Build.Content;
using UnityEngine;

public class HpPickup : MonoBehaviour
{

    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("HP Pickup Collected!");
            GMBehavior.instance.CollectItem();
            Destroy(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
