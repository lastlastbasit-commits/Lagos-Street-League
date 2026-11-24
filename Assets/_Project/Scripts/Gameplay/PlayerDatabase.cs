
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(
    fileName = "PlayerDatabase",
    menuName = "Lagos Street League/Player Database"
)]
public class PlayerDatabase : ScriptableObject
{
    [Header("Worldwide Player Collection")]
    public List<PlayerRecord> players = new List<PlayerRecord>();

    public int TotalPlayers => players.Count;

    public List<PlayerRecord> SearchByName(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return new List<PlayerRecord>();

        return players
            .Where(p => p != null &&
                !string.IsNullOrWhiteSpace(p.playerName) &&
                p.playerName.IndexOf(
                    search,
                    StringComparison.OrdinalIgnoreCase
                ) >= 0)
            .ToList();
    }

    public List<PlayerRecord> SearchByNationality(string nationality)
    {
        return players
            .Where(p => p != null &&
                !string.IsNullOrWhiteSpace(p.nationality) &&
                string.Equals(
                    p.nationality,
                    nationality,
                    StringComparison.OrdinalIgnoreCase
                ))
            .ToList();
    }

    public List<PlayerRecord> SearchByClub(string club)
    {
        return players
            .Where(p => p != null &&
                !string.IsNullOrWhiteSpace(p.club) &&
                p.club.IndexOf(
                    club,
                    StringComparison.OrdinalIgnoreCase
                ) >= 0)
            .ToList();
    }

    public List<PlayerRecord> SearchByPosition(PlayerPosition position)
    {
        return players
            .Where(p => p != null && p.position == position)
            .ToList();
    }

    public List<PlayerRecord> GetLegends()
    {
        return players
            .Where(p => p != null && p.isLegend)
            .ToList();
    }

    public List<PlayerRecord> GetRetiredPlayers()
    {
        return players
            .Where(p => p != null && p.isRetired)
            .ToList();
    }

    public List<PlayerRecord> GetTopRatedPlayers(int count)
    {
        return players
            .Where(p => p != null)
            .OrderByDescending(p => p.rating)
            .Take(Mathf.Max(0, count))
            .ToList();
    }

    public bool AddPlayer(PlayerRecord player)
    {
        if (player == null ||
            string.IsNullOrWhiteSpace(player.playerName))
            return false;

        bool alreadyExists = players.Any(p =>
            p != null &&
            string.Equals(
                p.playerName,
                player.playerName,
                StringComparison.OrdinalIgnoreCase
            ) &&
            p.birthYear == player.birthYear
        );

        if (alreadyExists)
            return false;

        players.Add(player);
        return true;
    }

    public bool RemovePlayer(string playerName, int birthYear)
    {
        PlayerRecord player = players.FirstOrDefault(p =>
            p != null &&
            string.Equals(
                p.playerName,
                playerName,
                StringComparison.OrdinalIgnoreCase
            ) &&
            p.birthYear == birthYear
        );

        if (player == null)
            return false;

        players.Remove(player);
        return true;
    }
}
