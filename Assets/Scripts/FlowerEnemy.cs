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
    float damagedPower = 35f; // 플레이어에게 공격당하는 데미지

    public Transform _headShotPoint;
    Animator _flowerAni;
    SpriteRenderer _flowerSpriteRenderer;



    //공격당했을 때 토해낼 아이템담아둘 배열을 선언한다.
    public GameObject[] items;

    float damage = 10f;  //꽃의 공격력

    private void Awake()
    {
        _flowerAni = GetComponent<Animator>();
        _flowerSpriteRenderer = GetComponent<SpriteRenderer>();


    }

    //다른 스크립트에서 생성을 시키기 위해 필요한 메소드 
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
                if (!stateInAttack) // 플레이어가 감지된 상태에서 공격상황으로 들어가게 함
                {
                    _flowerAni.SetBool("Attacking", true); //공격 애니메이션으로 변경
                    stateInAttack = true;

                }
                else
                {
                    //GetHeadShot(); // 플레이어에게 공격받을 때 작용
                    ActivateAttack(); // 공격 콜라이더를 활성화 
                }

            }
            else
            {
                if (stateInAttack)
                {
                    _flowerAni.SetBool("Attacking", false); //애니메이션 변경
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
            StartCoroutine(UpdateXdirection());  // 플립X 코루틴 메서드 활성화
            
            return true;
        }
        else 
        {
            float randomx = Random.RandomRange(this.transform.position.x - _detectRange, this.transform.position.x + _detectRange);
            Vector3 temPos = new Vector3(randomx, 0, 0);
            playerPos = temPos; // 자꾸 Transform 값 미싱오류나서 추가함 
            return false;
        } 
    }



    //머리 위에서 점프 공격 감지메서드
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

            //아이템 바운스 효과 줘야함 
            float rdForceX = Random.Range(-300f, 300f);
            selectedItem.GetComponent<Rigidbody2D>().AddForce((transform.right * rdForceX) + (transform.up * 700f)); // 아이템이 위로 솟도록 만든다. 
        }

    }

    //공격 콜라이더를 활성화 하고 콜라이더에 접근한 플레이어에게 데미지를 입히는 메서드
    private void ActivateAttack()
    {

        if((this.transform.position.x - playerPos.x)<0) //오른쪽 플레이어 감지 
        {
            Collider2D[] PlayerisRight = Physics2D.OverlapCircleAll((Vector2)_rightattackpoint.position, _attackRange, PlayerMask);
            if (PlayerisRight.Length > 0)
            {
                //Debug.Log("플레이어와 접촉했다."); // 점검완료
                for (int n = 0; n < PlayerisRight.Length; n++)
                {
                    PlayerLife attackPlayers = PlayerisRight[n].GetComponent<PlayerLife>();
                    attackPlayers?.OnDamage(damage);


                }
            }

        }
        else
        {
            //왼쪽 플레이어 감지
            Collider2D[] PlayerisLeft = Physics2D.OverlapCircleAll((Vector2)_leftattackpoint.position, _attackRange, PlayerMask); ; 
            if(PlayerisLeft.Length > 0)
            {
                //Debug.Log("플레이어와 접촉했다."); // 점검완료
                for (int n = 0; n < PlayerisLeft.Length; n++)
                {
                    PlayerLife attackPlayers = PlayerisLeft[n].GetComponent<PlayerLife>();
                    if (!attackPlayers.attacked) // 플레이어가 공격당해서 빨간 색으로 변했을 때에는 공격하지 않는다. 
                    {
                        attackPlayers.OnDamage(damage);
                    }
                }
            }
        }
    }

    // 색을 잠깐 변화시키고, 색이 변화된 시간동안 공격을 받지 않는다.
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
        //소리 재생메서드 추가 
    }

    public override void Die()    // 죽었을 때 효과 추가. 
    {
        AudioManager.instance.PlaySFX("FlowerDie");
        base.Die();

        //콜라이더 끄기
        Collider[] enemyColliders = GetComponents<Collider>();
        for (int i = 0; i < enemyColliders.Length; i++)
        {
            enemyColliders[i].enabled = false;
        }

        //★ 애니메이션 시꺼매지는거 고쳐야한다. 
        _flowerAni.SetTrigger("Dead");

        // ★ 죽을 때 아이템 뿜어내는 메서드 추가해야함
        Destroy(this.gameObject,0.7f);
    }


    // X좌표 
    private IEnumerator UpdateXdirection()
    {
        var timeBetUpdateXDirectionForCoroutine = new WaitForSeconds(_timeBetUpdateX);

        while (stateInAttack) // 공격이 감지가 되면 Flipx 실행? 
        {
            _flowerSpriteRenderer.flipX = ((this.transform.position.x - playerPos.x) < 0) ? true : false;
            yield return timeBetUpdateXDirectionForCoroutine;
        }

    }

}
