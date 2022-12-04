using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class BingoPanel : MonoBehaviour
{
    public event Action OnDisableEvent;
    [SerializeField] BingoCard[] myBingoCards = new BingoCard[9];
    [SerializeField] bool[] myBingoCompletedInfo = new bool[9];
    [SerializeField] Chest theChsetTouched = null; // Ŭ���� ���������� ������ ����ٰ� �����Ѵ�. 


    // Start is called before the first frame update

    private void OnEnable()
    {
        //Ŭ����  ü��Ʈ�� ���� ������ �����;� �Ѵ�.
        // �װ� ������UI�� ������Ѿ� �Ѵ�. 
    }


    // �ڿ���ٰ� Ŭ���ɶ����� ������ �ϼ� �����ؼ�, �������� Ȱ��ȭ. 
    private void Update()
    {
        // �� �迭�� �մ� ����ī����� Ŭ���̺�Ʈ�� �����ϴ� ����� ��������.   


    }

    //ü��Ʈ�� ������ �����´�. 
    public void getBingoCardsInfo(eBingoItem itemNameFromChest, int ItemNumFromChest, bool isCompleted, int index)
    {
        for (int n = 0; n < myBingoCards.Length; n++)
        {
            if (index == n)
            {
                myBingoCards[n].ShowBingoCardUI(itemNameFromChest, ItemNumFromChest, isCompleted);
                myBingoCompletedInfo[n] = isCompleted;// ���� �Ϸ�� ������ ������ ������ ��, ���ο� Ŭ���� ���Ѵ�.
            }
        }

    }



    public void showDelegateEvent() // 
    {
        string value = OnDisableEvent.ToString();
        Debug.Log(value + "��������Ʈ �̺�Ʈ�� ����� ������ Ȯ���϶�.");
    }

    public void DoOnDisableEvent() // Cancle ��ư�� ���� �� �� �޼ҵ尡 �۵��ǰ� ������Ʈ���� ����س��� ���̴�. 
    {
        if (OnDisableEvent != null)
        {
            OnDisableEvent();
            OnDisableEvent = null;

        }
    }

    public void GetNewClick()
    {
        for (int i = 0; i < myBingoCards.Length; i++) // ���� ���������� ���Ѵ�. 
        {
           if (myBingoCards[i].completed != myBingoCompletedInfo[i])
            {
                myBingoCompletedInfo[i] = myBingoCards[i].completed;
                TryNewLineEnable(i);
                theChsetTouched.GetCompletedinfoFromUI(i); // Ŭ���� �������� ������ ü��Ʈ�� �Ѱ��ش�.
            }

        }
    }


    public void TryNewLineEnable(int newBingoIndex)
    {
        int newLine = 0;
        // [1] ������ �˻� >> ĭ�� �ϳ� �ϼ��� ��, �ִ� ������ ���������� 1��  
        switch (newBingoIndex/3)
        {
            case 0: //���� ���� ù��°��
                if (ChecBingoCount(0, 1, 2) == 2)
                {
                    newLine++;
                }
                break;
            case 1://���� ���� �ι�°��
                if (ChecBingoCount(3, 4, 5) == 2)
                {
                    newLine++;
                }
                break;
            case 2://���� ���� ����°��
                if (ChecBingoCount(6, 7, 8) == 2)
                {
                    newLine++;
                }
                break;
        }
        //[2] ������ �˻� >> ĭ�� �ϳ� �ϼ��� ��, �ִ� ������ ���������� 1��
        switch (newBingoIndex % 2)
        {
            case 0: //���� ���� ù��°��
                if (ChecBingoCount(0, 3, 6) == 2)
                {
                    newLine++;
                }
                break;
            case 1://���� ���� �ι�°��
                if (ChecBingoCount(1, 4, 7) == 2)
                {
                    newLine++;
                }
                break;
            case 2://���� ���� ����°��
                if (ChecBingoCount(2, 5, 8) == 2)
                {
                    newLine++;
                }
                break;
        }
        //[3] �밢�� �˻� >> ĭ�� �ϳ� �ϼ��� ��, �ִ� �밢�� ���������� 2�� : newBingoIndex�� 4�϶� ����.

        if (newBingoIndex%2 == 0) //�밢���ٿ� �������� �ε��� ���� ¦������ �Ѵ�. 
        {
            switch (newBingoIndex)
            {
                case 0:
                case 8:
                    if (ChecBingoCount(0, 4, 8) == 2)
                    {
                        newLine++;
                    }
                        break;
                case 2:
                case 6:
                    if (ChecBingoCount(0, 2, 6) == 2)
                    {
                        newLine++;
                    }
                    break;
                case 4:
                    if (ChecBingoCount(0, 4, 8) == 2)
                    {
                        newLine++;
                    }
                    if (ChecBingoCount(0, 2, 6) == 2)
                    {
                        newLine++;
                    }
                    break;

            }
        }
        myBingoCompletedInfo[newBingoIndex] = true;

        //newLine ������ ���� �� �Դ� ������ Ȱ��ȭ
    }

    int ChecBingoCount(int a, int b, int c)
    {
        int completedCount = 0;
        if (myBingoCompletedInfo[a])
        {
            completedCount++;
        }

        if (myBingoCompletedInfo[b])
        {
            completedCount++;
        }

        if (myBingoCompletedInfo[c])
        {
            completedCount++;
        }

        return completedCount;
    }

    void EnableMoneyButton()
    {

    }

    public void SetChset(Chest Touched)
    {
        if(theChsetTouched != null)
        {
            theChsetTouched = null;

        }

        theChsetTouched = Touched;
    }
}
