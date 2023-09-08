using GameTrilha.GameDomain.Enums;
using GameTrilha.GameDomain.Helpers;

namespace GameTrilha.GameDomain.Entities;

public class Board
{
    private static readonly List<(byte line, byte column)> PositionsMoinhoCrossTracks = new()
    {
        new ValueTuple<byte, byte>(0, 1),
        new ValueTuple<byte, byte>(1, 0),
        new ValueTuple<byte, byte>(2, 1),
        new ValueTuple<byte, byte>(1, 2)
    };

    private Dictionary<Color, byte> PendingPieces { get; } = new()
    {
        { Color.White, 9 },
        { Color.Black, 9 }
    };

    private readonly bool _moinhoDuplo;
    private byte _pendingMovesToMoinhoWhite = 0;
    private byte _pendingMovesToMoinhoBlack = 0;

    private bool _pendingMoinhoWhite;
    private bool _pendingMoinhoBlack;

    private byte _whiteRemaningMoves = 10;
    private byte _blackRemaningMoves = 10;
    private bool _activateRemaningMoves = false;

    public Track[] Tracks { get; } = { new(), new(), new() };

    public Dictionary<Color, int> ColorPiecesAmount
    {
        get
        {
            var tracks = Tracks.ToList();

            var places = tracks.Select(track =>
                track.Places.Cast<Place>()
                    .Where(x => x.Piece is not null));

            var pieces = places.SelectMany(p => p.Select(x => x.Piece));

            var amount = pieces.GroupBy(x => x!.Color).ToDictionary(group => group.Key, group => group.Count());

            return amount;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color">Color of piece to place</param>
    /// <param name="track">Track position</param>
    /// <param name="line">Line position</param>
    /// <param name="column">Column position</param>
    /// <returns>
    ///    Dictionary with the amount of pieces pending to place for each color
    /// </returns>
    /// <exception cref="InvalidOperationException">Operation is not valid</exception>
    public Dictionary<Color, byte> PlacePiece(Color color, byte track, byte line, byte column)
    {
        if (PendingPieces[color] == 0)
            throw new InvalidOperationException("Não há mais peças disponíveis para serem colocadas");

        if (!Tracks[track].PlaceAvailable(line, column))
            throw new InvalidOperationException("Local de destino ocupado");

        Tracks[track].Places[line, column].Piece = new Piece(color);
        PendingPieces[color]--;

        return PendingPieces;
    }

    public (bool moinho, bool winner) Move(Color color, byte originTrack, byte originLine, byte originColumn, byte destinationTrail,
        byte destinationLine, byte destinationColumn)
    {
        if (_activateRemaningMoves && _whiteRemaningMoves == 0 && _blackRemaningMoves == 0)
            throw new InvalidOperationException("Não é possível mover mais peças, empate");

        if (destinationLine == 1 && destinationColumn == 1)
            throw new InvalidOperationException("Não é possível mover uma peça para o centro do tabuleiro");

        if (!Tracks[originTrack].MatchPiece(color, originLine, originColumn) || !Tracks[destinationTrail].PlaceAvailable(destinationLine, destinationColumn))
            throw new InvalidOperationException("Peça inválida ou local de destino ocupado");

        if (!ValidateMovement(originTrack, originLine, originColumn, destinationTrail, destinationLine, destinationColumn))
            throw new InvalidOperationException("Não é possível mover a peça para posições não adjacentes");

        var piece = Tracks[originTrack].Places[originLine, originColumn].Piece!;

        Tracks[originTrack].Places[originLine, originColumn].Piece = null;
        Tracks[destinationTrail].Places[destinationLine, destinationColumn].Piece = piece;

        if (_activateRemaningMoves)
        {
            if (color == Color.White)
                _whiteRemaningMoves--;
            else
                _blackRemaningMoves--;
        }

        var moinho = Moinho(piece, destinationTrail, destinationLine, destinationColumn);
        var opponentColor = color == Color.White ? Color.Black : Color.White;

        var winner = VerifyWinner(opponentColor);
        return (moinho, winner);
    }

    // TODO: Verify if can remove
    public bool RemovePiece(Color color, byte track, byte line, byte column)
    {
        if (color == Color.Black && !_pendingMoinhoBlack || color == Color.White && !_pendingMoinhoWhite)
            throw new InvalidOperationException("Moinho indisponivel");

        if (!Tracks[track].MatchPiece(color, line, column))
            return false;

        Tracks[track].Places[line, column].Piece = null;

        var colorPiecesAmount = ColorPiecesAmount;
        _pendingMoinhoBlack = false;
        _pendingMoinhoWhite = false;

        if (!_activateRemaningMoves && colorPiecesAmount[Color.Black] == 3 && colorPiecesAmount[Color.White] == 3)
        {
            _activateRemaningMoves = true;
        }
        return true;
    }

    #region Moinho
    public bool Moinho(Piece piece, byte track, byte line, byte column)
    {
        bool moinho;
        if (_moinhoDuplo)
        {
            moinho = MoinhoCrossTrail(line, column, piece.Color) || Tracks[track].Moinho(piece, line, column);

            if (moinho)
                switch (piece.Color)
                {
                    case Color.White:
                        _pendingMoinhoWhite = true;
                        break;
                    case Color.Black:
                        _pendingMoinhoBlack = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            return moinho;
        }

        if ((piece.Color != Color.White || _pendingMovesToMoinhoWhite != 0) &&
            (piece.Color != Color.Black || _pendingMovesToMoinhoBlack != 0))
        {
            switch (piece.Color)
            {
                case Color.White:
                    _pendingMovesToMoinhoWhite = 0;
                    break;
                case Color.Black:
                    _pendingMovesToMoinhoBlack = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        moinho = MoinhoCrossTrail(line, column, piece.Color) || Tracks[track].Moinho(piece, line, column);

        if (!moinho) return moinho;

        switch (piece.Color)
        {
            case Color.White:
                _pendingMovesToMoinhoWhite = 1;
                _pendingMoinhoWhite = true;
                break;
            case Color.Black:
                _pendingMovesToMoinhoBlack = 1;
                _pendingMoinhoBlack = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return moinho;

    }

    public bool MoinhoCrossTrail(byte line, byte column, Color color)
    {
        if (!PositionsMoinhoCrossTracks.Contains((line, column))) return false;

        var cont = Tracks.Count(track => track.Places[line, column].Piece?.Color == color);

        return cont == 3;
    }
    #endregion

    public static bool ValidateMovement(byte originTrack, byte originLine, byte originColumn, byte destinationTrack,
        byte destinationLine, byte destinationColumn)
    {
        if (destinationTrack > 2 || destinationColumn > 2 || destinationLine > 2)
            return false;

        return MoveVerification.MoveAllowed(new MoveVerification.Place(originTrack, originLine, originColumn),
            new MoveVerification.Place(destinationTrack, destinationLine, destinationColumn));
    }

    public bool VerifyWinner(Color opponentColor)
    {
        var winner = opponentColor switch
        {
            Color.White => ColorPiecesAmount[Color.White] == 2,
            Color.Black => ColorPiecesAmount[Color.Black] == 2,
            _ => false
        };

        if (!winner)
            winner = HaveValidMoves(opponentColor);

        return winner;
    }

    // TODO: Review this code and apply new logic
    private bool HaveValidMoves(Color color)
    {
        var validMove = false;


        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                for (var k = 0; k < 3; k++)
                {
                    if (j == 1 && k == 1)
                        continue;

                    if (Tracks[i].Places[j, k].Piece?.Color != color) continue;

                    switch (j)
                    {
                        case 0 when k == 0:
                            {
                                if (Tracks[i].Places[j, k + 1].Piece is null || Tracks[i].Places[j + 1, k].Piece is null)
                                    validMove = true;
                                break;
                            }
                        case 0 when k == 1:
                            {
                                if (Tracks[i].Places[j, k - 1].Piece is null || Tracks[i].Places[j, k + 1].Piece is null ||
                                    Tracks[i].Places[j + 1, k].Piece is null)
                                    validMove = true;
                                break;
                            }
                        case 0 when k == 2:
                            {
                                if (Tracks[i].Places[j, k - 1].Piece is null || Tracks[i].Places[j + 1, k].Piece is null)
                                    validMove = true;
                                break;
                            }
                        case 1 when k == 0:
                            {
                                if (Tracks[i].Places[j - 1, k].Piece is null || Tracks[i].Places[j, k + 1].Piece is null)
                                    validMove = true;
                                break;
                            }
                        case 1 when k == 2:
                            {
                                if (Tracks[i].Places[j - 1, k].Piece is null || Tracks[i].Places[j, k - 1].Piece is null)
                                    validMove = true;
                                break;
                            }
                        case 2 when k == 0:
                            {
                                if (Tracks[i].Places[j - 1, k].Piece is null || Tracks[i].Places[j - 1, k + 1].Piece is null)
                                    validMove = true;
                                break;
                            }
                        case 2 when k == 1:
                            {
                                if (Tracks[i].Places[j - 1, k].Piece is null || Tracks[i].Places[j, k - 1].Piece is null ||
                                    Tracks[i].Places[j, k + 1].Piece is null)
                                    validMove = true;
                                break;
                            }
                        case 2 when k == 2:
                            {
                                if (Tracks[i].Places[j - 1, k].Piece is null || Tracks[i].Places[j, k - 1].Piece is null)
                                    validMove = true;
                                break;
                            }
                    }
                }
            }
        }

        return validMove;
    }
}