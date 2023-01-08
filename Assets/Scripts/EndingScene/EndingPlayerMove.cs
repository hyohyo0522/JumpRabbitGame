using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingPlayerMove : MonoBehaviour
{
    //컴포넌트 가져오기
    Animator playerAnimator;
    SpriteRenderer playerSprite;
    Rigidbody2D playerRigidbody;
    Collider2D playerCollider;

    private float speed = 10f; // 좌우스피드값

    //좌우입력키 가져오기
    float m_HorizontalMovement;
    float m_VerticalMovement;

    //점프 카운트 관련 변수
    private int jumpCount = 0;  //당근 먹어서 생긴 점프 카운트
    private int jumpMaxCount = 1;



    //Ground체크
    [SerializeField] Transform groundCheckCollider;

    public LayerMask groundMask;
    const float groundCheckRadius = 0.6f;
    public bool isGrounded;
    static public bool isLadder = false;  //? 

    public bool JumpEnable = true; // 사다리 올라오다가 UP키 눌러서 자동으로 점프되는 것 방지
    const float gravityY = 9.81f;

    //사다리 건널 때 땅 Collision 무시해야함
    public Collider2D platformCollider;

    // 벌 위에 있을 때 플라잉 이벤트 발동
    int maskPlayer = 1 >> 8; // 플레이어 레이어 마스크 
    int maskGround = 1 >> 6; // Ground(사다리포함) 레이어 마스크
    int maskMonFlying = 1 >> 10; // 날아다니는 플라잉 몬스터 마스크
    bool nowFlying = false; // 날고 있는지를 체크하는 불린변수




    // Start is called before the first frame update
    void Start()
    {
        // 닉네임 가져오기!
        this.transform.GetChild(0).GetChild(0).GetComponentInChildren<Text>().text = PlayerPrefs.GetString("_myNick"); 

        // 졌는지 이겼는지 판단해서 애니메이터 세팅하기
        


        playerAnimator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();


    }

    private void FixedUpdate()
    {
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false); // 땅 콜라이더 감지한다.
        playerRigidbody.gravityScale = gravityY;
        GroundCheck();

        if (Input.GetKey(KeyCode.UpArrow))
        {
            Jump();

        }
        Walk();

        //CheckbeingFlying();


        //점프가 아니라 떨어질 때에도 떨어지는 애니메이션이 적용되어야 한다. 
        playerAnimator.SetFloat("yvelocity", playerRigidbody.velocity.y);
        playerAnimator.SetBool("DoJump", !isGrounded);


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
            if (jumpCount != 0) { resetJumpCount(); } // 점프카운트 갱신한다.


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

    void resetJumpCount()
    {
        jumpCount = 0;
    }

    void Walk()
    {

        //m_HorizontalMovement = Input.GetAxis("Horizontal");
        m_HorizontalMovement = Ending_UIManager.instance.GetHorizontalValue();
        playerRigidbody.velocity = new Vector2(speed * m_HorizontalMovement, playerRigidbody.velocity.y);

        //걷기 애니메이션
        if (m_HorizontalMovement != 0)
        {
            playerAnimator.SetBool("DoWalk", true);
        }
        else playerAnimator.SetBool("DoWalk", false);


    }

    public void Jump()
    {

 
        if (!isGrounded) return;
        if (!JumpEnable) return;

        //점프 최대 횟수 제한
        if (jumpCount >= jumpMaxCount) return;


        float jumpForce = 2000f;
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(new Vector2(0, jumpForce));
        jumpCount++;






    }

}
