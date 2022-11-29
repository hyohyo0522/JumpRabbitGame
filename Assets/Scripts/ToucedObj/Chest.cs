using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Chest : MonoBehaviour,ITouchedObj
{


    // ȭ��ǥ ������ ���� 
    [SerializeField] Collider2D directIcon;
    float iconSpeed = 1f;
    [SerializeField] float MaxTimeCycle = 1f;
    float moveTimeCycle = 1f;
    bool iconmoveUp = false;

    Animator chestAnimator;

    // �÷��̾ �����Ͽ� ���ڸ� ������
    bool isOpen; // ���ڸ� �����ߴ°�?
    [SerializeField] Transform playerDetectPoint;
    [SerializeField] float playerDetectRadius = 1.5f;
    [SerializeField] float openBouncePower = 2000f;

    //ü��Ʈ ���� ���� ����
    bingoCardInfo[] myBingoChest;  // ������ ���� ������ ����
    bool hasBingoInfo;// ������ ���� ������ �����Ǿ����� Ȯ��
    GameObject bingoUI;
    [SerializeField] GameObject bingGoUI; 

    public bool touchOn; // ����UI�� ���ȴ°�?



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
            if (this.GetComponent<BoxCollider2D>().IsTouchingLayers(1 << 8)) //����
            {
                Collider2D[] hasNearPlayer = Physics2D.OverlapCircleAll((Vector2)playerDetectPoint.position, playerDetectRadius, 1 << 8);
                if(hasNearPlayer != null)
                {
                    for(int n=0; n < hasNearPlayer.Length; n++)
                    {
                        PlayerMovement nearPlayer = hasNearPlayer[n].GetComponent<PlayerMovement>();
                        Debug.Log(nearPlayer.tag);
                        nearPlayer.AddForcetoBounce(Vector2.up*openBouncePower);
                        // Debug.Log("�÷��̾ ƨ���!!");
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

    public void Touch() // ���ڸ� Ŭ������ �� ��� ����
    {

        if (touchOn) 
        {
            return;
        }
        //Debug.Log("ToucH���� �Դ�.");
        if (!isOpen) return; // �÷��̾ ���ڸ� �����ؼ� ������� ������ Ŭ���� ����x 
        //Debug.Log("!isOpen�� ����ߴ�.");
        touchOn = true;
        directIcon.gameObject.SetActive(false);
       //Debug.Log("���ڰ� Ŭ���Ǿ���!!!!!! �������̽� ��ġ �۵�!!");

        //�ش� ü��Ʈ�� �����۰� �����۰����� ������ ������UI�� �ѱ��, ����UI�� ����. 

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

        if (serialziedbingGoUI != null) //���Ⱑ �۵��� �ȵǾ ������ ���� �� ����. 
        {
            serialziedbingGoUI.OnDisableEvent += () => TouchNull();

            for (int n = 0; n < myBingoChest.Length; n++) // Chest�� ������  �����ǿ� �Ѱ��ش�.
            {
                serialziedbingGoUI.getBingoCardsInfo(myBingoChest[n].whatItem, myBingoChest[n].ItemNumber, n);
            }

            touchOn = true;

        }







    }

    void TouchNull() // ���ڰ� Ŭ���� ���·� �ٸ� ���� Ŭ������ ��/ �������� �ݴ´�. 
    {
        if (touchOn)
        {
            touchOn = false;
            
        }

        if (isOpen)
        {
            closeChest();

        }
        // bingoUI.SetActive(false);  >>> UI�� X ��ư���� ó���Ѵ�.

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

        // Direct Icon �����̰� �ϴ� �ڵ� 
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

    void SetNewBingoChest() //ü��Ʈ�� ������ �ִ� ������ ����. 
    {
        myBingoChest = new bingoCardInfo[9];
        
        for(int n =0; n < myBingoChest.Length; n++) // 9���� ���ο� ����ī�带 ����.
        {
            myBingoChest[n] = new bingoCardInfo();
        }

        hasBingoInfo = true;

    }


    //���� ������ ��� ��øŬ���� 
    private class bingoCardInfo
    {
        public eBingoItem whatItem;
        public int ItemNumber;
        bool hasComplted { get;  set; } = false;
        PlayerMovement whoCliick = null;

        public bingoCardInfo()  // �����ڿ��� ���� �����۰� ���� ���Ѵ�.
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
            Debug.Log("���� ī�� �������̼��̾�!" + CardItem + ItemNumber);
        }

    }
}





