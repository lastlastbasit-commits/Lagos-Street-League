
using UnityEngine;
using System;
using System.Collections.Generic;

public enum PlayerPosition
{
    Goalkeeper,
    CentreBack,
    LeftBack,
    RightBack,
    DefensiveMidfielder,
    CentralMidfielder,
    AttackingMidfielder,
    LeftWinger,
    RightWinger,
    Striker
}

public enum TeamSide
{
    Home,
    Away
}

[Serializable]
public class PlayerRecord
{
    public string playerName;
    public string nationality;
    public string club;
    public int birthYear;
    public int shirtNumber;
    public int rating = 70;
    public PlayerPosition position;
    public bool isLegend;
    public bool isRetired;
}

[Serializable]
public class TeamRoster
{
    public string teamName = "New Team";

    public List<PlayerRecord> players =
        new List<PlayerRecord>();

    public List<int> startingEleven =
        new List<int>();

    public List<int> substitutes =
        new List<int>();
}

public class TeamManager : MonoBehaviour
{
    [Header("Teams")]
    public TeamRoster homeTeam = new TeamRoster();
    public TeamRoster awayTeam = new TeamRoster();

    [Header("Squad Settings")]
    public int maximumSubstitutes = 12;

    public TeamRoster GetTeam(TeamSide side)
    {
        return side == TeamSide.Home ? homeTeam : awayTeam;
    }

    public void AddPlayer(TeamSide side, PlayerRecord player)
    {
        if (player == null || string.IsNullOrWhiteSpace(player.playerName))
            return;

        GetTeam(side).players.Add(player);
    }

    public List<PlayerRecord> SearchPlayers(string search)
    {
        List<PlayerRecord> results = new List<PlayerRecord>();

        if (string.IsNullOrWhiteSpace(search))
            return results;

        SearchTeam(homeTeam, search, results);
        SearchTeam(awayTeam, search, results);

        return results;
    }

    private void SearchTeam(
        TeamRoster team,
        string search,
        List<PlayerRecord> results)
    {
        foreach (PlayerRecord player in team.players)
        {
            if (player.playerName.IndexOf(
                search,
                StringComparison.OrdinalIgnoreCase) >= 0)
            {
                results.Add(player);
            }
        }
    }

    public bool SetStartingEleven(
        TeamSide side,
        int positionSlot,
        int playerIndex)
    {
        TeamRoster team = GetTeam(side);

        if (positionSlot < 0 || positionSlot >= 11)
            return false;

        if (playerIndex < 0 || playerIndex >= team.players.Count)
            return false;

        if (team.startingEleven.Contains(playerIndex))
            return false;

        while (team.startingEleven.Count < 11)
            team.startingEleven.Add(-1);

        team.startingEleven[positionSlot] = playerIndex;

        return true;
    }

    public bool AddSubstitute(TeamSide side, int playerIndex)
    {
        TeamRoster team = GetTeam(side);

        if (playerIndex < 0 || playerIndex >= team.players.Count)
            return false;

        if (team.substitutes.Count >= maximumSubstitutes)
            return false;

        if (team.substitutes.Contains(playerIndex) ||
            team.startingEleven.Contains(playerIndex))
            return false;

        team.substitutes.Add(playerIndex);

        return true;
    }

    public bool RemoveSubstitute(TeamSide side, int playerIndex)
    {
        return GetTeam(side).substitutes.Remove(playerIndex);
    }

    public void ClearLineup(TeamSide side)
    {
        TeamRoster team = GetTeam(side);

        team.startingEleven.Clear();
        team.substitutes.Clear();
    }
}
