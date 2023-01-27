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

    Transform _slugTransform;
    float headShotBouncePower = 1000f;



    public int slugKillPlus = 1;
    public float destroyDelayTime = 10f;
    public float delayForUse = 0.5f; // 생성되자마자 바로 아이템이 사용되는 것을 방지하기 위한 딜레이타임
    bool afterDelay = false;

    // 스피드 관련 
    float maxspeed = 800f;
    float mInspeed = 500f;

    float maxCrawlTime = 8f;
    float minCrawlTime = 5f;

    float crawlTime;

    bool goingRight = false;

    //땅에 있는지 체크
    bool isgrounded = false;
    float groundCheckRadius = 1f;
    public LayerMask groundMask;


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
        if (isgrounded)
        {
            crawling();
        }

    }        

    void crawling()
    {
        float realSpeed = Random.Range(mInspeed, maxspeed);
        int direction = (goingRight)?1:-1;

        slugRigid.velocity = Vector2.zero; 
        slugRigid.AddForce(Vector2.right * realSpeed * direction * Time.deltaTime); ;

    }

    void ChoiceDirection()
    {
        float choicePoint = Random.Range(0, 10);
        if (choicePoint > 0.5f)
        {
            goingRight = true;
            slugSprite.flipX = true;

        }
        else
        {
            goingRight = false;
            slugSprite.flipX = true;
        }


    }

    void ResetCrawlTime()
    {
        crawlTime = Random.Range(minCrawlTime, maxCrawlTime);
    }

    //땅위에 있는지 검사 
    public void GroundCheck()
    {
        //땅출동 감지
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)this.transform.position, groundCheckRadius, groundMask);

        if (colliders.Length > 0) //땅에 닿았다.
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
                player.UpdateSlugUI(slugKillPlus);
                Vector2 DisappearPosition = this.transform.position;
                GameObject pungPlay = Instantiate(pung, DisappearPosition, Quaternion.identity);
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


    public void DetroySelf()
    {
        Destroy(this.gameObject);
    }

}

