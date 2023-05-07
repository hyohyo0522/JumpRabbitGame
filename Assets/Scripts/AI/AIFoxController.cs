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

    private int ladderMaskInt;
    private int monsterMaskInt;


    [SerializeField] public Transform target; // ������ǥ����
    [SerializeField] public Vector2 MiddlewayPoint { get; private set; } // ���� ��ǥ�� ���� ���� �߰� ��ǥ����

    // ��ٸ� ���������� ���� 
    [SerializeField] Collider2D currentLadder; //���� �����Ϸ��� �ϰų�, �̹� �����ϰ� �ִ� ��ٸ�
    [SerializeField] public bool initialTouchingurrentLadder;  // ��ٸ��� ó������ �ö����� Ȯ���ϱ� ���� Bool
    public bool InTheLadder;

    private const float detectRadius = 10f; // Ž���Ÿ�, �����۰� �÷��̾� Ž��
    private const float attackRadius = 2.5f; //�÷��̾� ���ݹ�������, �÷��̾�� �����ߴٰ� �����ϴ� ������
    [SerializeField] const float ladderDetectRange = 3f;
    [SerializeField] float _speed = 10f; // * ������ 
    [SerializeField] float _ladderSpeed = 7f; // ������ ���� ���̴�.
    float powerMakingAttackingPlayerToBounce = 1000; //������ �÷��̾� ������Ű�� ���

    private float damagePower = 10f;
    public float DamagePower => damagePower;



    //���� �ִ��� �˻�
    bool foxIsGrounded;
    [SerializeField] Transform groundCheckCollider_fox;
    float checkColliderRadiusByFoxYSize;


    // ������� ������ ���� 
    float hpFox;
    float maxhpFox = 30;
    public bool beDamaged = false;
    const float timebetGetDamage = 1f;
    [SerializeField] Transform headShotPoint_fox;
    const float RangeOfHeadshotAttacked = 0.7f;
    [SerializeField] PlayerLife whoAttacking;
    public GameObject pung; // �ν����Ϳ��� ���� / ���� �� �� �ִϸ��̼� ��� 
    public GameObject pungSmall; // �ν����Ϳ��� ���� / ������ �Ծ��� �� �� �ִϸ��̼� ���
    public event Action OnDeath; // ���� �� �̺�Ʈ

    //������� UI����
    public Slider _healthSlider; //�ν����Ϳ��� ����


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
        groundCheckCollider_fox = this.transform.GetChild(1).transform; //�׶��� üũ �ݶ��̴� ��������


        //������� UI����
        _healthSlider.maxValue = maxhpFox;
        hpFox = maxhpFox;
        _healthSlider.value = hpFox;

        #region ���������� ���츶�� ������ ���ǵ�,��ٸ����ǵ�,�ٿ�Ŀ��� ������ �Ѵ�. 
        float uniqueRandomValue = Random.Range(0.7f, 1.5f);
        _speed *= uniqueRandomValue;
        _ladderSpeed *= uniqueRandomValue;
        powerMakingAttackingPlayerToBounce *= uniqueRandomValue;
        #endregion ���������� ���츶�� ������ ���ǵ�,��ٸ����ǵ�,�ٿ�Ŀ��� ������ �Ѵ�. 

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


        ladderMaskInt = LayerMask.NameToLayer("Ladders");
        monsterMaskInt = this.gameObject.layer;

        checkColliderRadiusByFoxYSize = Math.Abs(headShotPoint_fox.position.y - groundCheckCollider_fox.position.y);


        //���� �ݶ��̴� �����ϵ��� ����
        Physics2D.IgnoreLayerCollision(this.gameObject.layer, monsterMaskInt);

    }


    private void Update()
    {
        stateMachine.Update(Time.deltaTime);


        if (target) //�÷��̾� ������ �ø�x
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

        return target;

    }

    public void GetHeadShot() //���찡 ��弦 ���ݴ����� �� 
    {

        float damage = 10f;

        Collider2D[] getHeadShotByPlayer = Physics2D.OverlapCircleAll((Vector2)headShotPoint_fox.position, RangeOfHeadshotAttacked, targetMask);

        if (getHeadShotByPlayer.Length > 0)
        {
            Collider2D playerWhoAttacked = getHeadShotByPlayer[0];

            whoAttacking = playerWhoAttacked.GetComponent<PlayerLife>();
            PlayerMovement whoAttackingMove = whoAttacking.GetComponent<PlayerMovement>();
            Vector2 cp = playerWhoAttacked.transform.position;


            Vector2 dir = cp-(Vector2)transform.position ; // �÷��̾ ƨ�ܾ��ϹǷ�, �÷��̾���� - ���� ���� �������� �� 
                                                           // rigidbody.AddForce((dir).normalized * 300f);


            if (StateMachine.CurrentState.GetType() == typeof(LadderMoveState_Fox)) 
            {
                //��ٸ� ���¿����� ���� ������ �ʴ´�. 
                return;
            }

            
            whoAttackingMove.AddForcetoBounce((dir).normalized * powerMakingAttackingPlayerToBounce);




            if (!beDamaged)
            {
                hpFox -= damage; // ü�� ����.
                _healthSlider.value = hpFox; // ü�� UI ǥ��
                _animator.SetTrigger(getHeadShot);

                if (hpFox > 0)
                {
                    AudioManager.instance.PlaySFX("AttackFox");
                }


                if (hpFox <= 0) // ü�� ��� ����
                {
                    whoAttacking.UpdatePlayerKillUI(1); // ������ �÷��̾��� playerKillUI �ø���.
                    Vector2 DisappearPosition = this.transform.position;
                    GameObject pungPlay = Instantiate(pung, DisappearPosition, Quaternion.identity);
                    AudioManager.instance.PlaySFX("FoxDie");
                    OnDeath?.Invoke();
                    Destroy(this.gameObject);
                    return;
                }

                StartCoroutine(onDamageEffect());
            }
        }


    }

    private IEnumerator onDamageEffect()
    {

        _spriteRenderer.color = Color.gray;
        beDamaged = true;

        var wait = new WaitForSeconds(timebetGetDamage);
        yield return (wait);
        _spriteRenderer.color = Color.white;
        beDamaged = false;


    }

    public Vector2 getDistance()
    {
        Vector2 distance = target.position - transform.position;
        return distance;
    }


    public void Walk(Vector2 dest) // �¿��̵�
    {
        Debug.Log("������� �Ƚ��ϴ�! walk!");
        float m_HorizontalMovement = (dest.x - transform.position.x > 0) ? 1 : -1;

        _rigidBody.gravityScale = 9.81f; //���� �� �߷� ���� 
        _rigidBody.velocity = new Vector2(_speed * m_HorizontalMovement, _rigidBody.velocity.y);
    }




    public bool isMoveLadderRight() // ��ٸ� ���������� �ؾ��ϴ��� üũ�ϴ� �Լ�
    {

        bool MovingLadder = false;
        Collider2D[] ladderDetect = Physics2D.OverlapCircleAll(transform.position, ladderDetectRange, ladderMask);

        if (ladderDetect.Length > 0)
        {
            Debug.Log("������ٸ� : ladderDetect.Length > 0 ����");
            GameObject ladderPosition = ladderDetect[0].transform.GetChild(0).gameObject; // ��ٸ� ��ġ ��������
            Transform ladderMiddlePoint = ladderPosition.GetComponent<Transform>();


            int resultCompareY = CompareYDirection(target.position, ladderMiddlePoint.position);
            if (resultCompareY == 0)
            {

                Physics2D.IgnoreCollision(_collider, ladderDetect[0], true);

                Debug.Log("������ٸ� : resultCompareY == 0 �̴�.");

                return MovingLadder; // ��ٸ� �����ϰ� ������
            }
            else if(resultCompareY == 1)
            {
                MiddlewayPoint = ladderDetect[0].transform.GetChild(1).gameObject.transform.position;
                Debug.Log("������ٸ� : resultCompareY == 1 �̴�.");

            }
            else if(resultCompareY == -1)
            {
                MiddlewayPoint = ladderDetect[0].transform.GetChild(2).gameObject.transform.position;
                Debug.Log("������ٸ� : resultCompareY == -1 �̴�.");

            }

            currentLadder = ladderDetect[0];
            //currentLadder = ladderDetect[0].GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(_collider, currentLadder, false); ;
            MovingLadder = true;
            Debug.Log("������ٸ� :  �������� �ʰ� MovingLadder�� true�� �������.");
            //�� ���⼭ �� ���� ���� �ʸӷ� �� �Ѿư� 
        }

        Debug.Log($"������ٸ��� MovingLadder ���� :  {MovingLadder}");
        return MovingLadder;

    }

    public void MovingLadder(float _moveY) // �� ���Ⱑ �����ε��ϴ�???
    {


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


        if (_moveY == -1) //�� �ݶ��̴� ���� 
        {

            if (isLadderDownTouched.Length > 0)
            {
                ClimbLadder(_moveY);

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
                ClimbLadder(_moveY);
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

        if (!initialTouchingurrentLadder) // ��ٸ��� ��Ī�� ��������, ��ٸ��� �������� �õ��ϰ� �ִ� ��� 
        {
            if (_moveY == 1) 
            {
                _rigidBody.velocity = new Vector2(0, _ladderSpeed* 2.5f * _moveY);
            }

            if (_moveY == -1)
            {


                _rigidBody.velocity = new Vector2(0, _ladderSpeed * _moveY);
            }

        }

    }

    void ClimbLadder(float moveYDirection)
    {
        string ladderMoveingInfoMsg = (moveYDirection == -1) ? "isLadderDownTouched" : 
            (moveYDirection == 1) ? "isLadderUpTouched" : "NoLadderDirectionInfo";


        IgnoreGround(true, "MovingLadder " + ladderMoveingInfoMsg + " Is True"); // �� �ݶ��̴� �����Ѵ�.
        Physics2D.IgnoreCollision(_collider, groundCollider, true);
        InTheLadder = true;


        _animator.SetBool("fxClimb", true);
        _rigidBody.velocity = new Vector2(0, _ladderSpeed * moveYDirection);

        if (!initialTouchingurrentLadder) //��ٸ��� ���˵� �ϰ� ù ���˵� �̷��� ����
        {
            initialTouchingurrentLadder = true;
            IgnoreGround(true, "MovingLadder" + "initialTouchingurrentLadder : true"); // �� �ݶ��̴� �����Ѵ�.
            ChageGavity(0f);
        }
    }





    public void IgnoreGround(bool value,string fromWhere =null)
    {

        //��ٸ� ���� ���� üũ�� ����� �޽��� 
        // Debug.Log(fromWhere);
        
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

    public int CompareYDirection(Vector2 target, Vector2 comparePosition)
    {
        int YDirection = 0; // Y�� ��ġ ���ϸ� 0�̴�.


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

                    return; // ��ٸ� ������ ���� ������ �� �˻� ���� �ʴ´�.
                }

            }
            else //�Ʒ��� �������� ��ٸ�
            {
                Collider2D[] isLadderDownTouched = Physics2D.OverlapCircleAll(groundCheckCollider_fox.position, checkColliderRadiusByFoxYSize*1.3f, ladderMask);
                if (isLadderDownTouched.Length > 0)
                {
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
                Debug.Log("������ٸ� : CheckFoxIsFalling���� Idle�� �ٲ���ϴ�.");
                stateMachine.ChangeState<IdleState_Fox>();  
            }


        }
        else // ���� ��Ҵ�.
        {

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
