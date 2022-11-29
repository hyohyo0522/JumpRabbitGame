using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BingoPanel : MonoBehaviour
{
    public event Action OnDisableEvent;
    [SerializeField] BingoCard[] myBingoCards = new BingoCard[9];

    // Start is called before the first frame update

    private void OnEnable()
    {
        //클릭한  체스트의 빙고 정보를 가져와야 한다.
        // 그걸 빙고판UI에 적용시켜야 한다. 
    }


    public void getBingoCardsInfo(eBingoItem itemNameFromChest, int ItemNumFromChest, int index)
    {
        for(int n =0; n< myBingoCards.Length; n++)
        {
            if(index == n)
            {
                myBingoCards[n].ShowBingoCardUI(itemNameFromChest, ItemNumFromChest);
            }
        }


    }

    public void showDelegateEvent() // 
    {
        string value = OnDisableEvent.ToString();
        Debug.Log(value + "딜리게이트 이벤트가 제대로 담겼는지 확인하라.");
    }

    public void DoOnDisableEvent() // Cancle 버튼이 눌릴 때 이 메소드가 작동되게 컴포넌트에서 등록해놓을 것이다. 
    {
        if (OnDisableEvent != null)
        {
            OnDisableEvent();
            OnDisableEvent = null;

        }
    }

}
