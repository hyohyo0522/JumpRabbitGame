using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBee : MonoBehaviour
{

    // �̵����� 
    float speed = 5f;
    Vector2 newDestination;
    private SpriteRenderer BeeSpRender;
    public LayerMask BorderMask;
    public Transform beeCenter;

    public Transform MaxUpLeft;
    public Transform MaxDownRight;

    //������ ��� ����
    public LayerMask GroundMask;
    public Transform rainPoint;
    public GameObject[] items;
    bool _floatingGround;

    float itemBetTime;
    float minBetTime = 4.5f;
    float maxBetTime = 7.5f;

    // Start is called before the first frame update
    void Start()
    {
        SetDestination();
        setitemBetTime();
        BeeSpRender = GetComponent<SpriteRenderer>(); 
    }

    // Update is called once per frame
    void Update()
    {
        CheckIsFloating();
        if (_floatingGround) // �� ���� �� ����������, ������ �� ���� ������ �����ϰ� �Ѵ�. 
        {
            itemBetTime -= Time.deltaTime;
            if (itemBetTime <= 0)
            {
                StartCoroutine(ItemRains());
                setitemBetTime();

            }
        }

        if ((newDestination.x - transform.position.x > 1f) || (newDestination.y - transform.position.y > 1f))
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, newDestination, step);
            checkBorder();

        }
        else
        {
            Debug.Log("���ο� ��ǥ������ �����Ѵ�.");
            SetDestination();
        }


        BeeSpRender.flipX = (transform.position.x < newDestination.x) ? true : false;
    }



    void SetDestination()
    {

        float newXposition = Random.Range(MaxUpLeft.transform.position.x, MaxDownRight.transform.position.x);
        float newYposition = Random.Range(MaxDownRight.transform.position.y, MaxUpLeft.transform.position.y);

        Vector2 setPosition;
        setPosition.x = newXposition;
        setPosition.y = newYposition;

        newDestination = setPosition;
    }

    void checkBorder()
    {
        Collider2D[] nearBorder = Physics2D.OverlapCircleAll((Vector2)beeCenter.position, 5f, BorderMask);
        if (nearBorder.Length > 0)
        {
            Debug.Log("��ó�� ��谡 �ִ�.");
            // �� ��迡 ���� �� ������ ���ο� ��ġ�� �̵��ϰ� �Ѵ�. 
            // �� ��迡 ����� �� �������� ���ư� ��ġ�� �迭�� �޾Ƽ� ����. 
            newDestination = Vector2.zero; 
        }
    }

    //�����ð��� ���� ������ ������ �ϴ� �޼���
    // ��¥ ���� ��/ ��� ���� ������� ��
    //void ItemRains()
    //{
    //    int ItemNumber = Random.Range(7, 11);

    //    for(int n =0; n<ItemNumber; n++)
    //    {
    //        int index = Random.Range(0, items.Length);
    //        GameObject ItemDrop = Instantiate(items[index], rainPoint.position, Quaternion.identity);

    //    }
    //}

    private IEnumerator ItemRains()
    {
        int ItemNumber = Random.Range(7, 11);

        for (int n = 0; n < ItemNumber; n++)
        {
            int index = Random.Range(0, items.Length);
            float timeBetDrop = Random.Range(0.1f, 0.3f);
            GameObject ItemDrop = Instantiate(items[index], rainPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);
        }

    }

    void setitemBetTime()
    {
        itemBetTime = Random.Range(minBetTime, maxBetTime);

    }

    void CheckIsFloating() 
    { 
        //�ؿ� ���� �ִ��� �˻��ϰ�? >> �̰� ��� �ɼ��� �ִ�. 
        //RaycastHit2D hitInfo = Physics2D.Raycast(rainPoint.position, -Vector2.up, 10f, GroundMask);

        //�� �ӿ� ���� ������ �������� �߻����� �ʴ´�. Collider�� �˻��Ѵ�.
        Collider2D[] buriedInGround = Physics2D.OverlapCircleAll((Vector2)rainPoint.position, 2f, GroundMask);



        if (buriedInGround.Length == 0 ) 
        {
            
            //Debug.Log("���� �ؿ� �ִ�!!!!!!!!!!!!!!!!!!!");
            _floatingGround = true;
        }
        else
        {
            //Debug.Log("���� �ؿ� ����!");
            _floatingGround = false;
        }
    }

}
