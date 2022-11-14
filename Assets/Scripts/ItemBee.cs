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
    public Transform beeCenter;

    public Transform MaxUpLeft;
    public Transform MaxDownRight;

    //아이템 드롭 관련
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
        if (_floatingGround) // 땅 위에 떠 있을때에만, 아이템 비에 대한 로직이 동작하게 한다. 
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
            Debug.Log("새로운 목표지점을 설정한다.");
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
            Debug.Log("근처에 경계가 있다.");
            // ★ 경계에 왔을 때 안쪽의 새로운 위치로 이동하게 한다. 
            // ★ 경계에 닿았을 때 안쪽으로 돌아갈 위치를 배열로 받아서 쓰자. 
            newDestination = Vector2.zero; 
        }
    }

    //랜덤시간에 따라 아이템 떨구게 하는 메서드
    // 진짜 가끔 별/ 당근 많이 쏟아지게 함
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
        //밑에 땅이 있는지 검사하고? >> 이거 없어도 될수도 있다. 
        //RaycastHit2D hitInfo = Physics2D.Raycast(rainPoint.position, -Vector2.up, 10f, GroundMask);

        //땅 속에 있을 때에는 아이템을 발사하지 않는다. Collider로 검사한다.
        Collider2D[] buriedInGround = Physics2D.OverlapCircleAll((Vector2)rainPoint.position, 2f, GroundMask);



        if (buriedInGround.Length == 0 ) 
        {
            
            //Debug.Log("땅이 밑에 있다!!!!!!!!!!!!!!!!!!!");
            _floatingGround = true;
        }
        else
        {
            //Debug.Log("땅이 밑에 없다!");
            _floatingGround = false;
        }
    }

}
