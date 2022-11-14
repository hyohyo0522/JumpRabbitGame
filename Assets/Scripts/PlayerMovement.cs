using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update

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
    private int jumpCount = 0;
    private int jumpMaxCount = 1;
    private int enableJumpCount = 10;



    //Groundüũ
    [SerializeField] Transform groundCheckCollider;
    [SerializeField] Transform ladderColliderCheck1;
    [SerializeField] Transform ladderColliderCheck2;

    //������ üũ 
    [SerializeField] Transform ItemCheckCollider;


    public LayerMask groundMask;
    public LayerMask LadderMask;
    const float groundCheckRadius = 0.6f;
    public bool isGrounded;
    static public bool isLadder = false;

    public bool JumpEnable = true; // ��ٸ� �ö���ٰ� UPŰ ������ �ڵ����� �����Ǵ� �� ����
    const float gravityY = 9.81f;

    //��ٸ� �ǳ� �� �� Collision �����ؾ���
    public Collider2D platformCollider;

    // �� ���� ���� �� �ö��� �̺�Ʈ �ߵ�
    int maskPlayer = 1 >> 8; // �÷��̾� ���̾� ����ũ 
    int maskGround = 1 >> 6; // Ground(��ٸ�����) ���̾� ����ũ
    int maskMonFlying = 1 >> 10; // ���ƴٴϴ� �ö��� ���� ����ũ
    bool nowFlying = false; // ���� �ִ����� üũ�ϴ� �Ҹ�����

    //�� ����Ʈ ��������
    public GameObject pung; // ������ �ִϸ��̼� ����� ���ӿ�����Ʈ����
    float pungAniPlayTime = 0.55f;
    private Vector2 pungDisapperPosition;
    private Vector2 pungRevivePosition;

    //��Ȱ���� �̺�Ʈ >> PlayerLife���� ü��ȸ���̺�Ʈ�� ����. 
    public event Action ImRevie;
    bool notRevive;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();


        //���� ī��Ʈ ������ ���� �̺�Ʈ ��� 
        //Ground ground = FindObjectOfType<Ground>();
        //ground.playerTouched += resetJumpCount;

        

    }



    private void FixedUpdate()
    {

        if (!LadderCheck())//��ٸ��� �ִµ�����  isGrounded �� true�� �Ǵ� ���� �����Ѵ�. 
        {
            Physics2D.IgnoreCollision(playerCollider, platformCollider, false); // �� �ݶ��̴� �����Ѵ�.
            Physics2D.IgnoreLayerCollision(maskPlayer, maskMonFlying, false); // �ö��׸��� ���̾� �����Ѵ�.
            playerRigidbody.gravityScale = gravityY;
            GroundCheck();
            Jump();
            Walk();
            HasNearItem();
            //CheckbeingFlying();


            //������ �ƴ϶� ������ ������ �������� �ִϸ��̼��� ����Ǿ�� �Ѵ�. 
            playerAnimator.SetFloat("yvelocity", playerRigidbody.velocity.y);
            playerAnimator.SetBool("DoJump", !isGrounded);
        }
        else
        {

            Physics2D.IgnoreCollision(playerCollider, platformCollider, true); // ��ٸ� �ִ� ������ Ground Collider ���� 
            Physics2D.IgnoreLayerCollision(maskPlayer, maskMonFlying, true); // ��ٸ��� �ִ� ������ �ö��׸��� ���̾� ���� 
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


        }//�ٸ� ���ٽ� ��ٸ��� �Ʒ��� �ִ� ���
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
            if (wasLaddered) { StartCoroutine("makeTermforJump"); }
            isLadder = false;
            playerAnimator.SetBool("DoClimb", false);
        }

        return isLadder;
    }

    void Walk()
    {
        if (notRevive) return;
        //�¿��̵�
        m_HorizontalMovement = Input.GetAxis("Horizontal");
        playerRigidbody.velocity = new Vector2(speed * m_HorizontalMovement, playerRigidbody.velocity.y);

        //�ȱ� �ִϸ��̼�
        if (m_HorizontalMovement != 0)
        {
            playerAnimator.SetBool("DoWalk", true);
        }
        else playerAnimator.SetBool("DoWalk", false);


    }

    void Jump()
    {
        if (notRevive) return;

        if (Input.GetKey(KeyCode.UpArrow))
        {

            if (!isGrounded) return;

            if (!JumpEnable) return;
            //���� �ִ� Ƚ�� ����
            if (jumpCount >= jumpMaxCount) return;

            if (enableJumpCount <= 0) return;

            float jumpForce = 2000f;
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
            jumpCount++;
            enableJumpCount--;
            UIManager.instance.UpdateCarrotText(enableJumpCount); // UI����
            Debug.Log("����ī��Ʈ�� 1�� �Ǿ���.");
        }

    }

    void resetJumpCount()
    {
        jumpCount = 0;
        Debug.Log("����ī��Ʈ�� 0�� �Ǿ���.");
    }

    //��� �Ծ��� �� ����ī��Ʈ �ö󰡴� ȿ��
    public void JumpCountUp(int value)
    {
        enableJumpCount += value;
        UIManager.instance.UpdateCarrotText(enableJumpCount); //UI ���� 
        //�Դ� �Ҹ��� �߰�����.
    }



    void MoveInLadder()
    {

        float speedup = 7f; // ��ٸ� ������ �ӵ�
        float speedJumpInLadder = -7f;
        m_VerticalMovement = Input.GetAxisRaw("Vertical");
        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, speedup * m_VerticalMovement);


        m_HorizontalMovement = Input.GetAxisRaw("Horizontal");
        if (m_HorizontalMovement != 0)
        {
            Collider2D[] groundedTouched = Physics2D.OverlapCircleAll((Vector2)ladderColliderCheck1.position, 0.7f, groundMask);
            if (groundedTouched.Length <= 0)
            {
                playerRigidbody.velocity = new Vector2(speed * m_HorizontalMovement, speedJumpInLadder);
            }
        }

    }


    IEnumerator makeTermforJump() // ��ٸ� ���� �ö���ڸ��� �ٷ� ������ ����
    {
        JumpEnable = false;
        yield return new WaitForSeconds(0.3f);
        JumpEnable = true;

    }

    public void AddForcetoBounce(Vector2 power)
    {
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.AddForce(power);

        //���� power�� float ���� �ٲ� �Ŀ�, playerRigidbody.AddForce(0,power)���� �߾���!
    }


    private void HasNearItem()
    {
        Collider2D[] nearItem = Physics2D.OverlapCircleAll((Vector2)ItemCheckCollider.position, 1.4f);

        if (nearItem[0].CompareTag("Item"))
        {
            IItem item = nearItem[0].GetComponent<IItem>();
            if (item != null)
            {

                ////ȣ��Ʈ�� ������ ���� ��밡��
                ////ȣ��Ʈ������ ������ ��� �Ļ��� ���������� ȿ���� ��� Ŭ���̾�Ʈ�� ����ȭ��Ŵ
                //if (PhotonNetwork.IsMasterClient)
                //{
                //    //Use�ż��带 �����Ͽ� �����ۻ��
                    item.Use(gameObject);
                //}

                ////������ ���� �Ҹ� ���
                //playerAudioPlayer.PlayOneShot(itemPickupClip); //ȿ������ ��� Ŭ���̾�Ʈ���� ����ȴ�. 
            }
        }

        // �����۰� �浹�� ��� �ش� �������� ����ϴ� ó��
        // ������� ���� ��쿡�� ������ ��� ���� 
        //�浹�� �������κ��� Item ������Ʈ �������� �õ�

            //�浹�� �������κ��� Item������Ʈ �������� �� �����ߴ�
    }


    public void CheckbeingFlying()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)groundCheckCollider.position, 0.5f, maskMonFlying);
        if (colliders.Length > 0 ) // �ö��� ���� ���� �ִ�. 
        {
            if (!nowFlying) 
            { nowFlying = true;
              Physics2D.IgnoreLayerCollision(maskPlayer, maskGround, true);
            }

        }
        else
        {
            if (nowFlying) // �ö��� ���� ���� ����. 
            {
                nowFlying = false;
                Physics2D.IgnoreLayerCollision(maskPlayer, maskGround, false);
            }

        }

    }


    //PlayerLife ��ũ��Ʈ���� �� �Լ��� ȣ���� �� ���̴�. 
    public void ImDead()
    {
        float reviveTime = 1.2f;

        // ���� �� ȿ�����
        playerAnimator.SetTrigger("Die");


        //���� ��ٸ� �ȿ� ������ ����߸���
        if (isLadder)
        {
            isLadder = false;
        }

        
        //������ٵ� �۵� ����
        this.playerRigidbody.Sleep();
        notRevive = true;  //��Ȱ�Ǳ� ������ �� �����̰� ��.


        //����Ʈ �ִϸ��̼� ��� : ��
        pungDisapperPosition = (Vector2)this.transform.position;




        //��Ȱ �Լ� �۵� 
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

        if (playerRigidbody.IsSleeping()==true) // ������ �ٵ� �ѱ� .. 
        //���̰� �ƹ����� �۵��� �ȵǴ� ���ϴ�?? ���⿡ notRevive = false; ������ �۵��ȵ�..
        //Rigidbody.Sleep�� �� �ȵǰ� ���� �� �����ؾ��ҵ�
        {
            playerRigidbody.WakeUp();

        }

        if (ImRevie != null) // ��Ȱ �̺�Ʈ �۵�
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

    //public void onDamageforChange() // �ٸ� ��ũ��Ʈ���� �� ��ũ��Ʈ �� �ڷ�ƾ �޼��� �����ϴ� ���� ������
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
