using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crow : MonoBehaviour, IEntity
{
    public bool isPreparing = false;
    public bool isAttacking = false;
    public Vector3 direction;
    public Vector3 targetPosition;
    public Vector3 startPosition;
    public Vector3 endPosition;

    public int attackWidth = 3;
    public int attackDelay = 3;
    public int attackDelayTimer = 0;
    public int initialAttackCooldown = 24;
    public int attackCooldown = 3;
    public int attackCooldownTimer = 0;

    private float attackTimer;

    private GameObject player;
    private Bee playerBee;
    private Waggler waggler;
    private GameManager gameManager;
    private int attackDistance = 8;

    [Header("Telegraph Settings")]
    public GameObject telegraphGameObject;
    public TelegraphAnimator telegraph;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        player = GameObject.Find("Player");
        playerBee = player.GetComponent<Bee>();
        waggler = GameObject.Find("Waggler").GetComponent<Waggler>();

        telegraphGameObject = GameObject.Find("CrowAttackWarning");
        telegraph = telegraphGameObject.GetComponentInChildren<TelegraphAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        // lerp position between start and end
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            var t = Mathf.Clamp01(attackTimer / gameManager.tickRate);
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
        }
    }

    public void Init()
    {
        attackCooldownTimer = initialAttackCooldown;
        attackDelayTimer = 0;
        isPreparing = false;
        isAttacking = false;
    }

    public void Tick()
    {
        if (playerBee.beeState == BeeState.Storing)
        {
            return;
        }

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer--;
            return;
        }

        if (isAttacking)
        {
            Attack();
            telegraph.Hide();
            isAttacking = false;
            attackTimer = 0.0f;
            attackCooldownTimer = attackCooldown;
            return;
        }

        if (isPreparing)
        {
            attackDelayTimer++;
            if (attackDelayTimer >= attackDelay)
            {
                isPreparing = false;
                attackDelayTimer = 0;
                isAttacking = true;
                attackTimer = 0.0f;
            }

            return;
        }

        PrepareForAttack();
    }

    void PrepareForAttack()
    {
        isPreparing = true;
        targetPosition = player.transform.position;

        if (waggler.lastDirection == Vector2.zero)
        {
            direction = Vector3.down;
        }
        else
        {
            direction = new Vector3(waggler.lastDirection.x, waggler.lastDirection.y, 0);
        }

        startPosition = targetPosition - (direction * attackDistance);
        endPosition = targetPosition + (direction * attackDistance * 2);
        transform.position = startPosition;

        telegraphGameObject.transform.position = targetPosition;
        telegraphGameObject.transform.rotation = Quaternion.Euler(
            0,
            0,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90
        );
        telegraph.Show(targetPosition, direction);
    }

    void Attack()
    {
        var playerPosition = player.transform.position;
        var attackDirection = (endPosition - startPosition).normalized;
        var perpendicular = new Vector3(attackDirection.y, -attackDirection.x, 0);
        var perpendicularDistance = Vector3.Dot(attackDirection, perpendicular);

        if (Mathf.Abs(perpendicularDistance) <= attackWidth / 2)
        {
            Debug.Log("Player hit!");
            playerBee.GetHit();
        }
    }
}