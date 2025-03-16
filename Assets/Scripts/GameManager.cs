using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    //  public static GameManager instance;
    public static GameManager instance;

    public Text playerNameText;
    public Text moneyText;
    public Text lastSavedTimeText;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        LoadGameData();
        
    }

    public void LoadGameData()
    {
        DataManager.instance.LoadGame(1); // 기본적으로 슬롯 1 불러오기 (임시)

        playerNameText.text = "이름: " + DataManager.instance.nowPlayer.playerName;
        moneyText.text = "돈: " + DataManager.instance.nowPlayer.money.ToString();

    }
    //!씬이동 
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
