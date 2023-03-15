using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlugMon : MonoBehaviour
{
    Rigidbody2D slugRigid;
    SpriteRenderer slugSprite;


    // 플레이어에게 공격당했을 때 
    public GameObject pung; // 죽을 때 펑 애니메이션 재생
    float pungAniPlayTime = 0.55f;

    //플레이어 체력 올려주는 기능
    float slugHeal = 20f;

    Transform _slugTransform;
    float headShotBouncePower = 1000f;



    public int slugKillPlus = 1;
    public float destroyDelayTime = 15f;
    public float delayForUse = 0.5f; // 생성되자마자 바로 아이템이 사용되는 것을 방지하기 위한 딜레이타임
    bool afterDelay = false;

    // 스피드 관련 
    float mInspeed = 3.5f;
    float maxspeed = 5.5f;
    [SerializeField] float realSpeed;

    float minCrawlTime = 3f;
    float maxCrawlTime = 5f;


    float crawlTime;

    bool goingRight = false;

    //땅에 있는지 체크
    [SerializeField] bool isgrounded = false;
    float groundCheckRadius = 2f;
    public LayerMask groundMask;

    //아이템 먹는 기능
    public LayerMask ItemMask;
    public GameObject pungSmall; // 아이템 먹었을 때 펑 애니메이션 재생


    private void OnEnable()
    {

        StartCoroutine(makeDelay());

        slugRigid = GetComponent<Rigidbody2D>();
        slugSprite = GetComponent<SpriteRenderer>();
        _slugTransform = GetComponent<Transform>();
        ResetCrawlTime();


    }

    private void Start()
    {
        Destroy(this.gameObject, destroyDelayTime); // 일정 시간 지나면 삭제한다.
    }

    private void Update()
    {
        crawlTime -= Time.deltaTime;
        if (crawlTime <= 0)
        {
            ResetCrawlTime();
            ChoiceDirection();
        }
    }

    private void FixedUpdate()
    {
        GroundCheck();
        if (isgrounded)
        {
            crawling();
            EatItems();
        }

    }        

    void crawling()
    {
        int direction = (goingRight) ? 1 : -1;

        realSpeed = Random.Range(mInspeed, maxspeed); // 달팽이가 움직이는 양이 조금씩 다르면 더 생동감 넘칠 것 같아 이렇게 시시각각 변하게 설정
        this.transform.rotation = Quaternion.identity;// 달팽이 뒤집어지는 것 방지.
        slugRigid.AddForce(Vector2.right * realSpeed * direction * Time.deltaTime, ForceMode2D.Impulse);

    }

    void ChoiceDirection()
    {
        float choicePoint = Random.Range(0, 10);
        if (choicePoint > 0.5f)
        {
            slugRigid.Sleep(); // 방향 바꿨을 때 이전 방향으로 이동하는 걸 방지하기 위해 Sleep - WakeUp 함
            slugRigid.velocity = Vector2.zero; //방향 바꿀 때마다 속도 초기화
            goingRight = true;
            slugSprite.flipX = true;
            slugRigid.WakeUp();

        }
        else
        {
            slugRigid.Sleep(); // 방향 바꿨을 때 이전 방향으로 이동하는 걸 방지하기 위해 Sleep - WakeUp 함
            slugRigid.velocity = Vector2.zero; //방향 바꿀 때마다 속도 초기화
            goingRight = false;
            slugSprite.flipX = true;
            slugRigid.WakeUp();
        }



    }

    void ResetCrawlTime()
    {
        crawlTime = Random.Range(minCrawlTime, maxCrawlTime);
    }

    //땅위에 있는지 검사 
    public void GroundCheck()
    {
        if (isgrounded) return; // 처음으로 땅에 닿았다면 검사하지 않는다. 
        //땅출동 감지
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)this.transform.position, groundCheckRadius, groundMask);

        if (colliders.Length > 0 && !isgrounded ) //땅에 닿았다.
        {
            isgrounded = true;
        }
        else   //땅에 닿지 않았다.
        {
            isgrounded = false;

        }
    }

    IEnumerator makeDelay()
    {
        afterDelay = false;
        yield return new WaitForSeconds(delayForUse);
        afterDelay = true;
    }

    // 플레이어에게 공격당헀을 때.
    public void GetHeadShot(PlayerLife player)
    {
        if (afterDelay)
        {

            if (!player.dead)
            {
                AudioManager.instance.PlaySFX("AttackSlug");
                player.UpdateSlugUI(slugKillPlus);
                Vector2 DisappearPosition = this.transform.position;
                GameObject pungPlay = Instantiate(pung, DisappearPosition, Quaternion.identity);

                //달팽이 플레이어 체력 올려주는 기능 
                player.RestoreHealth(slugHeal);

                Destroy(pungPlay.gameObject, pungAniPlayTime);
                Destroy(this.gameObject);



            }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            PlayerMovement attackingPlayers = collision.gameObject.GetComponent<PlayerMovement>();
            PlayerLife _attackingPlayerLife = collision.gameObject.GetComponent<PlayerLife>();
            if (!attackingPlayers.isGrounded) // 땅에 있을 때에는 작동하지 HeadShot 효과가 작동하지 않는다. 
            {

                ContactPoint2D cp = collision.GetContact(0);
                Vector2 dir = cp.point - (Vector2)_slugTransform.position; // 플레이어가 튕겨야하므로, 플레이어방향 - 현재 몬스터 방향으로 함 
                //rigidbody.AddForce((dir).normalized * 300f);

                attackingPlayers.AddForcetoBounce((dir).normalized * headShotBouncePower);
                
                GetHeadShot(_attackingPlayerLife);

            }

        }
    }


    private void EatItems()
    {
        Collider2D[] nearItems = Physics2D.OverlapCircleAll(transform.position, 1.0f, ItemMask);

        if (nearItems.Length > 0)
        {
            Vector2 ItemPosition = nearItems[0].transform.position;
            IItem touchedItem = nearItems[0].GetComponent<IItem>();
            touchedItem?.DestoySelf();

            SlugMon touchedslug = nearItems[0].GetComponent<SlugMon>();
            touchedslug?.DetroySelf();

            GameObject ItemDisappearPlay = Instantiate(pungSmall, ItemPosition, Quaternion.identity);
            Destroy(ItemDisappearPlay.gameObject, pungAniPlayTime);

        }

    }

    public void DetroySelf()
    {
        Destroy(this.gameObject);
    }

}

