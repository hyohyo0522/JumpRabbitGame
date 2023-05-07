using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerEnemy : LivingEntity

{
    public Transform _detectPoint;
    public LayerMask PlayerMask;
    public Transform _rightattackpoint;
    public Transform _leftattackpoint;
    Vector3 playerPos;
    bool stateInAttack;
    const float _attackRange = 0.6f;
    const float _detectRange = 10f;
    const float _timeBetUpdateX = 3f;

    bool beDamaged=false;
    const float timebetDamage = 1f;
    float damagedPower = 35f; // �÷��̾�� ���ݴ��ϴ� ������

    public Transform _headShotPoint;
    Animator _flowerAni;
    SpriteRenderer _flowerSpriteRenderer;



    //���ݴ����� �� ���س� �����۴�Ƶ� �迭�� �����Ѵ�.
    public GameObject[] items;

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
    private void FixedUpdate()
    {
        if (!dead)
        {
            if (DetectPlayers()) 
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

            }
            else
            {
                if (stateInAttack)
                {
                    _flowerAni.SetBool("Attacking", false); //�ִϸ��̼� ����
                    stateInAttack = false;
                }
            }
        }

    }


    private bool DetectPlayers() 
    {
        Collider2D[] hasPlayerClose = Physics2D.OverlapCircleAll(_detectPoint.position, _detectRange, PlayerMask);
        if (hasPlayerClose.Length > 0)
        {
            playerPos = hasPlayerClose[0].GetComponent<Transform>().position;
            StartCoroutine(UpdateXdirection());  // �ø�X �ڷ�ƾ �޼��� Ȱ��ȭ
            
            return true;
        }
        else 
        {
            float randomx = Random.RandomRange(this.transform.position.x - _detectRange, this.transform.position.x + _detectRange);
            Vector3 temPos = new Vector3(randomx, 0, 0);
            playerPos = temPos; // �ڲ� Transform �� �̽̿������� �߰��� 
            return false;
        } 
    }



    //�Ӹ� ������ ���� ���� �����޼���
    public void GetHeadShot()
    {
        if (!beDamaged)
        {

            OnDamage(damagedPower);
            if (hp > 0)
            {
                AudioManager.instance.PlaySFX("AttackFlower");
            }

            int r = Random.Range(0, items.Length);
            GameObject selectedItem = Instantiate(items[r], _headShotPoint.position, Quaternion.identity);

            //������ �ٿ ȿ�� ����� 
            float rdForceX = Random.Range(-300f, 300f);
            selectedItem.GetComponent<Rigidbody2D>().AddForce((transform.right * rdForceX) + (transform.up * 700f)); // �������� ���� �ڵ��� �����. 
        }

    }

    //���� �ݶ��̴��� Ȱ��ȭ �ϰ� �ݶ��̴��� ������ �÷��̾�� �������� ������ �޼���
    private void ActivateAttack()
    {

        if((this.transform.position.x - playerPos.x)<0) //������ �÷��̾� ���� 
        {
            Collider2D[] PlayerisRight = Physics2D.OverlapCircleAll((Vector2)_rightattackpoint.position, _attackRange, PlayerMask);
            if (PlayerisRight.Length > 0)
            {
                //Debug.Log("�÷��̾�� �����ߴ�."); // ���˿Ϸ�
                for (int n = 0; n < PlayerisRight.Length; n++)
                {
                    PlayerLife attackPlayers = PlayerisRight[n].GetComponent<PlayerLife>();
                    attackPlayers?.OnDamage(damage);


                }
            }

        }
        else
        {
            //���� �÷��̾� ����
            Collider2D[] PlayerisLeft = Physics2D.OverlapCircleAll((Vector2)_leftattackpoint.position, _attackRange, PlayerMask); ; 
            if(PlayerisLeft.Length > 0)
            {
                //Debug.Log("�÷��̾�� �����ߴ�."); // ���˿Ϸ�
                for (int n = 0; n < PlayerisLeft.Length; n++)
                {
                    PlayerLife attackPlayers = PlayerisLeft[n].GetComponent<PlayerLife>();
                    if (!attackPlayers.attacked) // �÷��̾ ���ݴ��ؼ� ���� ������ ������ ������ �������� �ʴ´�. 
                    {
                        attackPlayers.OnDamage(damage);
                    }
                }
            }
        }
    }

    // ���� ��� ��ȭ��Ű��, ���� ��ȭ�� �ð����� ������ ���� �ʴ´�.
    private IEnumerator onDamageEffect()
    {
        var timebetDamageForCoroutine= new WaitForSeconds(timebetDamage);
        if (!dead)
        {
            _flowerSpriteRenderer.color = Color.gray;
            beDamaged = true;
            yield return timebetDamageForCoroutine;
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
        AudioManager.instance.PlaySFX("FlowerDie");
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
        var timeBetUpdateXDirectionForCoroutine = new WaitForSeconds(_timeBetUpdateX);

        while (stateInAttack) // ������ ������ �Ǹ� Flipx ����? 
        {
            _flowerSpriteRenderer.flipX = ((this.transform.position.x - playerPos.x) < 0) ? true : false;
            yield return timeBetUpdateXDirectionForCoroutine;
        }

    }

}
