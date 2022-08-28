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
using Rudz.Chess;
using Rudz.Chess.Types;

namespace Chess.Test.PiecesTests;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class SliderMobilityFixture
{
    public int[] BishopExpected { get; } = { 7, 9, 11, 13 };

    public int[] RookExpected { get; } = { 14, 14, 14, 14 };

    public Func<Square, BitBoard, BitBoard>[] SliderAttacks { get; } = { MagicBB.BishopAttacks, MagicBB.RookAttacks, MagicBB.QueenAttacks };

    public int SliderIndex(PieceTypes pt)
        => pt.AsInt() - 3;
}