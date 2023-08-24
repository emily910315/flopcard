using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CardState cardState;//卡牌狀態
    public CardPattern cardPattern;//卡牌總類
    public GameManager gameManager;

    void Start()
    {
        cardState = CardState.colse;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();//找GameManager標籤的物件連接至GameManager內



    }
    public enum CardState
    {   
        //卡牌狀態
        open,colse,win
    }

    private void OnMouseUp()//滑鼠點擊時的狀態
    {
        if (cardState.Equals(CardState.open))//如果已翻開的不可再翻
        {
            return;
        }
        if (gameManager.ReadyToCompareCards)//如果兩張卡牌已打開就不能再開其他卡牌
        {
            return;
        }
        OpenCard();
        gameManager.AddCardInCardComparison(this);//翻開的卡牌加入比對清單
        gameManager.CompareCardsInList();

    }
    void OpenCard()
    {
        transform.eulerAngles = new Vector3(0, 180, 0);//讓卡牌翻面(旋轉180度)
        cardState = CardState.open;
    }

    public enum CardPattern
    {
        none,apple,grape,strawberry,watermelon

    }

}
