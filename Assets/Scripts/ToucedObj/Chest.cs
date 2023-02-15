using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;


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
    [SerializeField] float maxOpenTime = 5f;
    float realOpenTime = 0;
    [SerializeField] Transform playerDetectPoint;
    [SerializeField] float playerDetectRadius = 1.5f;
    [SerializeField] float openBouncePower = 2000f;

    //체스트 빙고 관련 정보
    bingoCardInfo[] myBingoChest = new bingoCardInfo[9];  // 빙고판 관련 아이템 정보
    bool hasBingoInfo;// 빙고판 관련 정보가 생성되었는지 확인
    [SerializeField] GameObject bingGoUI;


    public bool touchOn; // 빙고UI가 열렸는가?

    //빙고 다 완료될 때 필요한 변수,오브젝트들
    [SerializeField] int completedBingoNum = 0;
    public GameObject pung; // 죽을 때 펑 애니메이션 재생
    float pungAniPlayTime = 0.65f;
    string colorCodeUnEnable = "#477848";
    string colorCodeEnable = "#FFFFFF";
    private BoxCollider2D thisCollider;
    [SerializeField] float reSetTime;
    float maxResetTime = 20f;
    float minResetTime = 7f;





    private void OnEnable()
    {

        if (bingGoUI.activeSelf)
        {
            bingGoUI.SetActive(false);
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
        thisCollider = GetComponent<BoxCollider2D>();

        if (!hasBingoInfo)
        {
            SetNewBingoChest();

        }


    }

    // Update is called once per frame
    void Update()
    {

        if (!isOpen)
        {
            if (completedBingoNum == 9) // 빙고가 닫히면서, 빙고판이 다 완성 되었다면 일정시간이 지난 후, 새로운 빙고판을 만든다.
            {
                reSetTime = Random.Range(minResetTime, maxResetTime);

                // 효과 재생 : 펑 애니메이션
                GameObject pungPlay = Instantiate(pung, this.transform.position, Quaternion.identity);
                Destroy(pungPlay.gameObject, pungAniPlayTime);


                //빙고판을 비활성화시키는 기능.
                thisCollider.enabled = false;
                Color whenUnenableColor;
                ColorUtility.TryParseHtmlString(colorCodeUnEnable, out whenUnenableColor);
                this.GetComponent<SpriteRenderer>().color = whenUnenableColor;
                completedBingoNum = 0;
                return;

            }


            if (reSetTime > 0)
            {
                reSetTime -= Time.deltaTime;
                if (reSetTime <= 0)
                {
                    SetNewBingoChest();
                }
                return; // 리턴을 넣어서 빙고가 비활성화상태일 경우, 플레이어를 튕기는 기능까지 접근하지 않도록 한다. 
            }


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

            if (touchOn) return; // 상자가 클릭해서 오픈된 상태이면 뒤의 내용 실행x 

            realOpenTime += Time.deltaTime; // 최대 오픈 시간이 지나면 자동으로 잠기게 한다. 
            if (realOpenTime >= maxOpenTime)
            {
                closeChest();
            }

        }





    }


    void openChest()
    {
        isOpen = true;
        AudioManager.instance.PlaySFX("OpenChest");
        chestAnimator.SetBool("open", true);
    }

    void closeChest()
    {
        isOpen = false;
        chestAnimator.SetBool("open", false);
        directIcon.gameObject.SetActive(false);
        realOpenTime = 0f;
    }

    public void Touch() // 상자를 클릭했을 때 기능 정리
    {

        if (touchOn) 
        {
            return;
        }
        if (!isOpen) return; // 플레이어가 상자를 점프해서 열어놓지 않으면 클릭이 되지x 

        touchOn = true;
        directIcon.gameObject.SetActive(false);


        bingGoUI.SetActive(true);
        BingoPanel serialziedbingGoUI = bingGoUI.GetComponentInChildren<BingoPanel>();
        AudioManager.instance.PlayBGM("InGame_BINGO");

        if (serialziedbingGoUI != null) //여기가 작동이 안되어서 오류가 났던 것 같다. 
        {

            serialziedbingGoUI.OnDisableEvent += () => TouchNull();
            serialziedbingGoUI.OnDisableEvent += () => InputManager.instance.ExitITouchedObjPanel();
            serialziedbingGoUI.SetChset(this);

            for (int n = 0; n < myBingoChest.Length; n++) // Chest의 정보를  빙고판에 넘겨준다.
            {
                serialziedbingGoUI.getBingoCardsInfo(myBingoChest[n].whatItem, 
                    myBingoChest[n].ItemNumber, myBingoChest[n].isComplted, n); // 여기다가 Completed 정보를 집어넣자!! 
            }

            touchOn = true;

        }


    }

    void TouchNull() // 상자가 클릭된 상태로 다른 것을 클릭했을 때/ 빙고판을 닫는다. 
    {
        AudioManager.instance.PlayBGM("InGame_jumping");
        if (touchOn)
        {
            touchOn = false;
            
        }

        if (isOpen)
        {
            closeChest();

        }

    }

    
    
    public void SetInputManagerWhatEtoucedObj()
    {
        InputManager.instance.SetNoweTouchedObj(eWhatTouched.chest);

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


        Array.Clear(myBingoChest,0,9); // 빙고판 클리어 
        
        for(int n =0; n < myBingoChest.Length; n++) // 9개의 새로운 빙고카드를 생성.
        {
            myBingoChest[n] = new bingoCardInfo();
        }

        hasBingoInfo = true;

        if (thisCollider.enabled == false)
        {
            thisCollider.enabled = true; //클릭 가능한 상태로 만들기
            Color whenEnableColor;
            ColorUtility.TryParseHtmlString(colorCodeEnable, out whenEnableColor);
            this.GetComponent<SpriteRenderer>().color = whenEnableColor;   // 샐깔 원래 상태로 되돌리기
        }


    }


    //빙고 정보를 담는 중첩클래스 
    private class bingoCardInfo
    {
        public eBingoItem whatItem;
        public int ItemNumber;
        public bool isComplted  = false;
        PlayerMovement whoCliick = null;

        public bingoCardInfo()  // 생성자에서 빙고 아이템과 수를 정한다.
        {

            int eBingoIndex = Random.Range(0, 5);
            whatItem = (eBingoItem)eBingoIndex;

            int itemValueIndex = Random.RandomRange(1, 3);
            switch (whatItem)
            {
                case eBingoItem.carrot:
                    itemValueIndex *= 4;
                    break;
                case eBingoItem.flower:
                    itemValueIndex *= 2;
                    break;
                case eBingoItem.slug:
                    itemValueIndex *= 2;
                    break;
                case eBingoItem.star:
                    itemValueIndex *= 1;
                    break;
                case eBingoItem.player:
                    itemValueIndex *= 3;
                    break;
            }

            ItemNumber = itemValueIndex;
        }


        public void SetCompleted() // 나중에 여기에 빙고판 클릭한 플레이어 정보 집어넣기
        {
            isComplted = true;
        }

        public void getBingoInfo()
        {
            string CardItem = whatItem.ToString();
            Debug.Log("빙고 카드 인포메이션이야!" + CardItem + ItemNumber);
        }

    }

    public void GetCompletedinfoFromUI(int N)
    {
        completedBingoNum++;
        myBingoChest[N].SetCompleted();


    }



}





