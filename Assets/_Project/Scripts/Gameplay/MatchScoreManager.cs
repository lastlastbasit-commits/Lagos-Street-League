
using UnityEngine;
using System;

public class MatchScoreManager : MonoBehaviour
{
    [Header("Match Settings")]
    public float matchDuration = 90f;
    public bool startMatchAutomatically = true;

    [Header("Score")]
    public int homeScore = 0;
    public int awayScore = 0;

    [Header("Match State")]
    public bool matchStarted;
    public bool matchFinished;
    public float timeRemaining;

    public event Action<int, int> OnScoreChanged;
    public event Action<float> OnTimeUpdated;
    public event Action OnMatchStarted;
    public event Action OnMatchFinished;

    void Start()
    {
        timeRemaining = matchDuration;

        if (startMatchAutomatically)
            StartMatch();
    }

    void Update()
    {
        if (!matchStarted || matchFinished)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining < 0f)
            timeRemaining = 0f;

        OnTimeUpdated?.Invoke(timeRemaining);

        if (timeRemaining <= 0f)
            FinishMatch();
    }

    public void StartMatch()
    {
        if (matchFinished)
            return;

        matchStarted = true;
        timeRemaining = matchDuration;

        OnMatchStarted?.Invoke();

        Debug.Log("MATCH STARTED!");
    }

    public void ScoreHomeGoal()
    {
        if (!matchStarted || matchFinished)
            return;

        homeScore++;

        OnScoreChanged?.Invoke(homeScore, awayScore);

        Debug.Log("HOME GOAL! " + homeScore + " - " + awayScore);
    }

    public void ScoreAwayGoal()
    {
        if (!matchStarted || matchFinished)
            return;

        awayScore++;

        OnScoreChanged?.Invoke(homeScore, awayScore);

        Debug.Log("AWAY GOAL! " + homeScore + " - " + awayScore);
    }

    public void ResetMatch()
    {
        homeScore = 0;
        awayScore = 0;
        timeRemaining = matchDuration;

        matchStarted = false;
        matchFinished = false;

        OnScoreChanged?.Invoke(homeScore, awayScore);
    }

    public void FinishMatch()
    {
        if (matchFinished)
            return;

        matchStarted = false;
        matchFinished = true;

        OnMatchFinished?.Invoke();

        Debug.Log(
            "FULL TIME! " + homeScore + " - " + awayScore
        );
    }

    public string GetFormattedTime()
    {
        int totalSeconds = Mathf.CeilToInt(timeRemaining);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
