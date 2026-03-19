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

    public TextMeshProUGUI progressText;

    public TextMeshProUGUI progrText;


    [Header("Crosshair UI")]
    public Image crosshairImage;      // 조준점 이미지 연결
    public Color defaultColor = Color.white;
    public Color enemyTargetColor = Color.red;
    public Camera mainCamera;         // 레이캐스트를 쏠 메인 카메라

    public TextMeshProUGUI distanceText;

    [Header("Stamina UI")]
    public Slider staminaSlider;      // 스태미나 슬라이더 연결
    public PlayerController player;   // 플레이어 스크립트 연결 (정보 받아오기용)

    public Image staminaFillImage;          // 슬라이더 안의 'Fill' 이미지 연결

    Color normalStaminaColor = Color.green; // 평소 색상 (흰색 또는 초록색)
    Color exhaustedStaminaColor = Color.red; // 지쳤을 때 색상 (빨간색)

    [Header("Shoot UI")]
    public Image reloadIndicator;
    public TextMeshProUGUI ammoText;

    public TextMeshProUGUI reloadText;

    void Start()
    {
        WinButton.gameObject.SetActive(false);
        GameOverButton.gameObject.SetActive(false);
        distanceText.gameObject.SetActive(true);
        reloadText.gameObject.SetActive(false);
        progressText.gameObject.SetActive(true);
        progrText.gameObject.SetActive(true);


        // 초기 조준선 색상 설정
        if (crosshairImage != null)
            crosshairImage.color = defaultColor;
    }

    void Update()
    {
        if (GMBehavior.instance == null || player == null) return;
        UpdateCommonUI();

        if (!GMBehavior.instance.isGameOver && !GMBehavior.instance.isGameWin)
        {
            UpdateStaminaBar();
            UpdateReloadUI();
            UpdateAmmoText();
            UpdateDistanceText();


            if (!player.isReloading)
            {
                UpdateCrosshair();
            }


        }

    }

    void UpdateCommonUI()
    {

        if (GMBehavior.instance == null) return;

        #region Text UI & Game State
        hpText.text = "HP: " + GMBehavior.instance.playerHp;
        itemCountText.text = "Items: " + GMBehavior.instance.itemCollectCount;
        progressText.text = "Progress: " + GMBehavior.instance.gameProgress + " / " + GMBehavior.instance.gameProgressWin;
        progrText.text = "Destroy Enemy Spawners";


        if (GMBehavior.instance.isGameWin)
        {
            WinButton.gameObject.SetActive(true);
            distanceText.gameObject.SetActive(false);
        }
        else if (GMBehavior.instance.isGameOver)
        {
            GameOverButton.gameObject.SetActive(true);
            distanceText.gameObject.SetActive(false);
        }
        #endregion

    }

    void UpdateDistanceText() // 거리 표시하기
    {
        if (player == null || distanceText == null) return;

        distanceText.text = player.targetDistance.ToString("F1") + "m";
    }

    void UpdateAmmoText() // 재장전 상태 표시
    {
        if (ammoText == null || player == null) return;

        ammoText.text = player.isReloading ? "Reloading..." : $"{player.currentAmmo} / {player.maxAmmo}";

        if (player.currentAmmo <= 0 && !player.isReloading)
        {
            ammoText.color = Color.red;
            reloadText.gameObject.SetActive(true);
        }
        else
        {
            ammoText.color = Color.white;
        }

    }

    void UpdateReloadUI() // 재장전 UI
    {
        if (reloadIndicator == null || player == null || crosshairImage == null) return;


        if (player.isReloading)
        {
            reloadText.gameObject.SetActive(false);


            if (!reloadIndicator.gameObject.activeSelf)
                reloadIndicator.gameObject.SetActive(true);
            crosshairImage.gameObject.SetActive(false);


            reloadIndicator.fillAmount = player.reloadProgress;
        }
        else
        {
            if (reloadIndicator.gameObject.activeSelf)
                reloadIndicator.gameObject.SetActive(false);
            crosshairImage.gameObject.SetActive(true);
        }
    }





    // 조준선 색상 변경 로직
    void UpdateCrosshair()
    {

        if (crosshairImage == null || mainCamera == null || player == null) return; if (crosshairImage == null || mainCamera == null || player == null) return;

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

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
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

    void UpdateStaminaBar()
    {
        if (staminaSlider == null || player == null) return;


        staminaSlider.value = player.currentStamina / player.maxStamina;

        if (staminaFillImage != null)
        {
            if (player.playerCanRun)
            {
                staminaFillImage.color = normalStaminaColor;
            }
            else
            {
                staminaFillImage.color = exhaustedStaminaColor;
            }
        }
    }
}