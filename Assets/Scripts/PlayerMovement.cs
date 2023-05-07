using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update

    //������Ʈ ��������
    public Animator playerAnimator; //PlayerLife.cs���� �ѹ� ȣ���� �� ����
    SpriteRenderer playerSprite;
    Rigidbody2D playerRigidbody;
    Collider2D playerCollider;

    // �� ���̵� ����
    string _myNick;

    private float speed = 12f; // �¿콺�ǵ尪

    //�¿��Է�Ű ��������
    float m_HorizontalMovement;
    float m_VerticalMovement;


    //���� ī��Ʈ ���� ����(��� ����)
    private int jumpCount = 0;  //��� �Ծ ���� ���� ī��Ʈ
    private int jumpMaxCount = 1;
    private int enableJumpCount = 10; // ��� ����, ���� ������ Ƚ��
    private int countForWaringCarrotShortage = 5; // ��� ������ ����� �� ��� ����
    private int maxCarrot = 100; //��� �ִ� ����

    //���ı� + ��پ�� ����
    public GameObject CarrotInTheGround; //�ν����Ϳ��� �Ҵ�
    float carrotBouncePowerY = 500f;
    Vector2 carrotBouncePower;


    //Groundüũ
    [SerializeField] Transform groundCheckCollider;
    [SerializeField] Transform ladderColliderCheck1;
    [SerializeField] Transform ladderColliderCheck2;

    //������ üũ 
    [SerializeField] Transform ItemCheckCollider;

    //��弦üũ
    [SerializeField] Transform HeadShotCollider;
    float bouncePowerforOtherInMyHead = 600f;


    public LayerMask groundMask; 
    public LayerMask LadderMask;
    public LayerMask ItemMask;
    public LayerMask ItemBeeMask;

    // IgnoreLayerCollision �Լ��� Ȱ���ϱ� ���� ���� 
    int groundMaskInt;
    int playerMaskInt;
    int ItemBeeMaskInt;

    const float groundCheckRadius = 0.6f;
    public bool isGrounded;
    bool isLadder = false;

    public bool JumpEnable = true; // ��ٸ� �ö���ٰ� UPŰ ������ �ڵ����� �����Ǵ� �� ����   
    WaitForSeconds fimeForDelayJump = new WaitForSeconds(0.3f);
    float realTimeForCheckStuck=0f;  //��ٸ� �ö���ٰ� �߸��ؼ� ���� ���� �ð� üũ
    float timeForCheckStuck = 1.5f;
    const float gravityY = 9.81f;

    //��ٸ� �ǳ� �� �� Collision �����ؾ���
    public Collider2D platformCollider;
    bool DoNotTouchInLadder = false; // ��ٸ��� ���� �� �ǵ帮�� �ȵǴ� �������� Ȯ���Ѵ�. 

    // �� ���� ���� �� �ö��� �̺�Ʈ �ߵ�
    bool nowFlying = false; // ���� �ִ����� üũ�ϴ� �Ҹ�����

    //�� ����Ʈ ��������
    public GameObject pung; // ������ �ִϸ��̼� ����� ���ӿ�����Ʈ����
    public GameObject pungSmall;
    public GameObject pungStartObj; // ������ ������ ����� �� ������Ʈ ����
    public GameObject pungBigStar; // ���� ���� �� ����� �� ������Ʈ ���� 
    private Vector2 pungDisapperPosition;
    private Vector2 pungRevivePosition;

    //��Ȱ���� �̺�Ʈ >> PlayerLife���� ü��ȸ���̺�Ʈ�� ����. 
    public event Action ImRevie;
    public bool notRevive {get; private set;}




    void Start()
    {
        _myNick = PlayerPrefs.GetString("_myNick");
        this.transform.GetChild(0).GetChild(0).GetComponentInChildren<Text>().text = _myNick; // �г��� ����
        playerAnimator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();


        groundMaskInt =LayerMask.NameToLayer("Ground");
        playerMaskInt = LayerMask.NameToLayer("Player");
        ItemBeeMaskInt = LayerMask.NameToLayer("Monster");


        UIManager.instance.DigGageFullFilled += () => playGetCarrotInGround();
        carrotBouncePower = new Vector2(0, carrotBouncePowerY);
        notRevive = false;




    }



    private void FixedUpdate()
    {
        


        if (!LadderCheck())//��ٸ��� �ִµ�����  isGrounded �� true�� �Ǵ� ���� �����Ѵ�. 
        {
            Physics2D.IgnoreCollision(playerCollider, platformCollider, false); // �� �ݶ��̴� �����Ѵ�.
            Physics2D.IgnoreLayerCollision(playerMaskInt, groundMaskInt, false);
            Physics2D.IgnoreLayerCollision(playerMaskInt, ItemBeeMaskInt, false); // �ö��׸��� ���̾� �����Ѵ�.
            playerRigidbody.gravityScale = gravityY;
            GroundCheck();
            CheckStuckInTheGround();


            Walk();

            if (!InputManager.instance.touchOn) // InputManger���� �����ϴ� â���� �������� ������ �������� ���� �ʴ´�. 
            {

                if (notRevive) return; //ĳ���Ͱ� �׾��� �� �������� ��ó�� ������ ������ �߻��ϴ� �� ���� ���� �� �ڵ� �߰���
                HasNearItem();
            }
            //CheckbeingFlying();
            GetHeadShot();

            //������ �ƴ϶� ������ ������ �������� �ִϸ��̼��� ����Ǿ�� �Ѵ�. 
            playerAnimator.SetFloat("yvelocity", playerRigidbody.velocity.y);
            playerAnimator.SetBool("DoJump", !isGrounded);
        }
        else //��ٸ��� �ִ� ���! 
        {
            
            Physics2D.IgnoreCollision(playerCollider, platformCollider, true); // ��ٸ� �ִ� ������ Ground Collider ���� 
            Physics2D.IgnoreLayerCollision(playerMaskInt, groundMaskInt, true);
            Physics2D.IgnoreLayerCollision(playerMaskInt, ItemBeeMaskInt, true); // ��ٸ��� �ִ� ������ �ö��׸��� ���̾� ���� 
            playerRigidbody.gravityScale = 0f;
            MoveInLadder();

        }

        //x ������
        playerSprite.flipX = (m_HorizontalMovement < -0.1f) ? true : false;



    }

    public bool GroundCheck()
    {
        bool wasGrounded = isGrounded;


        //���⵿ ����
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)groundCheckCollider.position, groundCheckRadius, groundMask);

        if (colliders.Length > 0) //���� ��Ҵ�.
        {
            isGrounded = true;
            if(jumpCount != 0) { resetJumpCount(); } // ����ī��Ʈ �����Ѵ�.


            if (!wasGrounded) // ���� ������� ������ ���� �ʾҴ�.
            {
                playerRigidbody.velocity = Vector2.zero;

            }
        }
        else   //���� ���� �ʾҴ�.
        {
            isGrounded = false;

        }

        return isGrounded;
    }

    public bool LadderCheck()
    {
        bool wasLaddered = isLadder;
        //�ݶ��̴� üũ �ٲ��� �ϳ��� 
        Collider2D[] LadderUp = Physics2D.OverlapCircleAll((Vector2)ladderColliderCheck1.position, 0.8f, LadderMask);
        Collider2D[] LadderDown = Physics2D.OverlapCircleAll((Vector2)groundCheckCollider.position, 0.4f, LadderMask);
        Collider2D[] LadderDetect = Physics2D.OverlapCircleAll((Vector2)groundCheckCollider.position, 0.5f, LadderMask);
        Collider2D[] OnLadder = Physics2D.OverlapCircleAll((Vector2)groundCheckCollider.position, 0.1f, LadderMask);

        //��ٸ� ���ٽ� ��ٸ��� ���� �ִ� ���
        if (Input.GetKey(KeyCode.UpArrow) && LadderUp.Length > 0)
        {
            if (!wasLaddered)
            {
                playerRigidbody.velocity = Vector2.zero;
            }
            playerAnimator.SetBool("DoClimb", true);
            isLadder = true;


        }
        else if (LadderUp.Length > 0)
        {
            if (!wasLaddered)
            {
                playerRigidbody.velocity = Vector2.zero;
            }
            playerAnimator.SetBool("DoClimb", true);
            isLadder = true;

        }
        //�ٸ� ���ٽ� ��ٸ��� �Ʒ��� �ִ� ���
        else if (Input.GetKey(KeyCode.DownArrow) && LadderDown.Length > 0)
        {

            if (!wasLaddered)
            {
                playerRigidbody.velocity = Vector2.zero;

            }
            playerAnimator.SetBool("DoClimb", true);
            isLadder = true;

        }
        //��ٸ��� �ִ� ��, ��ٸ� �� �Ʒ� ����������
        else if (OnLadder.Length > 0)
        {
            playerAnimator.SetBool("DoClimb", true);
            playerRigidbody.velocity = Vector2.zero;
            isLadder = true;

        }
        else
        {
            if (wasLaddered) { StartCoroutine(makeTermforJump()); }
            isLadder = false;
            playerAnimator.SetBool("DoClimb", false);
        }

        return isLadder;
    }

    void Walk()
    {
        if (notRevive) return;
        //�¿��̵�


        m_HorizontalMovement = UIManager.instance.GetHorizontalValue();
        playerRigidbody.velocity = new Vector2(speed * m_HorizontalMovement, playerRigidbody.velocity.y);

        //�ȱ� �ִϸ��̼�
        if (m_HorizontalMovement != 0)
        {
            playerAnimator.SetBool("DoWalk", true);
            InputManager.instance.ClickAllCancleFamButton();
        }
        else playerAnimator.SetBool("DoWalk", false);


    }

    public void TouchUpBtnAndChageVerticalValue(bool pressBtn)
    {
        m_VerticalMovement = pressBtn ? 1 : 0;
        Jump();
    }

    public void Jump()
    {
        InputManager.instance.ClickAllCancleFamButton(); // ����� UIâ�� �ִٸ� Cancle��ư�� ���� ȿ��

        if (LadderCheck()) // ����Ͽ� ��ư�� ����鼭 �߰��� �ڵ�.
        {
            return;
        }

        if (notRevive) return;
        if (!isGrounded) return;
        if (!JumpEnable) return;

        //���� �ִ� Ƚ�� ����
        if (jumpCount >= jumpMaxCount) return;
        
        //��� ������ ���� ���� �ȵǰ� �����.
        if (enableJumpCount <= 0) return;

        // ����������� 1. 
        AudioManager.instance.PlaySFX("PlayerJump");
        float jumpForce = 2000f;
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(new Vector2(0, jumpForce));

        // ����������� 2. >> 1�� ���������Ƿ� �ּ�ó����
        // playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 40f); 

        jumpCount++;
        enableJumpCount--;
        ClampCarrotValue();
        UIManager.instance.UpdateCarrotText(enableJumpCount); // UI����
        if (enableJumpCount == countForWaringCarrotShortage)
        {
            UIManager.instance.UrgentGameTip(UIManager.CarrotShortage);
        }
        if(enableJumpCount == 0)
        {
            UIManager.instance.UrgentGameTip(UIManager.ZeroCarrot);
        }
        Debug.Log("����ī��Ʈ�� 1�� �Ǿ���.");

    }

    void resetJumpCount()
    {
        jumpCount = 0;
    }

    //��� �Ծ��� �� ����ī��Ʈ �ö󰡴� ȿ��
    public void JumpCountUp(int value)
    {
        enableJumpCount += value;
        ClampCarrotValue();
        UIManager.instance.UpdateCarrotText(enableJumpCount); //UI ���� 
        //�Դ� �Ҹ��� �߰�����.
    }

    //��� ���� ���� ��, ����ī��Ʈ �������� ȿ��
    public void JumpCountDown(int value) // PlayerLife���� �����
    {
        if (enableJumpCount >= value)
        {
            enableJumpCount -= value;
            UIManager.instance.UpdateCarrotText(enableJumpCount); //UI ���� 
        }

    }

    void ClampCarrotValue()
    {
        enableJumpCount = Mathf.Clamp(enableJumpCount, 0, maxCarrot);
    }




    void MoveInLadder()
    {

        float speedup = 7f; // ��ٸ� ������ �ӵ�
        float speedJumpInLadder = -7f; // ��ٸ����� �¿�Ű ������ ��, ��ٸ��� ����鼭 �Ʒ��� �������� ����� �ӵ��̴�.


        // m_HorizontalMovement = Input.GetAxisRaw("Horizontal");
        m_HorizontalMovement = UIManager.instance.GetHorizontalValue();
        playerRigidbody.velocity = new Vector2(0, speedup * m_VerticalMovement);

        if (m_HorizontalMovement != 0) // ���� �������� �ʰ�, ��ٸ����� �¿�Ű ������ �� �������� �����.
        {
            Collider2D[] groundedTouched = Physics2D.OverlapCircleAll((Vector2)ladderColliderCheck1.position, 1.5f, groundMask);
            DoNotTouchInLadder = (groundedTouched.Length > 0) ? true : false;
            if (!DoNotTouchInLadder) // ��ٸ��� ������ �ִµ� �׶��忡 ��ġ�Ǹ�.. Horizontal ���� �����Ѵ�. ���� �ڲ� ������ ���� ����
            {
                // ���� ���� �ʾ��� ������ Horizontal�� ��ȿ
                playerRigidbody.velocity = new Vector2(m_HorizontalMovement * speed,playerRigidbody.velocity.y);
                playerRigidbody.velocity = new Vector2(speed * m_HorizontalMovement, speedJumpInLadder);

            }

        }


    }


    IEnumerator makeTermforJump() // ��ٸ� ���� �ö���ڸ��� �ٷ� ������ ����
    {
        JumpEnable = false;
        yield return fimeForDelayJump;
        JumpEnable = true;

    }

    public void AddForcetoBounce(Vector2 power)
    {
        AudioManager.instance.PlaySFX("PlayerJump");


        if (playerRigidbody.velocity.y > 1000f)
        {
            return;
        }

        if (LadderCheck()) // ��ٸ��� �ִ� ���¶�� ��������ġ ������ �������� Ȯ���Ѵ�.
        {

            Debug.Log("Player: AddForcetoBounce >��ٸ��� �ִ� �� �÷��̾� ������ġ�� �õ��Ͽ����ϴ�.");
            if (DoNotTouchInLadder)
            {
                Debug.Log($"Player: AddForcetoBounce > DoNotTouchInLadder :{DoNotTouchInLadder} �� return�մϴ�.");
                return;
            }
            Debug.Log($"Player: AddForcetoBounce > DoNotTouchInLadder :{DoNotTouchInLadder} �� ������ġ�� �Ǿ����ϴ�. ");
        }
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(power);

        //���� power�� float ���� �ٲ� �Ŀ�, playerRigidbody.AddForce(0,power)���� �߾���!
    }


    private void HasNearItem()
    {
        Collider2D[] nearItem = Physics2D.OverlapCircleAll((Vector2)ItemCheckCollider.position, 1.4f,ItemMask);

        if (nearItem.Length>0)
        {
            //if (!nearItem[0].CompareTag("Item")) return; //�������� �ƴϸ� �������� �ʴ´�.

            IItem item = nearItem[0].GetComponent<IItem>();
            HouseKeyItem isKey = nearItem[0].GetComponent<HouseKeyItem>();
            if (item != null)
            {
                Vector2 DisappearItemPosition = nearItem[0].transform.position;
                item.Use(gameObject);

                // �� ����� ������ ��ũ��Ʈ ���� ������ 

                if (isKey == null)  // �������� Ű�� �ƴҰ��
                {
                    GameObject pungItemPlay = Instantiate(pungStartObj, DisappearItemPosition, Quaternion.identity);


                }
                else // Ű �������� ������?
                {
                    GameObject pungKeyPlay = isKey.isPaid ? Instantiate(pungBigStar, DisappearItemPosition, Quaternion.identity) : Instantiate(pungSmall, DisappearItemPosition, Quaternion.identity);

                }

            }


        }
    }

    public void GetHeadShot() // ���ݴ���
    {
        Collider2D[] GetOthersInMyHead = Physics2D.OverlapCircleAll(HeadShotCollider.transform.position, 1f, 1 << 10);
        if (GetOthersInMyHead.Length > 0)
        {
            for(int i=0;i< GetOthersInMyHead.Length; i++)
            {
                Rigidbody2D otherRigidBody = GetOthersInMyHead[i].GetComponent<Rigidbody2D>();
                if (otherRigidBody)
                {
                    Vector2 cp = GetOthersInMyHead[i].transform.position;
                    Vector2 dir = (cp - (Vector2)transform.position).normalized;

                    otherRigidBody.AddForce(dir * bouncePowerforOtherInMyHead);
                }

            }

        }
    }


    public void CheckbeingFlying()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)groundCheckCollider.position, 0.5f, ItemBeeMask);
        if (colliders.Length > 0 ) // �ö��� ���� ���� �ִ�. 
        {
            if (!nowFlying) 
            { nowFlying = true;
              Physics2D.IgnoreLayerCollision(playerMaskInt, groundMaskInt, true);
            }

        }
        else
        {
            if (nowFlying) // �ö��� ���� ���� ����. 
            {
                nowFlying = false;
                Physics2D.IgnoreLayerCollision(playerMaskInt, groundMaskInt, false);
            }

        }

    }

    //������� ������ ���� ������ ���� ����
    void CheckStuckInTheGround()
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)ladderColliderCheck1.position, 0.9f, groundMask);

        if (colliders.Length > 0) 
        {
            realTimeForCheckStuck += Time.deltaTime;

            if (realTimeForCheckStuck > timeForCheckStuck)
            {
                Vector2 myPosition = this.transform.position;
                myPosition += Vector2.down;
                this.transform.position = myPosition;
            }

        }
        else
        {
            realTimeForCheckStuck = 0f;
        }


    }


    //PlayerLife ��ũ��Ʈ���� �� �Լ��� ȣ���� �� ���̴�. 
    public void ImDead()
    {
        Debug.Log($"PlayerLife :  PlayerMovement ImDead Entered");
        float reviveTime = 1.2f;

        notRevive = true;  //��Ȱ�Ǳ� ������ �� �����̰� ��.
        // ���� �� ȿ�����
        playerAnimator.SetTrigger("Die");
        AudioManager.instance.PlaySFX("PlayerDie");

        //���� ��ٸ� �ȿ� ������ ����߸���
        if (isLadder)
        {
            isLadder = false;
        }

        Debug.Log($"PlayerLife :  PlayerMovement ImDead Entered And Course 1 Passed");

        //������ٵ� �۵� ����
        this.playerRigidbody.Sleep();

        //����Ʈ �ִϸ��̼� ��� : ��
        pungDisapperPosition = (Vector2)this.transform.position;


        Debug.Log($"PlayerLife :  PlayerMovement ImDead Entered And Course 2 Passed");

        if (PlayerHeartStat.Instance.Health == 0) //��Ʈ�� �� �������� 
        {
            PlayerPrefs.SetString("Winner", "NotYours");
            SceneManager.LoadScene("EndingScene");

        }

        Debug.Log($"PlayerLife :  PlayerMovement ImDead Entered And Course 3 Passed");
        // ���� �Ǵ��� Ȯ���غ���!

        //��Ȱ �Լ� �۵� 
        Invoke("Revive", reviveTime);
        Invoke("playDisappearPung",reviveTime);

        Debug.Log($"PlayerLife :  PlayerMovement ImDead Entered And Course 4 Passed");




    }

    public void DigGround()
    {
        if (!isGrounded) return;

        UIManager.instance.FillDigGage();

        //�÷��̾� �ִϸ����� ��� 
        playerAnimator.SetTrigger("Damaged");

        //�� �Ĵ� �Ҹ� ���
        AudioManager.instance.PlaySFX("PlayerDig");

        //�ֺ� ���� ȿ�� ��� 
        Vector2 playerPosition = this.transform.position;
        playerPosition.y -= 1.7f; 
        int PungCount = Random.Range(1, 3);
        for(int i =0; i < PungCount; i++)
        {
            Vector2 randomPungPosition = (Vector2)Random.insideUnitCircle + playerPosition; //������ġ
            float randomPungSize = Random.Range(0.5f, 1.0f); //���������� 
            GameObject RandomPung = Instantiate(pungSmall,randomPungPosition,Quaternion.identity);
            RandomPung.transform.localScale *= randomPungSize;

        }


    }

    public void playGetCarrotInGround()
    {
        // �ٷ� �Ʒ� �Լ��� �̺�Ʈ�� ����ϴ� �������� �Ȱ��� �Ǽ� ��ϵǹǷ� ���⼭ �Լ��� �����Ų��.
        GetCarrotInTheGround();
    }

    void GetCarrotInTheGround() //���� ��� ��� 
    {
        Vector2 origincarrotBouncePower = carrotBouncePower; //���� �� ���� 

        Vector3 CarrotPostion = groundCheckCollider.position;
        CarrotPostion.x += Random.Range(-1.2f, 1.2f);

        GameObject newCarrot = Instantiate(CarrotInTheGround, CarrotPostion, Quaternion.identity);
        Rigidbody2D carrotRigidbody = newCarrot.GetComponent<Rigidbody2D>();
        carrotBouncePower.x += Random.Range(-50f, 50f);

        if(carrotRigidbody != null)
        {
            carrotRigidbody.AddForce(carrotBouncePower);
            AudioManager.instance.PlaySFX("CarrotPulledOut");
        }
        carrotBouncePower = origincarrotBouncePower; //���������� 
    }

    public void Revive()
    {

        Debug.Log($"PlayerLife :  PlayerMovement Revive Entered");

        Transform newReSpot = PlayerManager.instance.randomReviveSpot();

        
        Debug.Log($"PlayerLife :  PlayerMovement Revive Entered And newReSpot get Successed : ");

        pungRevivePosition = newReSpot.position;
        bool ISnewReSpotNull = newReSpot == null ? true:false;

        Debug.Log($" PlayerLife : Detect newReSpot is Null? : {ISnewReSpotNull} / And pungRevivePosition Is Assigned? :  {pungRevivePosition } ");

        playRevivePung();


        //���⼭���� �ȵ� 
        Debug.Log($"PlayerLife :  PlayerMovement Revive Entered : Course 1 Passed");
        this.transform.position = newReSpot.position;
        notRevive = false;
        AudioManager.instance.PlaySFX("PlayerRevive");

        Debug.Log($"PlayerLife :  PlayerMovement Revive Entered : Course 2 Passed");

        if (playerRigidbody.IsSleeping()==true) // ������ �ٵ� �ѱ� .. 
        //���̰� �ƹ����� �۵��� �ȵǴ� ���ϴ�?? ���⿡ notRevive = false; ������ �۵��ȵ�..
        //Rigidbody.Sleep�� �� �ȵǰ� ���� �� �����ؾ��ҵ�
        {
            playerRigidbody.WakeUp();

        }


        Debug.Log($"PlayerLife :  PlayerMovement Revive Entered : Course 3 Passed");

        bool ImReviveIsNull = ImRevie == null ? true : false;
        if (ImRevie != null) // ��Ȱ �̺�Ʈ �۵�
        {
            ImRevie();


        }

        Debug.Log($"PlayerLife :  PlayerMovement Revive Entered : Course 4 Passed");
    }

    private void playDisappearPung()
    {
        Debug.Log($"PlayerLife :  PlayerMovement playDisappearPung Entered");
        GameObject pungPlay = Instantiate(pung, pungDisapperPosition, Quaternion.identity);

    }

    private void playRevivePung()
    {
        GameObject pungPlay = Instantiate(pung, pungRevivePosition, Quaternion.identity);

    }

    public void SetEnding()
    {
        // ���߿� ���⼭ ���� ��������? üũ�ϴ� ��� if������ �߰��Ѵ�.
        PlayerPrefs.SetString("Winner", _myNick);
        SceneManager.LoadScene("EndingScene");
    }




}
