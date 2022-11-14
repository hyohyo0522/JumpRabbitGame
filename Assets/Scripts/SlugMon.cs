using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlugMon : MonoBehaviour
{
    Rigidbody2D slugRigid;
    SpriteRenderer slugSprite;


    // �÷��̾�� ���ݴ����� �� 
    public GameObject pung; // ���� �� �� �ִϸ��̼� ���
    float pungAniPlayTime = 0.55f;

    Transform _slugTransform;
    float headShotBouncePower = 1000f;



    public int slugKillPlus = 1;
    public float destroyDelayTime = 10f;
    public float delayForUse = 0.5f; // �������ڸ��� �ٷ� �������� ���Ǵ� ���� �����ϱ� ���� ������Ÿ��
    bool afterDelay = false;

    // ���ǵ� ���� 
    float maxspeed = 800f;
    float mInspeed = 500f;

    float maxCrawlTime = 8f;
    float minCrawlTime = 5f;

    float crawlTime;

    bool goingRight = false;

    //���� �ִ��� üũ
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
        Destroy(this.gameObject, destroyDelayTime); // ���� �ð� ������ �����Ѵ�.
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

    //������ �ִ��� �˻� 
    public void GroundCheck()
    {
        //���⵿ ����
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)this.transform.position, groundCheckRadius, groundMask);

        if (colliders.Length > 0) //���� ��Ҵ�.
        {
            isgrounded = true;
        }
        else   //���� ���� �ʾҴ�.
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

    // �÷��̾�� ���ݴ����� ��.
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
            if (!attackingPlayers.isGrounded) // ���� ���� ������ �۵����� HeadShot ȿ���� �۵����� �ʴ´�. 
            {

                ContactPoint2D cp = collision.GetContact(0);
                Vector2 dir = cp.point - (Vector2)_slugTransform.position; // �÷��̾ ƨ�ܾ��ϹǷ�, �÷��̾���� - ���� ���� �������� �� 
                //rigidbody.AddForce((dir).normalized * 300f);

                attackingPlayers.AddForcetoBounce((dir).normalized * headShotBouncePower);
                
                GetHeadShot(_attackingPlayerLife);

            }

        }
    }

}

