using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


/*
private를 사용한 변수를 다른 스크립트에서 접근할 때 get과 set을 사용함.

지금 사용하지 않는 이유는 게임매니저가 싱글톤으로 구현됐기 때문이다.

*/
public class GMBehavior : MonoBehaviour
{

    public static GMBehavior instance;

    

    public float playerHp = 100f;
    public int itemCollectCount = 0;

    public int gameProgress = 0;

    public int gameProgressWin = 3;

    //public List<int> gameLevels = new List<int>();

    public bool isGameOver = false;

    public bool isGameWin = false;

    void Awake()
    {
        // 일반적인 싱글톤 패턴
        if (instance == null)
        {
            instance = this; // instance 변수에 자기 자신을 할당
        }
        else
        {
            // 이미 instance가 있다면 이 오브젝트는 파괴
            Destroy(gameObject);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Init()
    {
        playerHp = 100f;
        itemCollectCount = 0;
        isGameOver = false;
        isGameWin = false;
    }
    // Update is called once per frame
    void Update()
    {
        ProgressWin();

    }

    public void TakeDamage(float amount)
    {
        playerHp -= amount;
        if (playerHp <= 0)
        {
            Debug.Log("Player is dead!");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            isGameOver = true;
            Time.timeScale = 0f;
            // Handle player death (e.g., restart level, show game over screen)
        }
    }
/*
    public void CollectItem()
    {
        itemCollectCount++;
        Debug.Log("Item collected! Total items: " + itemCollectCount);


        if (itemCollectCount >= 1 && !isGameOver)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            isGameWin = true;
            Debug.Log("All items collected! You win!");
            Time.timeScale = 0f;
            // Handle game win (e.g., show victory screen)
        }
    }
*/



    void ProgressWin()
    {
        if (gameProgress >= gameProgressWin)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            isGameWin = true;
            Debug.Log("Win");
            Time.timeScale = 0f;
        }
    }


    public void RestartGame()
    {
        
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }
}
