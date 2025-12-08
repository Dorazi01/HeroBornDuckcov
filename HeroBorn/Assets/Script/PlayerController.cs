using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    
    [Header("Audio")]
    public AudioClip gunshotSound; 
    AudioSource audioSource;       
    
    [Header("Status Info")]
    public float targetDistance = 0f;




    #region PlayerInputVariables
    public Transform cameraTransform;
    float mouseSensitivity = 3f;

    float xRotation = 0f;
    float yRotation = 0f;
    private InputAction playerActions;
    private Rigidbody rb;
    private float moveSpeed = 5f;

    private bool isJump = false;
    private bool isGrounded = false;


#endregion



#region Stamina Variables
    public bool playerCanRun = true;
    bool isSprinting = false;

    public float playerMaxStamina = 100f;

    public float playerCurStamina = 100f;

#endregion

#region Shooting Variables


    public Transform firePoint;

    
    public LineRenderer laserLine;


    public int maxAmmo = 30;      // 최대 탄약 수
    public int currentAmmo;

    float reloadTime = 1.5f; // 장전 걸리는 시간

    [HideInInspector] // Inspector에서는 굳이 볼 필요 없음
    public float reloadProgress = 0f;
    public bool isReloading = false; // 장전 중인지 체크
    public float fireRate = 0.07f;    // 발사 간격 
    float nextFireTime = 0f;

    public GameObject bulletPrefab;

    //float BulletSpeed = 100f;

    bool isShooting = false;
    bool isFireHeld = false;
    
    float RecoilKickX = 2f;        
    float RecoilKickY = 0.5f;      
    float MaxRecoilX = 10f;        
    float MaxRecoilY = 3f;         
    float RecoilRecoverySpeed = 5f;

    float currentRecoilX = 0f; 
    float currentRecoilY = 0f;
    #endregion

    #region Aim Settings
    [Header("Aim Settings")]
    float aimSpeed = 2.5f;       
    Vector3 aimPositionOffset = new Vector3(0, 0, 3f); 
    float aimSmoothSpeed = 10f;  
    float aimRecoilMultiplier = 0.4f; 
    
    bool isAiming = false;      
    Vector3 defaultCameraPos;   



    #endregion

    

    //Vector2 move = new Vector2();

    private Vector2 inputVector;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            inputVector = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            inputVector = Vector2.zero;
        }

        

        
    }

    


    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            isJump = true;
        }

    }

    public void OnRun(InputAction.CallbackContext context)
{
    // 달리는 키 눌렀을 때
    if (context.phase == InputActionPhase.Performed)
    {
       //달리기 상태 플래그를 참으로 설정
        isSprinting = true;
    }
    // 달리는 키 뗐을 때
    else if (context.phase == InputActionPhase.Canceled)
    {
        //  달리기 상태 플래그를 거짓으로 설정
        isSprinting = false;
    }
    // Debug.LogFormat("Run State: {0}", isSprinting); // 디버깅용
}

    

    #region Shot

    public void OnShot(InputAction.CallbackContext context)
{   
    if (context.phase == InputActionPhase.Performed)
    {
        isFireHeld = true;
    }
    else if (context.phase == InputActionPhase.Canceled)
    {
        isFireHeld = false;
    }

    
}

    #endregion


    #region Aim

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) isAiming = true;
        else if (context.phase == InputActionPhase.Canceled) isAiming = false;
    }

    #endregion



    #region Reload

    public void OnReload(InputAction.CallbackContext context)
    {
        // 키를 눌렀고, 이미 장전 중이 아니고, 탄약이 꽉 차지 않았을 때만 실행
        if (context.phase == InputActionPhase.Performed && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }


    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        reloadProgress = 0f; // 0%에서 시작
        Debug.Log("Reloading...");


        float elapsed = 0f;
        while (elapsed < reloadTime)
        {
            elapsed += Time.deltaTime;
            reloadProgress = elapsed / reloadTime; // 0~1 사이 값 계산
            yield return null; // 한 프레임 대기
        }
        // 장전 완료 처리
        reloadProgress = 1f; // 100% 확인 사살
        currentAmmo = maxAmmo;
        isReloading = false;
        
        Debug.Log("Reload Complete!");
    }



    #endregion






    #region MouseLook
    public void OnMouseLook(InputAction.CallbackContext context)
{
    if (context.phase != InputActionPhase.Performed)
        return;
    
    Vector2 mouse = context.ReadValue<Vector2>();

    float currentSensitivity = isAiming ? mouseSensitivity * 0.2f : mouseSensitivity;
    //조준중 마우스 감도

    float mouseX = mouse.x * currentSensitivity * Time.deltaTime;
    float mouseY = mouse.y * currentSensitivity * Time.deltaTime;

    // 1. 상하(X축) 회전 -> 카메라만 담당
    xRotation -= mouseY;
    xRotation = Mathf.Clamp(xRotation, -15f, 15f); 
    // 2. 좌우(Y축) 회전 -> 플레이어 몸통 담당
    yRotation += mouseX;
}
    #endregion

    
    void RecoilRecover()
    {
        currentRecoilX = Mathf.Lerp(currentRecoilX, 0f, Time.deltaTime * RecoilRecoverySpeed);
        currentRecoilY = Mathf.Lerp(currentRecoilY, 0f, Time.deltaTime * RecoilRecoverySpeed);

    
        float finalXRotation = xRotation - currentRecoilX; 
        
        finalXRotation = Mathf.Clamp(finalXRotation, -15f, 15f);
        cameraTransform.localRotation = Quaternion.Euler(finalXRotation, 0f, 0f);

        float finalYRotation = yRotation + currentRecoilY;
    
    
        transform.localRotation = Quaternion.Euler(0f, finalYRotation, 0f);
    }




    void Awake ()
    {
        playerActions = new InputAction();
        rb = GetComponent<Rigidbody>();
    }


    void Start()
    {
        Application.targetFrameRate = 60;
        currentAmmo = maxAmmo;
        audioSource = GetComponent<AudioSource>();

        if (cameraTransform != null)
            defaultCameraPos = cameraTransform.localPosition;

    }



    // Update is called once per frame
    void Update()
    {
        RecoilRecover();
        HandleCameraZoom(); 
        Fire();
        DrawLaser();
    }

    void FixedUpdate()
    {

        Run();
        Move();
        Jump();

        



    
    } 

    void Run()
{
    bool isMoving = inputVector.magnitude > 0.01f;

    // 🛑 1. 달리기 상태 (스태미나 소모 및 제어)
    if (isSprinting && playerCanRun && isMoving)
    {
        playerCurStamina -= 20f * Time.fixedDeltaTime; 

        if (playerCurStamina < 0f)
        {
            playerCurStamina = 0f;
        }

        if (playerCurStamina <= 0f)
        {
            playerCanRun = false; // 달릴 수 없음
            isSprinting = false;  // 달리기 즉시 해제
        }
    }
    // 2. 걷거나 멈춘 상태 (스태미나 회복)
    else
    {
        playerCurStamina += 30f * Time.deltaTime; 

        if (playerCurStamina > playerMaxStamina)
        {
            playerCurStamina = playerMaxStamina;
        }

        // 3. 재활성화 로직 (최대치까지 회복되면 다시 달릴 수 있게)
        if (!playerCanRun && playerCurStamina >= playerMaxStamina)
        {
            playerCanRun = true;
        }
    }
}



    void HandleCameraZoom()
    {
        if (cameraTransform == null) return;

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
    }


    void Fire()
    {
        // 발사 조건
        if (isFireHeld && !isShooting && (!isSprinting || isAiming) && currentAmmo > 0 && !isReloading && Time.time >= nextFireTime)
        {
            isShooting = true;
            nextFireTime = Time.time + fireRate;
            currentAmmo--;


            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit, 1000f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(1000f);
            }

            Vector3 spawnPos = (firePoint != null) ? firePoint.position : transform.position;

            // 3. 발사 방향 (총구 -> 목표지점)
            Vector3 direction = (targetPoint - spawnPos).normalized;

            // 4. 총알 생성
            // 🛑 [중요] 총알이 '레이저 선'과 똑같은 각도로 생성됨
            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(direction));

            if (audioSource != null && gunshotSound != null)
            {
                audioSource.PlayOneShot(gunshotSound);
            }
            
            // 충돌 무시 설정
            Collider[] playerColliders = GetComponentsInChildren<Collider>();
            Collider bulletCollider = bullet.GetComponent<Collider>();
            foreach (Collider col in playerColliders) Physics.IgnoreCollision(col, bulletCollider);

            // 5. 발사 (초고속으로 설정하여 레이저 느낌 내기)
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.useGravity = false; 
            bulletRb.linearVelocity = direction * 300f; 

            // --- [반동 처리] ---
            #region Recoil
            float recoilMultiplier = isAiming ? aimRecoilMultiplier : 1f;
            currentRecoilX += Random.Range(RecoilKickX, RecoilKickX * 1.5f) * recoilMultiplier;
            float randomYKick = Random.Range(-RecoilKickY, RecoilKickY) * recoilMultiplier;
            currentRecoilY += randomYKick;
            currentRecoilX = Mathf.Clamp(currentRecoilX, 0f, MaxRecoilX);
            currentRecoilY = Mathf.Clamp(currentRecoilY, -MaxRecoilY, MaxRecoilY);
            #endregion

            isShooting = false;
        }
    }




    void Move()
    {
        // 🛑 [수정] 속도 결정 로직 (조준 > 달리기 > 걷기 순서로 우선순위)
        if (isAiming)
        {
            moveSpeed = aimSpeed; // 2.5f (조준 시 느리게)
        }
        else if (isSprinting && playerCanRun)
        {
            moveSpeed = 10f; // 달리기
        }
        else
        {
            moveSpeed = 5f; // 걷기
        }

        Vector3 moveDirection = transform.right * inputVector.x + transform.forward * inputVector.y;
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
    }


    void Jump()
    {
        
        if (isJump && isGrounded && playerCurStamina >= 20f && playerCanRun)
        {
            Debug.Log("Jump!");
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);

            playerCurStamina -= 20f;

            if (playerCurStamina <= 0f)
            {
                playerCurStamina = 0f;
                playerCanRun = false; // 지침 상태(UI 빨간색 변경)
                isSprinting = false;  // 달리기도 멈춤
            }

            isJump = false;
        }

        else if (isJump)
        {
            isJump = false;
        }
    }
        #region DrawLaser

    void DrawLaser()
    {
        if (laserLine == null || firePoint == null) return;

        // 1. 카메라 정중앙에서 목표 찾기
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        // 2. 목표지점 결정 (닿은 곳 or 허공)
        if (Physics.Raycast(ray, out hit, 1000f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100f);
        }

        // 3. 라인 렌더러로 선 그리기 (총구 -> 목표)
        laserLine.SetPosition(0, firePoint.position); // 시작점: 총구
        laserLine.SetPosition(1, targetPoint);        // 끝점: 크로스헤어가 닿은 곳

        targetDistance = Vector3.Distance(transform.position, targetPoint);
    }
    #endregion

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            GMBehavior.instance.TakeDamage(20f);
        }
        else if (collision.gameObject.CompareTag("HpPickup"))
        {
            //Debug.Log("Item Collected!");
            Destroy(collision.gameObject);
        }
    } 
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
