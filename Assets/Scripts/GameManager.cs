using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]GameObject bg;
    [SerializeField] GameObject title;
    [SerializeField] GameObject startbutton;
    [SerializeField] GameObject intructionbutton;
    [SerializeField] GameObject instruction;
    [SerializeField] GameObject win;

    public Text textshow;
    public int clickCount = 0;


    [Header("Compare card list")]
    [SerializeField] private List<Card> cardComparison;//卡牌比對清單

    public List<Card.CardPattern> cardsToBePutIn;//卡牌總類清單

    [SerializeField] private Transform[] positions;

    [Header("number of compare card is win")]
    int matchedCardsCount = 0;

    private static GameManager m_instance = null;

    public static GameManager Instance
    {
        get
        {
            //無找到GameManager，則創建一個GameManager
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance;
        }
    }

    void gamemeunopen()
    {
        Time.timeScale = 0f;
        bg.SetActive(true);
        title.SetActive(true); 
        startbutton. SetActive(true);
        intructionbutton. SetActive(true);
    }

    public void gamemeunclose()
    {
        Time.timeScale = 1f;
        bg.SetActive(false);
        title.SetActive(false);
        startbutton.gameObject. SetActive(false);
        intructionbutton.gameObject. SetActive(false);
        SceneManager.LoadScene("SampleSceen");
       

    }

    public void instructionopen()
    {
        Time.timeScale = 0f;
        instruction.SetActive(true);
        
    }

    public void instructionclose()
    {
        Time.timeScale = 1f;
        bg.SetActive(false);
        title.SetActive(false);
        startbutton.gameObject.SetActive(false);
        intructionbutton.gameObject.SetActive(false);
        instruction.SetActive(false);
        SceneManager.LoadScene("SampleSceen");
    }



    void Start()
    {
        //SetCardToBePutIn();
        //AddNewCard(Card.CardPattern.apple);
        gamemeunopen();
        GenerateRandomCards();
    }

    private void Update()
    {
        
            textshow.text = "你總共花了:" + clickCount + "次點擊";
        
    }

    public void grade()
    {
        clickCount++;
    }


    void SetCardToBePutIn()//enum轉list
    {
        Array array = Enum.GetValues(typeof(Card.CardPattern));
        foreach (var item in array)
        {
            cardsToBePutIn.Add((Card.CardPattern)item);//強制轉型態 
        }
        cardsToBePutIn.RemoveAt(0);//刪掉CardPattern.none
    }

    

    void AddNewCard(Card.CardPattern cardPattern, int positionIndex)
    {
        GameObject card = Instantiate(Resources.Load<GameObject>("prefabs/card"));//Resources裡面讀取card物件

        Card cardComponent = card.GetComponent<Card>();       
        cardComponent.cardPattern = cardPattern;// 設置卡片的總類
        cardComponent.cardState = Card.CardState.close;// 確保新生成的卡片是蓋著的

        card.GetComponent<Card>().cardPattern = cardPattern;//從Card腳本中取得卡牌總類
        card.name = "card_" + cardPattern.ToString();//把card和卡牌總類轉為文字
        card.transform.position = positions[positionIndex].position;//陣列裡面編號物件的位置套用至卡牌位置座標
        //cardComponent.cardState = Card.CardState.close;
        //card.GetComponent<Card>().cardState = Card.CardState.close;// 確保新生成的卡片是蓋著的

        GameObject graphic = Instantiate(Resources.Load<GameObject>("prefabs/graphic"));//Resources裡面讀取graphic物件
        graphic.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("graphic/" + cardPattern.ToString());////Resources裡面讀取graphic物件，抓取卡牌總類(物件)，最後轉為文字
        graphic.transform.SetParent(card.transform);//變卡牌的子物件
        graphic.transform.localPosition = new Vector3(0, 0, 0.1f);//設定座標
        graphic.transform.eulerAngles = new Vector3(0, 180, 0);//順著y軸轉180度(翻轉時不會左右顛倒)
    }

    public void AddCardInCardComparison(Card card)//卡牌放入清單
    {
        cardComparison.Add(card);
    }

    public bool ReadyToCompareCards
    {
        get
        {
            if (cardComparison.Count == 2)
            {
                //兩張卡牌才可以比對
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void CompareCardsInList()
    {
        if (ReadyToCompareCards)
        {
            //如果有兩張卡牌時
            //Debug.Log("You can compare card");
            if (cardComparison[0].cardPattern == cardComparison[1].cardPattern)
            {
                Debug.Log("card is same");
                foreach (var card in cardComparison)
                {
                    //配對成功
                    card.cardState = Card.CardState.win;
                }
                ClearCardComparsion();
                matchedCardsCount = matchedCardsCount + 2;
                if (matchedCardsCount >= positions.Length)//全部配對成功
                {
                    StartCoroutine(ReloadScrene());
                    wait();
                    win.SetActive(true);

                }               
                clickCount++;// 在這裡增加點擊次數
            }
            else
            {
                Debug.Log("card isn't same");
                StartCoroutine(MissNatchCards());//協程
            }
        }
    }

    void ClearCardComparsion()
    {
        //清空比對過的卡牌
        cardComparison.Clear();
    }

    void TurnBackCards()
    {
        foreach (var card in cardComparison)
        {
            //卡牌蓋回去
            card.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
            card.cardState = Card.CardState.close;
        }
    }

    IEnumerator MissNatchCards()
    {
        yield return new WaitForSeconds(1.5f);//暫停1.5秒蓋回去
        TurnBackCards();
        ClearCardComparsion();
    }

    IEnumerator ReloadScrene()
    {
        yield return new WaitForSeconds(3);//3秒過後重洗卡牌
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerable wait()
    {
        yield return new WaitForSeconds(2);
    }

    void GenerateRandomCards()//發卡牌
    {
        int positionIndex = 0;
        for (int i = 0; i < 2; i++)
        {
            SetCardToBePutIn();//準備卡牌
            int maxRandomNunber = cardsToBePutIn.Count;//最大亂數不超過4

            for (int j = 0; j < maxRandomNunber; maxRandomNunber--)
            {
                int randomNumber = UnityEngine.Random.Range(0, maxRandomNunber);//產生0-3的亂數         
                AddNewCard(cardsToBePutIn[randomNumber], positionIndex);//抽卡牌
                cardsToBePutIn.RemoveAt(randomNumber);//抽過的移除
                positionIndex++;
            }
        }
    }

}
