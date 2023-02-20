using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseDoor : MonoBehaviour, ITouchedObj
{
    private GameObject clodedDoor;
    private GameObject openDoor;
    [SerializeField] float maxOpenTime = 5f;
    [SerializeField] bool isOpen;

    //화살표 관련
    private GameObject directIcon;
    public Transform originalDirectIcon;
    float iconSpeed = 1f;
    [SerializeField] float MaxTimeCycle = 1f;
    float moveTimeCycle = 1f;
    bool iconmoveUp = false;

    // 터치 관련
    [SerializeField] GameObject houseUIMsg; //UI 메시지 창
    Transform YesBtnOriginTransform; // Yes 버튼(UI) 원위치 저장
    Transform NoBtnOriginTranform;  // No 버튼(UI) 원위치 저장
    [SerializeField] bool touchOn;

    // 트리거감지를 통해 플레이어 정보를 저장할 변수
    [SerializeField] PlayerLife whoknocked;
    
    private static int neededKeyNumForHouse = 3;//집을 구매하기 위한 키 갯수
    public static int NeededKeyNumForHouse { get { return neededKeyNumForHouse; } }


    // Start is called before the first frame update
    void Start()
    {
        clodedDoor = this.transform.GetChild(0).gameObject;
        openDoor = this.transform.GetChild(1).gameObject;
        directIcon = this.transform.GetChild(2).gameObject;

        maxOpenTime = 5f;
        isOpen = false;

        openDoor.SetActive(false);
        directIcon.SetActive(false);


        if (houseUIMsg.activeSelf)
        {
            YesBtnOriginTransform = houseUIMsg.transform.GetChild(1).GetComponent<RectTransform>().transform;
            NoBtnOriginTranform = houseUIMsg.transform.GetChild(2).GetComponent<RectTransform>().transform;
            houseUIMsg.SetActive(false);
            touchOn = false;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        {
            maxOpenTime -= Time.deltaTime;
            DirectIconMove();
            if (maxOpenTime <= 0)
            {
                CloseMyDoor();
                
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.tag == "Player") // 플레이어가 문에 닿았을 때.
        {
            if (!isOpen)
            {
                OpenMyDoor();
                whoknocked = collision.gameObject.GetComponent<PlayerLife>();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player") // 플레이어가 문에 닿았을 때.
        {
            if (!isOpen)
            {
            maxOpenTime = 5f;
            }
        }
    }

    void OpenMyDoor()
    {

        clodedDoor.SetActive(false);
        openDoor.SetActive(true);
        AudioManager.instance.PlaySFX("KnockSound");
        directIcon.gameObject.SetActive(true);
        isOpen = true;
    }


    void CloseMyDoor()
    {
        if (isOpen) { isOpen = false; }
        directIcon.transform.position = originalDirectIcon.position;
        directIcon.gameObject.SetActive(false);
        openDoor.SetActive(false);
        clodedDoor.SetActive(true);

        AudioManager.instance.PlaySFX("DoorClosed");

        maxOpenTime = 5f; // 최대 오픈 시간 

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

    public void Touch()
    {
        if (!isOpen) return;

        HouseMsgSetting();
        houseUIMsg.SetActive(true);
        touchOn = true;
    }

    public void TouchExit()
    {

        if (touchOn)
        {
            houseUIMsg.SetActive(false);
        }
    }

    public void SetInputManagerWhatEtoucedObj()
    {
        InputManager.instance.SetNoweTouchedObj(eWhatTouched.houseDoor);
    }

    void HouseMsgSetting() // 여기서 메시지 수정.
    {
        Text EndingMsg = houseUIMsg.transform.GetChild(0).GetComponent<Text>(); // 엔딩 메시지
        GameObject YesBtn = houseUIMsg.transform.GetChild(1).gameObject; // Yes 버튼
        GameObject NoBtn = houseUIMsg.transform.GetChild(2).gameObject; // No 버튼(Cancle)

        //플레이어에게 충분한 열쇠가 있는지 확인해서 메시지를 수정하자.
        if (whoknocked.CheckKeyNumForHouseMsg(neededKeyNumForHouse))
        {
            EndingMsg.text = "열쇠가 충분합니다!" + "\n" + "집의 주인이 되어 게임에 승리할까요?";

            YesBtn.transform.GetChild(0).GetComponent<Text>().text = "YES";
            YesBtn.transform.position = YesBtnOriginTransform.position;
            YesBtn.SetActive(true);

            NoBtn.transform.GetChild(0).GetComponent<Text>().text = "NO";
            NoBtn.transform.position = NoBtnOriginTranform.position;
            NoBtn.SetActive(true); // 예스와 노 버튼 모두 활성화

        }
        else
        {
            EndingMsg.text = "열쇠가 부족합니다." + "\n" + "필요한 열쇠 갯수는 총 " + neededKeyNumForHouse.ToString() + "개입니다.";

            YesBtn?.SetActive(false);

            NoBtn.transform.GetChild(0).GetComponent<Text>().text = "CLOSE";
            Vector3 OnlyNoBtnPosition = NoBtnOriginTranform.position;

            //OnlyNoBtnPosition.x = 345f; // 노 버튼 위치 가운데로 조정
            OnlyNoBtnPosition.x = houseUIMsg.transform.position.x;
            NoBtn.transform.position = OnlyNoBtnPosition;
            NoBtn.SetActive(true); // 노 버튼만 활성화
        }
        
    }


}
