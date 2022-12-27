using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseDoor : MonoBehaviour
{
    [SerializeField] GameObject clodedDoor;
    [SerializeField] GameObject openDoor;
    [SerializeField] float maxOpenTime = 5f;
    [SerializeField] bool isOpen;

    //화살표 관련
    [SerializeField] Collider2D directIcon;
    Transform originalDirectIcon;
    float iconSpeed = 1f;
    [SerializeField] float MaxTimeCycle = 1f;
    float moveTimeCycle = 1f;
    bool iconmoveUp = false;

    // Start is called before the first frame update
    void Start()
    {
        clodedDoor = this.transform.GetChild(0).gameObject;
        openDoor = this.transform.GetChild(1).gameObject;
        directIcon = openDoor.transform.GetChild(0).GetComponent<Collider2D>();
        originalDirectIcon = directIcon.transform;

        openDoor.SetActive(false);
        
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
        if(collision.tag == "Player") // 플레이어가 문에 닿았을 때.
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
}
