﻿using GameTrilha.GameDomain.Entities;
using GameTrilha.GameDomain.Enums;

namespace GameTrilha.GameDomain.Tests.Entities;

public class BoardTests
{
    private readonly Board _board;
    public BoardTests()
    {
        /*
         *      B------B------W
         *      | B----x----W |
         *      | | x--B--x | |
         *      x-x-x     x-w-x
         *      | | x--x--x | |
         *      | x----x----x |
         *      B------x------X
         */
        var tracks = new Track[3]
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
        _board = new Board(false) { Tracks = tracks };

    }

    [Fact]
    public void MoveWhitePiece_ToValidPosition_WithoutMoinho_WithoutWinner()
    {
        // Arrange
        var board = _board;
        var piece = board.Tracks[1].Places[0, 2].Piece;

        // Act
        var (moinho, winner) = board.Move(Color.White, 1, 0, 2, 1, 0, 1);

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

        // Act
        Action act = () => board.Move(Color.White, 1, 0, 2, 1, 1, 2);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void MoveWhitePiece_ToSamePosition_ShouldThrowException()
    {
        // Arrange
        var board = _board;

        // Act
        Action act = () => board.Move(Color.White, 1, 0, 2, 1, 0, 2);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }

    [Fact]
    public void MoveWhitePiece_MakeMoinho_WithoutWinner()
    {
        // Arrange
        var board = _board;

        // Act
        var (moinho, winner) = board.Move(Color.White, 0, 0, 2, 1, 2, 2);

        // Assert
        Assert.True(moinho);
        Assert.False(winner);
    }

    [Fact]
    public void MoveBlackPiece_MakeMoinho_ThenRemoveWhitePiece_WinMatch()
    {
        // Arrange
        var board = _board;

        // Act
        var (moinho, winner) = board.Move(Color.Black, 1, 0, 0, 1, 0, 1);

        // Assert
        Assert.True(moinho);
        Assert.True(winner);
    }
}
