using UnityEngine;

public class EnemyHpBar : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // 메인 카메라 찾기
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // 🛑 항상 카메라가 보는 방향과 일치시킴 (UI가 납작해지는 것 방지)
        transform.rotation = mainCamera.transform.rotation;
    }
}
