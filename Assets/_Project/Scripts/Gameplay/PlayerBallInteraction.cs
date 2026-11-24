
using UnityEngine;

public class PlayerBallInteraction : MonoBehaviour
{
    [Header("Ball Detection")]
    public FootballBallController ball;
    public float controlRange = 1.5f;

    [Header("Kick Direction")]
    public Transform kickDirection;

    [Header("Control Settings")]
    public KeyCode controlKey = KeyCode.E;
    public KeyCode passKey = KeyCode.Q;
    public KeyCode shootKey = KeyCode.Space;
    public KeyCode loftedPassKey = KeyCode.R;

    void Update()
    {
        if (ball == null) return;

        float distance = Vector3.Distance(
            transform.position,
            ball.transform.position
        );

        if (distance > controlRange) return;

        if (Input.GetKeyDown(controlKey))
        {
            ball.TakeControl(transform);
        }

        if (Input.GetKeyDown(passKey))
        {
            ball.GroundPass(GetKickDirection());
        }

        if (Input.GetKeyDown(loftedPassKey))
        {
            ball.LoftedPass(GetKickDirection());
        }

        if (Input.GetKeyDown(shootKey))
        {
            ball.Shoot(GetKickDirection());
        }
    }

    Vector3 GetKickDirection()
    {
        if (kickDirection != null)
            return kickDirection.forward;

        return transform.forward;
    }
}
