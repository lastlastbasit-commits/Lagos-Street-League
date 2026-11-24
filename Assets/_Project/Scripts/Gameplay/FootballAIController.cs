
using UnityEngine;

public enum AIDifficulty
{
    Easy,
    Medium,
    Hard,
    ExtremelyHard,
    Legends
}

public class FootballAIController : MonoBehaviour
{
    [Header("AI Difficulty")]
    public AIDifficulty difficulty = AIDifficulty.Medium;

    [Header("Target")]
    public Transform ball;
    public Transform opponentGoal;

    [Header("Movement")]
    public float baseMoveSpeed = 4f;
    public float turnSpeed = 360f;
    public float stoppingDistance = 1.2f;

    [Header("AI Behaviour")]
    public float decisionInterval = 0.5f;
    public float shootingRange = 12f;
    public float passingRange = 9f;

    private float nextDecisionTime;
    private Vector3 targetPosition;
    private Rigidbody rb;

    private float reactionSpeed;
    private float movementMultiplier;
    private float decisionQuality;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ApplyDifficulty();
    }

    void Update()
    {
        if (ball == null)
            return;

        if (Time.time >= nextDecisionTime)
        {
            MakeDecision();
            nextDecisionTime = Time.time + decisionInterval;
        }
    }

    void FixedUpdate()
    {
        MoveTowardsTarget();
    }

    public void SetDifficulty(AIDifficulty newDifficulty)
    {
        difficulty = newDifficulty;
        ApplyDifficulty();
    }

    void ApplyDifficulty()
    {
        switch (difficulty)
        {
            case AIDifficulty.Easy:
                reactionSpeed = 1.2f;
                movementMultiplier = 0.75f;
                decisionQuality = 0.45f;
                break;

            case AIDifficulty.Medium:
                reactionSpeed = 0.8f;
                movementMultiplier = 0.9f;
                decisionQuality = 0.65f;
                break;

            case AIDifficulty.Hard:
                reactionSpeed = 0.5f;
                movementMultiplier = 1.05f;
                decisionQuality = 0.8f;
                break;

            case AIDifficulty.ExtremelyHard:
                reactionSpeed = 0.3f;
                movementMultiplier = 1.15f;
                decisionQuality = 0.92f;
                break;

            case AIDifficulty.Legends:
                reactionSpeed = 0.15f;
                movementMultiplier = 1.25f;
                decisionQuality = 1f;
                break;
        }

        decisionInterval = reactionSpeed;
    }

    void MakeDecision()
    {
        float distanceToBall =
            Vector3.Distance(transform.position, ball.position);

        if (distanceToBall < 8f)
        {
            targetPosition = ball.position;
        }
        else if (opponentGoal != null)
        {
            Vector3 direction =
                (opponentGoal.position - ball.position).normalized;

            targetPosition = ball.position - direction * 2f;
        }
        else
        {
            targetPosition = ball.position;
        }

        if (distanceToBall < 1.5f)
        {
            TryShoot();
        }
    }

    void MoveTowardsTarget()
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.magnitude <= stoppingDistance)
            return;

        Vector3 movement = direction.normalized *
            baseMoveSpeed * movementMultiplier * Time.fixedDeltaTime;

        rb.MovePosition(transform.position + movement);

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.fixedDeltaTime
            );
        }
    }

    void TryShoot()
    {
        FootballBallController ballController =
            ball.GetComponent<FootballBallController>();

        if (ballController == null || opponentGoal == null)
            return;

        float distanceToGoal = Vector3.Distance(
            ball.position,
            opponentGoal.position
        );

        if (distanceToGoal <= shootingRange &&
            Random.value <= decisionQuality)
        {
            Vector3 direction =
                (opponentGoal.position - ball.position).normalized;

            ballController.Shoot(direction);
        }
    }
}
