using UnityEngine;
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float aimSensitivityMultiplier = 0.2f;
    [SerializeField] private float firingRecoilMultiplier = 0.4f;

    [SerializeField] private Vector3 aimPositionOffset = new Vector3(0, 0, 3f);
    [SerializeField] private float aimSmoothSpeed = 10f;

    [Header("Recoil")]
    [SerializeField] internal float recoilKickX = 1f;
    [SerializeField] internal float recoilKickY = 0.4f;
    [SerializeField] internal float maxRecoilX = 10f;
    [SerializeField] internal float maxRecoilY = 3f;
    [SerializeField] private float recoilRecoverySpeed = 5f;

    private float xRotation;
    private float yRotation;
    private float currentRecoilX;
    private float currentRecoilY;
    private Vector3 defaultCameraPos;

    private bool isAiming = false;
    private bool isFireHeld = false;

    void Start()
    {
        if (cameraTransform != null) { defaultCameraPos = cameraTransform.localPosition; }

    }

    void Update()
    {
        RecoilRecover();
        HandleCameraZoom();
    }

    public void HandleMouseLook(Vector2 mouseDelta)
    {
        float sensitivity = mouseSensitivity;
        if (isAiming)
            sensitivity *= aimSensitivityMultiplier;
        else if (isFireHeld)
            sensitivity *= firingRecoilMultiplier;

        float mouseX = mouseDelta.x * sensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * sensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -15f, 15f);
        yRotation += mouseX;
    }

    public void ApplyRecoil(float recoilX, float recoilY = 0f)
    {
        float recoilMult = isAiming ? 0.4f : 1f;
        currentRecoilX += recoilX * recoilMult;
        currentRecoilY += recoilY * recoilMult;
        currentRecoilX = Mathf.Clamp(currentRecoilX, 0f, maxRecoilX);
        currentRecoilY = Mathf.Clamp(currentRecoilY, -maxRecoilY, maxRecoilY);
    }

    private void RecoilRecover()
    {
        if (cameraTransform == null) return;
        currentRecoilX = Mathf.Lerp(currentRecoilX, 0f, Time.deltaTime * recoilRecoverySpeed);
        currentRecoilY = Mathf.Lerp(currentRecoilY, 0f, Time.deltaTime * recoilRecoverySpeed);

        float finalXRotation = xRotation - currentRecoilX;
        finalXRotation = Mathf.Clamp(finalXRotation, -15f, 15f);
        cameraTransform.localRotation = Quaternion.Euler(finalXRotation, 0f, 0f);

        float finalYRotation = yRotation + currentRecoilY;
        transform.localRotation = Quaternion.Euler(0f, finalYRotation, 0f);
    }

    private void HandleCameraZoom()
    {
        if (cameraTransform == null) { return; }

        Vector3 targetPos;
        if (isAiming)
        {
            // 조준 중: 기본 위치 + 앞으로 이동(aimOffset)
            targetPos = defaultCameraPos + aimPositionOffset;
        }
        else
        {
            // 평상시: 기본 위치로 복귀
            targetPos = defaultCameraPos;
        }

        // 부드럽게 이동 (Lerp)
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPos, Time.deltaTime * aimSmoothSpeed);

        // Y축 회전(좌우)은 보통 제한하지 않지만, 
        // 급격한 회전을 방지하기 위해 각도를 0~360 사이로 유지해주는 것이 깔끔합니다.
        if (yRotation > 360f) yRotation -= 360f;
        if (yRotation < -360f) yRotation += 360f;

    }

    public void SetAiming(bool value) => isAiming = value;
    public void SetFiring(bool value) => isFireHeld = value;
}