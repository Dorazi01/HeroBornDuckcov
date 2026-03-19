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
        if (cameraTransform != null)
            defaultCameraPos = cameraTransform.localPosition;
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
        if (cameraTransform == null) return;
        Vector3 targetPos = isAiming ? defaultCameraPos + aimPositionOffset : defaultCameraPos;
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPos,
                                                     Time.deltaTime * aimSmoothSpeed);
    }

    public void SetAiming(bool value) => isAiming = value;
    public void SetFiring(bool value) => isFireHeld = value;
}