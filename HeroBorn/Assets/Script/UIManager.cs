using UnityEngine;
using TMPro;
using UnityEngine.UI; // UI 컴포넌트 사용을 위해 필수

public class UIManager : MonoBehaviour
{
    [Header("Common UI")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI itemCountText;
    public Button WinButton;
    public Button GameOverButton;

    [Header("Crosshair UI")]
    public Image crosshairImage;      // 조준점 이미지 연결
    public Color defaultColor = Color.white;
    public Color enemyTargetColor = Color.red;
    public Camera mainCamera;         // 레이캐스트를 쏠 메인 카메라

    [Header("Stamina UI")]
    public Slider staminaSlider;      // 스태미나 슬라이더 연결
    public PlayerController player;   // 플레이어 스크립트 연결 (정보 받아오기용)

    [Header("Shoot UI")]
    public Image reloadIndicator;

    void Start()
    {
        WinButton.gameObject.SetActive(false);
        GameOverButton.gameObject.SetActive(false);

        
        
            
        // 초기 조준선 색상 설정
        if (crosshairImage != null)
            crosshairImage.color = defaultColor;
    }

    void Update()
    {
        
            

        #region Text UI & Game State
        hpText.text = "HP: " + GMBehavior.instance.playerHp;
        itemCountText.text = "Items: " + GMBehavior.instance.itemCollectCount;

        if (GMBehavior.instance.isGameWin)
        {
            WinButton.gameObject.SetActive(true);
        }
        else if (GMBehavior.instance.isGameOver)
        {
            GameOverButton.gameObject.SetActive(true);
        }
        #endregion

        // 🛑 추가된 기능 호출
        if (!GMBehavior.instance.isGameOver || !GMBehavior.instance.isGameWin) {UpdateCrosshair();UpdateStaminaBar();UpdateReloadUI();}

        
        
        
    }

    void UpdateReloadUI()
    {
        if (reloadIndicator == null || player == null) return;

        if (player.isReloading)
        {
            // 장전 중이면 UI 켜고 진행률 표시
            if (!reloadIndicator.gameObject.activeSelf)
                reloadIndicator.gameObject.SetActive(true);


            reloadIndicator.fillAmount = player.reloadProgress;
        }
        else
        {
            // 장전 중이 아니면 UI 끄기
            if (reloadIndicator.gameObject.activeSelf)
                reloadIndicator.gameObject.SetActive(false);
        }
    }





    // 조준선 색상 변경 로직
    void UpdateCrosshair()
    {
        
        if (crosshairImage == null || mainCamera == null || player == null) return;

        if (player.isReloading)
        {
            crosshairImage.gameObject.SetActive(false);
            return; 
        }
        else
        {
            if (!crosshairImage.gameObject.activeSelf)
                crosshairImage.gameObject.SetActive(true);
        }

        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, 1000f))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                crosshairImage.color = enemyTargetColor;
            }
            else
            {
                crosshairImage.color = defaultColor;
            }
        }
        else
        {
            // 허공을 볼 때
            crosshairImage.color = defaultColor;
        }
    }

    // 🏃 스태미나 바 업데이트 로직
    void UpdateStaminaBar()
    {
        if (staminaSlider == null || player == null) return;

        // 슬라이더 값 = 현재 스태미나 / 최대 스태미나 (0 ~ 1 사이 비율로 변환)
        staminaSlider.value = player.playerCurStamina / player.playerMaxStamina;
    }
}