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



    public LayerMask ladderMask;// ��ٸ� ���̾� ����ũ
    public LayerMask targetMask;
    public LayerMask ItemMask;
    public LayerMask groundMask;
    public Collider2D groundCollider;

    public int ladderMaskInt;
    public int monsterMaskInt;


    [SerializeField] public Transform target; // ������ǥ����
    [SerializeField] public Vector2 MiddlewayPoint; // ���� ��ǥ�� ���� ���� �߰� ��ǥ����

    [SerializeField] Collider2D currentLadder; //���� �����Ϸ��� �ϰų�, �̹� �����ϰ� �ִ� ��ٸ�
    [SerializeField] public bool initialTouchingurrentLadder;  // ��ٸ��� ó������ �ö����� ���ϰ� �ִ� ��ٸ�


    public float detectRadius = 500f; // Ž���Ÿ�, �����۰� �÷��̾� Ž��
    float attackRadius = 2.5f; //�÷��̾� ���ݹ�������, �÷��̾�� �����ߴٰ� �����ϴ� ������
    [SerializeField] float ladderDetectRange = 3f;
    [SerializeField] float _speed = 2f;
    [SerializeField] float _ladderSpeed = 7f;
    float powerMakingAttackingPlayerToBounce = 1000; //������ �÷��̾� ������Ű�� ���

    public float damagePower = 10f;

    //float velocityX; // Rigidbody2D���� x�ӵ��� �����ͼ� FlipX�� ������ ���̴�.

    //���� �ִ��� �˻�
    bool foxIsGrounded;
    [SerializeField] Transform groundCheckCollider_fox;
    float checkColliderRadiusByFoxYSize;


    // ������� ������ ���� 
    float hpFox;
    float maxhpFox = 30;
    public bool beDamaged = false;
    float timebetGetDamage = 1f;
    [SerializeField] Transform headShotPoint_fox;
    [SerializeField] PlayerLife whoAttacking;
    public GameObject pung; // ���� �� �� �ִϸ��̼� ���
    public GameObject pungSmall; // ������ �Ծ��� �� �� �ִϸ��̼� ���
    public event Action OnDeath; // ���� �� �̺�Ʈ

    //������� UI����
    public Slider _healthSlider;


    private int getHeadShot = Animator.StringToHash("fxHurt");
    private int isFalling = Animator.StringToHash("fxFall");
    private int velocityY = Animator.StringToHash("fxYvelocity");


    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;
    private Animator _animator;
    private PlatformEffector2D _pEffector; // ��ٸ� �������� �� �׶��� ����ũ�� �����ִ� �Ұ��̴�.
    int originColliderMaskFor_pEffector;
    int IgnoreGroundColliserMaks_pEffector = 128; // �׶��巹�̾ ���� �ݶ��̴� ����ũ 


    private void OnEnable()
    {


        headShotPoint_fox = this.transform.GetChild(0).transform; // ��弦 ����Ʈ�ݶ��̴� ��������
        groundCheckCollider_fox = this.transform.GetChild(1).transform;


        //������� UI����
        _healthSlider.maxValue = maxhpFox;
        hpFox = maxhpFox;
        _healthSlider.value = hpFox;

        #region �������������� ������ ���ǵ�,��ٸ����ǵ�,�ٿ�Ŀ��� ������ �Ѵ�. 
        float uniqueRandomValue = Random.Range(0.7f, 1.5f);
        _speed *= uniqueRandomValue;
        _ladderSpeed *= uniqueRandomValue;
        powerMakingAttackingPlayerToBounce *= uniqueRandomValue;
        #endregion �������������� ������ ���ǵ�,��ٸ����ǵ�,�ٿ�Ŀ��� ������ �Ѵ�. 

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

        // ���̾� �ݶ��̴�����ũ ���� 
        _pEffector = GetComponent<PlatformEffector2D>();
        originColliderMaskFor_pEffector = _pEffector.colliderMask;
        Debug.Log(originColliderMaskFor_pEffector+ "originColliderMaskFor_pEffector");


        ladderMaskInt = LayerMask.NameToLayer("Ladders");
        monsterMaskInt = this.gameObject.layer;

        checkColliderRadiusByFoxYSize = Math.Abs(headShotPoint_fox.position.y - groundCheckCollider_fox.position.y);
        Debug.Log("checkColliderRadiusByFoxYSize + " + checkColliderRadiusByFoxYSize);


        //�����۰� ���� �ݶ��� �����ϵ��� ����
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

            //�����ߴٰ� �����Ѵ�.
            return (HasArrived(target.transform.position, attackRadius));
        }

    }

    public Transform SearchTarget()
    {
        target = null;

        Collider2D[] playerInDetectRange = Physics2D.OverlapCircleAll(transform.position, detectRadius, targetMask);
        if (playerInDetectRange.Length > 0) // ������� �÷��̾ �����ϰ�(�÷��̾� ���ݿ켱)
        {
            //Debug.Log("���찡 �÷��̾ �迭�� ��Ҵ�");
            target = playerInDetectRange[0].transform;
        }
        //else // �ϴ� �÷��̾ �Ѱ� ������. 
        //{
        //    //�÷��̾� ������ ��ó ������ �����Ѵ�.
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

    public void GetHeadShot() //���찡 ��弦 ���ݴ����� �� 
    {

        

        float damage = 10f;

        Collider2D[] getHeadShotByPlayer = Physics2D.OverlapCircleAll((Vector2)headShotPoint_fox.position,1f, targetMask);

        if (getHeadShotByPlayer.Length > 0)
        {

            whoAttacking = getHeadShotByPlayer[0].GetComponent<PlayerLife>();
            PlayerMovement whoAttackingMove = whoAttacking.GetComponent<PlayerMovement>();
            //whoAttackingMove.AddForcetoBounce(new Vector2(0, 500f));


            Vector2 cp = getHeadShotByPlayer[0].transform.position;
            Vector2 dir = cp-(Vector2)transform.position ; // �÷��̾ ƨ�ܾ��ϹǷ�, �÷��̾���� - ���� ���� �������� �� 
                                                           //rigidbody.AddForce((dir).normalized * 300f);


            if (StateMachine.CurrentState.GetType() == typeof(LadderMoveState_Fox)) 
            {
                // ���찡 ��ٸ��� ���� ������ ������ �����Ѵ�.
                return;
            }

            
            whoAttackingMove.AddForcetoBounce((dir).normalized * powerMakingAttackingPlayerToBounce);
            Debug.Log("���찡 �ٿ���ִ� �Ŀ���!!! " + (dir).normalized * powerMakingAttackingPlayerToBounce);




            if (!beDamaged)
            {
                hpFox -= damage; // ü�� ����.
                _healthSlider.value = hpFox; // ü�� UI ǥ��
                //_animator.SetTrigger("fxHurt");
                _animator.SetTrigger(getHeadShot);

                if (hpFox > 0)
                {
                    AudioManager.instance.PlaySFX("AttackFox");
                }


                if (hpFox <= 0) // ������ ���ؼ� ü���� �� ����
                {
                    whoAttacking.UpdatePlayerKillUI(1); // ������ �÷��̾��� playerKillUI �ø���.
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


    //��ũ�� ���� �Լ������ؼ� states�鿡�� �� �� �ְ� ����!!!

    public void Walk(Vector2 dest) // �¿��̵�
    {
        float m_HorizontalMovement = (dest.x - transform.position.x > 0) ? 1 : -1;

        _rigidBody.gravityScale = 9.81f;
        _rigidBody.velocity = new Vector2(_speed * m_HorizontalMovement, _rigidBody.velocity.y);
    }




    public bool isMoveLadderRight() // ��ٸ� ������ �Լ�
    {

        bool MovingLadder = false;
        Collider2D[] ladderDetect = Physics2D.OverlapCircleAll(transform.position, ladderDetectRange, ladderMask);

        if (ladderDetect.Length > 0)
        {
            //Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), ladderDetect[0], false);
            //Transform ladderPosition = ladderDetect[0].transform.GetChild(0).GetComponent<Transform>(); // ��ٸ� ��ġ ��������
            GameObject ladderPosition = ladderDetect[0].transform.GetChild(0).gameObject; // ��ٸ� ��ġ ��������
            Transform ladderMiddlePoint = ladderPosition.GetComponent<Transform>();
            //Debug.Log("������ �ϴ� ��ٸ� ��ġ " + ladderPosition.transform.position);

            int resultCompareY = CompareYDirection(target.position, ladderMiddlePoint.position);
            if (resultCompareY == 0)
            {
                //������ ��ٸ��� �ƴϸ� �ش� ��ٸ� �ݶ��̴� �����ϱ�, ������ �� ��ٸ��� �ɸ��� �ʰ� �Ѵ�. 
                Debug.Log("���찡 ��ٸ��� �����Ѵ�.");
                Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), ladderDetect[0], true);

                return MovingLadder; // ��ġ ���ϸ� �Լ� ������.
            }
            else if(resultCompareY == 1)
            {
                MiddlewayPoint = ladderDetect[0].transform.GetChild(1).gameObject.transform.position;

            }
            else if(resultCompareY == -1)
            {
                MiddlewayPoint = ladderDetect[0].transform.GetChild(2).gameObject.transform.position;

            }

            // [��] ��ٸ� ���� Tranform���� �ٲٱ� ���� ��� ������ 
            //MiddlewayPoint = (ladderMiddlePoint.position - transform.position) * 1.5f;
            //MiddlewayPoint.x = ladderMiddlePoint.position.x;


            ////��ٸ� ���� �����̴� �����Լ�
            //_rigidBody.gravityScale = 0f;
            //_rigidBody.velocity = new Vector2(0, _ladderSpeed * yDirection);
            currentLadder = ladderDetect[0].GetComponent<Collider2D>();
            Debug.Log("���찡 ��ٸ��� �������� �ʴ´�.");
            Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), currentLadder, false);
            MovingLadder = true;

        }

        return MovingLadder;

    }

    public void MovingLadder(float _moveY) // �� ���Ⱑ �����ε��ϴ�???
    {
        //Debug.Log("���찡 ��ٸ� ���� �������� �õ��ϰ� �ִ�." + _moveY);

        //if(!initialTouchingurrentLadder && _mo)
        //Collider2D[] isLadderDownTouched = Physics2D.OverlapCircleAll(groundCheckCollider_fox.position, 0.8f, groundMask);
        //Collider2D[] isLadderUpTouched = Physics2D.OverlapCircleAll(headShotPoint_fox.position, 0.8f, groundMask);

        Collider2D[] isLadderDownTouched = Physics2D.OverlapCircleAll(groundCheckCollider_fox.position, checkColliderRadiusByFoxYSize, ladderMask);
        Collider2D[] isLadderUpTouched = Physics2D.OverlapCircleAll(headShotPoint_fox.position, checkColliderRadiusByFoxYSize, ladderMask);



        if (_collider.IsTouching(currentLadder)) // ��ٸ��� ������ ������ ����̴�. 
        {

            Debug.Log("�� ��ٸ� ���˼��� �ǰ� �ֳ�??");
            _animator.SetBool("fxClimb", true);
            _rigidBody.velocity = new Vector2(0, _ladderSpeed * _moveY);

            if (!initialTouchingurrentLadder) //��ٸ��� ���˵� �ϰ� ù ���˵� �̷��� ����
            {
                initialTouchingurrentLadder = true;
                IgnoreGround(true, "MovingLadder"); // �� �ݶ��̴� �����Ѵ�.
                ChageGavity(0f);
            }
        }

        //if (_moveY == -1) //�� �ݶ��̴� ���� 
        //{
        //    Collider2D[] isLadderDownTouched = Physics2D.OverlapCircleAll(groundCheckCollider_fox.position, 0.8f, groundMask);
        //    if (isLadderDownTouched.Length > 0)
        //    {
        //        IgnoreGround(true, "MovingLadder + isLadderDownTouched"); // �� �ݶ��̴� �����Ѵ�.
        //    }
        //    IgnoreGround(false, "MovingLadder + isLadderDownTouched"); // �� �ݶ��̴� �����Ѵ�.
        //}
        //else if (_moveY == 1)
        //{
        //    Collider2D[] isLadderUpTouched = Physics2D.OverlapCircleAll(headShotPoint_fox.position, 0.8f, groundMask);
        //    if (isLadderUpTouched.Length > 0)
        //    {
        //        IgnoreGround(true, "MovingLadder + isLadderUpTouched"); // �� �ݶ��̴� �����Ѵ�.
        //    }
        //    IgnoreGround(false, "MovingLadder + isLadderUpTouched"); // �� �ݶ��̴� �����Ѵ�.
        //}

        if (_moveY == -1) //�� �ݶ��̴� ���� 
        {

            if (isLadderDownTouched.Length > 0)
            {
                IgnoreGround(true, "MovingLadder + isLadderDownTouched"); // �� �ݶ��̴� �����Ѵ�.
                Physics2D.IgnoreCollision(_collider, groundCollider, true);
                InTheLadder = true;



                _animator.SetBool("fxClimb", true);
                _rigidBody.velocity = new Vector2(0, _ladderSpeed * _moveY);

                if (!initialTouchingurrentLadder) //��ٸ��� ���˵� �ϰ� ù ���˵� �̷��� ����
                {
                    initialTouchingurrentLadder = true;
                    IgnoreGround(true, "MovingLadder"); // �� �ݶ��̴� �����Ѵ�.
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
                IgnoreGround(true, "MovingLadder + isLadderUpTouched"); // �� �ݶ��̴� �����Ѵ�.
                Physics2D.IgnoreCollision(_collider, groundCollider, true);



                InTheLadder = true;
                _animator.SetBool("fxClimb", true);
                _rigidBody.velocity = new Vector2(0, _ladderSpeed * _moveY);

                if (!initialTouchingurrentLadder) //��ٸ��� ���˵� �ϰ� ù ���˵� �̷��� ����
                {
                    initialTouchingurrentLadder = true;
                    IgnoreGround(true, "MovingLadder"); // �� �ݶ��̴� �����Ѵ�.
                    ChageGavity(0f);
                }
            }
            else
            {
                InTheLadder = false;
                IgnoreGround(false, "MovingLadder + isLadderUpTouched"); // �� �ݶ��̴� �����Ѵ�.
            }

        }


        if (initialTouchingurrentLadder)
        {
            IgnoreGround(true, "MovingLadder"); // �� �ݶ��̴� �����Ѵ�.

        }

        if (!initialTouchingurrentLadder) // ��ٸ��� ��Ī�� ��������, ��ٸ��� �������� �õ��ϰ� �ִ� ����̴�.
        {
            if (_moveY == 1) 
            {
                _rigidBody.velocity = new Vector2(0, _ladderSpeed* 2.5f * _moveY);
            }

            if (_moveY == -1)
            {

                // if(CompareXDistance(MiddlewayPoint.x, 0.5f)) //����??
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
        Debug.Log("�� �׶��带 ���� "+ value.ToString()+ fromWhere+ "üũüũüũüũ!!!!!!!!!!!!!");
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


    // ��ٸ� �̵����� ������ ��ٸ� �� �ö�Դ���(target���� Y�� �˻�)�� �˻��� �� ����� �Լ�
    public int CompareYDirection(Vector2 target, Vector2 comparePosition)
    {
        int YDirection = 0; // Y�� ��ġ ���ϸ� 0�̴�.


        // �� ��ġ�� Ÿ���� ��ġ�� �������������� �� 0 ����
        //(== ��ٸ���ġ�� �翬�� ���������̻��̰�, �������� �ȿ� �ִٸ� �÷��̾ ���� Floor�� �ִٰ� �����ϰ� 0�� ��ȯ�ϵ��� �Ѵ�.)
        if (target.y - comparePosition.y < 0.5f)
        {
            return YDirection;

        }

        // ��ٸ� �������� ���� Y���� ����
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

        Debug.Log("�����Ͽ�����? " + arrived);
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
            bool isUpLadder = (MiddlewayPoint.y - transform.position.y) > 0; // ��ٸ� ���� ���� �����ΰ�?
            if (isUpLadder) // ���� ���� ��ٸ�
            {
                Collider2D[] isLadderUpTouched = Physics2D.OverlapCircleAll(headShotPoint_fox.position, checkColliderRadiusByFoxYSize, ladderMask);
                if (isLadderUpTouched.Length > 0)
                {

                    Debug.Log("��ٸ��� ������ CheckFoxIsFalling �Ǿ���. ��ٸ� ����");
                    return; // ��ٸ� ������ ���� ������ �� �˻� ���� �ʴ´�.
                }

            }
            else //�Ʒ��� �������� ��ٸ�
            {
                Collider2D[] isLadderDownTouched = Physics2D.OverlapCircleAll(groundCheckCollider_fox.position, checkColliderRadiusByFoxYSize*1.3f, ladderMask);
                if (isLadderDownTouched.Length > 0)
                {
                    Debug.Log("��ٸ��� ������ CheckFoxIsFalling �Ǿ���. ��ٸ� �Ʒ���");
                    return; // ��ٸ� ������ ���� ������ �� �˻� ���� �ʴ´�.
                }
            }

            stateMachine.ChangeState<IdleState_Fox>();
        }

        Collider2D[] touchedGrounded = Physics2D.OverlapCircleAll(groundCheckCollider_fox.position, 0.5f, groundMask);

        if (touchedGrounded.Length > 0) // ���� ��Ҵ�. 
        {
            //Debug.Log("���찡 ���� ��Ҵ�!!!!");
            if (!foxIsGrounded)
            {

                foxIsGrounded = true;
                _animator.SetBool(isFalling, false);
                stateMachine.ChangeState<IdleState_Fox>();  //�̰Ŷ����ΰ�??
            }

        }
        else // ���� ��Ҵ�.
        {
            Debug.Log("���찡 ���� ���� �ʾҴ�!!!!");
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
