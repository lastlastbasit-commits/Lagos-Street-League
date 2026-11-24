
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class CareerClub
{
    public string clubName = "New Club";
    public string managerName = "Manager";
    public int reputation = 1;
    public int budget = 1000000;
    public int season = 1;
    public int trophies = 0;
}

[Serializable]
public class CareerStatistics
{
    public int matchesPlayed;
    public int wins;
    public int draws;
    public int losses;
    public int goalsScored;
    public int goalsConceded;
}

[Serializable]
public class CareerSaveData
{
    public CareerClub club = new CareerClub();
    public CareerStatistics statistics = new CareerStatistics();
    public List<PlayerRecord> squad = new List<PlayerRecord>();
}

public class CareerModeManager : MonoBehaviour
{
    [Header("Career Information")]
    public CareerSaveData career = new CareerSaveData();

    [Header("Career Settings")]
    public int startingBudget = 1000000;
    public bool careerStarted;

    private const string SaveKey = "LagosStreetLeague_Career";

    public void StartNewCareer(string clubName, string managerName)
    {
        career = new CareerSaveData();

        career.club.clubName = clubName;
        career.club.managerName = managerName;
        career.club.budget = startingBudget;

        careerStarted = true;

        SaveCareer();

        Debug.Log("Career started: " + clubName);
    }

    public void RecordMatch(int goalsFor, int goalsAgainst)
    {
        if (!careerStarted)
            return;

        career.statistics.matchesPlayed++;
        career.statistics.goalsScored += goalsFor;
        career.statistics.goalsConceded += goalsAgainst;

        if (goalsFor > goalsAgainst)
            career.statistics.wins++;
        else if (goalsFor == goalsAgainst)
            career.statistics.draws++;
        else
            career.statistics.losses++;

        SaveCareer();
    }

    public bool SpendMoney(int amount)
    {
        if (amount <= 0 || amount > career.club.budget)
            return false;

        career.club.budget -= amount;

        SaveCareer();

        return true;
    }

    public void EarnMoney(int amount)
    {
        if (amount <= 0)
            return;

        career.club.budget += amount;

        SaveCareer();
    }

    public void AddTrophy()
    {
        career.club.trophies++;

        SaveCareer();
    }

    public void StartNextSeason()
    {
        if (!careerStarted)
            return;

        career.club.season++;

        SaveCareer();
    }

    public void SaveCareer()
    {
        string json = JsonUtility.ToJson(career);

        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public bool LoadCareer()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
            return false;

        string json = PlayerPrefs.GetString(SaveKey);

        career = JsonUtility.FromJson<CareerSaveData>(json);

        careerStarted = career != null;

        return careerStarted;
    }

    public void DeleteCareer()
    {
        PlayerPrefs.DeleteKey(SaveKey);

        career = new CareerSaveData();
        careerStarted = false;
    }
}
