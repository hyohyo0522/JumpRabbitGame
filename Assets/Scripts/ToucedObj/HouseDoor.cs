using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] bool touchOn;

    // Start is called before the first frame update
    void Start()
    {
        clodedDoor = this.transform.GetChild(0).gameObject;
        openDoor = this.transform.GetChild(1).gameObject;
        directIcon = openDoor.transform.GetChild(0).GetComponent<Collider2D>();
        originalDirectIcon = directIcon.transform;

        openDoor.SetActive(false);

        if (houseUIMsg.activeSelf)
        {
            houseUIMsg.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        {
            maxOpenTime -= Time.deltaTime;
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
            }
        }
    }

    void OpenMyDoor()
    {

        clodedDoor.SetActive(false);
        openDoor.SetActive(true);
        DirectIconMove();
        isOpen = true;
    }


    void CloseMyDoor()
    {
        if (isOpen) { isOpen = false; }
        directIcon.transform.position = originalDirectIcon.position;
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

        houseUIMsg.SetActive(true);
        touchOn = true;
    }

    public void TouchExit()
    {
        CloseMyDoor();
        InputManager.instance.ExitITouchedObjPanel();
    }

    public void SetInputManagerWhatEtoucedObj()
    {
        InputManager.instance.SetNoweTouchedObj(eWhatTouched.houseDoor);
    }
}
