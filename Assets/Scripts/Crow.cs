using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Difficulty
{
    public int attackWidth;
    public int attackDelay;
    public int minCooldown;
    public int maxCooldown;

    public Difficulty(int attackWidth, int attackDelay, int minCooldown, int maxCooldown)
    {
        this.attackWidth = attackWidth;
        this.attackDelay = attackDelay;
        this.minCooldown = minCooldown;
        this.maxCooldown = maxCooldown;
    }
}

public class Crow : MonoBehaviour, IEntity
{
    public bool isPreparing = false;
    public bool isAttacking = false;
    public Vector3 direction;
    public Vector3 targetPosition;
    public Vector3 startPosition;
    public Vector3 endPosition;

    public int attackDistance = 8;
    public int attackWidth = 3;
    public int attackDelay = 3;
    public int attackDelayTimer = 0;
    public int initialAttackCooldown = 24;
    public int attackCooldown = 3;
    public int attackCooldownTimer = 0;

    private float attackTimer;
    private bool hasAttacked = false;

    private GameObject player;
    private Bee playerBee;
    private Waggler waggler;
    private GameManager gameManager;
    private GameObject sprite;
    private GameObject cameraObject;
    private CameraShake cameraShake;

    private AudioSource audioSource;

    [Header("Telegraph Settings")]
    public GameObject telegraphGameObject;
    public TelegraphAnimator telegraph;

    public Difficulty[] difficulties = new Difficulty[]
    {
        new Difficulty(3, 4, 16, 26), // Easy
        new Difficulty(3, 3, 8, 16), // Medium
        new Difficulty(3, 2, 6, 12) // Hard
    };

    public void Summon()
    {
        attackCooldownTimer /= 2;
    }

    public void SetDifficulty(int difficulty)
    {
        var prevAttackCooldown = attackCooldown;
        attackWidth = difficulties[difficulty].attackWidth;
        attackDelay = difficulties[difficulty].attackDelay;
        attackCooldown = Random.Range(
            difficulties[difficulty].minCooldown,
            difficulties[difficulty].maxCooldown
        );

        if (!isAttacking && !isPreparing)
        {
            attackCooldownTimer = Mathf.Min(prevAttackCooldown, attackCooldown);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        player = GameObject.Find("Player");
        playerBee = player.GetComponent<Bee>();
        waggler = GameObject.Find("Waggler").GetComponent<Waggler>();

        telegraphGameObject = GameObject.Find("CrowAttackWarning");
        sprite = GetComponentInChildren<SpriteRenderer>().gameObject;
        telegraph = telegraphGameObject.GetComponentInChildren<TelegraphAnimator>();
        audioSource = GameObject.Find("CrowAudioSource").GetComponent<AudioSource>();

        cameraObject = GameObject.Find("Main Camera");
        cameraShake = cameraObject.GetComponentInChildren<CameraShake>();
        SetDifficulty(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerBee.beeState == BeeState.Storing)
        {
            attackDelayTimer = 0;
            attackCooldownTimer = attackCooldown;
            isPreparing = false;
            isAttacking = false;
            telegraph.Hide();
            audioSource.Stop();
            return;
        }
        // lerp position between start and end
        if (isAttacking)
        {
            sprite.SetActive(true);
            attackTimer += Time.deltaTime;
            var t = Mathf.Clamp01(attackTimer / gameManager.tickRate);
            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            if (!hasAttacked && attackTimer >= gameManager.tickRate / 2f)
            {
                hasAttacked = true;
                AttackPlayer();
                AttackFlowers();
            }
        }
    }

    public void Init()
    {
        attackCooldownTimer = initialAttackCooldown;
        attackDelayTimer = 0;
        isPreparing = false;
        isAttacking = false;
        sprite.SetActive(false);
        telegraph.Hide();
        SetDifficulty(0);
    }

    public void Tick()
    {
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer--;
            return;
        }

        if (isAttacking)
        {
            sprite.SetActive(false);
            telegraph.Hide();
            isAttacking = false;
            hasAttacked = false;
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
                audioSource.Play();
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

        sprite.transform.rotation = Quaternion.Euler(
            0,
            0,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90
        );
        telegraph.Show(targetPosition, direction);
    }

    void AttackPlayer()
    {
        var playerPosition = player.transform.position;

        if (IsHitByAttack(playerPosition))
        {
            cameraShake.Shake(gameManager.tickRate / 2f, 0.8f, 0.5f);
            playerBee.GetHit();
            return;
        }
        cameraShake.Shake(gameManager.tickRate / 2f, 0.4f, 2f);
    }

    void AttackFlowers()
    {
        var flowers = gameManager.flowers;
        var hitFlowers = new List<Flower>();
        foreach (var flower in flowers)
        {
            var flowerPosition = flower.transform.position;
            if (IsHitByAttack(flowerPosition))
            {
                // flower.GetComponent<Flower>().GetHit();
                hitFlowers.Add(flower.GetComponent<Flower>());
            }
        }

        foreach (var hitFlower in hitFlowers)
        {
            hitFlower.GetHit();
        }
    }

    bool IsHitByAttack(Vector3 position)
    {
        var attackDirection = (endPosition - startPosition).normalized;

        // calculate orthogonal distance based on perpendicular direction
        var perpendicularDirection = new Vector3(attackDirection.y, -attackDirection.x, 0);
        var perpendicularDistance = Vector3.Dot(position - targetPosition, perpendicularDirection);

        return Mathf.Abs(perpendicularDistance) <= attackWidth / 2;
    }
}
