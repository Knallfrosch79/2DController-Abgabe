using UnityEngine;


public class PlatformerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _jumpHeight = 8f;
    [SerializeField] private float _speedMultiplier = 1f;
    [SerializeField] private float _jumpMultiplier = 1f;

    // cache Komponenten
    private Rigidbody2D _rigidbody2D;
    private AudioSource _audioSource;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    [SerializeField] private GameObject BeanModel;
    [SerializeField] private GameObject FlyModel;

    // Form Varianten
    private bool BeanForm = true, FlyForm = false;

    private Vector2 _velocity;
    private Vector2 _startPosition;

    // Variablen für die Sprung- und Bewegungsphysik
    
    [Header("Ground Detection Settings")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private float castDistance;
    [SerializeField] private LayerMask MiddleGround;
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.5f); // Position relativ zum Spieler
    [SerializeField] private LayerMask groundLayer;

    // Angrifsvariablen
    bool attack = false;

    //Coyotetime Variablen
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    // "Jumpbuffer" Variablen, also damit sich auch im Sprung das Drücken der Sprungtaste gemerkt wird
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [SerializeField] private Collider2D BeanCollider, FlyCollider;

    private void Start()
    {
        BeanModel.SetActive(true); FlyModel.SetActive(false);
        BeanForm = true; FlyForm = false;
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _animator = transform.GetChild(0).GetComponent<Animator>();
        _startPosition = transform.position;
        _isGrounded = true;
    }

    private void Update()
    {
        attack = false; // Angriff wird zurückgesetzt, damit der Spieler sich wieder bewegen kann
        // Ermöglicht Formwechsel
        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchForm();
        }
        // Coyotetime implementieren
        if (GroundDetection())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        _isGrounded = GroundDetection();
        isAttacking();
        if (attack == false)
        {
            Movement();
        }
        HandleSpriteDirection(); 
        HandleAnimations(); 
        //uo879 8uHandleWinLooseConditions();
    }

    
    private bool GroundDetection()
    {
        // Verschiebt den Ursprung des BoxCast um den festgelegten Offset
        Vector2 castOrigin = (Vector2)transform.position + groundCheckOffset;

        // Führe den BoxCast aus. Hier kannst du den entsprechenden LayerMask (z. B. Middleground)
        // angeben – falls du 'groundLayer' verwenden möchtest, ersetze Middleground durch groundLayer.
        RaycastHit2D hit = Physics2D.BoxCast(castOrigin, boxSize, 0f, Vector2.down, castDistance, MiddleGround);

        return hit.collider != null;
    }
    private void OnDrawGizmos()
    {
        // Zeichnet im Editor eine rote Box an der Position (transform.position + groundCheckOffset + nach unten versetzt)
        Gizmos.color = Color.blue;
        Vector3 drawPosition = transform.position + (Vector3)groundCheckOffset + Vector3.down * castDistance;
        Gizmos.DrawWireCube(drawPosition, boxSize);
    }

    private void isAttacking()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && _isGrounded)
        {
            _animator.SetBool("isAttacking", true);
            attack = true;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && _isGrounded)
        {
            _animator.SetBool("isHeavyAttacking", true);
            attack = true;
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && !_isGrounded)
        {
            _animator.SetBool("isJumpAttacking", true);
            attack = true;
        }
    }


    //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
    //Debug.DrawRay(transform.position, Vector2.down * 0.1f, Color.red);
    //return hit.collider != null;


    private void Movement()
    {
        //Legt die Geschwindigkeit und Sprunghöhenmultiplikatoren der Katzenform fest
        if (FlyForm)
        {
            _speedMultiplier = 3f; _jumpMultiplier = 5f;
        }
        else
        {
            _speedMultiplier = 1f; _jumpMultiplier = 1f;
        }

        // Legt die Geschwindigkeit und Sprunghöhe des Spielers fest
        _velocity.x = Input.GetAxisRaw("Horizontal") * _moveSpeed * _speedMultiplier;
        _velocity.y = _rigidbody2D.linearVelocityY;

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Wenn der Spieler springt, wird die Sprunghöhe multipliziert & Coyotetime wird abgefragt
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            _velocity.y = _jumpHeight * _jumpMultiplier;
            _audioSource.Play();
            jumpBufferCounter = 0f;
        }
        // ermöglicht ein höheres Springen, wenn der Spieler die Sprungtaste länger drückt
        if (Input.GetButtonUp("Jump") && _velocity.y > 0f)
        {
            _velocity.y = _rigidbody2D.linearVelocityY * 0.5f;

            coyoteTimeCounter = 0f;
        }

        _rigidbody2D.linearVelocity = _velocity;
        return;
    }

    private void HandleSpriteDirection()
    {
        if (_velocity.x < 0) _spriteRenderer.flipX = true;
        else if (_velocity.x > 0) _spriteRenderer.flipX = false;
    }

    private void HandleAnimations()
    {
        _animator.SetBool("IsGrounded", _isGrounded);
        _animator.SetFloat("Speed", Mathf.Abs(_velocity.x));
        _animator.SetFloat("FallSpeed", _velocity.y);
    }

    //private void HandleWinLooseConditions()
    //{
    //    if (transform.position.y < -5f) transform.position = _startPosition;
    //    if (transform.position.x > 25f) GameManager.Instance.LoadNextLevel();
    //}

    private void SwitchForm()
    {
        if (BeanForm)
        {
            BeanForm = false; FlyForm = true;
            BeanModel.SetActive(false); FlyModel.SetActive(true);
            BeanCollider.enabled = false; FlyCollider.enabled = true;
        }
        else
        {
            BeanForm = true; FlyForm = false;
            BeanModel.SetActive(true); FlyModel.SetActive(false);
            BeanCollider.enabled = true; FlyCollider.enabled = false;
        }
    }

    public void endAttack()
    {
        _animator.SetBool("isAttacking", false);
        _animator.SetBool("isHeavyAttacking", false);
        _animator.SetBool("isJumpAttacking", false);
    }
}
