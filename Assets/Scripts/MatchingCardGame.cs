using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingCardGame : MonoBehaviour
{
    public Sprite cardBack;
    public Sprite[] cardFaces;
    public Button[] cardButtons;
    private int[] cardValues;
    private bool[] matchedCards;
    private int firstSelected = -1;
    private int secondSelected = -1;
    private bool isCheckingMatch = false;
    public int totalPairs = 4;
    public float timeLimit = 60f;
    private float remainingTime;
    public Text timerText;
    private int successNum = 0;

    public GameObject objPuzzle;
    public GameObject prefabBackCard;
    public GameManger gameManger;

    private void Start()
    {
        InitializeGame();
        remainingTime = timeLimit;
        StartCoroutine(Timer());
    }
    void InitializeGame()
    {
        gameManger = GameObject.Find("GameManager").GetComponent<GameManger>();
        cardFaces = Resources.LoadAll<Sprite>("Puzzle");
        objPuzzle = GameObject.Find("Puzzle");
        prefabBackCard = Resources.Load<GameObject>("Prefabs/BackCard");
        cardButtons = new Button[totalPairs * 2];
        for (int i = 0; i < totalPairs * 2; i++)
        {
            cardButtons[i] = Instantiate(prefabBackCard,objPuzzle.transform).GetComponent<Button>();
        }
        cardValues = new int[totalPairs * 2];
        matchedCards = new bool[totalPairs * 2];
        List<int> values = new List<int>();
        for (int i = 0;i < totalPairs; i++)
        {
            values.Add(i);
            values.Add(i);
        }
        Shuffle(values);
        for (int i = 0; i < totalPairs * 2; i++)
        {
            int index = i;
            cardValues[i] = values[i];
            Debug.Log(i);
            cardButtons[i].onClick.AddListener(() => OnCardSelected(index));
            cardButtons[i].image.sprite = cardBack;
        }
    }
    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    void OnCardSelected(int index)
    {
        if (matchedCards[index] || index == firstSelected || isCheckingMatch)
            return;
        cardButtons[index].image.sprite = cardFaces[cardValues[index]];
        if (firstSelected == -1)
        {
            firstSelected = index;
        }
        else
        {
            secondSelected = index;
            isCheckingMatch = true;
            StartCoroutine(CheckMatch());
        }
    }
    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1);
        if (cardValues[firstSelected] == cardValues[secondSelected])
        {
            matchedCards[firstSelected] = true;
            matchedCards[secondSelected] = true;
            successNum++;
            if (successNum == totalPairs)
            {
                ShowMenuUI(true);
            }
        }
        else
        {
            cardButtons[firstSelected].image.sprite = cardBack;
            cardButtons[secondSelected].image.sprite = cardBack;
        }
        firstSelected = -1;
        secondSelected = -1;
        isCheckingMatch = false;
    }
    IEnumerator Timer()
    {
        if (!(successNum == totalPairs))
        {
            while (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                //timerText.text = "Time : " + Mathf.Ceil(remainingTime);
                yield return null;
            }
        }
        ShowMenuUI(false);
    }
    void ShowMenuUI(bool isWin)
    {
        gameManger.SetGameClear(isWin);
        this.gameObject.SetActive(false);
    }
}
