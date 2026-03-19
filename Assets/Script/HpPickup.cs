using UnityEngine;

public class HpPickup : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("HP Pickup Collected!");
            //GMBehavior.instance.CollectItem();
            Destroy(gameObject);
        }
    }
}
