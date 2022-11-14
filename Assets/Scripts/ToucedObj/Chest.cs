using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chest : MonoBehaviour,ITouchedObj
{


    bool isOpen; // 상자를 오픈했는가?
    bool touchOn; // 상자를 플레이어가 눌렀느냐



    // 화살표 아이콘 관련 
    [SerializeField] Collider2D directIcon;
    float iconSpeed = 1f;
    [SerializeField] float MaxTimeCycle = 1f;
    float moveTimeCycle = 1f;
    bool iconmoveUp = false;

    Animator chestAnimator;





    // Start is called before the first frame update
    void Start()
    {
        //iscamera = Camera.main;
        touchOn = false;
        isOpen = false;
        chestAnimator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!isOpen)
        {
            if (this.GetComponent<BoxCollider2D>().IsTouchingLayers(1 << 8)) //상자
            {

                openChest();
            }

            if (this.GetComponent<BoxCollider2D>().IsTouching(FindObjectOfType<PlayerMovement>().GetComponent<CapsuleCollider2D>())){
                Debug.Log("플레이어가 상자에 뻥!!!");
            }

        }



        // Direct Icon 움직이게 하는 코드 
        if (iconmoveUp)
        {
            moveTimeCycle -= Time.deltaTime;
            directIcon.transform.Translate(Vector3.up * iconSpeed * Time.deltaTime);
            if (moveTimeCycle<=0)
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


    void openChest()
    {
        isOpen = true;
        chestAnimator.SetBool("open", true);
    }

    public void Touch()
    {
        touchOn = true;
        Debug.Log("상자가 클릭되었다!!!!!! 인터페이스 터치 작동!!");
        //여기에 상자 Touch 되었을 때의 동작을 넣는다.
    }



    //private void OnMouseDown()
    //{
    //    Debug.Log("상자가 눌렸다!!");
    //}
    


}
