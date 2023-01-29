using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class AIFoxController : MonoBehaviour
{
    protected StateMachine<AIFoxController> stateMachine;
    public StateMachine<AIFoxController> StateMachine => stateMachine;



    public LayerMask ladderMask;// ��ٸ� ���̾� ����ũ
    public LayerMask targetMask;
    public LayerMask ItemMask;
    public LayerMask groundMask;
    public Collider2D groundCollider;


    [SerializeField] public Transform target; // ������ǥ����
    [SerializeField] public Vector2 MiddlewayPoint; // ���� ��ǥ�� ���� ���� �߰� ��ǥ����

    [SerializeField] Collider2D currentLadder; //���� �����Ϸ��� �ϰų�, �̹� �����ϰ� �ִ� ��ٸ�
    [SerializeField] public bool initialTouchingurrentLadder;  // ��ٸ��� ó������ �ö����� ���ϰ� �ִ� ��ٸ�


    public float detectRadius = 300f; // Ž���Ÿ�, �����۰� �÷��̾� Ž��
    float attackRadius = 2.5f; //�÷��̾� ���ݹ�������, �÷��̾�� �����ߴٰ� �����ϴ� ������
    [SerializeField] float ladderDetectRange = 3f;
    [SerializeField] float _speed = 2f;
    [SerializeField] float _ladderSpeed = 7f;
    public float damagePower = 10f;
    public float powerMakingAttackingPlayerToBounce = 600f;
    float velocityX; // Rigidbody2D���� x�ӵ��� �����ͼ� FlipX�� ������ ���̴�.

    //���� �ִ��� �˻�
    bool foxIsGrounded;
    [SerializeField] Transform groundCheckCollider_fox;


    // ������� ������ ���� 
    float hpFox;
    float maxhpFox = 50;
    bool beDamaged = false;
    float timebetGetDamage = 0.5f;
    [SerializeField] Transform headShotPoint_fox;
    [SerializeField] PlayerLife whoAttacking;
    public GameObject pung; // ���� �� �� �ִϸ��̼� ���
    public GameObject pungSmall; // ������ �Ծ��� �� �� �ִϸ��̼� ���
    float pungAniPlayTime = 0.55f;
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

    private void OnEnable()
    {

        headShotPoint_fox = this.transform.GetChild(0).transform; // ��弦 ����Ʈ�ݶ��̴� ��������
        groundCheckCollider_fox = this.transform.GetChild(1).transform;


        //������� UI����
        _healthSlider.maxValue = maxhpFox;
        hpFox = maxhpFox;
        _healthSlider.value = hpFox;



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

    }

    private void Update()
    {
        stateMachine.Update(Time.deltaTime);

        //Debug.Log(stateMachine.CurrentState);


        velocityX = _rigidBody.velocity.x;
        if (_rigidBody.velocity.x != 0)
        {
            _spriteRenderer.flipX =
            ((target?.position.x - transform.position.x) < 0.1f) ? true : false;
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
            Debug.Log("���찡 �÷��̾ �迭�� ��Ҵ�");
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

    public void GetHeadShot() //��弦 ���ݴ����� �� 
    {

        float damage = 10f;

        Collider2D[] getHeadShotByPlayer = Physics2D.OverlapCircleAll((Vector2)headShotPoint_fox.position, 0.5f, targetMask);

        if (getHeadShotByPlayer.Length > 0)
        {
            whoAttacking = getHeadShotByPlayer[0].GetComponent<PlayerLife>();
            PlayerMovement whoAttackingMove = whoAttacking.GetComponent<PlayerMovement>();
            //whoAttackingMove.AddForcetoBounce(new Vector2(0, 500f));


            Vector2 cp = getHeadShotByPlayer[0].transform.position;
            Vector2 dir = cp - (Vector2)transform.position; // �÷��̾ ƨ�ܾ��ϹǷ�, �÷��̾���� - ���� ���� �������� �� 
                                                            //rigidbody.AddForce((dir).normalized * 300f);

            whoAttackingMove.AddForcetoBounce((dir).normalized * powerMakingAttackingPlayerToBounce);



            if (!beDamaged)
            {
                hpFox -= damage; // ü�� ����.
                _healthSlider.value = hpFox; // ü�� UI ǥ��
                //_animator.SetTrigger("fxHurt");
                _animator.SetTrigger(getHeadShot);


                if (hpFox <= 0) // ������ ���ؼ� ü���� �� ����
                {
                    whoAttacking.UpdatePlayerKillUI(1); // ������ �÷��̾��� playerKillUI �ø���.
                    Vector2 DisappearPosition = this.transform.position;
                    GameObject pungPlay = Instantiate(pung, DisappearPosition, Quaternion.identity);
                    Destroy(pungPlay.gameObject, pungAniPlayTime);
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

            Transform ladderPosition = ladderDetect[0].transform.GetChild(0).GetComponent<Transform>(); // ��ٸ� ��ġ ��������
            Debug.Log("������ �ϴ� ��ٸ� ��ġ " + ladderPosition.transform.position);

            if (CompareYDirection(target.position, ladderPosition.position) == 0) return MovingLadder; // ��ġ ���ϸ� �Լ� ������.


            MiddlewayPoint = (ladderPosition.position - transform.position) * 2.05f;
            MiddlewayPoint.x = ladderPosition.position.x;

            ////��ٸ� ���� �����̴� �����Լ�
            //_rigidBody.gravityScale = 0f;
            //_rigidBody.velocity = new Vector2(0, _ladderSpeed * yDirection);
            currentLadder = ladderDetect[0].GetComponent<Collider2D>();
            MovingLadder = true;

        }

        return MovingLadder;

    }

    public void MovingLadder(float _moveY)
    {
        Debug.Log("���찡 ��ٸ� ���� �������� �õ��ϰ� �ִ�." + _moveY);


        if (_collider.IsTouching(currentLadder))
        {
            _animator.SetBool("fxClimb", true);
            _rigidBody.velocity = new Vector2(0, _ladderSpeed * _moveY);

            if (!initialTouchingurrentLadder)
            {
                initialTouchingurrentLadder = true;
                Physics2D.IgnoreCollision(_collider, groundCollider, true);
                ChageGavity(0f);
            }
        }

        if (!initialTouchingurrentLadder)
        {
            if (_moveY == 1)
            {
                _rigidBody.velocity = new Vector2(0, _ladderSpeed * _moveY);
            }

            if (_moveY == -1)
            {
                Physics2D.IgnoreCollision(_collider, groundCollider, true);
                _rigidBody.velocity = new Vector2(0, _ladderSpeed * _moveY);
            }

        }

    }


    public bool InTheLadder()
    {

        // �ֺ��� ��ٸ��� �ִ��� Ž���ϴ� ��ɰ��� �ٸ� ����̴�,
        // ��ٸ� �ȿ� �ִٰ� �������� �浹�̳� ��Ÿ������ ��ٸ��� ������ ���������� Ȯ��
        // ������ �������ٸ� �ٷ� LadderState���� MoveState�� ��ȯ�ϰ� �� ���̴�.

        //������ �����ϰ� �ִ� ��ٸ��� ������������ Ȯ���� ���̴�.
        bool result = false;
        result = _collider.IsTouching(currentLadder);

        return result;

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
            Destroy(ItemDisappearPlay.gameObject, pungAniPlayTime);

        }

    }



    void CheckFoxIsFalling()
    {
        if (stateMachine.CurrentState.GetType() == typeof(LadderMoveState_Fox)) return; // ��ٸ� ������ ���� ������ �� �˻� ���� �ʴ´�.
        Collider2D[] touchedGrounded = Physics2D.OverlapCircleAll(groundCheckCollider_fox.position, 0.5f, groundMask);

        if (touchedGrounded.Length <= 0) // ���� ����. 
        {

            Debug.Log("���찡 ���� ���� �ʾҴ�!!!!");
            if (foxIsGrounded)
            {
                foxIsGrounded = false;
                _animator.SetBool(isFalling, true);
            }

            _animator.SetFloat(velocityY, _rigidBody.velocity.y);

        }
        else // ���� ��Ҵ�.
        {

            Debug.Log("���찡 ���� ��Ҵ�!!!!");
            if (!foxIsGrounded)
            {
                foxIsGrounded = true;
                _animator.SetBool(isFalling, false);
                stateMachine.ChangeState<IdleState_Fox>();
            }
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
