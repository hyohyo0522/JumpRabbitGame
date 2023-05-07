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

    // ��ư �÷���
    const string NormalBtnEnable = "#FF0000";
    const string NormalBtnNotEnable = "#4B0000";
    const string ComboBtnEnable = "#00FF2D";
    const string ComboBtnNotEnable = "#002E08";


    Color ButtonColor; // �÷��� ����� ������ ��Ƽ� �� out ���� 




    //ü��Ʈ���� �� �Լ��� ȣ���Ͽ� ������ �����´�. 
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

    public void GetNewClick(int n)
    {
        TryNewLineEnable(n); // 
        myBingoCards[n].completed = true;
        theChsetTouched.GetCompletedinfoFromUI(n);


    }




    public void TryNewLineEnable(int newBingoIndex)
    {
        int newLine = 0;
        // [1] ������ �˻� >> ĭ�� �ϳ� �ϼ��� ��, �ִ� ������ ���������� 1��  
        switch (newBingoIndex/3)
        {
            case 0: //���� ���� ù��°��
                if (GetNumOfCompletedBinggoCard(0, 1, 2) == 2)
                {
                    newLine++;
                    Debug.Log("������ ���� 1 ");
                }
                break;
            case 1://���� ���� �ι�°��
                if (GetNumOfCompletedBinggoCard(3, 4, 5) == 2)
                {
                    newLine++;
                    Debug.Log("������ ���� 2 ");
                }
                break;
            case 2://���� ���� ����°��
                if (GetNumOfCompletedBinggoCard(6, 7, 8) == 2)
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
                if (GetNumOfCompletedBinggoCard(0, 3, 6) == 2)
                {
                    newLine++;
                    Debug.Log("������ ���� 1 ");
                }
                break;
            case 1://���� ���� �ι�°�� /
                if (GetNumOfCompletedBinggoCard(1, 4, 7) == 2)
                {
                    newLine++;
                    Debug.Log("������ ���� 2 ");
                }
                break;
            case 2://���� ���� ����°��
                if (GetNumOfCompletedBinggoCard(2, 5, 8) == 2)
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
                    if (GetNumOfCompletedBinggoCard(0, 4, 8) == 2)
                    {
                        newLine++;
                        Debug.Log("������ �밢�� 1 ");
                    }
                    break;
                case 2:
                case 6:
                    if (GetNumOfCompletedBinggoCard(0, 2, 6) == 2)
                    {
                        newLine++;
                        Debug.Log("������ �밢�� 2 ");
                    }
                    break;
                case 4: 
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
                MakeNormalBtnInteractableByRandom(2);
                break;
            case 1:
                MakeNormalBtnInteractableByRandom(1);
                break;

        }

        string audioClip = "BingoEnableClick";
        if (newLine > 0) //���߿� ���⿡ ������ �߰��ؼ� �޺��� �ӴϹ�ư Ȱ��ȭ�Ǹ� �׿� �´� �Ҹ��� ����ǰ� �ص� �ǰڴ�. 
        {
            audioClip = "BingGoEnableMoney";
        }
        AudioManager.instance.PlaySFX(audioClip);

    }



    void MakeNormalBtnInteractableByRandom(int lineNum) //����Ʈ�� �̿��� ���� ��ưȰ��ȭ �ȵ� ��ư�� �������� Ȱ��ȭ
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



    int GetNumOfCompletedBinggoCard(int a, int b, int c) // ���� �ϼ��� ĭ ����� Ȯ�� 
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

        //��ư �÷� Ȱ��ȭ �÷��� ����  : �޺����ι�ư�̳�, ��ֹ�ư�̳Ŀ� ���� ������ �ٸ��� �����Ѵ�. 
        if (CoinButtonObj.name.StartsWith("C")) // �޺�
        {
            CoinButtonObj.GetComponent<Image>().color = ChangeButtonColorResult(ComboBtnEnable);

        }
        else
        {
            CoinButtonObj.GetComponent<Image>().color = ChangeButtonColorResult(NormalBtnEnable);
        }


            
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


    private Color ChangeButtonColorResult(string hexcod)
    {

        ColorUtility.TryParseHtmlString(hexcod, out ButtonColor);

        return ButtonColor;

    }

    public void OnBinggoButtonClick(int binggoIndex)
    {
        //�÷��̾� ĳ������ ���Դ� ����� ������Ʈ OnClick���� ���� playerLife�� �Լ��� �����Ѵ�.
        if(binggoIndex == 3)
        {
            ComboCoin.gameObject.GetComponent<Button>().interactable = false;
            ComboCoin.GetComponent<Image>().color = ChangeButtonColorResult(ComboBtnNotEnable);


            return;
        }

        NormalButtons[binggoIndex].gameObject.GetComponent<Button>().interactable = false;
        NormalButtons[binggoIndex].GetComponent<Image>().color = ChangeButtonColorResult(NormalBtnNotEnable);
    }
}
