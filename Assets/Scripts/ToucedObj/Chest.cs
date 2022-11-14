using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chest : MonoBehaviour,ITouchedObj
{


    bool isOpen; // ���ڸ� �����ߴ°�?
    bool touchOn; // ���ڸ� �÷��̾ ��������



    // ȭ��ǥ ������ ���� 
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
            if (this.GetComponent<BoxCollider2D>().IsTouchingLayers(1 << 8)) //����
            {

                openChest();
            }

            if (this.GetComponent<BoxCollider2D>().IsTouching(FindObjectOfType<PlayerMovement>().GetComponent<CapsuleCollider2D>())){
                Debug.Log("�÷��̾ ���ڿ� ��!!!");
            }

        }



        // Direct Icon �����̰� �ϴ� �ڵ� 
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
        Debug.Log("���ڰ� Ŭ���Ǿ���!!!!!! �������̽� ��ġ �۵�!!");
        //���⿡ ���� Touch �Ǿ��� ���� ������ �ִ´�.
    }



    //private void OnMouseDown()
    //{
    //    Debug.Log("���ڰ� ���ȴ�!!");
    //}
    


}
