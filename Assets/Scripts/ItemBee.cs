using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBee : MonoBehaviour
{

    // 이동관련 
    float speed = 5f;
    Vector2 newDestination;
    private SpriteRenderer BeeSpRender;
    public LayerMask BorderMask;
    const float boarderCheckRadius = 5f;
    public LayerMask playerMask;
    public Transform beeCenter;

    public Transform MaxUpLeft;
    public Transform MaxDownRight;

    //아이템 드롭 관련
    public LayerMask GroundMask;
    const float groundCheckRadius = 5f;
    public Transform rainPoint;
    public GameObject[] items;

    //아이템 드롭 시 랜덤효과 주기 위해서 3개의 timeBetItemRainDrop을 만듦 (캐싱)
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


        if (CheckIsFloating()) // 땅 위에 떠 있을때에만, 아이템 비에 대한 로직이 동작하게 한다. 
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
            // Debug.Log("근처에 경계가 있다.");
            // ★ 경계에 왔을 때 안쪽의 새로운 위치로 이동하게 한다. 
            // ★ 경계에 닿았을 때 안쪽으로 돌아갈 위치를 배열로 받아서 쓰자. 
            newDestination = Vector2.zero; 
        }
    }

    private IEnumerator ItemRains()
    {
        int ItemNumber = Random.Range(7, 11);

        for (int n = 0; n < ItemNumber; n++)
        {
            int index = Random.Range(0, items.Length);

            if (CheckIsFloating()) //아이템 뿌리기 전 한번 더 땅에 떠 있는지 체크한다. 
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

        //땅 속에 있을 때에는 아이템을 발사하지 않는다. Collider로 검사한다.
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
