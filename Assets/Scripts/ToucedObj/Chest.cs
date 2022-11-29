using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Chest : MonoBehaviour,ITouchedObj
{


    // 화살표 아이콘 관련 
    [SerializeField] Collider2D directIcon;
    float iconSpeed = 1f;
    [SerializeField] float MaxTimeCycle = 1f;
    float moveTimeCycle = 1f;
    bool iconmoveUp = false;

    Animator chestAnimator;

    // 플레이어가 점프하여 상자를 오픈함
    bool isOpen; // 상자를 오픈했는가?
    [SerializeField] Transform playerDetectPoint;
    [SerializeField] float playerDetectRadius = 1.5f;
    [SerializeField] float openBouncePower = 2000f;

    //체스트 빙고 관련 정보
    bingoCardInfo[] myBingoChest;  // 빙고판 관련 아이템 정보
    bool hasBingoInfo;// 빙고판 관련 정보가 생성되었는지 확인
    GameObject bingoUI;
    [SerializeField] GameObject bingGoUI; 

    public bool touchOn; // 빙고UI가 열렸는가?



    private void OnEnable()
    {
        bingoUI = GameObject.FindGameObjectWithTag("BINGOCHEST");
        if (bingoUI.activeSelf)
        {
            bingoUI.SetActive(false);
        }

        if (!hasBingoInfo)
        {
            SetNewBingoChest();

        }

    }

    // Start is called before the first frame update
    void Start()
    {
        //iscamera = Camera.main;
        touchOn = false;
        isOpen = false;
        chestAnimator = GetComponent<Animator>();
        directIcon.gameObject.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {

        if (!isOpen)
        {
            if (this.GetComponent<BoxCollider2D>().IsTouchingLayers(1 << 8)) //상자
            {
                Collider2D[] hasNearPlayer = Physics2D.OverlapCircleAll((Vector2)playerDetectPoint.position, playerDetectRadius, 1 << 8);
                if(hasNearPlayer != null)
                {
                    for(int n=0; n < hasNearPlayer.Length; n++)
                    {
                        PlayerMovement nearPlayer = hasNearPlayer[n].GetComponent<PlayerMovement>();
                        Debug.Log(nearPlayer.tag);
                        nearPlayer.AddForcetoBounce(Vector2.up*openBouncePower);
                        // Debug.Log("플레이어를 튕겼다!!");
                    }
                }
                openChest();
            }

        }


        if (isOpen)
        {
            DirectIconMove();

        }





    }


    void openChest()
    {
        isOpen = true;
        chestAnimator.SetBool("open", true);
    }

    void closeChest()
    {
        isOpen = false;
        chestAnimator.SetBool("open", false);
        directIcon.gameObject.SetActive(false);
    }

    public void Touch() // 상자를 클릭했을 때 기능 정리
    {

        if (touchOn) 
        {
            return;
        }
        //Debug.Log("ToucH까지 왔다.");
        if (!isOpen) return; // 플레이어가 상자를 점프해서 열어놓지 않으면 클릭이 되지x 
        //Debug.Log("!isOpen도 통과했다.");
        touchOn = true;
        directIcon.gameObject.SetActive(false);
       //Debug.Log("상자가 클릭되었다!!!!!! 인터페이스 터치 작동!!");

        //해당 체스트의 아이템과 아이템갯수의 정보를 빙고판UI에 넘기고, 빙고UI를 연다. 

        { 
            //bingoUI.SetActive(true);
            //BingoPanel bingoUISc = bingoUI.GetComponentInChildren<BingoPanel>();

            //if (bingoUISc != null)
            //{
            //    bingoUISc.OnDisableEvent += () => TouchNull();

            //    for (int n = 0; n < myBingoChest.Length; n++)
            //    {
            //        bingoUISc.getBingoCardsInfo(myBingoChest[n].whatItem, myBingoChest[n].ItemNumber, n);
            //    }

            //    touchOn = true;

            //}
        }


        bingGoUI.SetActive(true);
        BingoPanel serialziedbingGoUI = bingGoUI.GetComponentInChildren<BingoPanel>();

        if (serialziedbingGoUI != null) //여기가 작동이 안되어서 오류가 났던 것 같다. 
        {
            serialziedbingGoUI.OnDisableEvent += () => TouchNull();

            for (int n = 0; n < myBingoChest.Length; n++) // Chest의 정보를  빙고판에 넘겨준다.
            {
                serialziedbingGoUI.getBingoCardsInfo(myBingoChest[n].whatItem, myBingoChest[n].ItemNumber, n);
            }

            touchOn = true;

        }







    }

    void TouchNull() // 상자가 클릭된 상태로 다른 것을 클릭했을 때/ 빙고판을 닫는다. 
    {
        if (touchOn)
        {
            touchOn = false;
            
        }

        if (isOpen)
        {
            closeChest();

        }
        // bingoUI.SetActive(false);  >>> UI의 X 버튼에서 처리한다.

    }

    
    
    public eWhatTouched Return_WhatTouched()
    {
        return eWhatTouched.chest;
    }

    void DirectIconMove()
    {

        if (!directIcon.gameObject.activeSelf)
        {
            directIcon.gameObject.SetActive(true);
            
        }

        // Direct Icon 움직이게 하는 코드 
        if (iconmoveUp)
        {
            moveTimeCycle -= Time.deltaTime;
            directIcon.transform.Translate(Vector3.up * iconSpeed * Time.deltaTime);
            if (moveTimeCycle <= 0)
            {
                iconmoveUp = false;
                moveTimeCycle = MaxTimeCycle;
            }
        }
        else
        {
            moveTimeCycle -= Time.deltaTime;
            directIcon.transform.Translate(Vector3.down * iconSpeed * Time.deltaTime);
            if (moveTimeCycle <= 0)
            {
                iconmoveUp = true;
                moveTimeCycle = MaxTimeCycle;
            }
        }
    }

    void SetNewBingoChest() //체스트가 가지고 있는 빙고판 정보. 
    {
        myBingoChest = new bingoCardInfo[9];
        
        for(int n =0; n < myBingoChest.Length; n++) // 9개의 새로운 빙고카드를 생성.
        {
            myBingoChest[n] = new bingoCardInfo();
        }

        hasBingoInfo = true;

    }


    //빙고 정보를 담는 중첩클래스 
    private class bingoCardInfo
    {
        public eBingoItem whatItem;
        public int ItemNumber;
        bool hasComplted { get;  set; } = false;
        PlayerMovement whoCliick = null;

        public bingoCardInfo()  // 생성자에서 빙고 아이템과 수를 정한다.
        {

            int eBingoIndex = Random.Range(0, 5);
            whatItem = (eBingoItem)eBingoIndex;

            int itemValueIndex = Random.RandomRange(1, 6);
            switch (whatItem)
            {
                case eBingoItem.carrot:
                    itemValueIndex *= 5;
                    break;
                case eBingoItem.flower:
                    itemValueIndex *= 3;
                    break;
                case eBingoItem.slug:
                    itemValueIndex *= 2;
                    break;
                case eBingoItem.star:
                    itemValueIndex *= 2;
                    break;
                case eBingoItem.player:
                    break;
            }

            ItemNumber = itemValueIndex;
        }


        void SaveCompleteInfo()
        {

        }

        public void getBingoInfo()
        {
            string CardItem = whatItem.ToString();
            Debug.Log("빙고 카드 인포메이션이야!" + CardItem + ItemNumber);
        }

    }
}





