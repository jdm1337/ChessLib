﻿/*
ChessLib, a chess data structure library

MIT License

Copyright (c) 2017-2018 Rudy Alex Kohn

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

namespace ChessLibTest
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Rudz.Chess;
    using Rudz.Chess.Enums;
    using Rudz.Chess.Types;

    [TestFixture]
    public class FoolsCheckMateTests {
        
        [Test]
        public void FoolsCheckMateTest()
        {
            // generate moves
            IList<Move> moves = new List<Move>(4) {
                                                      new Move(EPieces.WhitePawn, ESquare.f2, ESquare.f3),
                                                      new Move(EPieces.BlackPawn, ESquare.e7, ESquare.e5, EMoveType.Doublepush, EPieces.NoPiece),
                                                      new Move(EPieces.WhitePawn, ESquare.g2, ESquare.g4, EMoveType.Doublepush, EPieces.NoPiece),
                                                      new Move(EPieces.BlackQueen, ESquare.d8, ESquare.h4)
                                                  };

            // construct game and start a new game
            Game game = new Game();
            game.NewGame();

            // make the moves neccesary to create a mate
            foreach (Move move in moves)
                Assert.IsTrue(game.MakeMove(move));

            // verify in check is actually true
            Assert.IsTrue(game.State.InCheck);

            game.State.Flags = Emgf.Legalmoves;

            // generate all legal moves
            game.State.GenerateMoves(true);

            // verify that no legal moves actually exists.
            Assert.Zero(game.State.Moves.Count);
        }
    }
}