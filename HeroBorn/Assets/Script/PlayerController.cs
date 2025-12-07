using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    

    


    #region PlayerInputVariables
public Transform cameraTransform;
public float mouseSensitivity = 100f;

    public float xRotation = 0f;
    public float yRotation = 0f;
    private InputAction playerActions;
    private Rigidbody rb;
    private float moveSpeed = 5f;

    private bool isJump = false;
    private bool isGrounded = false;


#endregion



#region Running Variables
    bool playerCanRun = true;
    bool isSprinting = false;

    public float playerMaxStamina = 100f;

    public float playerCurStamina = 100f;

#endregion

#region Shooting Variables

    public int maxAmmo = 30;      // 최대 탄약 수
    public int currentAmmo;

    public float reloadTime = 1.5f; // 장전 걸리는 시간

    [HideInInspector] // Inspector에서는 굳이 볼 필요 없음
    public float reloadProgress = 0f;
    public bool isReloading = false; // 장전 중인지 체크
    public float fireRate = 0.001f;    // 발사 간격 
    private float nextFireTime = 0f;

    public GameObject bulletPrefab;

    public float BulletSpeed = 100f;

    private bool isShooting = false;

    bool isFireHeld = false;


    

    
    public float RecoilKickX = 2f;         // 한 발당 수직 반동 증가량
    public float RecoilKickY = 0.5f;       // 한 발당 수평 반동 증가량
    public float MaxRecoilX = 10f;         // 최대 수직 반동 누적 한계
    public float MaxRecoilY = 3f;          // 최대 수평 반동 누적 한계
    public float RecoilRecoverySpeed = 5f; // 반동 회복 속도 (높을수록 빠름)

    private float currentRecoilX = 0f; // 현재 누적된 수직 반동 값
    private float currentRecoilY = 0f; // 현재 누적된 수평 반동 값


    
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

        // 🛑 [수정] 단순 대기가 아니라, 시간을 재면서 진행률 업데이트
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
    float mouseX = mouse.x * mouseSensitivity * Time.deltaTime;
    float mouseY = mouse.y * mouseSensitivity * Time.deltaTime;

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
    }



    // Update is called once per frame
    void Update()
    {
        RecoilRecover();
        Fire();
        
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
        playerCurStamina += 10f * Time.deltaTime; 

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


    void Fire()
    {
        
    if (isFireHeld && !isShooting && !isSprinting && currentAmmo > 0 && !isReloading && Time.time >= nextFireTime)
    {
        isShooting = true;
        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        #region Recoil (기존과 동일)
        currentRecoilX += Random.Range(RecoilKickX, RecoilKickX * 1.5f);
        float randomYKick = Random.Range(-RecoilKickY, RecoilKickY);
        currentRecoilY += randomYKick;
        currentRecoilX = Mathf.Clamp(currentRecoilX, 0f, MaxRecoilX);
        currentRecoilY = Mathf.Clamp(currentRecoilY, -MaxRecoilY, MaxRecoilY);
        #endregion

        // 🛑 1. 총알 생성 위치 설정 (캐릭터 기준)
        // 캐릭터 중심에서 앞(Forward)으로 0.8m, 위(Up)로 1.5m (어깨/총구 높이)
        Vector3 spawnPosition = transform.position + transform.forward * 0.8f;

        Vector3 targetPoint;

        // 2. 레이캐스트로 목표 지점 탐색
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, 1000f))
        {
            // 🛑 [핵심 수정] 목표 지점이 총구와 너무 가까운지 확인 (예: 내 발등)
            if (Vector3.Distance(spawnPosition, hit.point) < 2.0f) 
            {
                // 너무 가까우면(내 발등이면) 레이캐스트 무시하고 카메라 앞쪽 멀리를 목표로 설정
                targetPoint = cameraTransform.position + cameraTransform.forward * 1000f;
            }
            else
            {
                targetPoint = hit.point;
            }
        }
        else
        {
            targetPoint = cameraTransform.position + cameraTransform.forward * 1000f;
        }

        // 3. 발사 방향 계산 (생성 위치 -> 목표 지점)
        Vector3 shootDirection = (targetPoint - spawnPosition).normalized;

        // 4. (안전 장치) 만약 계산된 방향이 캐릭터 뒤쪽을 향한다면?
        // (Dot Product가 0보다 작으면 각도가 90도 이상 벌어진 것 = 뒤로 쏘는 것)
        if (Vector3.Dot(transform.forward, shootDirection) < 0f)
        {
             // 강제로 카메라가 보는 방향으로 수정
            shootDirection = cameraTransform.forward;
        }

        // 5. 총알 생성 및 발사
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        
        // **중요**: 플레이어 자신과 총알 충돌 무시 (발사하자마자 나한테 맞는 것 방지)
        Physics.IgnoreCollision(GetComponent<Collider>(), bullet.GetComponent<Collider>());

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.linearVelocity = shootDirection * BulletSpeed;

        isShooting = false;
        
    }
    else if (isFireHeld && currentAmmo <= 0)
        {
            Debug.Log("Out of Ammo! Press R to Reload.");
        }
    
    }




    void Move()
{
    // 🛑 여기서 moveSpeed를 최종적으로 결정합니다.
    if (isSprinting && playerCanRun)
    {
        moveSpeed = 10f; // 달리기 속도
    }
    else
    {
        moveSpeed = 5f; // 기본 속도
    }

    Vector3 moveDirection = transform.right * inputVector.x + transform.forward * inputVector.y;
    rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
}


    void Jump()
    


    {
        if (isJump && isGrounded)
        {
            Debug.Log("Jump!");
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            isJump = false;

        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log("Player Hit by Enemy!");
            GMBehavior.instance.playerHp -= 10;
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
