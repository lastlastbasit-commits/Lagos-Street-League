
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class FootballBallController : MonoBehaviour
{
    [Header("Ball Physics")]
    public float mass = 0.43f;
    public float drag = 0.08f;
    public float angularDrag = 0.05f;
    public float maxSpeed = 35f;

    [Header("Kick Settings")]
    public float groundPassForce = 8f;
    public float loftedPassForce = 10f;
    public float shotForce = 18f;
    public float liftForce = 0.25f;

    [Header("Ball Control")]
    public float controlDistance = 1.5f;
    public Transform controllingPlayer;

    private Rigidbody rb;
    private SphereCollider ballCollider;

    public bool IsControlled { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ballCollider = GetComponent<SphereCollider>();

        rb.mass = mass;
        rb.linearDamping = drag;
        rb.angularDamping = angularDrag;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        ballCollider.radius = 0.11f;
        ballCollider.material = new PhysicsMaterial("BallMaterial")
        {
            dynamicFriction = 0.35f,
            staticFriction = 0.4f,
            bounciness = 0.65f
        };
    }

    void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        if (IsControlled && controllingPlayer != null)
        {
            CheckControlDistance();
        }
    }

    public void TakeControl(Transform player)
    {
        if (player == null) return;

        controllingPlayer = player;
        IsControlled = true;
    }

    public void ReleaseControl()
    {
        IsControlled = false;
        controllingPlayer = null;
    }

    void CheckControlDistance()
    {
        float distance = Vector3.Distance(
            transform.position,
            controllingPlayer.position
        );

        if (distance > controlDistance)
        {
            ReleaseControl();
        }
    }

    public void GroundPass(Vector3 direction)
    {
        Kick(direction, groundPassForce, 0.05f);
    }

    public void LoftedPass(Vector3 direction)
    {
        Kick(direction, loftedPassForce, liftForce);
    }

    public void Shoot(Vector3 direction)
    {
        Kick(direction, shotForce, 0.12f);
    }

    void Kick(Vector3 direction, float force, float lift)
    {
        if (direction.sqrMagnitude < 0.001f) return;

        ReleaseControl();

        rb.AddForce(
            direction.normalized * force + Vector3.up * lift,
            ForceMode.Impulse
        );
    }

    public void StopBall()
    {
        ReleaseControl();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
