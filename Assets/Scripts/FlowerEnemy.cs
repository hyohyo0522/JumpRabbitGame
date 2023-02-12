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

    bool beDamaged=false;
    float timebetDamage = 1f;

    public Transform _headShotPoint;
    Animator _flowerAni;
    SpriteRenderer _flowerSpriteRenderer;

    // ★ 소리 재생 메서드 해야함 


    //공격당했을 때 토해낼 아이템담아둘 배열을 선언한다.
    public GameObject[] items;
   //  public GameObject rewardItem; enemy spawner에서 하자.

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

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (!dead)
        {
            if (DetectPlayers()) //이렇게 코드를 짜면 DetectPlayers()메서드가 주기적으로 한번씩은 꼭 실행되는 걸까?
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


    private bool DetectPlayers() //이걸 bool로 만드는게 좋은지가 늘 궁금하다.
    {
        Collider2D[] hasPlayerClose = Physics2D.OverlapCircleAll(_detectPoint.position, 10f, PlayerMask);
        if (hasPlayerClose.Length > 0)
        {
            //int r = Random.Range(0, hasPlayerClose.Length); // 플레이어가 2명이상 감지될 경우 랜덤으로 한명의 위치를 받아온다.
            playerPos = hasPlayerClose[0].GetComponent<Transform>().position;
            StartCoroutine(UpdateXdirection());  // 플립X 코루틴 메서드 활성화
            
            return true;
        }
        else 
        {
            float randomx = Random.RandomRange(this.transform.position.x - 10, this.transform.position.x + 10);
            Vector3 temPos = new Vector3(randomx, 0, 0);
            playerPos = temPos; // 자꾸 Transform 값 미싱오류나서 추가함 
            return false;
        } 
    }


    // 자식오브젝트-헤드샷에 있는 콜라이더 충돌을 감지한다?

    private void OnCollisionEnter2D(Collision2D collision)
    {

        //_tfGenPos = transform.GetChild(0);??
        //if (collision.gameObject.CompareTag("Player") && !beDamaged)
        //{
        //    Debug.Log(collision.gameObject.name);
        //    PlayerMovement attackingPlayers = collision.gameObject.GetComponent<PlayerMovement>();
        //    attackingPlayers.AddForcetoBounce(2500f);

        //    //공격당했을때 아이템을 토해내는 메서드.
        //    int r = Random.Range(0, items.Length);
        //    GameObject selectedItem = Instantiate(items[r], _headShotPoint.position, Quaternion.identity);
        //    float rdForceX = Random.Range(-200f, 200f);
        //    selectedItem.GetComponent<Rigidbody2D>().AddForce((transform.right * rdForceX) + (transform.up * 700f)); // 아이템이 위로 솟도록 만든다. 
        //}
    }

    //머리 위에서 점프 공격 감지메서드
    public void GetHeadShot()
    {
        if (!beDamaged)
        {
            OnDamage(35); 
            int r = Random.Range(0, items.Length);
            GameObject selectedItem = Instantiate(items[r], _headShotPoint.position, Quaternion.identity);

            //아이템 바운스 효과 줘야함 
            float rdForceX = Random.Range(-300f, 300f);
            selectedItem.GetComponent<Rigidbody2D>().AddForce((transform.right * rdForceX) + (transform.up * 700f)); // 아이템이 위로 솟도록 만든다. 
        }
        //원래 썼던 코드 ★
        //Collider2D[] GetTreaded  = Physics2D.OverlapCircleAll((Vector2)_headShotPoint.position, 0.6f, PlayerMask);
        //if(GetTreaded.Length>0 && GetTreaded[0].GetComponent<Rigidbody2D>().velocity.y < 0 && !beDamaged)
        //{

        //    // ★ 플레이어의 공격력 수치를 가져올 메서드를 여기에 추가한다. 
        //    OnDamage(35); // 35데미지 만큼 공격을 당한다. 
        //    Debug.Log("공격당했다.");


        //    //Debug.Log("플레이어와 접촉했다."); // 점검완료

        //    //헤드샷을 한 플레이어를 위로 바운스 시킨다. 
        //    for (int n = 0; n < GetTreaded.Length; n++)
        //    {
        //        PlayerMovement attackingPlayers = GetTreaded[n].GetComponent<PlayerMovement>();
        //        attackingPlayers.AddForcetoBounce(2500f);

        //        // 공격당했을때 아이템을 토해내는 메서드.


        //    }


        //}
    }

    //공격 콜라이더를 활성화 하고 콜라이더에 접근한 플레이어에게 데미지를 입히는 메서드
    private void ActivateAttack()
    {

        if((this.transform.position.x - playerPos.x)<0)
        {
            //Debug.Log("플레이가 오른쪽에 있다."); // 점검완료
            Collider2D[] PlayerisRight = Physics2D.OverlapCircleAll((Vector2)_rightattackpoint.position,0.6f, PlayerMask);
            if (PlayerisRight.Length > 0)
            {
                //Debug.Log("플레이어와 접촉했다."); // 점검완료
                for (int n = 0; n < PlayerisRight.Length; n++)
                {
                    PlayerLife attackPlayers = PlayerisRight[n].GetComponent<PlayerLife>();
                    attackPlayers?.OnDamage(damage);

                    // 아래는 PlayerLife 의  OnDamage에 if (attacked) return; 한줄 추가하며 삭제함
                    //if (!attackPlayers.attacked) // 플레이어가 공격당해서 빨간 색으로 변했을 때에는 공격하지 않는다. 
                    //{
                    //    attackPlayers.OnDamage(damage);
                    //    // 플레이어 공격
                    //    //Debug.Log("플레이가 공격당했다.");
                    //}

                }
            }

        }
        else
        {
            //Debug.Log("플레이가 왼쪽에 있다.");
            Collider2D[] PlayerisLeft = Physics2D.OverlapCircleAll((Vector2)_leftattackpoint.position, 0.8f, PlayerMask); ; 
            if(PlayerisLeft.Length > 0)
            {
                //Debug.Log("플레이어와 접촉했다."); // 점검완료
                for (int n = 0; n < PlayerisLeft.Length; n++)
                {
                    PlayerLife attackPlayers = PlayerisLeft[n].GetComponent<PlayerLife>();
                    if (!attackPlayers.attacked) // 플레이어가 공격당해서 빨간 색으로 변했을 때에는 공격하지 않는다. 
                    {
                        attackPlayers.OnDamage(damage);
                        // 플레이어 공격
                        //Debug.Log("플레이가 공격당했다.");
                    }
                }
            }
        }
    }

    // 색을 잠깐 변화시키고, 색이 변화된 시간동안 공격을 받지 않는다.
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
        //소리 재생메서드 추가 
    }

    public override void Die()    // 죽었을 때 효과 추가. 
    {
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
        while (stateInAttack)
        {

            _flowerSpriteRenderer.flipX = ((this.transform.position.x - playerPos.x) < 0) ? true : false;
            yield return new WaitForSeconds(3f);
        }

    }

}
