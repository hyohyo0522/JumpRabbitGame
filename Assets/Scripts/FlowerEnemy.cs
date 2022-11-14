using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerEnemy : LivingEntity

{
    public Transform _detectPoint;
    public LayerMask PlayerMask;
    public Transform _rightattackpoint;
    public Transform _leftattackpoint;
    Transform playerPos;
    bool stateInAttack;

    bool beDamaged=false;
    float timebetDamage = 1f;

    public Transform _headShotPoint;
    Animator _flowerAni;
    SpriteRenderer _flowerSpriteRenderer;

    // �� �Ҹ� ��� �޼��� �ؾ��� 


    //���ݴ����� �� ���س� �����۴�Ƶ� �迭�� �����Ѵ�.
    public GameObject[] items;
   //  public GameObject rewardItem; enemy spawner���� ����.

    float damage = 10f;  //���� ���ݷ�

    private void Awake()
    {
        _flowerAni = GetComponent<Animator>();
        _flowerSpriteRenderer = GetComponent<SpriteRenderer>();


    }

    //�ٸ� ��ũ��Ʈ���� ������ ��Ű�� ���� �ʿ��� �޼ҵ� 
    public void Setup(float newHealth, float newDamage)
    {
        hp = newHealth;
        damage = newDamage;
    }

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if(DetectPlayers()) //�̷��� �ڵ带 ¥�� DetectPlayers()�޼��尡 �ֱ������� �ѹ����� �� ����Ǵ� �ɱ�?
        {
            if (!stateInAttack) // �÷��̾ ������ ���¿��� ���ݻ�Ȳ���� ���� ��
            {
                _flowerAni.SetBool("Attacking", true); //���� �ִϸ��̼����� ����
                stateInAttack = true;

            }
            else
            {
                //GetHeadShot(); // �÷��̾�� ���ݹ��� �� �ۿ�
                ActivateAttack(); // ���� �ݶ��̴��� Ȱ��ȭ 
            }
              
        }else
        {
            if (stateInAttack)
            {
                _flowerAni.SetBool("Attacking", false); //�ִϸ��̼� ����
                stateInAttack = false;
            }
        }
    }


    private bool DetectPlayers() //�̰� bool�� ����°� �������� �� �ñ��ϴ�.
    {
        Collider2D[] hasPlayerClose = Physics2D.OverlapCircleAll((Vector2)_detectPoint.position, 10f, PlayerMask);
        if (hasPlayerClose.Length > 0)
        {
            int r = Random.Range(0, hasPlayerClose.Length); // �÷��̾ 2���̻� ������ ��� �������� �Ѹ��� ��ġ�� �޾ƿ´�.
            playerPos = hasPlayerClose[r].GetComponent<Transform>();
            StartCoroutine(UpdateXdirection());  // �ø�X �ڷ�ƾ �޼��� Ȱ��ȭ
            return true;
        }
        else return false;
    }


    // �ڽĿ�����Ʈ-��弦�� �ִ� �ݶ��̴� �浹�� �����Ѵ�?

    private void OnCollisionEnter2D(Collision2D collision)
    {

        //_tfGenPos = transform.GetChild(0);??
        //if (collision.gameObject.CompareTag("Player") && !beDamaged)
        //{
        //    Debug.Log(collision.gameObject.name);
        //    PlayerMovement attackingPlayers = collision.gameObject.GetComponent<PlayerMovement>();
        //    attackingPlayers.AddForcetoBounce(2500f);

        //    //���ݴ������� �������� ���س��� �޼���.
        //    int r = Random.Range(0, items.Length);
        //    GameObject selectedItem = Instantiate(items[r], _headShotPoint.position, Quaternion.identity);
        //    float rdForceX = Random.Range(-200f, 200f);
        //    selectedItem.GetComponent<Rigidbody2D>().AddForce((transform.right * rdForceX) + (transform.up * 700f)); // �������� ���� �ڵ��� �����. 
        //}
    }

    //�Ӹ� ������ ���� ���� �����޼���
    public void GetHeadShot()
    {
        if (!beDamaged)
        {
            OnDamage(35); 
            int r = Random.Range(0, items.Length);
            GameObject selectedItem = Instantiate(items[r], _headShotPoint.position, Quaternion.identity);

            //������ �ٿ ȿ�� ����� 
            float rdForceX = Random.Range(-300f, 300f);
            selectedItem.GetComponent<Rigidbody2D>().AddForce((transform.right * rdForceX) + (transform.up * 700f)); // �������� ���� �ڵ��� �����. 
        }
        //���� ��� �ڵ� ��
        //Collider2D[] GetTreaded  = Physics2D.OverlapCircleAll((Vector2)_headShotPoint.position, 0.6f, PlayerMask);
        //if(GetTreaded.Length>0 && GetTreaded[0].GetComponent<Rigidbody2D>().velocity.y < 0 && !beDamaged)
        //{

        //    // �� �÷��̾��� ���ݷ� ��ġ�� ������ �޼��带 ���⿡ �߰��Ѵ�. 
        //    OnDamage(35); // 35������ ��ŭ ������ ���Ѵ�. 
        //    Debug.Log("���ݴ��ߴ�.");


        //    //Debug.Log("�÷��̾�� �����ߴ�."); // ���˿Ϸ�

        //    //��弦�� �� �÷��̾ ���� �ٿ ��Ų��. 
        //    for (int n = 0; n < GetTreaded.Length; n++)
        //    {
        //        PlayerMovement attackingPlayers = GetTreaded[n].GetComponent<PlayerMovement>();
        //        attackingPlayers.AddForcetoBounce(2500f);

        //        // ���ݴ������� �������� ���س��� �޼���.


        //    }


        //}
    }

    //���� �ݶ��̴��� Ȱ��ȭ �ϰ� �ݶ��̴��� ������ �÷��̾�� �������� ������ �޼���
    private void ActivateAttack()
    {

        if((this.transform.position.x - playerPos.position.x)<0)
        {
            //Debug.Log("�÷��̰� �����ʿ� �ִ�."); // ���˿Ϸ�
            Collider2D[] PlayerisRight = Physics2D.OverlapCircleAll((Vector2)_rightattackpoint.position,0.6f, PlayerMask);
            if (PlayerisRight.Length > 0)
            {
                //Debug.Log("�÷��̾�� �����ߴ�."); // ���˿Ϸ�
                for (int n = 0; n < PlayerisRight.Length; n++)
                {
                    PlayerLife attackPlayers = PlayerisRight[n].GetComponent<PlayerLife>();
                    if (!attackPlayers.attacked) // �÷��̾ ���ݴ��ؼ� ���� ������ ������ ������ �������� �ʴ´�. 
                    {
                        attackPlayers.OnDamage(damage);
                        // �÷��̾� ����
                        //Debug.Log("�÷��̰� ���ݴ��ߴ�.");
                    }

                }
            }

        }
        else
        {
            //Debug.Log("�÷��̰� ���ʿ� �ִ�.");
            Collider2D[] PlayerisLeft = Physics2D.OverlapCircleAll((Vector2)_leftattackpoint.position, 0.6f, PlayerMask); ; 
            if(PlayerisLeft.Length > 0)
            {
                //Debug.Log("�÷��̾�� �����ߴ�."); // ���˿Ϸ�
                for (int n = 0; n < PlayerisLeft.Length; n++)
                {
                    PlayerLife attackPlayers = PlayerisLeft[n].GetComponent<PlayerLife>();
                    if (!attackPlayers.attacked) // �÷��̾ ���ݴ��ؼ� ���� ������ ������ ������ �������� �ʴ´�. 
                    {
                        attackPlayers.OnDamage(damage);
                        // �÷��̾� ����
                        //Debug.Log("�÷��̰� ���ݴ��ߴ�.");
                    }
                }
            }
        }
    }

    // ���� ��� ��ȭ��Ű��, ���� ��ȭ�� �ð����� ������ ���� �ʴ´�.
    private IEnumerator onDamageEffect()
    {
        if (!dead)
        {
            _flowerSpriteRenderer.color = Color.gray;
            beDamaged = true;
            yield return new WaitForSeconds(timebetDamage);
            _flowerSpriteRenderer.color = Color.white;
            beDamaged = false;
        }

    }

    public override void OnDamage(float value)
    {
        base.OnDamage(value);
        StartCoroutine(onDamageEffect());
        //�Ҹ� ����޼��� �߰� 
    }

    public override void Die()    // �׾��� �� ȿ�� �߰�. 
    {
        base.Die();

        //�ݶ��̴� ����
        Collider[] enemyColliders = GetComponents<Collider>();
        for (int i = 0; i < enemyColliders.Length; i++)
        {
            enemyColliders[i].enabled = false;
        }

        //�� �ִϸ��̼� �ò������°� ���ľ��Ѵ�. 
        _flowerAni.SetTrigger("Dead");

        // �� ���� �� ������ �վ�� �޼��� �߰��ؾ���
        Destroy(this.gameObject,0.7f);
    }


    // X��ǥ 
    private IEnumerator UpdateXdirection()
    {
        while (stateInAttack)
        {

            _flowerSpriteRenderer.flipX = ((this.transform.position.x - playerPos.position.x) < 0) ? true : false;
            yield return new WaitForSeconds(3f);
        }

    }

}
