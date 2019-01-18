﻿/*
ChessLib - A complete chess data structure library

MIT License

Copyright (c) 2017-2019 Rudy Alex Kohn

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

using Rudz.Chess.Types;

namespace Chess.Tests
{
    using System;
    using Xunit;

    public class PawnPushTests
    {
        private readonly Func<BitBoard, BitBoard>[] _pawnPushDel =
            {
                BitBoards.NorthOne, BitBoards.SouthOne
            };

        [Fact]
        public void PawnPush()
        {
            BitBoard fullBoard = 0xffffffffffff00;

            Player side = 0;

            int expected = 8;

            foreach (Square square in fullBoard)
            {
                BitBoard targetPosition = _pawnPushDel[side.Side](square.BitBoardSquare());
                Square toSquare = targetPosition.Lsb();
                int distance = toSquare.ToInt() - square.ToInt();
                Assert.Equal(expected, distance);
            }

            side = ~side;
            expected = -expected;

            foreach (Square square in fullBoard)
            {
                BitBoard targetPosition = _pawnPushDel[side.Side](square.BitBoardSquare());
                Square toSquare = targetPosition.Lsb();
                int distance = toSquare.ToInt() - square.ToInt();
                Assert.Equal(expected, distance);
            }
        }
    }
}