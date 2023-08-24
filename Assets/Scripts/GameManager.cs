using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Compare card list")]
    public List<Card> cardComparison;//卡牌比對清單

    [Header("cardpattern list")]
    public List<Card.CardPattern> cardsToBePutIn;//卡牌總類清單

    public Transform[] positions;

    [Header("number of compare card is win")]
    int matchedCardsCount = 0;

    void Start()
    {
        //SetCardToBePutIn();
        //AddNewCard(Card.CardPattern.apple);
        GenerateRandomCards();
    }
    void SetCardToBePutIn()//enum轉list
    {
        Array array = Enum.GetValues(typeof(Card.CardPattern));
        foreach(var item in array)
        {
               cardsToBePutIn.Add((Card.CardPattern)item);//強制轉型態 
        }
        //cardsToBePutIn.RemoveAt(0);//刪掉CardPattern.none
        
    } 

    void GenerateRandomCards()//發卡牌
    {
        int positionIndex = 0;
        for (int i = 0; i < 2;i++)
        {
            SetCardToBePutIn();//準備卡牌
            int maxRandomNunber = cardsToBePutIn.Count;//最大亂數不超過4

            for(int j = 0; j < maxRandomNunber; maxRandomNunber--)
            {
                int randomNumber = UnityEngine.Random.Range(0, maxRandomNunber);//產生0-3的亂數         
                AddNewCard(cardsToBePutIn[randomNumber],0);//抽卡牌
                cardsToBePutIn.RemoveAt(randomNumber);//抽過的移除
                positionIndex++;
            }
        }

        

        
    }
    void AddNewCard(Card.CardPattern cardPattern,int positionIndex)
    {
        GameObject card = Instantiate(Resources.Load<GameObject>("prefabs/card"));//Resources裡面讀取card物件
        card.GetComponent<Card>().cardPattern = cardPattern;//從Card腳本中取得卡牌總類
        card.name = "card_" + cardPattern.ToString();//把card和卡牌總類轉為文字
        card.transform.position = positions[positionIndex].position;//陣列裡面編號物件的位置套用至卡牌位置座標

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
                foreach(var card in cardComparison)
                {
                    //配對成功
                    card.cardState = Card.CardState.win;
                }
                ClearCardComparsion();
                matchedCardsCount = matchedCardsCount + 2;
                if (matchedCardsCount >= positions.Length)//全部配對成功
                {
                    StartCoroutine(ReloadScrene());
                }
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
        foreach(var card in cardComparison)
        {
            //卡牌蓋回去
            card.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
            card.cardState = Card.CardState.open;
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

}
