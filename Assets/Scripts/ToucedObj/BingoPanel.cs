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
    [SerializeField] Chest theChsetTouched = null; // 클릭한 보물상자의 정보를 여기다가 저장한다. 

    //코인 버튼
    [SerializeField] GameObject[] NormalButtons = new GameObject[3];
    [SerializeField] List<GameObject> NotInteratctableNormal= new List<GameObject>(); 
    [SerializeField] GameObject ComboCoin;

    // 코인버튼 관련 값
    public static int ComboMoney;

    // 버튼 컬러값
    const string NormalBtnEnable = "#FF0000";
    const string NormalBtnNotEnable = "#4B0000";
    const string ComboBtnEnable = "#00FF2D";
    const string ComboBtnNotEnable = "#002E08";





    //체스트에서 이 함수를 호출하여 정보를 가져온다. 
    public void getBingoCardsInfo(eBingoItem itemNameFromChest, int ItemNumFromChest, bool isCompleted, int index)
    {
        for (int n = 0; n < myBingoCards.Length; n++)
        {
            if (index == n)
            {
                myBingoCards[n].ShowBingoCardUI(itemNameFromChest, ItemNumFromChest, isCompleted);
                myBingoCompletedInfo[n] = isCompleted;// 기존 완료된 빙고의 정보를 저장한 후, 새로운 클릭과 비교한다.
            }
        }

    }



    public void showDelegateEvent() // 
    {
        string value = OnDisableEvent.ToString();
    }

    public void DoOnDisableEvent() // Cancle 버튼이 눌릴 때 이 메소드가 작동되게 컴포넌트에서 등록해놓을 것이다. 
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
        // [1] 가로줄 검사 >> 칸이 하나 완성될 때, 최대 가로줄 생성갯수는 1개  
        switch (newBingoIndex/3)
        {
            case 0: //빙고 가로 첫번째줄
                if (GetNumOfCompletedBinggoCard(0, 1, 2) == 2)
                {
                    newLine++;
                    Debug.Log("빙고줄 가로 1 ");
                }
                break;
            case 1://빙고 가로 두번째줄
                if (GetNumOfCompletedBinggoCard(3, 4, 5) == 2)
                {
                    newLine++;
                    Debug.Log("빙고줄 가로 2 ");
                }
                break;
            case 2://빙고 가로 세번째줄
                if (GetNumOfCompletedBinggoCard(6, 7, 8) == 2)
                {
                    newLine++;
                    Debug.Log("빙고줄 가로 3 ");
                }
                break;
        }
        //[2] 세로줄 검사 >> 칸이 하나 완성될 때, 최대 세로줄 생성갯수는 1개
        switch ((int)(newBingoIndex % 3))
        {
            case 0: //빙고 세로 첫번째줄
                if (GetNumOfCompletedBinggoCard(0, 3, 6) == 2)
                {
                    newLine++;
                    Debug.Log("빙고줄 세로 1 ");
                }
                break;
            case 1://빙고 세로 두번째줄 /
                if (GetNumOfCompletedBinggoCard(1, 4, 7) == 2)
                {
                    newLine++;
                    Debug.Log("빙고줄 세로 2 ");
                }
                break;
            case 2://빙고 세로 세번째줄
                if (GetNumOfCompletedBinggoCard(2, 5, 8) == 2)
                {
                    newLine++;
                    Debug.Log("빙고줄 세로 3 ");
                }
                break;
        }
        //[3] 대각선 검사 >> 칸이 하나 완성될 때, 최대 대각선 생성갯수는 2개 : newBingoIndex가 4일때 유일.
        if (newBingoIndex%2 == 0) //대각선줄에 있으려면 인덱스 수가 짝수여야 한다. 
        {
            switch (newBingoIndex)
            {
                case 0:
                case 8:
                    if (GetNumOfCompletedBinggoCard(0, 4, 8) == 2)
                    {
                        newLine++;
                        Debug.Log("빙고줄 대각선 1 ");
                    }
                    break;
                case 2:
                case 6:
                    if (GetNumOfCompletedBinggoCard(0, 2, 6) == 2)
                    {
                        newLine++;
                        Debug.Log("빙고줄 대각선 2 ");
                    }
                    break;
                case 4: 
                    if(myBingoCompletedInfo[0] && myBingoCompletedInfo[8])
                    {
                        newLine++;
                        Debug.Log("빙고줄 대각선 3 ");
                    }
                    if (myBingoCompletedInfo[2] && myBingoCompletedInfo[6])
                    {
                        newLine++;
                        Debug.Log("빙고줄 대각선 4 ");
                    }
                    break;

            }
        }
        myBingoCompletedInfo[newBingoIndex] = true;

        //newLine 갯수에 따라 돈 먹는 아이콘 활성화
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
        if (newLine > 0) //나중에 여기에 조건을 추가해서 콤보돈 머니버튼 활성화되면 그에 맞는 소리가 재생되게 해도 되겠다. 
        {
            audioClip = "BingGoEnableMoney";
        }
        AudioManager.instance.PlaySFX(audioClip);

    }



    void MakeNormalBtnInteractableByRandom(int lineNum) //리스트를 이용해 아직 버튼활성화 안된 버튼을 램덤으로 활성화
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

            if (NotInteratctableNormal.Count > 0) //버튼활성화 안된 버튼이 하나라도 있는지 확인.
            {
                int a = Random.Range(0, NotInteratctableNormal.Count);
                EnableMoneyButton(NotInteratctableNormal[a]);
            }

        }

    }



    int GetNumOfCompletedBinggoCard(int a, int b, int c) // 빙고 완성된 칸 몇개인지 확인 
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

        //버튼 컬러 활성화 컬러로 수정  : 콤보코인버튼이냐, 노멀버튼이냐에 따라 색깔을 다르게 지정한다. 
        if (CoinButtonObj.name.StartsWith("C")) // 콤보
        {
            CoinButtonObj.GetComponent<Image>().color = ChangeButtonColorResult(ComboBtnEnable);

        }
        else
        {
            CoinButtonObj.GetComponent<Image>().color = ChangeButtonColorResult(NormalBtnEnable);
        }


            
        //버튼 활성화
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

    Color ButtonColor; // 컬러값 변경될 때마다 담아서 쓸 out 변수 
    private Color ChangeButtonColorResult(string hexcod)
    {

        ColorUtility.TryParseHtmlString(hexcod, out ButtonColor);

        return ButtonColor;

    }

    public void OnBinggoButtonClick(int binggoIndex)
    {
        //플레이어 캐릭터의 돈먹는 기능은 컴포넌트 OnClick에서 직접 playerLife의 함수에 접근한다.
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
