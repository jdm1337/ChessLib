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
using Rudz.Chess.Types;

namespace Rudz.Chess.Enums;

[Flags]
public enum CastlelingRights
{
    None = 0,
    WhiteOo = 1,
    WhiteOoo = WhiteOo << 1,
    BlackOo = WhiteOo << 2,
    BlackOoo = WhiteOo << 3,

    KingSide = WhiteOo | BlackOo,
    QueenSide = WhiteOoo | BlackOoo,
    WhiteCastleling = WhiteOo | WhiteOoo,
    BlackCastleling = BlackOo | BlackOoo,

    Any = WhiteCastleling | BlackCastleling,
    CastleRightsNb = 16
}

public static class CastlelingSideExtensions
{
    public static CastlelingRights MakeCastlelingRights(this CastlelingRights cs, Player p)
        => p.IsWhite
            ? cs == CastlelingRights.QueenSide
                ? CastlelingRights.WhiteOoo
                : CastlelingRights.WhiteOo
            : cs == CastlelingRights.QueenSide
                ? CastlelingRights.BlackOoo
                : CastlelingRights.BlackOo;
}

public enum CastlelingPerform
{
    Do,
    Undo
}

public static class CastlelingExtensions
{
    public static string GetCastlelingString(Square toSquare, Square fromSquare) => toSquare < fromSquare ? "O-O-O" : "O-O";

    public static bool HasFlagFast(this CastlelingRights value, CastlelingRights flag) => (value & flag) != 0;

    public static int AsInt(this CastlelingRights value) => (int)value;

    public static CastlelingRights Without(this CastlelingRights @this, CastlelingRights remove)
        => @this & ~remove;
}
