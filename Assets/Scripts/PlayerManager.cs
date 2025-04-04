using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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
    public float tookDamage = 0;

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
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentFlasks = maxFlasks;

        if (uiManager != null) {
            uiManager.UpdateUI(currentHealth, maxHealth, currentStamina, maxStamina, currentFlasks);
                }
    }

    private void Update()
    {

        if (tookDamage == 1)
        {
            iFrames += Time.deltaTime;
        }
        if (iFrames >= 0.5)
        {
            tookDamage = 0;
            iFrames = 0;
        }

        if (Input.GetMouseButtonDown(0))
        {
            lastAttackInputTime = Time.time;
        }

        if (!isAttacking && (Time.time - lastAttackInputTime <= attackBufferTime))
        {
            StartCoroutine(SwingSword());
        }

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
        if (tookDamage == 0)
        {

                TakeDamage(1);
                tookDamage = 1;
        }
    }

    private void HandleMovement()
    {
        if (isRolling) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            lastMoveDirection = moveDirection;

            if (Input.GetKey(KeyCode.LeftShift) && currentStamina > sprintStaminaCost * Time.deltaTime)
            {
                isSprinting = true;
                currentStamina -= sprintStaminaCost * Time.deltaTime;
            }
            else
            {
                isSprinting = false;
            }

            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
            Vector3 moveVelocity = moveDirection * currentSpeed;
            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);

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
        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking && currentStamina >= 15f)
        {
            StartCoroutine(Roll());
        }
        else if (Input.GetMouseButtonDown(0) && currentStamina >= attackStaminaCost)
        {
            if (isRolling)
            {
                StopCoroutine(Roll());
            }
            StartCoroutine(SwingSword());
        }
        if (Input.GetMouseButtonDown(0) && currentStamina >= attackStaminaCost && !isAttacking)
        {
            StartCoroutine(SwingSword());
        }
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
        if (lastMoveDirection.magnitude == 0) yield break;

        currentStamina -= 10f;
        isRolling = true;
        isInvincible = true;

        Collider playerCollider = GetComponent<Collider>();
        float originalY = transform.position.y;
        Vector3 rollDirection = lastMoveDirection.normalized;

        float elapsedTime = 0f;
        float totalRotation = 360f;

        yield return new WaitForEndOfFrame();
        playerCollider.enabled = false;

        while (elapsedTime < rollDuration)
        {
            transform.position += rollDirection * rollSpeed * Time.deltaTime;

            float rotationAngle = (totalRotation / rollDuration) * Time.deltaTime;
            transform.Rotate(Vector3.right * rotationAngle, Space.Self);

            transform.position = new Vector3(transform.position.x, originalY, transform.position.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isInvincible = false;
        isRolling = false;

        playerCollider.enabled = true;
    }


    private bool queuedAttack = false;
    private bool isComboAttack = false;

    private IEnumerator SwingSword()
    {
        if (isAttacking)
        {
            if (currentStamina >= attackStaminaCost) queuedAttack = true;
            yield break;
        }

        if (currentStamina < attackStaminaCost) yield break;

        isAttacking = true;
        currentStamina -= attackStaminaCost;
        queuedAttack = false;

        if (isComboAttack)
        {
            sword.transform.position = swordSwingPosition.position;
            sword.transform.rotation = swordSwingPosition.rotation;
        }

        yield return SwingMotion(swordSwingPosition);
        yield return new WaitForSeconds(attackDuration / 4);
        yield return SwingMotion(swordIdlePosition);

        isAttacking = false;

        if (queuedAttack && currentStamina >= attackStaminaCost)
        {
            isComboAttack = true;
            StartCoroutine(SwingSword());
        }
        else
        {
            isComboAttack = false;
        }

        yield return new WaitForSeconds(attackDuration);
        StartCoroutine(AttackRecovery());
    }

    private IEnumerator SwingMotion(Transform target)
    {
        float elapsedTime = 0f;
        while (elapsedTime < attackDuration / 2)
        {
            sword.transform.position = Vector3.Lerp(sword.transform.position, target.position, swordSwingSpeed * Time.deltaTime);
            sword.transform.rotation = Quaternion.Lerp(sword.transform.rotation, target.rotation, swordSwingSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private float recoverySlowdown = 0.5f;
    private float recoveryTime = 0.3f;

    private IEnumerator AttackRecovery()
    {
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
        if (Input.GetKeyDown(KeyCode.E) && currentFlasks > 0 && currentHealth < maxHealth)
        {
            StartCoroutine(DrinkFlask());
        }
    }

    private IEnumerator DrinkFlask()
    {
        yield return new WaitForSeconds(drinkTime);

        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        currentFlasks--;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Debug.Log("you have died.");
        gameObject.SetActive(false);
    }

    private void StartBlock()
    {
        if (currentStamina < blockStaminaDrain) return;

        isBlocking = true;
    }

    private void StopBlock()
    {
        isBlocking = false;
    }

    private void UpdateShieldPosition()
    {
        Transform targetPosition = isBlocking ? shieldFrontPosition : shieldSidePosition;
        shield.transform.position = Vector3.Lerp(shield.transform.position, targetPosition.position, shieldMoveSpeed * Time.deltaTime);
        shield.transform.rotation = Quaternion.Lerp(shield.transform.rotation, targetPosition.rotation, shieldRotationSpeed * Time.deltaTime);
    }

    private void UpdateSwordPosition()
    {
        if (!isAttacking)
        {
            sword.transform.position = Vector3.Lerp(sword.transform.position, swordIdlePosition.position, swordSwingSpeed * Time.deltaTime);
            sword.transform.rotation = Quaternion.Lerp(sword.transform.rotation, swordIdlePosition.rotation, swordSwingSpeed * Time.deltaTime);
        }
    }

    private void RegenerateStamina()
    {
        if (!isRolling && !isBlocking && !isSprinting)
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.deltaTime);
        }
    }

    private void HandleLockOn()
    {
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
        Collider[] enemies = Physics.OverlapSphere(transform.position, 10f, LayerMask.GetMask("Enemy"));
        if (enemies.Length > 0)
        {
            targetEnemy = enemies[0].transform;
            isLockedOn = true;
        }
    }

    private void RotateTowardsEnemy()
    {
        Vector3 direction = (targetEnemy.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }
}
