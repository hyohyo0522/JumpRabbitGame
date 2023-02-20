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

    //ȭ��ǥ ����
    private GameObject directIcon;
    public Transform originalDirectIcon;
    float iconSpeed = 1f;
    [SerializeField] float MaxTimeCycle = 1f;
    float moveTimeCycle = 1f;
    bool iconmoveUp = false;

    // ��ġ ����
    [SerializeField] GameObject houseUIMsg; //UI �޽��� â
    Transform YesBtnOriginTransform; // Yes ��ư(UI) ����ġ ����
    Transform NoBtnOriginTranform;  // No ��ư(UI) ����ġ ����
    [SerializeField] bool touchOn;

    // Ʈ���Ű����� ���� �÷��̾� ������ ������ ����
    [SerializeField] PlayerLife whoknocked;
    
    private static int neededKeyNumForHouse = 3;//���� �����ϱ� ���� Ű ����
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
        if(collision.tag == "Player") // �÷��̾ ���� ����� ��.
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
        if (collision.tag == "Player") // �÷��̾ ���� ����� ��.
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

        maxOpenTime = 5f; // �ִ� ���� �ð� 

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

    void HouseMsgSetting() // ���⼭ �޽��� ����.
    {
        Text EndingMsg = houseUIMsg.transform.GetChild(0).GetComponent<Text>(); // ���� �޽���
        GameObject YesBtn = houseUIMsg.transform.GetChild(1).gameObject; // Yes ��ư
        GameObject NoBtn = houseUIMsg.transform.GetChild(2).gameObject; // No ��ư(Cancle)

        //�÷��̾�� ����� ���谡 �ִ��� Ȯ���ؼ� �޽����� ��������.
        if (whoknocked.CheckKeyNumForHouseMsg(neededKeyNumForHouse))
        {
            EndingMsg.text = "���谡 ����մϴ�!" + "\n" + "���� ������ �Ǿ� ���ӿ� �¸��ұ��?";

            YesBtn.transform.GetChild(0).GetComponent<Text>().text = "YES";
            YesBtn.transform.position = YesBtnOriginTransform.position;
            YesBtn.SetActive(true);

            NoBtn.transform.GetChild(0).GetComponent<Text>().text = "NO";
            NoBtn.transform.position = NoBtnOriginTranform.position;
            NoBtn.SetActive(true); // ������ �� ��ư ��� Ȱ��ȭ

        }
        else
        {
            EndingMsg.text = "���谡 �����մϴ�." + "\n" + "�ʿ��� ���� ������ �� " + neededKeyNumForHouse.ToString() + "���Դϴ�.";

            YesBtn?.SetActive(false);

            NoBtn.transform.GetChild(0).GetComponent<Text>().text = "CLOSE";
            Vector3 OnlyNoBtnPosition = NoBtnOriginTranform.position;

            //OnlyNoBtnPosition.x = 345f; // �� ��ư ��ġ ����� ����
            OnlyNoBtnPosition.x = houseUIMsg.transform.position.x;
            NoBtn.transform.position = OnlyNoBtnPosition;
            NoBtn.SetActive(true); // �� ��ư�� Ȱ��ȭ
        }
        
    }


}
