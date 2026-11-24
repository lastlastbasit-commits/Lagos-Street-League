
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GoalDetector : MonoBehaviour
{
    [Header("Match Manager")]
    public MatchScoreManager scoreManager;

    [Header("Goal Settings")]
    public bool isHomeGoal;

    private void Awake()
    {
        BoxCollider goalTrigger = GetComponent<BoxCollider>();
        goalTrigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (scoreManager == null)
            return;

        FootballBallController ball =
            other.GetComponent<FootballBallController>();

        if (ball == null)
            return;

        if (isHomeGoal)
            scoreManager.ScoreAwayGoal();
        else
            scoreManager.ScoreHomeGoal();

        ball.StopBall();

        Debug.Log(isHomeGoal ? "AWAY TEAM SCORED!" : "HOME TEAM SCORED!");
    }
}
