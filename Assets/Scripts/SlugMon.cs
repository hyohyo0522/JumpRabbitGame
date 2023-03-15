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

    //�÷��̾� ü�� �÷��ִ� ���
    float slugHeal = 20f;

    Transform _slugTransform;
    float headShotBouncePower = 1000f;



    public int slugKillPlus = 1;
    public float destroyDelayTime = 15f;
    public float delayForUse = 0.5f; // �������ڸ��� �ٷ� �������� ���Ǵ� ���� �����ϱ� ���� ������Ÿ��
    bool afterDelay = false;

    // ���ǵ� ���� 
    float mInspeed = 3.5f;
    float maxspeed = 5.5f;
    [SerializeField] float realSpeed;

    float minCrawlTime = 3f;
    float maxCrawlTime = 5f;


    float crawlTime;

    bool goingRight = false;

    //���� �ִ��� üũ
    [SerializeField] bool isgrounded = false;
    float groundCheckRadius = 2f;
    public LayerMask groundMask;

    //������ �Դ� ���
    public LayerMask ItemMask;
    public GameObject pungSmall; // ������ �Ծ��� �� �� �ִϸ��̼� ���


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

        realSpeed = Random.Range(mInspeed, maxspeed); // �����̰� �����̴� ���� ���ݾ� �ٸ��� �� ������ ��ĥ �� ���� �̷��� �ýð��� ���ϰ� ����
        this.transform.rotation = Quaternion.identity;// ������ ���������� �� ����.
        slugRigid.AddForce(Vector2.right * realSpeed * direction * Time.deltaTime, ForceMode2D.Impulse);

    }

    void ChoiceDirection()
    {
        float choicePoint = Random.Range(0, 10);
        if (choicePoint > 0.5f)
        {
            slugRigid.Sleep(); // ���� �ٲ��� �� ���� �������� �̵��ϴ� �� �����ϱ� ���� Sleep - WakeUp ��
            slugRigid.velocity = Vector2.zero; //���� �ٲ� ������ �ӵ� �ʱ�ȭ
            goingRight = true;
            slugSprite.flipX = true;
            slugRigid.WakeUp();

        }
        else
        {
            slugRigid.Sleep(); // ���� �ٲ��� �� ���� �������� �̵��ϴ� �� �����ϱ� ���� Sleep - WakeUp ��
            slugRigid.velocity = Vector2.zero; //���� �ٲ� ������ �ӵ� �ʱ�ȭ
            goingRight = false;
            slugSprite.flipX = true;
            slugRigid.WakeUp();
        }



    }

    void ResetCrawlTime()
    {
        crawlTime = Random.Range(minCrawlTime, maxCrawlTime);
    }

    //������ �ִ��� �˻� 
    public void GroundCheck()
    {
        if (isgrounded) return; // ó������ ���� ��Ҵٸ� �˻����� �ʴ´�. 
        //���⵿ ����
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)this.transform.position, groundCheckRadius, groundMask);

        if (colliders.Length > 0 && !isgrounded ) //���� ��Ҵ�.
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
                AudioManager.instance.PlaySFX("AttackSlug");
                player.UpdateSlugUI(slugKillPlus);
                Vector2 DisappearPosition = this.transform.position;
                GameObject pungPlay = Instantiate(pung, DisappearPosition, Quaternion.identity);

                //������ �÷��̾� ü�� �÷��ִ� ��� 
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

