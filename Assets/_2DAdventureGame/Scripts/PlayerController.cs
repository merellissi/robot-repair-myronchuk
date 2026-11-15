using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputAction MoveAction;
    public InputAction LaunchAction;
    public InputAction TalkAction;
    public GameObject projectilePrefab;

    public float speed = 3.0f;
    public int maxHealth = 5;
    public float timeInvincible = 2.0f;

    public int health { get { return currentHealth; } }

    int currentHealth;
    bool isInvincible;
    float damageCooldown;

    Rigidbody2D rigidbody2d;
    Vector2 move;

    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

    NonPlayerCharacter lastNonPlayerCharacter;

    void Start()
    {
        MoveAction.Enable();
        LaunchAction.Enable();
        TalkAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        if (LaunchAction.WasPressedThisFrame())
        {
            Launch();
        }

        CheckForNPC();
    }

    void FixedUpdate()
    {
        Vector2 position = (Vector2)rigidbody2d.position
            + move * speed * Time.fixedDeltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            damageCooldown = timeInvincible;
            animator.SetTrigger("Hit");
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        // (float) Ч щоб д≥ленн€ було дробовим: 2 / 5.0 = 0.4, а не 0
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(
            projectilePrefab,
            rigidbody2d.position + Vector2.up * 0.5f,
            Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 300);

        animator.SetTrigger("Launch");
    }

    // ѕром≥нь у напр€мку погл€ду робота: чи Ї попереду NPC
    void CheckForNPC()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            rigidbody2d.position + Vector2.up * 0.2f,
            moveDirection, 1.5f, LayerMask.GetMask("NPC"));

        NonPlayerCharacter npc = null;
        if (hit.collider != null)
        {
            npc = hit.collider.GetComponent<NonPlayerCharacter>();
        }

        // ƒивимос€ вже не на того NPC Ч ховаЇмо стару бульбашку
        if (lastNonPlayerCharacter != null && lastNonPlayerCharacter != npc)
        {
            lastNonPlayerCharacter.dialogueBubble.SetActive(false);
            lastNonPlayerCharacter = null;
        }

        if (npc != null)
        {
            npc.dialogueBubble.SetActive(true);
            lastNonPlayerCharacter = npc;
            FindFriend();
        }
    }

    // Ќатиснули X, коли дивимос€ на NPC Ч показуЇмо репл≥ку
    void FindFriend()
    {
        if (TalkAction.WasPressedThisFrame())
        {
            UIHandler.instance.DisplayDialogue();
        }
    }
}