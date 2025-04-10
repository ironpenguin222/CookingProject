using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Target")]
    public Transform player;
    public Transform cameraPivot;
    public Vector3 cameraOffset = new Vector3(0, 1.5f, -3f);

    [Header("Smoothness Settings")]
    public float followSpeed = 10f;
    public float rotationSpeed = 8f;

    [Header("Lock-On System")]
    public Transform lockOnTarget;
    public float lockOnDistance = 10f;
    public LayerMask enemyLayer;
    private bool isLockedOn = false;

    private Vector3 velocity = Vector3.zero;
    private Rigidbody playerRb;

    private void Start()
    {
        playerRb = player.GetComponent<Rigidbody>();

        cameraPivot.position = player.position;
        transform.position = cameraPivot.position + cameraOffset;
    }

    private void FixedUpdate()
    {
        if (isLockedOn)
        {
            LockOnToTarget();
        }
        else
        {
            SmoothFollow();
        }
    }

    private void SmoothFollow()
    {
        Vector3 targetPosition = player.position + (playerRb.velocity * Time.fixedDeltaTime);

        cameraPivot.position = Vector3.Lerp(cameraPivot.position, targetPosition, followSpeed * Time.fixedDeltaTime);

        Vector3 finalCameraPosition = cameraPivot.position + cameraOffset;

        transform.position = Vector3.SmoothDamp(transform.position, finalCameraPosition, ref velocity, 0.08f);

        Quaternion targetRotation = Quaternion.LookRotation(cameraPivot.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private void LockOnToTarget()
    {
        if (lockOnTarget == null)
        {
            isLockedOn = false;
            return;
        }

        Vector3 direction = (lockOnTarget.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    public void ToggleLockOn()
    {
        if (isLockedOn)
        {
            isLockedOn = false;
            lockOnTarget = null;
        }
        else
        {
            FindNearestEnemy();
        }
    }

    private void FindNearestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(player.position, lockOnDistance, enemyLayer);
        if (enemies.Length > 0)
        {
            lockOnTarget = enemies[0].transform;
            isLockedOn = true;
        }
    }
}
