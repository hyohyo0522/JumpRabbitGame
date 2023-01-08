using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingPlayerMove : MonoBehaviour
{
    //������Ʈ ��������
    Animator playerAnimator;
    SpriteRenderer playerSprite;
    Rigidbody2D playerRigidbody;
    Collider2D playerCollider;

    private float speed = 10f; // �¿콺�ǵ尪

    //�¿��Է�Ű ��������
    float m_HorizontalMovement;
    float m_VerticalMovement;

    //���� ī��Ʈ ���� ����
    private int jumpCount = 0;  //��� �Ծ ���� ���� ī��Ʈ
    private int jumpMaxCount = 1;



    //Groundüũ
    [SerializeField] Transform groundCheckCollider;

    public LayerMask groundMask;
    const float groundCheckRadius = 0.6f;
    public bool isGrounded;
    static public bool isLadder = false;  //? 

    public bool JumpEnable = true; // ��ٸ� �ö���ٰ� UPŰ ������ �ڵ����� �����Ǵ� �� ����
    const float gravityY = 9.81f;

    //��ٸ� �ǳ� �� �� Collision �����ؾ���
    public Collider2D platformCollider;

    // �� ���� ���� �� �ö��� �̺�Ʈ �ߵ�
    int maskPlayer = 1 >> 8; // �÷��̾� ���̾� ����ũ 
    int maskGround = 1 >> 6; // Ground(��ٸ�����) ���̾� ����ũ
    int maskMonFlying = 1 >> 10; // ���ƴٴϴ� �ö��� ���� ����ũ
    bool nowFlying = false; // ���� �ִ����� üũ�ϴ� �Ҹ�����




    // Start is called before the first frame update
    void Start()
    {
        // �г��� ��������!
        this.transform.GetChild(0).GetChild(0).GetComponentInChildren<Text>().text = PlayerPrefs.GetString("_myNick"); 

        // ������ �̰���� �Ǵ��ؼ� �ִϸ����� �����ϱ�
        


        playerAnimator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();


    }

    private void FixedUpdate()
    {
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false); // �� �ݶ��̴� �����Ѵ�.
        playerRigidbody.gravityScale = gravityY;
        GroundCheck();

        if (Input.GetKey(KeyCode.UpArrow))
        {
            Jump();

        }
        Walk();

        //CheckbeingFlying();


        //������ �ƴ϶� ������ ������ �������� �ִϸ��̼��� ����Ǿ�� �Ѵ�. 
        playerAnimator.SetFloat("yvelocity", playerRigidbody.velocity.y);
        playerAnimator.SetBool("DoJump", !isGrounded);


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
            if (jumpCount != 0) { resetJumpCount(); } // ����ī��Ʈ �����Ѵ�.


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

    void resetJumpCount()
    {
        jumpCount = 0;
    }

    void Walk()
    {

        //m_HorizontalMovement = Input.GetAxis("Horizontal");
        m_HorizontalMovement = Ending_UIManager.instance.GetHorizontalValue();
        playerRigidbody.velocity = new Vector2(speed * m_HorizontalMovement, playerRigidbody.velocity.y);

        //�ȱ� �ִϸ��̼�
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

        //���� �ִ� Ƚ�� ����
        if (jumpCount >= jumpMaxCount) return;


        float jumpForce = 2000f;
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(new Vector2(0, jumpForce));
        jumpCount++;






    }

}
