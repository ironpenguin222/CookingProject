using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("UI References")]
    public PlayerUIManager uiManager;

    [Header("Health & Stamina")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f;
    public bool tookDamage = false;

    [Header("Healing System")]
    public int maxFlasks = 3;
    private int currentFlasks;
    public float healAmount = 50f;
    public float drinkTime = 2f;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private Animator animator;

    [Header("Rolling")]
    public float rollSpeed = 8f;
    public float rollDuration = 0.8f;
    public float invincibilityTime = 0.5f;
    private bool isRolling = false;
    private bool isInvincible = false;
    private Vector3 lastMoveDirection;

    [Header("Combat")]
    public GameObject sword;
    public Transform swordIdlePosition;
    public Transform swordSwingPosition;
    public float swordSwingSpeed = 10f;
    public float attackStaminaCost = 20f;
    public float attackDuration = 0.5f;
    public bool isAttacking = false;
    public float attackBufferTime = 0.3f;
    private float lastAttackInputTime = 0f;
    public GameObject enemy;
    public float iFrames;
    public float attackCount;
    public float timeWindow;
    public float lastAttackTime = -999f;
    public EnemyHealth em;

    [Header("Blocking")]
    public GameObject shield;
    public Transform shieldSidePosition;
    public Transform shieldFrontPosition;
    public float shieldMoveSpeed = 10f;
    public float shieldRotationSpeed = 10f;
    private bool isBlocking = false;
    public float blockStaminaDrain = 5f;

    [Header("Lock-On System")]
    public Transform targetEnemy;
    private bool isLockedOn = false;

    [Header("Sprinting")]
    public float sprintSpeed = 8f;
    public float sprintStaminaCost = 15f;
    private bool isSprinting = false;


    private void Start()
    {
        // Sets up important parameters for use. Default values

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentFlasks = maxFlasks;

        if (uiManager != null)
        {
            uiManager.UpdateUI(currentHealth, maxHealth, currentStamina, maxStamina, currentFlasks);
        }
        lastAttackInputTime = Time.time;
    }

    private void Update()
    {
        // Checks when last attacked and increases suspicion if it's been too long

        if (Time.time - lastAttackTime > timeWindow)
        {
            em.suspicion += 0.4f * Time.deltaTime;
        }
        if (tookDamage)
        {
            iFrames += Time.deltaTime;
        }
        if (iFrames >= 2)
        {
            tookDamage = false;
            iFrames = 0;
        }

        // Checks if can swing sword

        if (!isAttacking && (Time.time - lastAttackInputTime <= attackBufferTime))
        {
            StartCoroutine(SwingSword());
        }

        // Handles all the functions the player has

        if (isRolling || isAttacking) return;

        HandleMovement();
        HandleCombat();
        HandleLockOn();
        HandleHealing();

        RegenerateStamina();
        UpdateShieldPosition();
        UpdateSwordPosition();

        uiManager.UpdateUI(currentHealth, maxHealth, currentStamina, maxStamina, currentFlasks);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checks if hit by arrow and loses health

        if (other.gameObject.tag == "arrow")
        {
            Debug.Log("arrow hit");
            Destroy(other);
            TakeDamage(10);
            tookDamage = true;
        }

        // Sees if player is being hit by the damage window of the enemy attack and if blocking, it prevents the damage but takes stamina

        if (!tookDamage && AttackPlayerAT.damageWindow)
        {
            if (isBlocking)
            {
                currentStamina -= blockStaminaDrain;
            }
            else
            {
                TakeDamage(10);
                tookDamage = true;
            }
        }
    }

    private void HandleMovement()
    {
        if (isRolling) return;

        // Get movement values to set movement

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            lastMoveDirection = moveDirection;

            // Sprinting logic, takes more stamina

            if (Input.GetKey(KeyCode.LeftShift) && currentStamina > sprintStaminaCost * Time.deltaTime)
            {
                isSprinting = true;
                currentStamina -= sprintStaminaCost * Time.deltaTime;
            }
            else
            {
                isSprinting = false;
            }

            // makes current speed the sprinting speed

            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
            Vector3 moveVelocity = moveDirection * currentSpeed;
            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);

            // Checks if player is locked on to set rotation to face target

            if (!isLockedOn)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }


    private void HandleCombat()
    {
        // Checks if should roll

        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking && currentStamina >= 15f)
        {
            StartCoroutine(Roll());
        }

        // Queues up the attack while rolling so that you can attack out of a roll

        else if (Input.GetMouseButtonDown(0) && currentStamina >= attackStaminaCost)
        {
            if (isRolling)
            {
                StopCoroutine(Roll());
            }
            StartCoroutine(SwingSword());
            lastAttackInputTime = Time.time;
        }

        // Basic attack woth sword, make sure you can swing, then makes it swing

        if (Input.GetMouseButtonDown(0) && currentStamina >= attackStaminaCost && !isAttacking)
        {
            StartCoroutine(SwingSword());
            lastAttackInputTime = Time.time;
        }

        // Blocks on key press

        if (Input.GetMouseButtonDown(1))
        {
            StartBlock();
        }
        if (Input.GetMouseButtonUp(1))
        {
            StopBlock();
        }
    }

    private IEnumerator Roll()
    {
        // Checks if it has direction and if not then its done

        if (lastMoveDirection.magnitude == 0) yield break;

        // sets the rolling state and reduces stamina

        currentStamina -= 10f;
        isRolling = true;
        isInvincible = true;

        // Sets the roll direction and gets the collider

        Collider playerCollider = GetComponent<Collider>();
        float originalY = transform.position.y;
        Vector3 rollDirection = lastMoveDirection.normalized;

        float elapsedTime = 0f;
        float totalRotation = 360f;

        // Makes collider false for smooth rotation without bumping on ground

        yield return new WaitForEndOfFrame();
        playerCollider.enabled = false;

        // Moves and rotates the player based on the parameters for as long as the duration, keeping y the same so they don't phase through the floor

        while (elapsedTime < rollDuration)
        {
            transform.position += rollDirection * rollSpeed * Time.deltaTime;

            float rotationAngle = (totalRotation / rollDuration) * Time.deltaTime;
            transform.Rotate(Vector3.right * rotationAngle, Space.Self);

            transform.position = new Vector3(transform.position.x, originalY, transform.position.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ending the roll / resetting values

        isInvincible = false;
        isRolling = false;

        playerCollider.enabled = true;
    }


    private bool queuedAttack = false;
    private bool isComboAttack = false;

    private IEnumerator SwingSword()
    {
        // Starts the attack

        attackCount++;
        if (isAttacking)
        {
            // queues up attacks if you have enough resources

            if (currentStamina >= attackStaminaCost) queuedAttack = true;
            yield break;
        }

        // if not enough stamina it doesn't work

        if (currentStamina < attackStaminaCost) yield break;

        // sets player as attacking and reduces stamina

        isAttacking = true;
        currentStamina -= attackStaminaCost;
        queuedAttack = false;

        // checks if the attack is a combo

        if (isComboAttack)
        {
            sword.transform.position = swordSwingPosition.position;
            sword.transform.rotation = swordSwingPosition.rotation;
        }

        // plays the swing motion from these positions

        yield return SwingMotion(swordSwingPosition);
        yield return new WaitForSeconds(attackDuration / 4);
        yield return SwingMotion(swordIdlePosition);

        isAttacking = false;

        if (queuedAttack && currentStamina >= attackStaminaCost)
        {
            // if queued attack it plays again

            isComboAttack = true;
            StartCoroutine(SwingSword());
        }
        else
        {
            isComboAttack = false;
        }

        // Wait for the attac duration and then recover

        yield return new WaitForSeconds(attackDuration);
        StartCoroutine(AttackRecovery());
        lastAttackTime = Time.time;
    }

    private IEnumerator SwingMotion(Transform target)
    {
        // Uses lerps based on the attack duration to smoothly move the sword

        float elapsedTime = 0f;
        while (elapsedTime < attackDuration / 2)
        {
            sword.transform.position = Vector3.Lerp(sword.transform.position, target.position, swordSwingSpeed * Time.deltaTime);
            sword.transform.rotation = Quaternion.Lerp(sword.transform.rotation, target.rotation, swordSwingSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    // Recovery variables

    private float recoverySlowdown = 0.5f;
    private float recoveryTime = 0.3f;

    private IEnumerator AttackRecovery()
    {
        // Time needed for the player to recover from the attack. Temp slowdown

        float elapsedTime = 0f;
        float originalSpeed = moveSpeed;

        while (elapsedTime < recoveryTime)
        {
            moveSpeed = Mathf.Lerp(originalSpeed * recoverySlowdown, originalSpeed, elapsedTime / recoveryTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        moveSpeed = originalSpeed;
    }

    private void HandleHealing()
    {
        // When you press E, you heal

        if (Input.GetKeyDown(KeyCode.E) && currentFlasks > 0 && currentHealth < maxHealth)
        {
            StartCoroutine(DrinkFlask());
        }
    }

    private IEnumerator DrinkFlask()
    {
        // Waits drinking time and then heals player

        yield return new WaitForSeconds(drinkTime);

        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        currentFlasks--;
    }

    public void TakeDamage(int damage)
    {
        // Deals damage to the player which lowers their current health and if 0 they die

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        attackCount--;
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        // dead player

        Debug.Log("you have died.");
        gameObject.SetActive(false);
    }

    private void StartBlock()
    {
        // If has stamina you can block

        if (currentStamina < blockStaminaDrain) return;

        isBlocking = true;
    }

    // Stops blocking

    private void StopBlock()
    {
        isBlocking = false;
    }

    private void UpdateShieldPosition()
    {
        // Lerps to move shield pos based on necessary pos

        Transform targetPosition = isBlocking ? shieldFrontPosition : shieldSidePosition;
        shield.transform.position = Vector3.Lerp(shield.transform.position, targetPosition.position, shieldMoveSpeed * Time.deltaTime);
        shield.transform.rotation = Quaternion.Lerp(shield.transform.rotation, targetPosition.rotation, shieldRotationSpeed * Time.deltaTime);
    }

    private void UpdateSwordPosition()
    {
        // Updates the sword position based on lerp of transforms

        if (!isAttacking)
        {
            sword.transform.position = Vector3.Lerp(sword.transform.position, swordIdlePosition.position, swordSwingSpeed * Time.deltaTime);
            sword.transform.rotation = Quaternion.Lerp(sword.transform.rotation, swordIdlePosition.rotation, swordSwingSpeed * Time.deltaTime);
        }
    }

    private void RegenerateStamina()
    {
        // Regen stamina if not doing anything stamina taking

        if (!isRolling && !isBlocking && !isSprinting)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.deltaTime);
        }
    }

    private void HandleLockOn()
    {
        // Handles lock on

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isLockedOn)
            {
                isLockedOn = false;
                targetEnemy = null;
            }
            else
            {
                LockOnToNearestEnemy();
            }
        }

        if (isLockedOn && targetEnemy != null)
        {
            RotateTowardsEnemy();
        }
    }

    private void LockOnToNearestEnemy()
    {
        // Finds an enemy and locks onto them based on the layermask

        Collider[] enemies = Physics.OverlapSphere(transform.position, 10f, LayerMask.GetMask("Enemy"));
        if (enemies.Length > 0)
        {
            targetEnemy = enemies[0].transform;
            isLockedOn = true;
        }
    }

    private void RotateTowardsEnemy()
    {
        // Consistently rotates to face the enemy while locked on

        Vector3 direction = (targetEnemy.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }
}
