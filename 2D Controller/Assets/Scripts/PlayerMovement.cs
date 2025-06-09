using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Bewegungseinstellungen
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float jumpHeight = 6f;

    private float speedMultiplier = 1f;
    private float jumpMultiplier = 1f;

    // Komponenten
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Formwechsel
    [SerializeField] private GameObject humanModel;
    [SerializeField] private GameObject catModel;
    private bool humanForm = true;
    private bool catForm = false;
    private SpriteRenderer humanSpriteRenderer;
    private SpriteRenderer catSpriteRenderer;

    // Collider für Formen
    [SerializeField] private Collider2D humanCollider;
    [SerializeField] private Collider2D catCollider;

    // Ground Check
    [Header("Ground Detection Settings")]
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.5f); // Position relativ zum Spieler
    [SerializeField] private Vector2 boxSize = new Vector2(0.5f, 0.1f);           // Größe der Box
    [SerializeField] private float castDistance = 0.1f;                           // Wie weit gecastet wird
    [SerializeField] private LayerMask groundLayer;                               // Welche Layer als Boden zählen
    [SerializeField] private bool isGrounded;

    // Startposition (für Reset)
    private Vector2 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Form initialisieren
        humanModel.SetActive(true);
        catModel.SetActive(false);
        humanForm = true;
        catForm = false;


        // Komponenten holen
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        animator = transform.GetChild(0).GetComponent<Animator>();
        //Sprite Renderer der Formen holen
        humanSpriteRenderer = humanModel.GetComponent<SpriteRenderer>();
        catSpriteRenderer = catModel.GetComponent<SpriteRenderer>();

        // Startposition speichern
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Formwechsel mit Taste "F"
        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchForm();
        }

        // Boden überprüfen
        isGrounded = GroundDetection();

        // Bewegung & Logik
        Movement();
        HandleSpriteDirection();
        HandleAnimations();
        HandleWinLooseConditions();
    }


    private bool GroundDetection()
    {
        Vector2 castOrigin = (Vector2)transform.position + groundCheckOffset;

        RaycastHit2D hit = Physics2D.BoxCast(castOrigin, boxSize, 0f, Vector2.down, castDistance, groundLayer);
        isGrounded = hit.collider != null;
        return isGrounded;
    }

    // Zum Visualisieren im Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 drawPosition = transform.position + (Vector3)groundCheckOffset + Vector3.down * castDistance;
        Gizmos.DrawWireCube(drawPosition, boxSize);
    }

    /// <summary>
    /// Bewegt den Spieler basierend auf Eingabe.
    /// Nutzt altes Input-System.
    /// </summary>
    private void Movement()
    {
        // Formabhängige Werte setzen
        if (catForm)
        {
            speedMultiplier = 1.2f; // Katze ist 20% schneller
            jumpMultiplier = 0.666f; // Katze springt 2/3 so hoch
        }
        else
        {
            speedMultiplier = 1f;
            jumpMultiplier = 1f;
        }

        // Horizontalen Input lesen (-1, 0, 1)
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Neue X-Position berechnen
        Vector2 targetPosition = rb.position + new Vector2(moveInput * moveSpeed * speedMultiplier * Time.fixedDeltaTime, 0);

        // Position bewegen
        rb.MovePosition(targetPosition);

        // Springen bei Space, wenn grounded
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Null Y-Velocity vor dem Sprung (besser für gleichmäßiges Springen)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            // AddForce benutzen für Sprung
            rb.AddForce(Vector2.up * jumpHeight * jumpMultiplier, ForceMode2D.Impulse);

            // Sound abspielen
            audioSource.Play();
        }
    }

    /// <summary>
    /// Dreht das Sprite abhängig von Bewegungsrichtung.
    /// </summary>
    private void HandleSpriteDirection()
    {
        float velocityX = rb.linearVelocity.x;

        if (humanForm == true)
        {
            if (velocityX < 0)
                humanSpriteRenderer.flipX = true;
            else if (velocityX > 0)
                humanSpriteRenderer.flipX = false;
        }
        else if (catForm == true)
        {
            if (velocityX < 0)
                catSpriteRenderer.flipX = true;
            else if (velocityX > 0)
                catSpriteRenderer.flipX = false;
        }
    }

    /// <summary>
    /// Spielt Animationen abhängig von Bewegung und Zustand ab.
    /// </summary>
    private void HandleAnimations()
    {
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocityX));
        animator.SetFloat("FallSpeed", rb.linearVelocityY);
    }

    /// <summary>
    /// �berpr�ft Sieg/Niederlage und l�dt Level neu oder weiter.
    /// </summary>
    private void HandleWinLooseConditions()
    {
        //// Verlust, wenn unter Y = -5 gefallen
        //if (transform.position.y < -5f)
        //{
        //    transform.position = startPosition;
        //}

        //// Sieg, wenn X > 25 erreicht
        //if (transform.position.x > 25f)
        //{
        //    //GameManager.Instance.LoadNextLevel();
        //}
    }

    /// <summary>
    /// Wechselt zwischen Mensch und Katze.
    /// </summary>
    private void SwitchForm()
    {
        if (humanForm)
        {
            // Wechsel zu Katze
            humanForm = false;
            catForm = true;

            humanModel.SetActive(false);
            catModel.SetActive(true);

            humanCollider.enabled = false;
            catCollider.enabled = true;
        }
        else
        {
            // Wechsel zu Mensch
            humanForm = true;
            catForm = false;

            humanModel.SetActive(true);
            catModel.SetActive(false);

            humanCollider.enabled = true;
            catCollider.enabled = false;
        }
    }
}
