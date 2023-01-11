using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update

    //컴포넌트 가져오기
    Animator playerAnimator;
    SpriteRenderer playerSprite;
    Rigidbody2D playerRigidbody;
    Collider2D playerCollider;

    // 내 아이디 정보
    string _myNick;

    private float speed = 10f; // 좌우스피드값

    //좌우입력키 가져오기
    float m_HorizontalMovement;
    float m_VerticalMovement;


    //점프 카운트 관련 변수
    private int jumpCount = 0;  //당근 먹어서 생긴 점프 카운트
    private int jumpMaxCount = 1;
    private int enableJumpCount = 10;



    //Ground체크
    [SerializeField] Transform groundCheckCollider;
    [SerializeField] Transform ladderColliderCheck1;
    [SerializeField] Transform ladderColliderCheck2;

    //아이템 체크 
    [SerializeField] Transform ItemCheckCollider;


    public LayerMask groundMask;
    public LayerMask LadderMask;
    const float groundCheckRadius = 0.6f;
    public bool isGrounded;
    static public bool isLadder = false;

    public bool JumpEnable = true; // 사다리 올라오다가 UP키 눌러서 자동으로 점프되는 것 방지
    const float gravityY = 9.81f;

    //사다리 건널 때 땅 Collision 무시해야함
    public Collider2D platformCollider;

    // 벌 위에 있을 때 플라잉 이벤트 발동
    int maskPlayer = 1 >> 8; // 플레이어 레이어 마스크 
    int maskGround = 1 >> 6; // Ground(사다리포함) 레이어 마스크
    int maskMonFlying = 1 >> 10; // 날아다니는 플라잉 몬스터 마스크
    bool nowFlying = false; // 날고 있는지를 체크하는 불린변수

    //펑 이펙트 가져오기
    public GameObject pung; // 펑폭발 애니메이션 재생될 게임오브젝트파일
    float pungAniPlayTime = 0.55f;
    private Vector2 pungDisapperPosition;
    private Vector2 pungRevivePosition;

    //부활관련 이벤트 >> PlayerLife에서 체력회복이벤트로 쓴다. 
    public event Action ImRevie;
    bool notRevive;




    void Start()
    {
        _myNick = PlayerPrefs.GetString("_myNick");
        this.transform.GetChild(0).GetChild(0).GetComponentInChildren<Text>().text = _myNick; // 닉네임 설정
        playerAnimator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();



        //점프 카운트 리셋을 위한 이벤트 등록 
        //Ground ground = FindObjectOfType<Ground>();
        //ground.playerTouched += resetJumpCount;


    }



    private void FixedUpdate()
    {

        if (!LadderCheck())//사다리에 있는동안은  isGrounded 가 true가 되는 것을 방지한다. 
        {
            Physics2D.IgnoreCollision(playerCollider, platformCollider, false); // 땅 콜라이더 감지한다.
            Physics2D.IgnoreLayerCollision(maskPlayer, maskMonFlying, false); // 플라잉몬스터 레이어 감지한다.
            playerRigidbody.gravityScale = gravityY;
            GroundCheck();

            if (Input.GetKey(KeyCode.UpArrow))
            {
                Jump();

            }
            Walk();

            if (!InputManager.instance.touchOn) // InputManger에서 관리하는 창들이 열려있을 때에는 아이템을 먹지 않는다. 
            {
                HasNearItem();
            }
            //CheckbeingFlying();


            //점프가 아니라 떨어질 때에도 떨어지는 애니메이션이 적용되어야 한다. 
            playerAnimator.SetFloat("yvelocity", playerRigidbody.velocity.y);
            playerAnimator.SetBool("DoJump", !isGrounded);
        }
        else
        {

            Physics2D.IgnoreCollision(playerCollider, platformCollider, true); // 사다리 있는 동안은 Ground Collider 무시 
            Physics2D.IgnoreLayerCollision(maskPlayer, maskMonFlying, true); // 사다리에 있는 동안은 플라잉몬스터 레이어 무시 
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


        }//다리 접근시 사다리가 아래에 있는 경우
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
            if (wasLaddered) { StartCoroutine("makeTermforJump"); }
            isLadder = false;
            playerAnimator.SetBool("DoClimb", false);
        }

        return isLadder;
    }

    void Walk()
    {
        if (notRevive) return;
        //좌우이동


        //m_HorizontalMovement = Input.GetAxis("Horizontal");
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

    public void Jump()
    {
        InputManager.instance.ClickAllCancleFamButton(); // 띄워진 UI창이 있다면 Cancle버튼을 누른 효과

        if (LadderCheck()) return; // 모바일용 버튼을 만들면서 추가한 코드.
        if (notRevive) return;
        if (!isGrounded) return;
        if (!JumpEnable) return;

        //점프 최대 횟수 제한
        if (jumpCount >= jumpMaxCount) return;
        if (enableJumpCount <= 0) return;

        // 점프구현방법 1. 
        float jumpForce = 2000f;
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(new Vector2(0, jumpForce));

        // 점프구현방법 2. >> 1을 선택했으므로 주석처리함
        // playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 40f); 

        jumpCount++;
        enableJumpCount--;
        UIManager.instance.UpdateCarrotText(enableJumpCount); // UI갱신
        Debug.Log("점프카운트는 1이 되었다.");




    }

    void resetJumpCount()
    {
        jumpCount = 0;
        Debug.Log("점프카운트는 0이 되었다.");
    }

    //당근 먹었을 때 점프카운트 올라가는 효과
    public void JumpCountUp(int value)
    {
        enableJumpCount += value;
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



    void MoveInLadder()
    {

        float speedup = 7f; // 사다리 오르는 속도
        float speedJumpInLadder = -7f;
        m_VerticalMovement = Input.GetAxisRaw("Vertical");
        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, speedup * m_VerticalMovement);


        // m_HorizontalMovement = Input.GetAxisRaw("Horizontal");
        m_HorizontalMovement = UIManager.instance.GetHorizontalValue();
        if (m_HorizontalMovement != 0)
        {
            Collider2D[] groundedTouched = Physics2D.OverlapCircleAll((Vector2)ladderColliderCheck1.position, 0.7f, groundMask);
            if (groundedTouched.Length <= 0)
            {
                playerRigidbody.velocity = new Vector2(speed * m_HorizontalMovement, speedJumpInLadder);
            }
        }

    }


    IEnumerator makeTermforJump() // 사다리 위로 올라오자마자 바로 점프됨 방지
    {
        JumpEnable = false;
        yield return new WaitForSeconds(0.3f);
        JumpEnable = true;

    }

    public void AddForcetoBounce(Vector2 power)
    {
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(power);

        //원래 power를 float 으로 바꾼 후에, playerRigidbody.AddForce(0,power)으로 했었다!
    }


    private void HasNearItem()
    {
        Collider2D[] nearItem = Physics2D.OverlapCircleAll((Vector2)ItemCheckCollider.position, 1.4f);

        if (nearItem[0].CompareTag("Item"))
        {
            IItem item = nearItem[0].GetComponent<IItem>();
            if (item != null)
            {

                ////호스트만 아이템 직접 사용가능
                ////호스트에서는 아이템 사용 후사용된 아이템이의 효과를 모든 클라이언트에 동기화시킴
                //if (PhotonNetwork.IsMasterClient)
                //{
                //    //Use매서드를 실행하여 아이템사용
                    item.Use(gameObject);
                //}

                ////아이템 습득 소리 재생
                //playerAudioPlayer.PlayOneShot(itemPickupClip); //효과음은 모든 클라이언트에서 실행된다. 
            }
        }

        // 아이템과 충돌한 경우 해당 아이템을 사용하는 처리
        // 사망하지 않은 경우에만 아이템 사용 가능 
        //충돌한 상대방으로부터 Item 컴포넌트 가져오기 시도

            //충돌한 상대방으로부터 Item컴포넌트 가져오는 데 성공했다
    }


    public void CheckbeingFlying()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)groundCheckCollider.position, 0.5f, maskMonFlying);
        if (colliders.Length > 0 ) // 플라잉 몬스터 위에 있다. 
        {
            if (!nowFlying) 
            { nowFlying = true;
              Physics2D.IgnoreLayerCollision(maskPlayer, maskGround, true);
            }

        }
        else
        {
            if (nowFlying) // 플라잉 몬스터 위에 없다. 
            {
                nowFlying = false;
                Physics2D.IgnoreLayerCollision(maskPlayer, maskGround, false);
            }

        }

    }


    //PlayerLife 스크립트에서 이 함수를 호출해 쓸 것이다. 
    public void ImDead()
    {
        float reviveTime = 1.2f;

        // 죽을 때 효과재생
        playerAnimator.SetTrigger("Die");


        //만약 사다리 안에 있으면 떨어뜨리기
        if (isLadder)
        {
            isLadder = false;
        }

        
        //리지드바디 작동 끄기
        this.playerRigidbody.Sleep();
        notRevive = true;  //부활되기 전까지 못 움직이게 함.


        //이펙트 애니메이션 재생 : 펑
        pungDisapperPosition = (Vector2)this.transform.position;




        //부활 함수 작동 
        Invoke("Revive", reviveTime);
        Invoke("playDisappearPung",reviveTime);


    }

    public void Revive()
    {
        Transform newReSpot = PlayerManager.randomReviveSpot();
        pungRevivePosition = newReSpot.position;
        playRevivePung();


        this.transform.position = newReSpot.position;
        notRevive = false;

        if (playerRigidbody.IsSleeping()==true) // 리디즈 바디 켜기 .. 
        //★이거 아무래도 작동이 안되는 듯하다?? 여기에 notRevive = false; 넣으니 작동안됨..
        //Rigidbody.Sleep이 왜 안되고 뭔지 좀 공부해야할듯
        {
            playerRigidbody.WakeUp();

        }

        if (ImRevie != null) // 부활 이벤트 작동
        {
            ImRevie();


        }
    }

    private void playDisappearPung()
    {
        GameObject pungPlay = Instantiate(pung, pungDisapperPosition, Quaternion.identity);
        Destroy(pungPlay.gameObject, pungAniPlayTime);

    }

    private void playRevivePung()
    {
        GameObject pungPlay = Instantiate(pung, pungRevivePosition, Quaternion.identity);
        Destroy(pungPlay.gameObject, pungAniPlayTime);

    }

    public void SetEnding()
    {
        // 나중에 여기서 내가 위너인지? 체크하는 기능 if문으로 추가한다.
        PlayerPrefs.SetString("Winner", _myNick);
        SceneManager.LoadScene("EndingScene");
    }

    //public void onDamageforChange() // 다른 스크립트에서 이 스크립트 내 코루틴 메서드 실행하는 것을 도와줌
    //{
    //    StartCoroutine(DamageOncolorChange());
    //}

    //IEnumerator DamageOncolorChange()
    //{
    //    playerSprite.color = Color.red;

    //    yield return new WaitForSeconds(2f);
    //    playerSprite.color = Color.white;
    //}




}
