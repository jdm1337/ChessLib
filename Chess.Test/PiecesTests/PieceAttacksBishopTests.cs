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

namespace Chess.Test.PiecesTest;

using Chess.Test.PiecesTests;
using FluentAssertions;
using Rudz.Chess;
using Rudz.Chess.Enums;
using Rudz.Chess.Types;
using System.Linq;
using Xunit;

/// <inheritdoc/>
public sealed class PieceAttacksBishopTests : PieceAttacks, IClassFixture<SliderMobilityFixture>
{
    private readonly SliderMobilityFixture fixture;

    public PieceAttacksBishopTests(SliderMobilityFixture fixture)
    {
        this.fixture = fixture;
    }

    // Slider index 

    [Theory]
    [InlineData(Alpha, 0, 7)]
    [InlineData(Beta, 0, 9)]
    [InlineData(Gamma, 0, 11)]
    [InlineData(Delta, 0, 13)]
    public void BishopMobility(ulong pattern, int sliderIndex, int expectedMobility)
    {
        var bb = new BitBoard(pattern);

        var expected = bb.Count * expectedMobility;
        var actual = bb.Select(x => fixture.SliderAttacks[sliderIndex](x, BitBoard.Empty).Count).Sum();

        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// Testing results of blocked bishop attacks, they should always return 2 on the sides, and
    /// 1 in the corner
    /// </summary>
    [Fact]
    public void BishopBorderBlocked()
    {
        BitBoard border = Alpha;
        BitBoard borderInner = Beta;
        var corners = BitBoards.MakeBitboard(Squares.a1, Squares.a8, Squares.h1, Squares.h8);

        const int expectedCorner = 1; // just a single attack square no matter what
        const int expectedSide = 2;

        /*
                     * borderInner (X = set bit) :
                     *
                     * 0 0 0 0 0 0 0 0
                     * 0 X X X X X X 0
                     * 0 X 0 0 0 0 X 0
                     * 0 X 0 0 0 0 X 0
                     * 0 X 0 0 0 0 X 0
                     * 0 X 0 0 0 0 X 0
                     * 0 X X X X X X 0
                     * 0 0 0 0 0 0 0 0
                     *
                     */
        foreach (var square in border)
        {
            var attacks = square.BishopAttacks(borderInner);
            attacks.IsEmpty.Should().BeFalse();
            var expected = corners & square ? expectedCorner : expectedSide;
            var actual = attacks.Count;
            Assert.Equal(expected, actual);
        }
    }
}
