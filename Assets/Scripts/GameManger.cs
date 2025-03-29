using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManger : MonoBehaviour
{
    public MapControl mapControl;
    public PlayerControl playerControl;
    public MonsterControl monsterControl;
    public DelayText delayText;
    public Text ChestNum;
    public Slider hpBar;
    public Slider O2Bar;
    public MatchingCardGame gameCard;
    public GameObject menuUI;
    public Text totalTime;
    
    public void InitGame()
    {
        if (GameObject.Find("GameData") == null)
        {
            delayText.SetText("GameData가 생성되지않았습니다. 메인에서 실행 ㄱㄱ");
            return;
        }
        playerControl = new GameObject("Player").AddComponent<PlayerControl>();
        monsterControl = new GameObject("MonsterControl").AddComponent<MonsterControl>();
        mapControl = new GameObject("MapControl").AddComponent<MapControl>();
        ChestNum = GameObject.Find("ChestNum").GetComponent<Text>();
        totalTime = GameObject.Find("TotalTime").GetComponent<Text>();
        gameCard  = new GameObject("dasdasdas").AddComponent<MatchingCardGame>();
        GameData.Instance.isMouse = false;

        hpBar = GameObject.Find("HpBar").GetComponent<Slider>();
        O2Bar = GameObject.Find("O2Bar").GetComponent<Slider>();

    }
    private void Start()
    {
        InitGame();
    }
    public void GameModeChange()
    {
        if (!GameData.Instance.isMouse)
        {
            Cursor.lockState = CursorLockMode.None;
            GameData.Instance.isMouse = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            GameData.Instance.isMouse = false;
        }
    }

    public void SetChest(int chestNum)
    {
        ChestNum.text = "X" + chestNum;
    }
    public void SetGameClear(bool isGame)
    {
        gameCard.gameObject.SetActive(false);
        menuUI.SetActive(true);
        GameModeChange();
        if (isGame)
        {
            menuUI.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Game Clear";
        }
        else
        {
            menuUI.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Game Over";
        }        
    }
    public void BtnMenu(int num)
    {
        switch (num)
        {
            case 0:
                {
                    GameData.Instance.isGame = false;
                    SceneManager.LoadScene("Main");
                }
                break;
            case 1:
                {
                    GameData.Instance.isGame = false;
                    SceneManager.LoadScene("Game");
                }
                break;
            case 2:
                {
                    Application.Quit();
                }
                break;
        }
    }
}
