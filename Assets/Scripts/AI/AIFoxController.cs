using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;


public class AIFoxController : MonoBehaviour
{
    protected StateMachine<AIFoxController> stateMachine;
    public StateMachine<AIFoxController> StateMachine => stateMachine;



    public LayerMask ladderMask;// 사다리 레이어 마스크
    public LayerMask targetMask;
    public LayerMask ItemMask;
    public LayerMask groundMask;
    public Collider2D groundCollider;

    public int ladderMaskInt;
    public int monsterMaskInt;


    [SerializeField] public Transform target; // 최종목표지점
    [SerializeField] public Vector2 MiddlewayPoint; // 최종 목표로 가기 위한 중간 목표지점

    [SerializeField] Collider2D currentLadder; //현재 접촉하려고 하거나, 이미 접촉하고 있는 사다리
    [SerializeField] public bool initialTouchingurrentLadder;  // 사다리에 처음으로 올랐는지 인하고 있는 사다리


    public float detectRadius = 500f; // 탐지거리, 아이템과 플레이어 탐지
    float attackRadius = 2.5f; //플레이어 공격범위이자, 플레이어에게 도착했다고 감지하는 범위값
    [SerializeField] float ladderDetectRange = 3f;
    [SerializeField] float _speed = 2f;
    [SerializeField] float _ladderSpeed = 7f;
    float powerMakingAttackingPlayerToBounce = 1000; //공격한 플레이어 점프시키는 기능

    public float damagePower = 10f;

    //float velocityX; // Rigidbody2D에서 x속도를 가져와서 FlipX를 적용할 것이다.

    //땅에 있는지 검사
    bool foxIsGrounded;
    [SerializeField] Transform groundCheckCollider_fox;
    float checkColliderRadiusByFoxYSize;


    // 여우몬스터 라이프 관련 
    float hpFox;
    float maxhpFox = 30;
    public bool beDamaged = false;
    float timebetGetDamage = 1f;
    [SerializeField] Transform headShotPoint_fox;
    [SerializeField] PlayerLife whoAttacking;
    public GameObject pung; // 죽을 때 펑 애니메이션 재생
    public GameObject pungSmall; // 아이템 먹었을 때 펑 애니메이션 재생
    public event Action OnDeath; // 죽을 때 이벤트

    //여우몬스터 UI관련
    public Slider _healthSlider;


    private int getHeadShot = Animator.StringToHash("fxHurt");
    private int isFalling = Animator.StringToHash("fxFall");
    private int velocityY = Animator.StringToHash("fxYvelocity");


    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private Animator _animator;
    private PlatformEffector2D _pEffector; // 사다리 오르내릴 때 그라운드 마스크를 껐다켯다 할것이다.
    int originColliderMaskFor_pEffector;
    int IgnoreGroundColliserMaks_pEffector = 128; // 그라운드레이어가 빠진 콜라이더 마스크 


    private void OnEnable()
    {


        headShotPoint_fox = this.transform.GetChild(0).transform; // 헤드샷 포인트콜라이더 가져오기
        groundCheckCollider_fox = this.transform.GetChild(1).transform;


        //여우몬스터 UI관련
        _healthSlider.maxValue = maxhpFox;
        hpFox = maxhpFox;
        _healthSlider.value = hpFox;

        #region 고유랜덤값으로 고유의 스피드,사다리스피드,바운스파워를 가지게 한다. 
        float uniqueRandomValue = Random.Range(0.7f, 1.5f);
        _speed *= uniqueRandomValue;
        _ladderSpeed *= uniqueRandomValue;
        powerMakingAttackingPlayerToBounce *= uniqueRandomValue;
        #endregion 고유랜덤값으로 고유의 스피드,사다리스피드,바운스파워를 가지게 한다. 

    }

    private void Start()
    {
        stateMachine = new StateMachine<AIFoxController>(this, new IdleState_Fox());
        stateMachine.AddState(new MoveState_Fox());
        stateMachine.AddState(new AttackState_Fox());
        stateMachine.AddState(new LadderMoveState_Fox());

        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        // 레이어 콜라이더마스크 관련 
        _pEffector = GetComponent<PlatformEffector2D>();
        originColliderMaskFor_pEffector = _pEffector.colliderMask;
        Debug.Log(originColliderMaskFor_pEffector+ "originColliderMaskFor_pEffector");


        ladderMaskInt = LayerMask.NameToLayer("Ladders");
        monsterMaskInt = this.gameObject.layer;

        checkColliderRadiusByFoxYSize = Math.Abs(headShotPoint_fox.position.y - groundCheckCollider_fox.position.y);
        Debug.Log("checkColliderRadiusByFoxYSize + " + checkColliderRadiusByFoxYSize);


        //아이템과 몬스터 콜라디어 무시하도록 세팅
        Physics2D.IgnoreLayerCollision(this.gameObject.layer, monsterMaskInt);

    }

    //private void FixedUpdate()
    //{
    //    //Debug.Log(stateMachine.CurrentState);
    //}

    private void Update()
    {
          stateMachine.Update(Time.deltaTime);


        if (target)
        {
            _spriteRenderer.flipX = ((target?.position.x - transform.position.x) < 0.1f) ? true : false;
        }


        GetHeadShot();
        EatItems();
        CheckFoxIsFalling();
    }


    public bool IsAvailableAttack
    {
        get
        {
            if (!target)
            {
                return false;
            }

            //도착했다고 가정한다.
            return (HasArrived(target.transform.position, attackRadius));
        }

    }

    public Transform SearchTarget()
    {
        target = null;

        Collider2D[] playerInDetectRange = Physics2D.OverlapCircleAll(transform.position, detectRadius, targetMask);
        if (playerInDetectRange.Length > 0) // 가장먼저 플레이어를 감지하고(플레이어 공격우선)
        {
            //Debug.Log("여우가 플레이어를 배열에 담았다");
            target = playerInDetectRange[0].transform;
        }
        //else // 일단 플레이어만 쫓게 만들자. 
        //{
        //    //플레이어 없으면 근처 아이템 감지한다.
        //    Collider2D[] ItemInDectectRange = Physics2D.OverlapCircleAll(transform.position, detectRadius, maskItem);
        //    if (ItemInDectectRange.Length > 0)
        //    {
        //        target = ItemInDectectRange[0].transform;
        //    }
        //    else
        //    {
        //        target = null;
        //    }
        //}

        return target;

    }

    public void GetHeadShot() //여우가 헤드샷 공격당했을 때 
    {

        

        float damage = 10f;

        Collider2D[] getHeadShotByPlayer = Physics2D.OverlapCircleAll((Vector2)headShotPoint_fox.position,1f, targetMask);

        if (getHeadShotByPlayer.Length > 0)
        {

            whoAttacking = getHeadShotByPlayer[0].GetComponent<PlayerLife>();
            PlayerMovement whoAttackingMove = whoAttacking.GetComponent<PlayerMovement>();
            //whoAttackingMove.AddForcetoBounce(new Vector2(0, 500f));


            Vector2 cp = getHeadShotByPlayer[0].transform.position;
            Vector2 dir = cp-(Vector2)transform.position ; // 플레이어가 튕겨야하므로, 플레이어방향 - 현재 몬스터 방향으로 함 
                                                           //rigidbody.AddForce((dir).normalized * 300f);


            if (StateMachine.CurrentState.GetType() == typeof(LadderMoveState_Fox)) 
            {
                // 여우가 사다리에 있을 때에는 공격을 무시한다.
                return;
            }

            
            whoAttackingMove.AddForcetoBounce((dir).normalized * powerMakingAttackingPlayerToBounce);
            Debug.Log("여우가 바운스해주는 파워는!!! " + (dir).normalized * powerMakingAttackingPlayerToBounce);




            if (!beDamaged)
            {
                hpFox -= damage; // 체력 깍임.
                _healthSlider.value = hpFox; // 체력 UI 표시
                //_animator.SetTrigger("fxHurt");
                _animator.SetTrigger(getHeadShot);

                if (hpFox > 0)
                {
                    AudioManager.instance.PlaySFX("AttackFox");
                }


                if (hpFox <= 0) // 공격을 당해서 체력이 다 쓰임
                {
                    whoAttacking.UpdatePlayerKillUI(1); // 공격한 플레이어의 playerKillUI 올리기.
                    Vector2 DisappearPosition = this.transform.position;
                    GameObject pungPlay = Instantiate(pung, DisappearPosition, Quaternion.identity);
                    AudioManager.instance.PlaySFX("FoxDie");
                    Destroy(this.gameObject);
                    if (OnDeath != null)
                    {
                     OnDeath();
                    }
                    return;
                }

                StartCoroutine(onDamageEffect());
            }
        }


    }

    private IEnumerator onDamageEffect()
    {
        // if (!dead)
        // {
        _spriteRenderer.color = Color.gray;
        beDamaged = true;
        yield return new WaitForSeconds(timebetGetDamage);
        _spriteRenderer.color = Color.white;
        beDamaged = false;
        // }

    }

    public Vector2 getDistance()
    {
        Vector2 distance = target.position - transform.position;
        return distance;
    }


    //워크와 무브 함수구현해서 states들에서 쓸 수 있게 하자!!!

    public void Walk(Vector2 dest) // 좌우이동
    {
        float m_HorizontalMovement = (dest.x - transform.position.x > 0) ? 1 : -1;

        _rigidBody.gravityScale = 9.81f;
        _rigidBody.velocity = new Vector2(_speed * m_HorizontalMovement, _rigidBody.velocity.y);
    }




    public bool isMoveLadderRight() // 사다리 오르는 함수
    {

        bool MovingLadder = false;
        Collider2D[] ladderDetect = Physics2D.OverlapCircleAll(transform.position, ladderDetectRange, ladderMask);

        if (ladderDetect.Length > 0)
        {
            //Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), ladderDetect[0], false);
            //Transform ladderPosition = ladderDetect[0].transform.GetChild(0).GetComponent<Transform>(); // 사다리 위치 가져오기
            GameObject ladderPosition = ladderDetect[0].transform.GetChild(0).gameObject; // 사다리 위치 가져오기
            Transform ladderMiddlePoint = ladderPosition.GetComponent<Transform>();
            //Debug.Log("가고자 하는 사다리 위치 " + ladderPosition.transform.position);

            int resultCompareY = CompareYDirection(target.position, ladderMiddlePoint.position);
            if (resultCompareY == 0)
            {
                //가야할 사다리가 아니면 해당 사다리 콜라이더 무시하기, 움직일 때 사다리에 걸리지 않게 한다. 
                Debug.Log("여우가 사다리를 무시한다.");
                Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), ladderDetect[0], true);

                return MovingLadder; // 일치 안하면 함수 나오기.
            }
            else if(resultCompareY == 1)
            {
                MiddlewayPoint = ladderDetect[0].transform.GetChild(1).gameObject.transform.position;

            }
            else if(resultCompareY == -1)
            {
                MiddlewayPoint = ladderDetect[0].transform.GetChild(2).gameObject.transform.position;

            }

            // [★] 사다리 개별 Tranform으로 바꾸기 전에 썼던 포지션 
            //MiddlewayPoint = (ladderMiddlePoint.position - transform.position) * 1.5f;
            //MiddlewayPoint.x = ladderMiddlePoint.position.x;


            ////사다리 위를 움직이는 물리함수
            //_rigidBody.gravityScale = 0f;
            //_rigidBody.velocity = new Vector2(0, _ladderSpeed * yDirection);
            currentLadder = ladderDetect[0].GetComponent<Collider2D>();
            Debug.Log("여우가 사다리를 무시하지 않는다.");
            Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), currentLadder, false);
            MovingLadder = true;

        }

        return MovingLadder;

    }

    public void MovingLadder(float _moveY) // ★ 여기가 문제인듯하다???
    {
        //Debug.Log("여우가 사다리 위를 오르려고 시도하고 있다." + _moveY);

        //if(!initialTouchingurrentLadder && _mo)
        //Collider2D[] isLadderDownTouched = Physics2D.OverlapCircleAll(groundCheckCollider_fox.position, 0.8f, groundMask);
        //Collider2D[] isLadderUpTouched = Physics2D.OverlapCircleAll(headShotPoint_fox.position, 0.8f, groundMask);

        Collider2D[] isLadderDownTouched = Physics2D.OverlapCircleAll(groundCheckCollider_fox.position, checkColliderRadiusByFoxYSize, ladderMask);
        Collider2D[] isLadderUpTouched = Physics2D.OverlapCircleAll(headShotPoint_fox.position, checkColliderRadiusByFoxYSize, ladderMask);



        if (_collider.IsTouching(currentLadder)) // 사다리에 접촉을 성공한 경우이다. 
        {

            Debug.Log("★ 사다리 접촉성공 되고 있나??");
            _animator.SetBool("fxClimb", true);
            _rigidBody.velocity = new Vector2(0, _ladderSpeed * _moveY);

            if (!initialTouchingurrentLadder) //사다리에 접촉도 하고 첫 접촉도 이뤄진 순간
            {
                initialTouchingurrentLadder = true;
                IgnoreGround(true, "MovingLadder"); // 땅 콜라이더 무시한다.
                ChageGavity(0f);
            }
        }

        //if (_moveY == -1) //땅 콜라이더 무시 
        //{
        //    Collider2D[] isLadderDownTouched = Physics2D.OverlapCircleAll(groundCheckCollider_fox.position, 0.8f, groundMask);
        //    if (isLadderDownTouched.Length > 0)
        //    {
        //        IgnoreGround(true, "MovingLadder + isLadderDownTouched"); // 땅 콜라이더 무시한다.
        //    }
        //    IgnoreGround(false, "MovingLadder + isLadderDownTouched"); // 땅 콜라이더 무시한다.
        //}
        //else if (_moveY == 1)
        //{
        //    Collider2D[] isLadderUpTouched = Physics2D.OverlapCircleAll(headShotPoint_fox.position, 0.8f, groundMask);
        //    if (isLadderUpTouched.Length > 0)
        //    {
        //        IgnoreGround(true, "MovingLadder + isLadderUpTouched"); // 땅 콜라이더 무시한다.
        //    }
        //    IgnoreGround(false, "MovingLadder + isLadderUpTouched"); // 땅 콜라이더 무시한다.
        //}

        if (_moveY == -1) //땅 콜라이더 무시 
        {

            if (isLadderDownTouched.Length > 0)
            {
                IgnoreGround(true, "MovingLadder + isLadderDownTouched"); // 땅 콜라이더 무시한다.
                Physics2D.IgnoreCollision(_collider, groundCollider, true);
                InTheLadder = true;



                _animator.SetBool("fxClimb", true);
                _rigidBody.velocity = new Vector2(0, _ladderSpeed * _moveY);

                if (!initialTouchingurrentLadder) //사다리에 접촉도 하고 첫 접촉도 이뤄진 순간
                {
                    initialTouchingurrentLadder = true;
                    IgnoreGround(true, "MovingLadder"); // 땅 콜라이더 무시한다.
                    ChageGavity(0f);
                }

            }
            else
            {
                InTheLadder = false;
                IgnoreGround(false, "MovingLadder + isLadderDownTouched"); 
            }

        }
        else if (_moveY == 1)
        {
            if (isLadderUpTouched.Length > 0)
            {
                IgnoreGround(true, "MovingLadder + isLadderUpTouched"); // 땅 콜라이더 무시한다.
                Physics2D.IgnoreCollision(_collider, groundCollider, true);



                InTheLadder = true;
                _animator.SetBool("fxClimb", true);
                _rigidBody.velocity = new Vector2(0, _ladderSpeed * _moveY);

                if (!initialTouchingurrentLadder) //사다리에 접촉도 하고 첫 접촉도 이뤄진 순간
                {
                    initialTouchingurrentLadder = true;
                    IgnoreGround(true, "MovingLadder"); // 땅 콜라이더 무시한다.
                    ChageGavity(0f);
                }
            }
            else
            {
                InTheLadder = false;
                IgnoreGround(false, "MovingLadder + isLadderUpTouched"); // 땅 콜라이더 무시한다.
            }

        }


        if (initialTouchingurrentLadder)
        {
            IgnoreGround(true, "MovingLadder"); // 땅 콜라이더 무시한다.

        }

        if (!initialTouchingurrentLadder) // 사다리에 터칭은 못했지만, 사다리에 오르려고 시도하고 있는 경우이다.
        {
            if (_moveY == 1) 
            {
                _rigidBody.velocity = new Vector2(0, _ladderSpeed* 2.5f * _moveY);
            }

            if (_moveY == -1)
            {

                // if(CompareXDistance(MiddlewayPoint.x, 0.5f)) //여기??
                // {
                //    IgnoreGround(true, "MovingLadder + -1");
                //}
                //else
                //{
                //    IgnoreGround(false, "MovingLadder + -1");
                //}

                _rigidBody.velocity = new Vector2(0, _ladderSpeed * _moveY);
            }

        }

        
    }


    public bool InTheLadder;

    public void IgnoreGround(bool value,string fromWhere =null)
    {
        Debug.Log("땅 그라운드를 지금 "+ value.ToString()+ fromWhere+ "체크체크체크체크!!!!!!!!!!!!!");
        Physics2D.IgnoreCollision(_collider, groundCollider, value);
        if (value)
        {
            _pEffector.colliderMask = IgnoreGroundColliserMaks_pEffector;
        }
        else
        {
            _pEffector.colliderMask = originColliderMaskFor_pEffector;
        }
    }



    public void ChageGavity(float value)
    {
        _rigidBody.gravityScale = value;
    }


    // 사다리 이동할지 유무와 사다리 다 올라왔는지(target과의 Y값 검사)를 검사할 때 사용할 함수
    public int CompareYDirection(Vector2 target, Vector2 comparePosition)
    {
        int YDirection = 0; // Y값 일치 안하면 0이다.


        // 내 위치와 타겟의 위치가 일정범위이하일 때 0 도출
        //(== 사다리위치는 당연히 일정범위이상이고, 일정범위 안에 있다면 플레이어가 같은 Floor에 있다고 생각하고 0을 반환하도록 한다.)
        if (target.y - comparePosition.y < 0.5f)
        {
            return YDirection;

        }

        // 사다리 오르내릴 때의 Y방향 도출
        if (target.y - transform.position.y < 0 && comparePosition.y - transform.position.y < 0) { YDirection = -1; }
        if (target.y - transform.position.y > 0 && comparePosition.y - transform.position.y > 0) { YDirection = 1; }

        return YDirection;


    }

    public bool CompareXDistance(float dest, float range)
    {
        bool result = false;
        float XDistance = Mathf.Abs(dest - transform.position.x);
        if (XDistance < range)
        {
            result = true;
        }
        return result;
    }

    public bool HasArrived(Vector2 destination, float minDestinationRadius)
    {
        bool arrived = false;
        float Xresult = Mathf.Abs(destination.x - transform.position.x);
        float Yresult = Mathf.Abs(destination.y - transform.position.y);

        if (Xresult <= minDestinationRadius && Yresult <= minDestinationRadius)
        {
            arrived = true;
        }

        // if(destination.x-transform.position.x<minDestinationRadius &&
        //      destination.y - transform.position.y < minDestinationRadius)
        // {
        //      arrived = true;
        // }

        Debug.Log("도착하였나요? " + arrived);
        return arrived;

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

        }

    }



    public void CheckFoxIsFalling()
    {
        if (stateMachine.CurrentState.GetType() == typeof(LadderMoveState_Fox)) 
        {
            _animator.SetBool("fxClimb", true);
            bool isUpLadder = (MiddlewayPoint.y - transform.position.y) > 0; // 사다리 위로 가기 위함인가?
            if (isUpLadder) // 위로 가는 사다리
            {
                Collider2D[] isLadderUpTouched = Physics2D.OverlapCircleAll(headShotPoint_fox.position, checkColliderRadiusByFoxYSize, ladderMask);
                if (isLadderUpTouched.Length > 0)
                {

                    Debug.Log("사다리에 오르다 CheckFoxIsFalling 되었다. 사다리 위로");
                    return; // 사다리 오르고 있을 때에는 땅 검사 하지 않는다.
                }

            }
            else //아래로 가기위한 사다리
            {
                Collider2D[] isLadderDownTouched = Physics2D.OverlapCircleAll(groundCheckCollider_fox.position, checkColliderRadiusByFoxYSize*1.3f, ladderMask);
                if (isLadderDownTouched.Length > 0)
                {
                    Debug.Log("사다리에 오르다 CheckFoxIsFalling 되었다. 사다리 아래로");
                    return; // 사다리 오르고 있을 때에는 땅 검사 하지 않는다.
                }
            }

            stateMachine.ChangeState<IdleState_Fox>();
        }

        Collider2D[] touchedGrounded = Physics2D.OverlapCircleAll(groundCheckCollider_fox.position, 0.5f, groundMask);

        if (touchedGrounded.Length > 0) // 땅에 닿았다. 
        {
            //Debug.Log("여우가 땅에 닿았다!!!!");
            if (!foxIsGrounded)
            {

                foxIsGrounded = true;
                _animator.SetBool(isFalling, false);
                stateMachine.ChangeState<IdleState_Fox>();  //이거때문인가??
            }

        }
        else // 땅에 닿았다.
        {
            Debug.Log("여우가 땅에 닿지 않았다!!!!");
            if (foxIsGrounded)
            {
                foxIsGrounded = false;
                _animator.SetBool(isFalling, true);
            }

            _animator.SetFloat(velocityY, _rigidBody.velocity.y);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, ladderDetectRange);
    }



}
