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
    bool touchOn; // ����UI�� ���ȴ°�?


    private void OnEnable()
    {
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
        bingoUI = GameObject.FindGameObjectWithTag("BINGOCHEST");
        bingoUI.SetActive(false);


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

    public void Touch() // ���ڸ� Ŭ������ �� ��� ����
    {
        if (touchOn) return;
        if (!isOpen) return; // �÷��̾ ���ڸ� �����ؼ� ������� ������ Ŭ���� ����x 
        touchOn = true;
        directIcon.gameObject.SetActive(false);
        Debug.Log("���ڰ� Ŭ���Ǿ���!!!!!! �������̽� ��ġ �۵�!!");
        bingoUI.SetActive(true);
        touchOn = true;
        //���⿡ ���� Touch �Ǿ��� ���� ������ �ִ´�.
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

    void SetNewBingoChest()
    {
        myBingoChest = new bingoCardInfo[9];
        
        for(int n =0; n < myBingoChest.Length; n++) // 9���� ���ο� ����ī�带 ����.
        {
            myBingoChest[n] = new bingoCardInfo();
        }

        hasBingoInfo = true;

    }

    //���ڿ� ��� ������ �� ���� ���� ������ �迭�� �����Ѵ�.


    //private void OnMouseDown()
    //{
    //    Debug.Log("���ڰ� ���ȴ�!!");
    //}
    



    //���� ������ ��� ��øŬ���� 
    private class bingoCardInfo
    {
        eBingoItem whatItem;
        int ItemNumber;
        bool hasComplted = false;
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

        public void getBingoInfo()
        {
            string CardItem = whatItem.ToString();
            Debug.Log("���� ī�� �������̼��̾�!" + CardItem + ItemNumber);
        }

    }
}





