using GameTrilha.Domain.Enums;

namespace GameTrilha.Domain.Entities;

public class Match
{
    public Guid Id { get; set; }
    public User Player1 { get; set; }
    public User Player2 { get; set; }
    public User? Winner { get; set; }
    public MatchStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}