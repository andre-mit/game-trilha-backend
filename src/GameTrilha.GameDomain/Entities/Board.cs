using GameTrilha.GameDomain.Enums;
using GameTrilha.GameDomain.Helpers;

namespace GameTrilha.GameDomain.Entities;

// Todo: Verificar vitoria ao não ter movimentos validos
public class Board
{
    private static readonly List<(byte line, byte column)> PositionsMoinhoCrossTracks = new()
    {
        new ValueTuple<byte, byte>(0, 1),
        new ValueTuple<byte, byte>(1, 0),
        new ValueTuple<byte, byte>(2, 1),
        new ValueTuple<byte, byte>(1, 2)
    };

    public Dictionary<Color, byte> PendingPieces { get; } = new()
    {
        { Color.White, 9 },
        { Color.Black, 9 }
    };

    public bool MoinhoDuplo { get; init; }
    public readonly List<Guid[]> PendingMoinhoDuplo = new();

    public bool PendingMoinhoWhite { get; private set; }
    public bool PendingMoinhoBlack { get; private set; }

    public byte WhiteRemaningMoves { get; private set; }
    public byte BlackRemaningMoves { get; private set; }
    public bool ActivateRemaningMoves { get; private set; }
    public Color Turn { get; set; } = Color.White;
    public Dictionary<string, Color> Players { get; init; }

    public GameStage Stage { get; set; }

    public Board(bool moinhoDuplo, Dictionary<string, Color> players, byte maxDrawMoves = 10)
    {
        MoinhoDuplo = moinhoDuplo;
        Stage = GameStage.Place;
        WhiteRemaningMoves = maxDrawMoves;
        BlackRemaningMoves = maxDrawMoves;
        Players = players;
    }

    public Track[] Tracks { get; init; } = { new(), new(), new() };

    public Dictionary<Color, int> ColorPiecesAmount
    {
        get
        {
            var tracks = Tracks.ToList();

            var places = tracks.Select(track =>
                track.Places.Cast<Place>()
                    .Where(x => x?.Piece is not null));

            var pieces = places.SelectMany(p => p.Select(x => x.Piece));

            var amount = pieces.GroupBy(x => x!.Color).ToDictionary(group => group.Key, group => group.Count());

            return amount;
        }
    }

    /// <summary>
    /// Place a piece on the board
    /// </summary>
    /// <summary xml:lang="pt-BR">
    /// Coloca uma peça no tabuleiro
    /// </summary>
    /// <param name="player">Player identifier</param>
    /// <param name="track">Track position</param>
    /// <param name="line">Line position</param>
    /// <param name="column">Column position</param>
    /// <returns>
    ///    Dictionary with the amount of pieces pending to place for each color
    /// </returns>
    /// <exception cref="InvalidOperationException">Operation is not valid</exception>
    public (Dictionary<Color, byte>? pendingPieces, bool moinho, bool winner, Guid pieceId) PlacePiece(string player, byte track, byte line, byte column)
    {
        var color = Players[player];

        if (Stage != GameStage.Place)
            throw new InvalidOperationException("Não é possível colocar peças pois já está na fase de jogo");

        if (color != Turn)
            throw new InvalidOperationException("Não é a vez do jogador");

        if (PendingPieces[color] == 0)
            throw new InvalidOperationException("Não há mais peças disponíveis para serem colocadas");

        if (PendingMoinhoBlack || PendingMoinhoWhite)
            throw new InvalidOperationException("Não é possível mover peças pois é preciso remover uma peça antes");

        if (!Tracks[track].PlaceAvailable(line, column))
            throw new InvalidOperationException("Local de destino ocupado");

        var piece = new Piece(color);
        Tracks[track].Places[line, column].Piece = piece;
        PendingPieces[color]--;

        var moinho = Moinho(piece, track, line, column);

        var opponentColor = color == Color.White ? Color.Black : Color.White;
        var winner = (moinho && PendingPieces[opponentColor] == 0) || VerifyWinner(color);
        ToggleTurn(moinho);
        if(PendingPieces.All(x => x.Value ==0))
            Stage = GameStage.Game;
        return (PendingPieces, moinho, winner, piece.Id);
    }

    /// <summary>
    /// Piece movement
    /// </summary>
    /// <summary xml:lang="pt-BR">
    /// Movimentação de peças
    /// </summary>
    /// <param name="player">Player identifier</param>
    /// <param name="originTrack">Origin Track. Allowed values: 0 | 1 | 2</param>
    /// <param name="originLine">Origin Line. Allowed values: 0 | 1 | 2</param>
    /// <param name="originColumn">Origin Column. Allowed values: 0 | 1 | 2</param>
    /// <param name="destinationTrack">Destination Track. Allowed values: 0 | 1 | 2</param>
    /// <param name="destinationLine">Destination Line. Allowed values: 0 | 1 | 2</param>
    /// <param name="destinationColumn">Destination Column. Allowed values: 0 | 1 | 2</param>
    /// <returns>
    /// moinho: if there is a moinho after the movement
    /// winner: if the player has won after the movement (Can return null if it is a draw)
    /// </returns>
    /// <returns xml:lang="pt-BR">
    /// moinho: caso haja um moinho após a movimentação
    /// winner: caso o jogador tenha vencido após a movimentação (Pode retornar null caso seja empate)
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public (bool moinho, bool? winner) MovePiece(string player, byte originTrack, byte originLine, byte originColumn, byte destinationTrack,
        byte destinationLine, byte destinationColumn)
    {
        var color = Players[player];

        if (Stage == GameStage.Place)
            throw new InvalidOperationException("Não é possível mover peças pois ainda está na fase de colocação");

        if (Turn != color)
            throw new InvalidOperationException("Não é a vez do jogador");

        if (ActivateRemaningMoves && WhiteRemaningMoves == 0 && BlackRemaningMoves == 0)
            return (false, null);

        if (destinationLine == 1 && destinationColumn == 1)
            throw new InvalidOperationException("Não é possível mover uma peça para o centro do tabuleiro");

        if (!Tracks[originTrack].MatchPiece(color, originLine, originColumn) || !Tracks[destinationTrack].PlaceAvailable(destinationLine, destinationColumn))
            throw new InvalidOperationException("Peça inválida ou local de destino ocupado");

        if (ColorPiecesAmount[color] > 3 && !ValidateMovement(originTrack, originLine, originColumn, destinationTrack, destinationLine, destinationColumn))
            throw new InvalidOperationException("Não é possível mover a peça para posições não adjacentes");

        if (color == Color.White && PendingMoinhoBlack || color == Color.Black && PendingMoinhoWhite)
            throw new InvalidOperationException("Não é possível mover peças pois é a vez do adversário realizar o moinho");

        var piece = Tracks[originTrack].Places[originLine, originColumn].Piece!;

        Tracks[originTrack].Places[originLine, originColumn].Piece = null;
        Tracks[destinationTrack].Places[destinationLine, destinationColumn].Piece = piece;

        if (ActivateRemaningMoves)
        {
            if (color == Color.White)
                WhiteRemaningMoves--;
            else
                BlackRemaningMoves--;
        }

        var moinho = Moinho(piece, destinationTrack, destinationLine, destinationColumn);
        var opponentColor = color == Color.White ? Color.Black : Color.White;

        if (moinho && ColorPiecesAmount[opponentColor] == 3)
        {
            return (moinho, true);
        }

        var winner = VerifyWinner(opponentColor);

        if (ActivateRemaningMoves && !moinho && !winner && VerifyDraw())
            return (false, null);

        ToggleTurn(moinho);
        return (moinho, winner);
    }

    /// <summary>
    /// Remove a piece of the opponent from the board according to the coordinates
    /// </summary>
    /// <summary xml:lang="pt-BR">
    /// Remove uma peça do adversário do tabuleiro de acordo com as coordenadas
    /// </summary>
    /// <param name="player">Player Identifier</param>
    /// <param name="track">Track Allowed values: 0 | 1 | 2</param>
    /// <param name="line">Line Allowed values: 0 | 1 | 2</param>
    /// <param name="column">Column Allowed values: 0 | 1 | 2</param>
    /// <returns>Winner</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public bool RemovePiece(string player, byte track, byte line, byte column)
    {
        var color = Players.First(x => x.Key != player).Value;

        if (Turn == color)
            throw new InvalidOperationException("Não é a vez do jogador");

        if (color == Color.Black && !PendingMoinhoWhite || color == Color.White && !PendingMoinhoBlack)
            throw new InvalidOperationException("Moinho indisponivel");

        if (!Tracks[track].MatchPiece(color, line, column))
            throw new InvalidOperationException("Peça inválida");

        var (moinho, positions) = MoinhoCrossTrack(line, column, color);
        if (!moinho)
            (moinho, positions) = Tracks[track].Moinho(color, line, column);

        if (moinho && !CanRemoveMoinhoPiece(color, positions.ToList()))
            throw new InvalidOperationException("Não é possível remover uma peça que faz parte de um moinho");

        Tracks[track].Places[line, column].Piece = null;

        var colorPiecesAmount = ColorPiecesAmount;
        PendingMoinhoBlack = PendingMoinhoWhite = false;

        if (!ActivateRemaningMoves && colorPiecesAmount[Color.Black] == 3 && colorPiecesAmount[Color.White] == 3)
        {
            ActivateRemaningMoves = true;
        }

        if (PendingPieces.All(x => x.Value == 0))
            Stage = GameStage.Game;

        ToggleTurn();
        return VerifyWinner(color);
    }

    /// <summary>
    /// Timeout in the "place" stage
    /// </summary>
    /// <summary xml:lang="pt-BR">
    /// Estouro de tempo na fase de colocação
    /// </summary>
    /// <param name="player"></param>
    /// <returns>
    /// All pending pieces to place, if there is a moinho after the placement and if the player has won after the placement
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public (Dictionary<Color, byte>? pendingPieces, bool moinho, bool winner, Guid pieceId) PlaceTimeout(string player)
    {
        if (Stage != GameStage.Place)
            throw new InvalidOperationException("Não é possível realizar o timeout pois não está na fase de colocação");

        var color = Players[player];
        if (color != Turn)
            throw new InvalidOperationException("Não é a vez do jogador");

        var availablePlaces = GetAvailablePlaces();
        var (track, line, column) = availablePlaces[new Random().Next(0, availablePlaces.Count)];
        return PlacePiece(player, track, line, column);
    }

    /// <summary>
    /// Timeout in the "move" stage
    /// </summary>
    /// <summary xml:lang="pt-BR">
    /// Estouro de tempo na fase de movimentação
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public bool GameTimeout(string player)
    {
        var color = Players[player];
        if (color != Turn)
            throw new InvalidOperationException("Não é a vez do jogador");

        if (ActivateRemaningMoves)
            switch (Turn)
            {
                case Color.White:
                    WhiteRemaningMoves--;
                    break;
                case Color.Black:
                    BlackRemaningMoves--;
                    break;
            }

        PendingMoinhoBlack = PendingMoinhoWhite = false;
        ToggleTurn();

        return VerifyDraw();
    }

    private bool CanRemoveMoinhoPiece(Color color, List<Guid> moinhoPlaces)
    {
        if (ColorPiecesAmount[color] == 4) return false;

        var haveAnother = false;

        for (byte i = 0; i < 3; i++)
        {
            for (byte j = 0; j < 3; j++)
            {
                for (byte k = 0; k < 3; k++)
                {
                    if (j == 1 && k == 1 ||
                        Tracks[i].Places[j, k].Piece == null ||
                        Tracks[i].Places[j, k].Piece!.Color != color ||
                        moinhoPlaces.Any(x => x == Tracks[i].Places[j, k].Piece?.Id)) continue;

                    var (moinho, places) = MoinhoCrossTrack(j, k, color);

                    haveAnother = !moinho;

                    if (haveAnother) break;
                    moinhoPlaces.AddRange(places);
                }
                if (haveAnother) break;
            }
            if (haveAnother) break;
        }

        return !haveAnother;
    }

    private List<(byte track, byte line, byte column)> GetAvailablePlaces()
    {
        var availablePlaces = new List<(byte track, byte line, byte column)>();
        for (byte i = 0; i < 3; i++)
        {
            for (byte j = 0; j < 3; j++)
            {
                for (byte k = 0; k < 3; k++)
                {
                    if (j == 1 && k == 1 ||
                        Tracks[i].Places[j, k].Piece != null) continue;
                    availablePlaces.Add((i, j, k));
                }
            }
        }
        return availablePlaces;
    }

    #region Moinho
    private bool Moinho(Piece piece, byte track, byte line, byte column)
    {
        if (MoinhoDuplo)
        {
            var (moinho, _) = MoinhoCrossTrack(line, column, piece.Color);
            if (!moinho)
                (moinho, _) = Tracks[track].Moinho(piece.Color, line, column);

            if (!moinho) return moinho;

            switch (piece.Color)
            {
                case Color.White:
                    PendingMoinhoWhite = true;
                    break;
                case Color.Black:
                    PendingMoinhoBlack = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return moinho;
        }


        if (Stage == GameStage.Game && PendingMoinhoDuplo.Any(x => x.Any(g => g == piece.Id)))
        {
            PendingMoinhoDuplo.RemoveAll(x => x.Any(g => g == piece.Id));

            return false;
        }


        var (isMoinho, matches) = MoinhoCrossTrack(line, column, piece.Color);
        if (!isMoinho) (isMoinho, matches) = Tracks[track].Moinho(piece.Color, line, column);

        if (!isMoinho) return false;

        if (Stage == GameStage.Game)
            PendingMoinhoDuplo.Add(matches);

        switch (piece.Color)
        {
            case Color.White:
                PendingMoinhoWhite = true;
                break;
            case Color.Black:
                PendingMoinhoBlack = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return true;
    }

    private (bool, Guid[]) MoinhoCrossTrack(byte line, byte column, Color color)
    {
        if (!PositionsMoinhoCrossTracks.Contains((line, column))) return (false, null)!;

        var matches = new Guid?[] { null, null, null };

        for (var i = 0; i < 3; i++)
        {
            if (Tracks[i].Places[line, column].Piece?.Color == color)
                matches[i] = Tracks[i].Places[line, column].Piece?.Id;
            else
                return (false, null!);
        }

        return (true, matches.Select(x => x!.Value).ToArray());
    }
    #endregion

    private static bool ValidateMovement(byte originTrack, byte originLine, byte originColumn, byte destinationTrack,
        byte destinationLine, byte destinationColumn)
    {
        if (destinationTrack > 2 || destinationColumn > 2 || destinationLine > 2)
            return false;

        return MoveVerification.MoveAllowed(new MoveVerification.Place(originTrack, originLine, originColumn),
            new MoveVerification.Place(destinationTrack, destinationLine, destinationColumn));
    }

    private bool VerifyWinner(Color opponentColor)
    {
        var winner = opponentColor switch
        {
            Color.White => ColorPiecesAmount[Color.White] <= 2 && PendingPieces[Color.White] == 0,
            Color.Black => ColorPiecesAmount[Color.Black] <= 2 && PendingPieces[Color.Black] == 0,
            _ => false
        };

        if (!winner)
            winner = !HaveValidMoves(opponentColor);

        return winner;
    }

    private bool VerifyDraw() => ActivateRemaningMoves && BlackRemaningMoves == 0 && WhiteRemaningMoves == 0;


    private bool HaveValidMoves(Color color)
    {
        var validMove = false;

        for (byte i = 0; i < 3 && !validMove; i++)
        {
            for (byte j = 0; j < 3 && !validMove; j++)
            {
                for (byte k = 0; k < 3 && !validMove; k++)
                {
                    if (j == 1 && k == 1)
                        continue;

                    if (Tracks[i].Places[j, k].Piece?.Color != color) continue;

                    var availablePlaces = MoveVerification.AllowedPlaces(new MoveVerification.Place(i, j, k));
                    
                    validMove = availablePlaces.Any(x => Tracks[x.Track].PlaceAvailable(x.Line, x.Column));
                }
            }
        }

        return validMove;
    }

    private void ToggleTurn(bool moinho = false) => Turn = moinho ? Turn : Turn == Color.Black ? Color.White : Color.Black;
}