using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI itemCountText;

    public Button WinButton;
    public Button GameOverButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WinButton.gameObject.SetActive(false);
        GameOverButton.gameObject.SetActive(false);
    }



    // Update is called once per frame
    void Update()
    {


        #region Text UI
        hpText.text = "HP: " + GMBehavior.instance.playerHp;
        itemCountText.text = "Items: " + GMBehavior.instance.itemCollectCount;

        #endregion


        if (GMBehavior.instance.isGameWin)
        {
            WinButton.gameObject.SetActive(true);
        }
        else if (GMBehavior.instance.isGameOver)
        {
            GameOverButton.gameObject.SetActive(true);
        }

    }
}
