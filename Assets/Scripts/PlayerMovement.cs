using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update

    //컴포넌트 가져오기
    public Animator playerAnimator; //PlayerLife.cs에서 한번 호출할 일 있음
    SpriteRenderer playerSprite;
    Rigidbody2D playerRigidbody;
    Collider2D playerCollider;

    // 내 아이디 정보
    string _myNick;

    private float speed = 12f; // 좌우스피드값

    //좌우입력키 가져오기
    float m_HorizontalMovement;
    float m_VerticalMovement;


    //점프 카운트 관련 변수(당근 갯수)
    private int jumpCount = 0;  //당근 먹어서 생긴 점프 카운트
    private int jumpMaxCount = 1;
    private int enableJumpCount = 10; // 당근 갯수, 점프 가능한 횟수
    private int countForWaringCarrotShortage = 5; // 당근 부족을 경고할 현 당근 갯수
    private int maxCarrot = 100; //당근 최대 갯수

    //땅파기 + 당근얻기 관련
    public GameObject CarrotInTheGround; //인스펙터에서 할당
    float carrotBouncePowerY = 500f;
    Vector2 carrotBouncePower;


    //Ground체크
    [SerializeField] Transform groundCheckCollider;
    [SerializeField] Transform ladderColliderCheck1;
    [SerializeField] Transform ladderColliderCheck2;

    //아이템 체크 
    [SerializeField] Transform ItemCheckCollider;

    //헤드샷체크
    [SerializeField] Transform HeadShotCollider;
    float bouncePowerforOtherInMyHead = 600f;


    public LayerMask groundMask; 
    public LayerMask LadderMask;
    public LayerMask ItemMask;
    public LayerMask ItemBeeMask;

    // IgnoreLayerCollision 함수를 활용하기 위한 변수 
    int groundMaskInt;
    int playerMaskInt;
    int ItemBeeMaskInt;

    const float groundCheckRadius = 0.6f;
    public bool isGrounded;
    bool isLadder = false;

    public bool JumpEnable = true; // 사다리 올라오다가 UP키 눌러서 자동으로 점프되는 것 방지   
    WaitForSeconds fimeForDelayJump = new WaitForSeconds(0.3f);
    float realTimeForCheckStuck=0f;  //사다리 올라오다가 잘못해서 땅에 갇힌 시간 체크
    float timeForCheckStuck = 1.5f;
    const float gravityY = 9.81f;

    //사다리 건널 때 땅 Collision 무시해야함
    public Collider2D platformCollider;
    bool DoNotTouchInLadder = false; // 사다리에 있을 때 건드리면 안되는 상태인지 확인한다. 

    // 벌 위에 있을 때 플라잉 이벤트 발동
    bool nowFlying = false; // 날고 있는지를 체크하는 불린변수

    //펑 이펙트 가져오기
    public GameObject pung; // 펑폭발 애니메이션 재생될 게임오브젝트파일
    public GameObject pungSmall;
    public GameObject pungStartObj; // 아이템 먹으면 재생될 펑 오브젝트 파일
    public GameObject pungBigStar; // 열쇠 먹을 때 재생될 펑 오브젝트 파일 
    private Vector2 pungDisapperPosition;
    private Vector2 pungRevivePosition;

    //부활관련 이벤트 >> PlayerLife에서 체력회복이벤트로 쓴다. 
    public event Action ImRevie;
    public bool notRevive {get; private set;}




    void Start()
    {
        _myNick = PlayerPrefs.GetString("_myNick");
        this.transform.GetChild(0).GetChild(0).GetComponentInChildren<Text>().text = _myNick; // 닉네임 설정
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
        


        if (!LadderCheck())//사다리에 있는동안은  isGrounded 가 true가 되는 것을 방지한다. 
        {
            Physics2D.IgnoreCollision(playerCollider, platformCollider, false); // 땅 콜라이더 감지한다.
            Physics2D.IgnoreLayerCollision(playerMaskInt, groundMaskInt, false);
            Physics2D.IgnoreLayerCollision(playerMaskInt, ItemBeeMaskInt, false); // 플라잉몬스터 레이어 감지한다.
            playerRigidbody.gravityScale = gravityY;
            GroundCheck();
            CheckStuckInTheGround();


            Walk();

            if (!InputManager.instance.touchOn) // InputManger에서 관리하는 창들이 열려있을 때에는 아이템을 먹지 않는다. 
            {

                if (notRevive) return; //캐릭터가 죽었을 때 아이템이 근처에 있으면 오류가 발생하는 것 같아 지금 이 코드 추가함
                HasNearItem();
            }
            //CheckbeingFlying();
            GetHeadShot();

            //점프가 아니라 떨어질 때에도 떨어지는 애니메이션이 적용되어야 한다. 
            playerAnimator.SetFloat("yvelocity", playerRigidbody.velocity.y);
            playerAnimator.SetBool("DoJump", !isGrounded);
        }
        else //사다리에 있는 경우! 
        {
            
            Physics2D.IgnoreCollision(playerCollider, platformCollider, true); // 사다리 있는 동안은 Ground Collider 무시 
            Physics2D.IgnoreLayerCollision(playerMaskInt, groundMaskInt, true);
            Physics2D.IgnoreLayerCollision(playerMaskInt, ItemBeeMaskInt, true); // 사다리에 있는 동안은 플라잉몬스터 레이어 무시 
            playerRigidbody.gravityScale = 0f;
            MoveInLadder();

        }

        //x 뒤집기
        playerSprite.flipX = (m_HorizontalMovement < -0.1f) ? true : false;



    }

    public bool GroundCheck()
    {
        bool wasGrounded = isGrounded;


        //땅출동 감지
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)groundCheckCollider.position, groundCheckRadius, groundMask);

        if (colliders.Length > 0) //땅에 닿았다.
        {
            isGrounded = true;
            if(jumpCount != 0) { resetJumpCount(); } // 점프카운트 갱신한다.


            if (!wasGrounded) // 땅에 닿았지만 이전에 닿지 않았다.
            {
                playerRigidbody.velocity = Vector2.zero;

            }
        }
        else   //땅에 닿지 않았다.
        {
            isGrounded = false;

        }

        return isGrounded;
    }

    public bool LadderCheck()
    {
        bool wasLaddered = isLadder;
        //콜라이더 체크 바꾸자 하나로 
        Collider2D[] LadderUp = Physics2D.OverlapCircleAll((Vector2)ladderColliderCheck1.position, 0.8f, LadderMask);
        Collider2D[] LadderDown = Physics2D.OverlapCircleAll((Vector2)groundCheckCollider.position, 0.4f, LadderMask);
        Collider2D[] LadderDetect = Physics2D.OverlapCircleAll((Vector2)groundCheckCollider.position, 0.5f, LadderMask);
        Collider2D[] OnLadder = Physics2D.OverlapCircleAll((Vector2)groundCheckCollider.position, 0.1f, LadderMask);

        //사다리 접근시 사다리가 위에 있는 경우
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
        //다리 접근시 사다리가 아래에 있는 경우
        else if (Input.GetKey(KeyCode.DownArrow) && LadderDown.Length > 0)
        {

            if (!wasLaddered)
            {
                playerRigidbody.velocity = Vector2.zero;

            }
            playerAnimator.SetBool("DoClimb", true);
            isLadder = true;

        }
        //사다리에 있는 중, 사다리 위 아래 오르내림중
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
        //좌우이동


        m_HorizontalMovement = UIManager.instance.GetHorizontalValue();
        playerRigidbody.velocity = new Vector2(speed * m_HorizontalMovement, playerRigidbody.velocity.y);

        //걷기 애니메이션
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
        InputManager.instance.ClickAllCancleFamButton(); // 띄워진 UI창이 있다면 Cancle버튼을 누른 효과

        if (LadderCheck()) // 모바일용 버튼을 만들면서 추가한 코드.
        {
            return;
        }

        if (notRevive) return;
        if (!isGrounded) return;
        if (!JumpEnable) return;

        //점프 최대 횟수 제한
        if (jumpCount >= jumpMaxCount) return;
        
        //당근 갯수에 따라 점프 안되게 만든다.
        if (enableJumpCount <= 0) return;

        // 점프구현방법 1. 
        AudioManager.instance.PlaySFX("PlayerJump");
        float jumpForce = 2000f;
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(new Vector2(0, jumpForce));

        // 점프구현방법 2. >> 1을 선택했으므로 주석처리함
        // playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 40f); 

        jumpCount++;
        enableJumpCount--;
        ClampCarrotValue();
        UIManager.instance.UpdateCarrotText(enableJumpCount); // UI갱신
        if (enableJumpCount == countForWaringCarrotShortage)
        {
            UIManager.instance.UrgentGameTip(UIManager.CarrotShortage);
        }
        if(enableJumpCount == 0)
        {
            UIManager.instance.UrgentGameTip(UIManager.ZeroCarrot);
        }
        Debug.Log("점프카운트는 1이 되었다.");

    }

    void resetJumpCount()
    {
        jumpCount = 0;
    }

    //당근 먹었을 때 점프카운트 올라가는 효과
    public void JumpCountUp(int value)
    {
        enableJumpCount += value;
        ClampCarrotValue();
        UIManager.instance.UpdateCarrotText(enableJumpCount); //UI 갱신 
        //먹는 소리도 추가하자.
    }

    //당근 빙고에 썼을 때, 점프카운트 내려가는 효과
    public void JumpCountDown(int value) // PlayerLife에서 사용함
    {
        if (enableJumpCount >= value)
        {
            enableJumpCount -= value;
            UIManager.instance.UpdateCarrotText(enableJumpCount); //UI 갱신 
        }

    }

    void ClampCarrotValue()
    {
        enableJumpCount = Mathf.Clamp(enableJumpCount, 0, maxCarrot);
    }




    void MoveInLadder()
    {

        float speedup = 7f; // 사다리 오르는 속도
        float speedJumpInLadder = -7f; // 사다리에서 좌우키 눌렀을 때, 사다리를 벗어나면서 아래로 떨어지게 만드는 속도이다.


        // m_HorizontalMovement = Input.GetAxisRaw("Horizontal");
        m_HorizontalMovement = UIManager.instance.GetHorizontalValue();
        playerRigidbody.velocity = new Vector2(0, speedup * m_VerticalMovement);

        if (m_HorizontalMovement != 0) // 땅에 도착하지 않고, 사다리에서 좌우키 눌렀을 때 떨어지게 만든다.
        {
            Collider2D[] groundedTouched = Physics2D.OverlapCircleAll((Vector2)ladderColliderCheck1.position, 1.5f, groundMask);
            DoNotTouchInLadder = (groundedTouched.Length > 0) ? true : false;
            if (!DoNotTouchInLadder) // 사다리에 오르고 있는데 그라운드에 터치되면.. Horizontal 값을 무시한다. 땅에 자꾸 갇히는 현상 막기
            {
                // 땅에 닿지 않았을 때에만 Horizontal값 유효
                playerRigidbody.velocity = new Vector2(m_HorizontalMovement * speed,playerRigidbody.velocity.y);
                playerRigidbody.velocity = new Vector2(speed * m_HorizontalMovement, speedJumpInLadder);

            }

        }


    }


    IEnumerator makeTermforJump() // 사다리 위로 올라오자마자 바로 점프됨 방지
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

        if (LadderCheck()) // 사다리에 있는 상태라면 물리적터치 가능한 상태인지 확인한다.
        {

            Debug.Log("Player: AddForcetoBounce >사다리에 있는 중 플레이어 물리터치를 시도하였습니다.");
            if (DoNotTouchInLadder)
            {
                Debug.Log($"Player: AddForcetoBounce > DoNotTouchInLadder :{DoNotTouchInLadder} 로 return합니다.");
                return;
            }
            Debug.Log($"Player: AddForcetoBounce > DoNotTouchInLadder :{DoNotTouchInLadder} 로 물리터치가 되었습니다. ");
        }
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(power);

        //원래 power를 float 으로 바꾼 후에, playerRigidbody.AddForce(0,power)으로 했었다!
    }


    private void HasNearItem()
    {
        Collider2D[] nearItem = Physics2D.OverlapCircleAll((Vector2)ItemCheckCollider.position, 1.4f,ItemMask);

        if (nearItem.Length>0)
        {
            //if (!nearItem[0].CompareTag("Item")) return; //아이템이 아니면 실행하지 않는다.

            IItem item = nearItem[0].GetComponent<IItem>();
            HouseKeyItem isKey = nearItem[0].GetComponent<HouseKeyItem>();
            if (item != null)
            {
                Vector2 DisappearItemPosition = nearItem[0].transform.position;
                item.Use(gameObject);

                // ★ 사운드는 아이템 스크립트 별로 적용함 

                if (isKey == null)  // 아이템이 키가 아닐경우
                {
                    GameObject pungItemPlay = Instantiate(pungStartObj, DisappearItemPosition, Quaternion.identity);


                }
                else // 키 아이템이 맞으면?
                {
                    GameObject pungKeyPlay = isKey.isPaid ? Instantiate(pungBigStar, DisappearItemPosition, Quaternion.identity) : Instantiate(pungSmall, DisappearItemPosition, Quaternion.identity);

                }

            }


        }
    }

    public void GetHeadShot() // 공격당함
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
        if (colliders.Length > 0 ) // 플라잉 몬스터 위에 있다. 
        {
            if (!nowFlying) 
            { nowFlying = true;
              Physics2D.IgnoreLayerCollision(playerMaskInt, groundMaskInt, true);
            }

        }
        else
        {
            if (nowFlying) // 플라잉 몬스터 위에 없다. 
            {
                nowFlying = false;
                Physics2D.IgnoreLayerCollision(playerMaskInt, groundMaskInt, false);
            }

        }

    }

    //여우몬스터 때문에 땅에 갇히는 현상 방지
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


    //PlayerLife 스크립트에서 이 함수를 호출해 쓸 것이다. 
    public void ImDead()
    {
        Debug.Log($"PlayerLife :  PlayerMovement ImDead Entered");
        float reviveTime = 1.2f;

        notRevive = true;  //부활되기 전까지 못 움직이게 함.
        // 죽을 때 효과재생
        playerAnimator.SetTrigger("Die");
        AudioManager.instance.PlaySFX("PlayerDie");

        //만약 사다리 안에 있으면 떨어뜨리기
        if (isLadder)
        {
            isLadder = false;
        }

        Debug.Log($"PlayerLife :  PlayerMovement ImDead Entered And Course 1 Passed");

        //리지드바디 작동 끄기
        this.playerRigidbody.Sleep();

        //이펙트 애니메이션 재생 : 펑
        pungDisapperPosition = (Vector2)this.transform.position;


        Debug.Log($"PlayerLife :  PlayerMovement ImDead Entered And Course 2 Passed");

        if (PlayerHeartStat.Instance.Health == 0) //하트가 다 떨어지면 
        {
            PlayerPrefs.SetString("Winner", "NotYours");
            SceneManager.LoadScene("EndingScene");

        }

        Debug.Log($"PlayerLife :  PlayerMovement ImDead Entered And Course 3 Passed");
        // 여기 되는지 확인해보자!

        //부활 함수 작동 
        Invoke("Revive", reviveTime);
        Invoke("playDisappearPung",reviveTime);

        Debug.Log($"PlayerLife :  PlayerMovement ImDead Entered And Course 4 Passed");




    }

    public void DigGround()
    {
        if (!isGrounded) return;

        UIManager.instance.FillDigGage();

        //플레이어 애니메이터 재생 
        playerAnimator.SetTrigger("Damaged");

        //땅 파는 소리 재생
        AudioManager.instance.PlaySFX("PlayerDig");

        //주변 먼지 효과 재생 
        Vector2 playerPosition = this.transform.position;
        playerPosition.y -= 1.7f; 
        int PungCount = Random.Range(1, 3);
        for(int i =0; i < PungCount; i++)
        {
            Vector2 randomPungPosition = (Vector2)Random.insideUnitCircle + playerPosition; //랜덤위치
            float randomPungSize = Random.Range(0.5f, 1.0f); //랜덤사이즈 
            GameObject RandomPung = Instantiate(pungSmall,randomPungPosition,Quaternion.identity);
            RandomPung.transform.localScale *= randomPungSize;

        }


    }

    public void playGetCarrotInGround()
    {
        // 바로 아래 함수를 이벤트로 등록하니 랜덤값이 똑같이 되서 등록되므로 여기서 함수를 실행시킨다.
        GetCarrotInTheGround();
    }

    void GetCarrotInTheGround() //땅속 당근 얻기 
    {
        Vector2 origincarrotBouncePower = carrotBouncePower; //원래 값 저장 

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
        carrotBouncePower = origincarrotBouncePower; //원래값복원 
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


        //여기서부터 안됨 
        Debug.Log($"PlayerLife :  PlayerMovement Revive Entered : Course 1 Passed");
        this.transform.position = newReSpot.position;
        notRevive = false;
        AudioManager.instance.PlaySFX("PlayerRevive");

        Debug.Log($"PlayerLife :  PlayerMovement Revive Entered : Course 2 Passed");

        if (playerRigidbody.IsSleeping()==true) // 리디즈 바디 켜기 .. 
        //★이거 아무래도 작동이 안되는 듯하다?? 여기에 notRevive = false; 넣으니 작동안됨..
        //Rigidbody.Sleep이 왜 안되고 뭔지 좀 공부해야할듯
        {
            playerRigidbody.WakeUp();

        }


        Debug.Log($"PlayerLife :  PlayerMovement Revive Entered : Course 3 Passed");

        bool ImReviveIsNull = ImRevie == null ? true : false;
        if (ImRevie != null) // 부활 이벤트 작동
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
        // 나중에 여기서 내가 위너인지? 체크하는 기능 if문으로 추가한다.
        PlayerPrefs.SetString("Winner", _myNick);
        SceneManager.LoadScene("EndingScene");
    }




}
