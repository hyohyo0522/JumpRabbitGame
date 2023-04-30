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

    //점프 카운트 관련 변수
    private int jumpCount = 0;  //
    private int jumpMaxCount = 1;



    //Ground체크
    [SerializeField] Transform groundCheckCollider;

    public LayerMask groundMask;
    const float groundCheckRadius = 0.6f;
    public bool isGrounded;
    static public bool isLadder = false;  //? 

    const float gravityY = 9.81f;

    //사다리 건널 때 땅 Collision 무시해야함
    public Collider2D platformCollider;


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

        Walk();

        //CheckbeingFlying();


        //점프가 아니라 떨어질 때에도 떨어지는 애니메이션이 적용되어야 한다. 
        playerAnimator.SetFloat("yvelocity", playerRigidbody.velocity.y);
        playerAnimator.SetBool("DoJump", !isGrounded);


        //x 뒤집기
        playerSprite.flipX = (m_HorizontalMovement < -0.1f) ? true : false;

    }


    public void GroundCheck()
    {
        bool wasGrounded = isGrounded;


        //땅출동 감지
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)groundCheckCollider.position, groundCheckRadius, groundMask);

        if (colliders.Length > 0) //땅에 닿았다.
        {
            isGrounded = true;
            if (jumpCount != 0) { resetJumpCount(); } // 점프카운트 갱신한다.


            if (!wasGrounded) // 땅에 이제 막 닿았다면 속도를 0으로 만들어준다.
            {
                playerRigidbody.velocity = Vector2.zero;

            }
        }
        else   //땅에 닿지 않았다.
        {
            isGrounded = false;

        }
    }

    void resetJumpCount()
    {
        jumpCount = 0;
    }

    void Walk()
    {

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

 
        if (!isGrounded) return; //땅에 있을 때에만 점프 

        //점프 최대 횟수 제한
        if (jumpCount >= jumpMaxCount) return;


        float jumpForce = 2000f;
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(new Vector2(0, jumpForce));
        jumpCount++;






    }

}
