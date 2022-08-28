/*
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

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Rudz.Chess.Enums;
using Rudz.Chess.MoveGeneration;
using Rudz.Chess.Types;

namespace Rudz.Chess.Notation.Notations;

public abstract class Notation : INotation
{
    protected readonly IPosition _pos;

    protected Notation(IPosition pos)
    {
        _pos = pos;
    }

    public abstract string Convert(Move move);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected char GetCheckChar()
        => _pos.GenerateMoves().Length != 0 ? '+' : '#';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MoveAmbiguities Ambiguity(Square from, BitBoard similarTypeAttacks)
    {
        var ambiguity = MoveAmbiguities.None;
        var c = _pos.SideToMove;
        var pinned = _pos.PinnedPieces(c);

        while (similarTypeAttacks)
        {
            var square = BitBoards.PopLsb(ref similarTypeAttacks);

            if (pinned & square)
                continue;

            if (_pos.GetPieceType(from) != _pos.GetPieceType(square))
                continue;

            // ReSharper disable once InvertIf
            if (_pos.Pieces(c) & square)
            {
                if (square.File == from.File)
                    ambiguity |= MoveAmbiguities.File;
                else if (square.Rank == from.Rank)
                    ambiguity |= MoveAmbiguities.Rank;

                ambiguity |= MoveAmbiguities.Move;
            }
        }

        return ambiguity;
    }

    /// <summary>
    /// Disambiguation.
    /// <para>If we have more then one piece with destination 'to'.</para>
    /// <para>Note that for pawns is not needed because starting file is explicit.</para>
    /// </summary>
    /// <param name="move">The move to check</param>
    /// <param name="from">The from square</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected IEnumerable<char> Disambiguation(Move move, Square from)
    {
        var similarAttacks = GetSimilarAttacks(move);
        var ambiguity = Ambiguity(from, similarAttacks);

        if (!ambiguity.HasFlagFast(MoveAmbiguities.Move))
            yield break;

        if (!ambiguity.HasFlagFast(MoveAmbiguities.File))
            yield return from.FileChar;
        else if (!ambiguity.HasFlagFast(MoveAmbiguities.Rank))
            yield return from.RankChar;
        else
        {
            yield return from.FileChar;
            yield return from.RankChar;
        }
    }

    /// <summary>
    /// Get similar attacks based on the move
    /// </summary>
    /// <param name="move">The move to get similar attacks from</param>
    /// <returns>Squares for all similar attacks without the moves from square</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private BitBoard GetSimilarAttacks(Move move)
    {
        var from = move.FromSquare();
        var pt = _pos.GetPieceType(from);

        return pt is PieceTypes.Pawn or PieceTypes.King
            ? BitBoard.Empty
            : _pos.GetAttacks(move.ToSquare(), pt, _pos.Pieces()) ^ from;
    }
}