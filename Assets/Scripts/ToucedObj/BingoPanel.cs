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
        //Ŭ����  ü��Ʈ�� ���� ������ �����;� �Ѵ�.
        // �װ� ������UI�� ������Ѿ� �Ѵ�. 
    }


    private void OnDisable()
    {
        InputManager.instance.ExitITouchedObjPanel();
        if (OnDisableEvent != null)
        {
            OnDisableEvent();
            OnDisableEvent = null;

        }


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

    public void showDelegateEvent()
    {
        string value = OnDisableEvent.ToString();
        Debug.Log(value + "��������Ʈ �̺�Ʈ�� ����� ������ Ȯ���϶�.");
    }


}
