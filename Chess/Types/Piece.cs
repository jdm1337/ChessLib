﻿/*
ChessLib, a chess data structure library

MIT License

Copyright (c) 2017-2022 Rudy Alex Kohn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Rudz.Chess.Enums;
using Rudz.Chess.Extensions;

namespace Rudz.Chess.Types;

public enum Pieces : byte
{
    NoPiece = 0,
    WhitePawn = 1,
    WhiteKnight = 2,
    WhiteBishop = 3,
    WhiteRook = 4,
    WhiteQueen = 5,
    WhiteKing = 6,
    BlackPawn = 9,
    BlackKnight = 10,
    BlackBishop = 11,
    BlackRook = 12,
    BlackQueen = 13,
    BlackKing = 14,
    PieceNb = 15
}

public enum PieceTypes
{
    NoPieceType = 0,
    Pawn = 1,
    Knight = 2,
    Bishop = 3,
    Rook = 4,
    Queen = 5,
    King = 6,
    PieceTypeNb = 7,
    AllPieces = 0
}

public static class PieceTypesExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int AsInt(this PieceTypes p) => (int)p;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Piece MakePiece(this PieceTypes @this, Player side) => (int)@this | (side.Side << 3);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSlider(this PieceTypes @this) => @this.InBetween(PieceTypes.Bishop, PieceTypes.Queen);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InBetween(this PieceTypes v, PieceTypes min, PieceTypes max) =>
        (uint)v - (uint)min <= (uint)max - (uint)min;
}

/// <summary>
/// Piece. Contains the piece type which indicate what type and color the piece is
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 1)]
public readonly struct Piece : IEquatable<Piece>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Piece(int piece) => Value = (Pieces)piece;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Piece(Piece piece) => Value = piece.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Piece(Pieces piece) => Value = piece;

    [FieldOffset(0)] public readonly Pieces Value;

    public bool IsWhite => ColorOf().IsWhite;

    public bool IsBlack => ColorOf().IsBlack;

    public static Piece EmptyPiece { get; } = new(Pieces.NoPiece);

    public static Piece WhitePawn { get; } = new(Pieces.WhitePawn);

    public static Piece BlackPawn { get; } = new(Pieces.BlackPawn);

    public static Piece WhiteKnight { get; } = new(Pieces.WhiteKnight);

    public static Piece BlackKnight { get; } = new(Pieces.BlackKnight);

    public static Piece WhiteBishop { get; } = new(Pieces.WhiteBishop);

    public static Piece BlackBishop { get; } = new(Pieces.BlackBishop);

    public static Piece WhiteRook { get; } = new(Pieces.WhiteRook);

    public static Piece BlackRook { get; } = new(Pieces.BlackRook);

    public static Piece WhiteQueen { get; } = new(Pieces.WhiteQueen);

    public static Piece BlackQueen { get; } = new(Pieces.BlackQueen);

    public static Piece WhiteKing { get; } = new(Pieces.WhiteKing);

    public static Piece BlackKing { get; } = new(Pieces.BlackKing);

    public static Piece[] AllPieces { get; } =
    {
        WhitePawn,
        WhiteKnight,
        WhiteBishop,
        WhiteRook,
        WhiteQueen,
        WhiteKing,
        BlackPawn,
        BlackKnight,
        BlackBishop,
        BlackRook,
        BlackQueen,
        BlackKing
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Piece(char value) => new(GetPiece(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Piece(int value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Piece(Pieces value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Piece operator ~(Piece piece) => piece.AsInt() ^ 8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Piece operator +(Piece left, Player right) => new(left.Value + (byte)(right << 3));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Piece operator >> (Piece left, int right) => (int)left.Value >> right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Piece operator <<(Piece left, int right) => (int)left.Value << right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Piece left, Piece right) => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Piece left, Piece right) => !left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Piece left, Pieces right) => left.Value == right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Piece left, Pieces right) => left.Value != right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(Piece left, Pieces right) => left.Value <= right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(Piece left, Pieces right) => left.Value >= right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(Piece left, Pieces right) => left.Value < right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(Piece left, Pieces right) => left.Value > right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Piece operator ++(Piece left) => new(left.Value + 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Piece operator --(Piece left) => new(left.Value - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(Piece piece) => piece.Value != EmptyPiece;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(Piece piece) => piece.Value == EmptyPiece;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Player ColorOf() => (int)Value >> 3;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Piece other) => Value == other.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object obj) => obj is Piece piece && Equals(piece);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => (int)Value << 16;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString() => this.GetPieceString();

    private static Piece GetPiece(char character)
    {
        return character switch
        {
            'P' => WhitePawn,
            'p' => BlackPawn,
            'N' => WhiteKnight,
            'B' => WhiteBishop,
            'R' => WhiteRook,
            'Q' => WhiteQueen,
            'K' => WhiteKing,
            'n' => BlackKnight,
            'b' => BlackBishop,
            'r' => BlackRook,
            'q' => BlackQueen,
            'k' => BlackKing,
            _ => EmptyPiece
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int AsInt() => (int)Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PieceTypes Type() => (PieceTypes)(AsInt() & 0x7);

    private sealed class PieceRelationalComparer : Comparer<Piece>
    {
        private readonly IPieceValue _pieceValue;

        public PieceRelationalComparer(IPieceValue pieceValue)
        {
            _pieceValue = pieceValue;
        }

        public override int Compare(Piece x, Piece y)
        {
            if (x.Value == y.Value)
                return 0;

            // this is dangerous (fear king leopold III ?), king has no value and is considered
            // to be uniq
            if (x.Type() == PieceTypes.King || y.Type() == PieceTypes.King)
                return 1;

            var xValue = _pieceValue.GetPieceValue(x, Phases.Mg);
            var yValue = _pieceValue.GetPieceValue(y, Phases.Mg);

            if (xValue < yValue)
                return -1;

            if (xValue == yValue)
                return 0;

            return xValue > yValue ? 1 : x.AsInt().CompareTo(y.AsInt());
        }
    }
}