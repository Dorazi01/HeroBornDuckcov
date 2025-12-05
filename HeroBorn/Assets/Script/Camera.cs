using UnityEngine;

public class Camera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Vector3 CamOffset = new Vector3(0, 1.2f, -2.6f);

    private Transform playerTransform;
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            transform.position = playerTransform.TransformPoint(CamOffset);

            transform.LookAt(playerTransform);
        }
    }
        
    }

