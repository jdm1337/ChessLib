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

namespace Chess.Test.Pieces;

using Rudz.Chess.Enums;
using Rudz.Chess.Types;
using System;

public abstract class PieceAttacksRegular : PieceAttacks
{
    // special case with alpha and beta bands in the corners are taken care of
    protected static readonly int[] KnightExpected = { 4, 6, 8, 8 };

    // special case with alpha band is taken care of
    protected static readonly int[] KingExpected = { 5, 8, 8, 8 };

    protected static readonly BitBoard BoardCorners
        = BitBoards.MakeBitboard(Squares.a1) | BitBoards.MakeBitboard(Squares.a8)
        | BitBoards.MakeBitboard(Squares.h1) | BitBoards.MakeBitboard(Squares.h8);

    // pawn = 0 (N/A for now), knight = 1, king = 2
    protected readonly Func<Square, BitBoard>[] RegAttacks = { BitBoards.KnightAttacks, BitBoards.KnightAttacks, BitBoards.KingAttacks };

    public abstract override void AlphaPattern();

    public abstract override void BetaPattern();

    public abstract override void GammaPattern();

    public abstract override void DeltaPattern();
}
