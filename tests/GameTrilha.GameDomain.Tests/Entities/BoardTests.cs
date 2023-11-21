using GameTrilha.GameDomain.Entities;
using GameTrilha.GameDomain.Enums;

namespace GameTrilha.GameDomain.Tests.Entities;

public class BoardTests
{
    private readonly Board _board;
    private readonly Dictionary<Guid, Color> _players;
    private static readonly Guid Player1Id = Guid.NewGuid();
    private static readonly Guid Player2Id = Guid.NewGuid();

    public BoardTests()
    {
        /*
         *      B----0-B------W
         *      | B--1-x----W |
         *      | | x2-B--x | |
         *      x-x-x     x-w-x
         *      | | x--x--x | |
         *      | x----x----x |
         *      B------x------X
         *
         *      X = Espaço vazio
         *      B = peça preta
         *      W = Peça branca
         */
        var tracks = new Track[]
        {
            new()
            {
                Places = new Place[,]
                {
                    // 1º line
                    {
                        new()
                        {
                            Piece = new Piece(Color.Black)
                        },
                        new()
                        {
                            Piece = new Piece(Color.Black)
                        },
                        new()
                        {
                            Piece = new Piece(Color.White)
                        }
                    },
                    // 2º line
                    {
                        new(),
                        null!,
                        new()
                    },
                    // 3º line
                    {
                        new()
                        {
                            Piece = new Piece(Color.Black)
                        },
                        new(),
                        new()
                    }
                },
            },
            new()
            {
                Places = new Place[,]
                {
                    // 1º line
                    {
                        new()
                        {
                            Piece = new Piece(Color.Black)
                        },
                        new(),
                        new()
                        {
                            Piece = new Piece(Color.White)
                        }
                    },
                    // 2º line
                    {
                        new(),
                        null!,
                        new()
                        {
                            Piece = new Piece(Color.White)
                        }
                    },
                    // 3º line
                    {
                        new(),
                        new(),
                        new()
                    }
                },
            },
            new()
            {
                Places = new Place[,]
                {
                    // 1º line
                    {
                        new(),
                        new()
                        {
                            Piece = new Piece(Color.Black)
                        },
                        new()
                    },
                    // 2º line
                    {
                        new(),
                        null!,
                        new()
                    },
                    // 3º line
                    {
                        new(),
                        new(),
                        new()
                    }
                },
            }
        };

        _players = new Dictionary<Guid, Color> { { Player1Id, Color.White }, { Player2Id, Color.Black } };
        _board = new Board(false, _players)
        {
            Tracks = tracks, Stage = GameStage.Game,
            PendingPieces =
            {
                [Color.Black] = 0,
                [Color.White] = 0
            }
        };
    }

    [Fact]
    public void MoveWhitePiece_ToValidPosition_WithoutMoinho_WithoutWinner()
    {
        // Arrange
        var board = _board;
        var piece = board.Tracks[1].Places[0, 2].Piece;
        board.Stage = GameStage.Game;

        // Act
        var (moinho, winner) = board.MovePiece(Player1Id, 1, 0, 2, 1, 0, 1);

        // Assert
        Assert.False(winner);
        Assert.False(moinho);

        Assert.Null(board.Tracks[1].Places[0, 2].Piece);
        Assert.Equal(piece, board.Tracks[1].Places[0, 1].Piece);
    }

    [Fact]
    public void MoveWhitePiece_ToInvalidPosition_ShouldThrowException()
    {
        // Arrange
        var board = _board;
        board.Stage = GameStage.Game;

        // Act
        Action act = () => board.MovePiece(Player1Id, 1, 0, 2, 1, 1, 2);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void MoveWhitePiece_ToSamePosition_ShouldThrowException()
    {
        // Arrange
        var board = _board;
        board.Stage = GameStage.Game;

        // Act
        Action act = () => board.MovePiece(Player1Id, 1, 0, 2, 1, 0, 2);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void MoveWhitePiece_MakeMoinho_WithoutWinner()
    {
        // Arrange
        var board = _board;
        board.Stage = GameStage.Game;

        // Act
        var (moinho, winner) = board.MovePiece(Player1Id, 0, 0, 2, 1, 2, 2);

        // Assert
        Assert.True(moinho);
        Assert.False(winner);
    }

    [Fact]
    public void MoveWhitePiece_MakeMoinho_Then_RemoveBlackPiece()
    {
        // Arrange
        var board = _board;
        board.Stage = GameStage.Game;

        // Act
        var (moinho, winner) = board.MovePiece(Player1Id, 0, 0, 2, 1, 2, 2);
        var result = board.RemovePiece(Player1Id, 0, 2, 0);

        // Assert
        Assert.True(moinho);
        Assert.False(winner);
        Assert.False(result);
        Assert.Null(board.Tracks[0].Places[2, 0].Piece);
    }

    [Fact]
    public void MoveBlackPiece_MakeMoinho_Then_WinMatch()
    {
        // Arrange
        var board = _board;
        board.Turn = Color.Black;
        board.Stage = GameStage.Game;

        // Act
        var (moinho, winner) = board.MovePiece(Player2Id, 1, 0, 0, 1, 0, 1);

        // Assert
        Assert.True(moinho);
        Assert.True(winner);
    }

    [Fact]
    public void MoveBlackPiece_MakeMoinho_WithoutWinner_Then_DoNotAllowWhiteMove_While_NotRemoveWhitePiece()
    {
        // Arrange
        var board = _board;
        board.Turn = Color.Black;
        board.Stage = GameStage.Game;

        board.Tracks[0].Places[2, 2].Piece = new Piece(Color.White);

        // Act
        var (moinho, winner) = board.MovePiece(Player2Id, 1, 0, 0, 1, 0, 1);
        Action act = () => board.MovePiece(Player1Id, 0, 0, 2, 0, 1, 2);

        // Assert
        Assert.True(moinho);
        Assert.False(winner);
        Assert.Throws<InvalidOperationException>(act);

    }
    [Fact]
    public void WhenYouReach3Pieces_AllowAnyMoveToAnyEmptyBoardPlace()
    {
        // Arrange
        var board = _board;
        board.Stage = GameStage.Game;

        board.Tracks[0].Places[0, 0].Piece = null;

        // Act
        board.MovePiece(Player1Id, 0, 0, 2, 1, 2, 2);
        board.RemovePiece(Player1Id, 1, 0, 0);
        var (moinho, winner) = board.MovePiece(Player2Id, 0, 2, 0, 0, 0, 2);

        // Assert
        Assert.False(moinho);
        Assert.False(winner);
    }

    [Fact]
    public void MoveWhitePiece_ToValidPosition_BlockAllBlackMoves_WhiteShouldWinMatch()
    {
        // Arrange
        var board = _board;

        board.Stage = GameStage.Game;

        board.Tracks[0].Places[1, 0].Piece = new Piece(Color.White);
        board.Tracks[0].Places[2, 1].Piece = new Piece(Color.White);
        board.Tracks[1].Places[1, 0].Piece = new Piece(Color.White);
        board.Tracks[2].Places[0, 1].Piece = null;

        // Act
        var (_, winner) = board.MovePiece(Player1Id, 1, 0, 2, 1, 0, 1);

        // Assert
        Assert.True(winner);
    }

    [Fact]
    public void PlacePieceWhiteColor_MakeMoinho_RemoveBlackPiece_WithoutWinning()
    {
        // Arrange
        var board = new Board(false, _players);

        board.PlacePiece(Player1Id, 0, 0, 0);
        board.PlacePiece(Player2Id, 0, 2, 0);
        board.PlacePiece(Player1Id, 0, 0, 2);
        board.PlacePiece(Player2Id, 0, 2, 2);

        // Act
        var (_, moinho, _, _) = board.PlacePiece(Player1Id, 0, 0, 1);
        var winner = board.RemovePiece(Player1Id, 0, 2, 2);

        // Assert
        Assert.True(moinho);
        Assert.False(winner);
    }

    [Fact]
    public void PlacePieceWhiteColor_MakeMoinho_RemoveBlackPieces_ToWinMatch()
    {
        // Arrange
        var board = new Board(false, _players);

        board.Turn = Color.White;

        board.PlacePiece(Player1Id, 0, 0, 0);
        board.PlacePiece(Player2Id, 0, 2, 0);
        board.PlacePiece(Player1Id, 0, 0, 2);
        board.PlacePiece(Player2Id, 0, 2, 2);
        board.PlacePiece(Player1Id, 1, 0, 1);
        board.PlacePiece(Player2Id, 0, 2, 1);

        board.RemovePiece(Player2Id, 1, 0, 1);

        bool winner = false;
        int count = 0;
        // Act
        while (board.PendingPieces[Color.White] != 0)
        {
            count++;
            board.PlacePiece(Player1Id, 0, 0, 1);
            board.RemovePiece(Player1Id, 0, 2, 1);

            var (_, _, w, _) = board.PlacePiece(Player2Id, 0, 2, 1);
            if (w)
                winner = w;
            else
                board.RemovePiece(Player2Id, 0, 0, 1);
        }


        // Assert
        Assert.True(winner);
    }

    [Fact]
    public void MovePiece_MakeCenterMoinho_ShouldBlockAnotherMoinho_WhenMoinhoDuploIsDisabled()
    {
        // Arrange
        var board = _board;
        board.Tracks[1].Places[2, 0].Piece = new Piece(Color.Black);
        board.Tracks[2].Places[1, 0].Piece = new Piece(Color.Black);

        board.Tracks[0].Places[2, 2].Piece = new Piece(Color.White);

        board.Stage = GameStage.Game;
        board.Turn = Color.Black;

        // Act
        (var moinhoShouldBeTrue, _) = board.MovePiece(Player2Id, 2, 1, 0, 1, 1, 0);
        board.RemovePiece(Player2Id, 0, 2, 2);
        board.MovePiece(Player1Id, 0, 0, 2, 0, 1, 2);
        (var moinhoShouldBeFalse, _) = board.MovePiece(Player2Id, 1, 1, 0, 0, 1, 0);

        // Assert
        Assert.True(moinhoShouldBeTrue);
        Assert.False(moinhoShouldBeFalse);
    }

    [Fact]
    public void MovePiece_MakeCenterAndBorderMoinho_ShouldBlockAnotherMoinho_WhenMoinhoDuploIsDisabled()
    {
        // Arrange
        var board = _board;
        board.Tracks[0].Places[1, 0].Piece = new Piece(Color.Black);
        board.Tracks[0].Places[2, 2].Piece = new Piece(Color.Black);
        board.Tracks[2].Places[2, 1].Piece = new Piece(Color.Black);
        board.Tracks[1].Places[2, 1].Piece = new Piece(Color.Black);

        board.Tracks[0].Places[2, 0].Piece = null;

        board.Tracks[0].Places[0, 1].Piece = new Piece(Color.White);

        board.Stage = GameStage.Game;
        board.Turn = Color.Black;

        // Act
        (var moinhoShouldBeTrue, _) = board.MovePiece(Player2Id, 0, 2, 2, 0, 2, 1);
        board.RemovePiece(Player2Id, 0, 0, 1);
        board.MovePiece(Player1Id, 0, 0, 2, 0, 1, 2);
        (var moinhoShouldBeFalse, _) = board.MovePiece(Player2Id, 0, 2, 1, 0, 2, 0);

        // Assert
        Assert.True(moinhoShouldBeTrue);
        Assert.False(moinhoShouldBeFalse);
    }



    [Fact]
    public void MovePiece_MakeCenterMoinho_ShouldAllowAnotherMoinho_WhenMoinhoDuploIsEnabled()
    {
        // Arrange
        var board = new Board(true, _players)
        {
            Tracks = _board.Tracks,
            Stage = GameStage.Game,
            Turn = Color.Black
        };

        board.Tracks[1].Places[2, 0].Piece = new Piece(Color.Black);
        board.Tracks[2].Places[1, 0].Piece = new Piece(Color.Black);

        board.Tracks[0].Places[2, 2].Piece = new Piece(Color.White);

        // Act
        (var firstMoinho, _) = board.MovePiece(Player2Id, 2, 1, 0, 1, 1, 0);
        board.RemovePiece(Player2Id, 0, 2, 2);
        board.MovePiece(Player1Id, 0, 0, 2, 0, 1, 2);
        (var secondMoinho, _) = board.MovePiece(Player2Id, 1, 1, 0, 0, 1, 0);

        // Assert
        Assert.True(firstMoinho);
        Assert.True(secondMoinho);
    }

    [Fact]
    public void MoveBlackPiece_MakeMoinho_WhenWhiteHave_3Pieces_EnableLast10Moves_ForBoth_ShouldBeDraw()
    {
        // Arrange
        var board = _board;
        board.Turn = Color.White;
        board.Stage = GameStage.Game;
        board.Tracks[0].Places[0, 1].Piece = null;

        board.MovePiece(Player1Id, 0, 0, 2, 1, 2, 2);
        board.RemovePiece(Player1Id, 0, 2, 0);

        board.MovePiece(Player2Id, 2, 0, 1, 2, 0, 0);
        board.MovePiece(Player1Id, 1, 2, 2, 2, 2, 1);

        // Act
        var draw = false;
        for (int i = 0; i < 5; i++)
        {
            board.MovePiece(Player2Id, 2, 0, 0, 2, 0, 1);
            var (_, winner) = board.MovePiece(Player1Id, 2, 2, 1, 2, 2, 0);
            draw = !winner.HasValue;
            if (draw) continue;

            board.MovePiece(Player2Id, 2, 0, 1, 2, 0, 0);
            board.MovePiece(Player1Id, 2, 2, 0, 2, 2, 1);
        }

        // Assert
        Assert.True(draw);
    }

    [Fact]
    public void MoveWhitePiece_MakeMoinho_ShouldThrowError_To_BlockRemovePieceOfOpponentMoinho_WhenHavePiecesOutsideMoinho()
    {
        // Arrange
        var board = _board;
        board.Tracks[1].Places[2, 0].Piece = new Piece(Color.Black);
        board.Tracks[2].Places[1, 0].Piece = new Piece(Color.Black);

        board.Tracks[0].Places[2, 2].Piece = new Piece(Color.White);

        board.Stage = GameStage.Game;
        board.Turn = Color.Black;

        // Act
        board.MovePiece(Player2Id, 2, 1, 0, 1, 1, 0);
        board.RemovePiece(Player2Id, 0, 2, 2);
        board.MovePiece(Player1Id, 0, 0, 2, 2, 2, 2);
        Action act = () => board.RemovePiece(Player2Id, 1, 1, 0);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void MoveWhitePiece_WhenElapsedTimeout_LostTurn()
    {
        // Arrange
        var board = _board;
        bool? expectedWinner = false;

        // Act
        var draw = board.GameTimeout(Player1Id);
        Action actWhite = () => board.MovePiece(Player1Id, 0, 0, 2, 0, 1, 2);
        var actBlack = (bool moinho, bool? winner) () => board.MovePiece(Player2Id, 1, 0, 0, 1, 1, 0);

        // Assert
        Assert.False(draw);
        Assert.Throws<InvalidOperationException>(actWhite);
        Assert.Equivalent((false, expectedWinner), actBlack());
    }

    [Fact]
    public void MoveBlackPiece_MakeMoinho_WhenElapsedTimeout_LostTurn_WithoutDraw_ThenWhiteMakeMoinho_WinGame()
    {
        // Arrange
        var board = _board;
        bool? expectedWinner = true;
        board.Tracks[1].Places[1, 0].Piece = new Piece(Color.Black);

        // Act
        (var moinho, _) = board.MovePiece(Player1Id, 0, 0, 2, 1, 2, 2);
        var draw = board.GameTimeout(Player1Id);
        Action actWhite = () => board.RemovePiece(Player1Id, 0, 0, 0);
        var actBlack = (bool moinho, bool? winner) () => board.MovePiece(Player2Id, 1, 1, 0, 0, 1, 0);

        // Assert
        Assert.True(moinho);
        Assert.False(draw);
        Assert.Throws<InvalidOperationException>(actWhite);
        Assert.Equivalent((true, expectedWinner), actBlack());
    }

    [Fact]
    public void PlaceTimeout_ShouldPlaceAPieceatRandomPlace_ReturnFalse_WhenTimeoutElapsed()
    {
        // Arrange
        var board = new Board(false, _players);

        // Act
        var (pendingPieces, moinho, winner, _) = board.PlaceTimeout(Player1Id);

        // Assert
        Assert.Equal(8, pendingPieces![_players[Player1Id]]);
        Assert.False(moinho);
        Assert.False(winner);
    }
}
