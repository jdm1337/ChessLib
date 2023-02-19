﻿/*
ChessLib, a chess data structure library

MIT License

Copyright (c) 2017-2023 Rudy Alex Kohn

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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Rudzoft.ChessLib.Enums;
using Rudzoft.ChessLib.Fen;
using Rudzoft.ChessLib.MoveGeneration;
using Rudzoft.ChessLib.ObjectPoolPolicies;
using Rudzoft.ChessLib.Types;

namespace Rudzoft.ChessLib.Test.GameplayTests;

public sealed class FoolsCheckMateTests
{
    private readonly IServiceProvider _serviceProvider;

    public FoolsCheckMateTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddTransient<IBoard, Board>()
            .AddSingleton<IValues, Values>()
            .AddTransient<IPosition, Position>()
            .AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>()
            .AddSingleton(static serviceProvider =>
            {
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                var policy = new MoveListPolicy();
                return provider.Create(policy);
            })
            .BuildServiceProvider();
    }

    [Fact]
    public void FoolsCheckMate()
    {
        // generate moves
        var moves = new[]
        {
            Move.Create(Square.F2, Square.F3),
            Move.Create(Square.E7, Square.E5),
            Move.Create(Square.G2, Square.G4),
            Move.Create(Square.D8, Square.H4)
        };

        var pos = _serviceProvider.GetRequiredService<IPosition>();
        var fenData = new FenData(Fen.Fen.StartPositionFen);
        var state = new State();
        pos.Set(in fenData, ChessMode.Normal, state);

        // make the moves necessary to create a mate
        foreach (var move in moves)
            pos.MakeMove(move, state);

        // verify in check is actually true
        Assert.True(pos.InCheck);

        var isMate = pos.IsMate;

        Assert.True(isMate);
    }
}