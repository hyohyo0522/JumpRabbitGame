using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HouseDoor : MonoBehaviour, ITouchedObj
{
    [SerializeField] GameObject clodedDoor;
    [SerializeField] GameObject openDoor;
    [SerializeField] float maxOpenTime = 5f;
    [SerializeField] bool isOpen;

    //ȭ��ǥ ����
    [SerializeField] Collider2D directIcon;
    Transform originalDirectIcon;
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
    [SerializeField] int NeededKeyNumForHouse = 10;//���� �����ϱ� ���� Ű �ѹ�

    // Start is called before the first frame update
    void Start()
    {
        clodedDoor = this.transform.GetChild(0).gameObject;
        openDoor = this.transform.GetChild(1).gameObject;
        directIcon = openDoor.transform.GetChild(0).GetComponent<Collider2D>();
        originalDirectIcon = directIcon.transform;
        maxOpenTime = 5f;
        isOpen = false;

        openDoor.SetActive(false);

        if (houseUIMsg.activeSelf)
        {
            YesBtnOriginTransform = houseUIMsg.transform.GetChild(1).GetComponent<Transform>();
            NoBtnOriginTranform = houseUIMsg.transform.GetChild(2).GetComponent<Transform>();
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
        isOpen = true;
    }


    void CloseMyDoor()
    {
        Debug.Log("������!!");
        if (isOpen) { isOpen = false; }
        directIcon.transform.position = originalDirectIcon.position;
        directIcon.gameObject.SetActive(false);
        openDoor.SetActive(false);
        clodedDoor.SetActive(true);

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
        if (whoknocked.CheckKeyNumForHouseMsg(NeededKeyNumForHouse))
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
            EndingMsg.text = "���谡 �����մϴ�." + "\n" + "�ʿ��� ���� ������ �� " + NeededKeyNumForHouse.ToString() + "���Դϴ�.";

            YesBtn?.SetActive(false);

            NoBtn.transform.GetChild(0).GetComponent<Text>().text = "CLOSE";
            Vector3 OnlyNoBtnPosition = NoBtnOriginTranform.position;
            OnlyNoBtnPosition.x = 345; // �� ��ư ��ġ ����� ����
            NoBtn.transform.position = OnlyNoBtnPosition;
            NoBtn.SetActive(true); // �� ��ư�� Ȱ��ȭ
        }
        
    }
}
