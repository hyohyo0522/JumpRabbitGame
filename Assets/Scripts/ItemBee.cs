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
    const float boarderCheckRadius = 5f;
    public LayerMask playerMask;
    public Transform beeCenter;

    public Transform MaxUpLeft;
    public Transform MaxDownRight;

    //������ ��� ����
    public LayerMask GroundMask;
    const float groundCheckRadius = 5f;
    public Transform rainPoint;
    public GameObject[] items;

    //������ ��� �� ����ȿ�� �ֱ� ���ؼ� 3���� timeBetItemRainDrop�� ���� (ĳ��)
    readonly WaitForSeconds[] timeBetItemRainDrop = { new WaitForSeconds(0.3f), new WaitForSeconds(0.4f), new WaitForSeconds(0.5f) };

    float itemBetTime;
    const float minBetTime = 4.5f;
    const float maxBetTime = 7.5f;

    // Start is called before the first frame update
    void Start()
    {

        //playerMask = LayerMask.NameToLayer("Player");
        SetDestination();
        setitemBetTime();
        BeeSpRender = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {


        if (CheckIsFloating()) // �� ���� �� ����������, ������ �� ���� ������ �����ϰ� �Ѵ�. 
        {
            itemBetTime -= Time.deltaTime;

            if (itemBetTime <= 0)
            {
                StartCoroutine(ItemRains());
                setitemBetTime();

            }
        }
        else
        {
        }




        if ((newDestination.x - transform.position.x > 1f) || (newDestination.y - transform.position.y > 1f))
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, newDestination, step);
            checkBorder();

        }
        else
        {
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
        Collider2D[] nearBorder = Physics2D.OverlapCircleAll((Vector2)beeCenter.position, boarderCheckRadius, BorderMask);
        if (nearBorder.Length > 0)
        {
            // Debug.Log("��ó�� ��谡 �ִ�.");
            // �� ��迡 ���� �� ������ ���ο� ��ġ�� �̵��ϰ� �Ѵ�. 
            // �� ��迡 ����� �� �������� ���ư� ��ġ�� �迭�� �޾Ƽ� ����. 
            newDestination = Vector2.zero; 
        }
    }

    private IEnumerator ItemRains()
    {
        int ItemNumber = Random.Range(7, 11);

        for (int n = 0; n < ItemNumber; n++)
        {
            int index = Random.Range(0, items.Length);

            if (CheckIsFloating()) //������ �Ѹ��� �� �ѹ� �� ���� �� �ִ��� üũ�Ѵ�. 
            {
                GameObject ItemDrop = Instantiate(items[index], rainPoint.position, Quaternion.identity);
            }
            int randomDropTimeIndex = Random.Range(0, timeBetItemRainDrop.Length);
            yield return timeBetItemRainDrop[randomDropTimeIndex];
        }

    }

    void setitemBetTime()
    {
        itemBetTime = Random.Range(minBetTime, maxBetTime);

    }

    bool CheckIsFloating() 
    {

        bool NotTouchingGround;

        //�� �ӿ� ���� ������ �������� �߻����� �ʴ´�. Collider�� �˻��Ѵ�.
        Collider2D[] buriedInGround = Physics2D.OverlapCircleAll((Vector2)rainPoint.position, groundCheckRadius, GroundMask);



        if (buriedInGround.Length == 0 ) 
        {
            NotTouchingGround = true;
        }
        else
        {
            NotTouchingGround = false;
        }

        return NotTouchingGround;
    }


    bool CheckPlayerIsRiding()
    {
        bool isRiding = false;

        Collider2D[] touchingPlayer = Physics2D.OverlapCircleAll((Vector2)rainPoint.position, 2f, playerMask);

        if (touchingPlayer.Length > 0)
        {
            isRiding = true;

        }
        return isRiding;
            
    }
}
