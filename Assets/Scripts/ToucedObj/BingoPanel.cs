using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;


public class BingoPanel : MonoBehaviour
{
    public event Action OnDisableEvent;
    [SerializeField] BingoCard[] myBingoCards = new BingoCard[9];
    [SerializeField] bool[] myBingoCompletedInfo = new bool[9];
    [SerializeField] Chest theChsetTouched = null; // Ŭ���� ���������� ������ ����ٰ� �����Ѵ�. 

    //���� ��ư
    [SerializeField] GameObject[] NormalButtons = new GameObject[3];
    [SerializeField] List<GameObject> NotInteratctableNormal= new List<GameObject>(); 
    [SerializeField] GameObject ComboCoin;

    // ���ι�ư ���� ��
    public static int ComboMoney;
    Color ButtonColor;

    
    [SerializeField] Color UnEnableforNormal;
    [SerializeField] Color EnableforNormal;
    [SerializeField] Color UnEnableforCombo;
    [SerializeField] Color EnableforCombo;


    //�÷��̾� ���� ��������
    PlayerLife Myplayer;



    // Start is called before the first frame update

    private void OnEnable()
    {



    }


    private void Update()
    {
       


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
    }

    public void DoOnDisableEvent() // Cancle ��ư�� ���� �� �� �޼ҵ尡 �۵��ǰ� ������Ʈ���� ����س��� ���̴�. 
    {
        if (OnDisableEvent != null)
        {
            OnDisableEvent();
            OnDisableEvent = null;

        }
    }

    public void GetNewClick(int n )
    {
        TryNewLineEnable(n); // 
        myBingoCards[n].completed = true;
        theChsetTouched.GetCompletedinfoFromUI(n);


        //for (int i = 0; i < myBingoCards.Length; i++) // ���� ���������� ���Ѵ�. 
        //{
        //   if (myBingoCards[n].completed != myBingoCompletedInfo[n])
        //    {
        //        Debug.Log("���� Ŭ�� �ѹ� : " + i);

        //        TryNewLineEnable(i);
        //        myBingoCompletedInfo[i] = myBingoCards[i].completed;
        //        theChsetTouched.GetCompletedinfoFromUI(i); // Ŭ���� �������� ������ ü��Ʈ�� �Ѱ��ش�.

        //    }

        //}
    }

   


    public void TryNewLineEnable(int newBingoIndex)
    {
        int newLine = 0;
        // [1] ������ �˻� >> ĭ�� �ϳ� �ϼ��� ��, �ִ� ������ ���������� 1��  
        switch (newBingoIndex/3)
        {
            case 0: //���� ���� ù��°��
                if (ChecKBingoCount(0, 1, 2) == 2)
                {
                    newLine++;
                    Debug.Log("������ ���� 1 ");
                }
                break;
            case 1://���� ���� �ι�°��
                if (ChecKBingoCount(3, 4, 5) == 2)
                {
                    newLine++;
                    Debug.Log("������ ���� 2 ");
                }
                break;
            case 2://���� ���� ����°��
                if (ChecKBingoCount(6, 7, 8) == 2)
                {
                    newLine++;
                    Debug.Log("������ ���� 3 ");
                }
                break;
        }
        //[2] ������ �˻� >> ĭ�� �ϳ� �ϼ��� ��, �ִ� ������ ���������� 1��
        switch ((int)(newBingoIndex % 3))
        {
            case 0: //���� ���� ù��°��
                if (ChecKBingoCount(0, 3, 6) == 2)
                {
                    newLine++;
                    Debug.Log("������ ���� 1 ");
                }
                break;
            case 1://���� ���� �ι�°�� /
                if (ChecKBingoCount(1, 4, 7) == 2)
                {
                    newLine++;
                    Debug.Log("������ ���� 2 ");
                }
                break;
            case 2://���� ���� ����°��
                if (ChecKBingoCount(2, 5, 8) == 2)
                {
                    newLine++;
                    Debug.Log("������ ���� 3 ");
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
                    if (ChecKBingoCount(0, 4, 8) == 2)
                    {
                        newLine++;
                        Debug.Log("������ �밢�� 1 ");
                    }
                        break;
                case 2:
                case 6:
                    if (ChecKBingoCount(0, 2, 6) == 2)
                    {
                        newLine++;
                        Debug.Log("������ �밢�� 2 ");
                    }
                    break;
                case 4: //���⸦ �պ������ .. 2���� ���� �� �ֵ��� �ؾ��� 
                    if(myBingoCompletedInfo[0] && myBingoCompletedInfo[8])
                    {
                        newLine++;
                        Debug.Log("������ �밢�� 3 ");
                    }

                    if (myBingoCompletedInfo[2] && myBingoCompletedInfo[6])
                    {
                        newLine++;
                        Debug.Log("������ �밢�� 4 ");
                    }
                    //if (ChecKBingoCount(0, 4, 8) == 2)
                    //{
                    //    newLine++;

                    //}
                    //if (ChecKBingoCount(2, 4, 6) == 2)
                    //{
                    //    newLine++;
                    //    Debug.Log("������ �밢�� 4 ");
                    //}
                    break;

            }
        }
        myBingoCompletedInfo[newBingoIndex] = true;

        //newLine ������ ���� �� �Դ� ������ Ȱ��ȭ
        switch (newLine)
        {
            case 4:
                ComboMoney = 1000;
                ComboCoin.transform.GetChild(0).GetComponent<Text>().text = "Combo" + "\n" + "\n" + "\n" + "\n" + ComboMoney.ToString();
                EnableMoneyButton(ComboCoin);
                for (int i = 0; i < NormalButtons.Length; i++)
                {
                    if (NormalButtons[i].GetComponent<Button>().interactable == false)
                    {
                        EnableMoneyButton(NormalButtons[i]);
                    }

                }
                break;
            case 3:
                ComboMoney = 500;
                ComboCoin.transform.GetChild(0).GetComponent<Text>().text = "Combo" + "\n" + "\n" + "\n" + "\n" + ComboMoney.ToString();
                EnableMoneyButton(ComboCoin);
 
                for (int i = 0; i < NormalButtons.Length; i++)
                {
                    if (NormalButtons[i].GetComponent<Button>().interactable == false)
                    {
                        EnableMoneyButton(NormalButtons[i]);
                    }

                }
                break;
            case 2:
                ComboMoney = 300;
                ComboCoin.transform.GetChild(0).GetComponent<Text>().text = "Combo" + "\n" + "\n" + "\n" + "\n" + ComboMoney.ToString();
                EnableMoneyButton(ComboCoin);
                MakeNewListOfNotInteratctableNormal(2);
                break;
            case 1:
                MakeNewListOfNotInteratctableNormal(1);
                break;

        }



    }

    void MakeNewListOfNotInteratctableNormal(int lineNum) //����Ʈ�� �̿��� ���� ��ưȰ��ȭ �ȵ� ��ư�� �������� Ȱ��ȭ
    {
        for (int n = 0; n < lineNum; n++)
        {
            NotInteratctableNormal?.Clear();
            for (int i = 0; i < NormalButtons.Length; i++)
            {
                if (NormalButtons[i].gameObject.GetComponent<Button>().interactable == false)
                {
                    NotInteratctableNormal.Add(NormalButtons[i]);
                }
            }

            if (NotInteratctableNormal.Count > 0) //��ưȰ��ȭ �ȵ� ��ư�� �ϳ��� �ִ��� Ȯ��.
            {
                int a = Random.Range(0, NotInteratctableNormal.Count);
                EnableMoneyButton(NotInteratctableNormal[a]);
            }

        }

    }



    int ChecKBingoCount(int a, int b, int c)
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

    void EnableMoneyButton(GameObject CoinButtonObj)
    {
        //��ư �÷� Ȱ��ȭ �÷��� ����
        if (CoinButtonObj.name.StartsWith("C")) // �޺�
        {
            ChangeButtonColor("#00FF2D"); // (!) ����Ƽ �÷��ȷ�Ʈ���� ���� hexadecimal���� #�� �ٿ��� �Ѵ�.
        }
        else
        {
            ChangeButtonColor("#FF0000"); // ���
        }
        //�޺����ι�ư�̳�, ��ֹ�ư�̳Ŀ� ���� ������ �ٸ��� �����Ѵ�. 

        Debug.Log(ButtonColor);
        CoinButtonObj.GetComponent<Image>().color = ButtonColor;

            
        //��ư Ȱ��ȭ
        Button myButton = CoinButtonObj.GetComponent<Button>();
        myButton.interactable = true;


    }

    public void SetChset(Chest Touched)
    {
        if(theChsetTouched != null)
        {
            theChsetTouched = null;

        }

        theChsetTouched = Touched;
    }

    void ChangeButtonColor(string hexcod)
    {
        
        ColorUtility.TryParseHtmlString(hexcod, out ButtonColor);

    }

    public void OnBinggoButtonClick(int binggoIndex)
    {
        //�÷��̾� ĳ������ ���Դ� ����� ������Ʈ OnClick���� ���� playerLife�� �Լ��� �����Ѵ�.
        if(binggoIndex == 3)
        {
            ComboCoin.gameObject.GetComponent<Button>().interactable = false;
            ChangeButtonColor("#002E08");
            ComboCoin.GetComponent<Image>().color = ButtonColor;


            return;
        }

        NormalButtons[binggoIndex].gameObject.GetComponent<Button>().interactable = false;
        ChangeButtonColor("#4B0000");
        NormalButtons[binggoIndex].GetComponent<Image>().color = ButtonColor;
    }
}
