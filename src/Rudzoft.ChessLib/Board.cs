/*
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

using System.Collections;
using System.Diagnostics;
using Rudzoft.ChessLib.Extensions;
using Rudzoft.ChessLib.Types;

namespace Rudzoft.ChessLib;

public sealed class Board : IBoard
{
    private readonly Piece[] _pieces;

    private readonly BitBoard[] _bySide;

    private readonly BitBoard[] _byType;

    private readonly int[] _pieceCount;

    private readonly Square[][] _pieceList;

    private readonly int[] _index;

    public Board()
    {
        _pieces = new Piece[Types.Square.Count];
        _bySide = new BitBoard[Player.Count];
        _byType = new BitBoard[PieceTypes.PieceTypeNb.AsInt()];
        _pieceCount = new int[Piece.Count];
        _pieceList = new Square[Types.Square.Count][];
        for (var i = 0; i < _pieceList.Length; i++)
        {
            _pieceList[i] =
            [
                Types.Square.None, Types.Square.None, Types.Square.None, Types.Square.None,
                Types.Square.None, Types.Square.None, Types.Square.None, Types.Square.None,
                Types.Square.None, Types.Square.None, Types.Square.None, Types.Square.None,
                Types.Square.None, Types.Square.None, Types.Square.None, Types.Square.None
            ];
        }

        _index = new int[Types.Square.Count];
    }

    public void Clear()
    {
        Array.Fill(_pieces, Piece.EmptyPiece);
        _bySide[0] = _bySide[1] = BitBoard.Empty;
        _byType.Fill(in _bySide[0]);
        Array.Fill(_pieceCount, 0);
        foreach (var s in _pieceList)
            Array.Fill(s, Types.Square.None);
        Array.Fill(_index, 0);
    }

    public Piece PieceAt(Square sq) => _pieces[sq.AsInt()];

    public bool IsEmpty(Square sq) => _pieces[sq.AsInt()] == Piece.EmptyPiece;

    public void AddPiece(Piece pc, Square sq)
    {
        _pieces[sq.AsInt()] = pc;
        _byType[PieceTypes.AllPieces.AsInt()] |= sq;
        _byType[pc.Type().AsInt()] |= sq;
        _bySide[pc.ColorOf().Side] |= sq;
        _index[sq.AsInt()] = _pieceCount[pc.AsInt()]++;
        _pieceList[pc.AsInt()][_index[sq.AsInt()]] = sq;
        _pieceCount[PieceTypes.AllPieces.MakePiece(pc.ColorOf()).AsInt()]++;
    }

    public void RemovePiece(Square sq)
    {
        Debug.Assert(sq.IsOk);
        
        // WARNING: This is not a reversible operation. If we remove a piece in MakeMove() and
        // then replace it in TakeMove() we will put it at the end of the list and not in its
        // original place, it means index[] and pieceList[] are not invariant to a MakeMove() +
        // TakeMove() sequence.
        var pc = _pieces[sq.AsInt()];
        _byType[PieceTypes.AllPieces.AsInt()] ^= sq;
        _byType[pc.Type().AsInt()] ^= sq;
        _bySide[pc.ColorOf().Side] ^= sq;
        /* board[s] = NO_PIECE;  Not needed, overwritten by the capturing one */
        var lastSquare = _pieceList[pc.AsInt()][--_pieceCount[pc.AsInt()]];
        _index[lastSquare.AsInt()] = _index[sq.AsInt()];
        _pieceList[pc.AsInt()][_index[lastSquare.AsInt()]] = lastSquare;
        _pieceList[pc.AsInt()][_pieceCount[pc.AsInt()]] = Types.Square.None;
        _pieceCount[PieceTypes.AllPieces.MakePiece(pc.ColorOf()).AsInt()]--;
    }

    public void ClearPiece(Square sq)
        => _pieces[sq.AsInt()] = Piece.EmptyPiece;

    public void MovePiece(Square from, Square to)
    {
        // _index[from] is not updated and becomes stale. This works as long as _index[] is
        // accessed just by known occupied squares.
        var f = from.AsInt();
        var t = to.AsInt();
        
        var pc = _pieces[f];
        var fromTo = from | to;
        _byType[PieceTypes.AllPieces.AsInt()] ^= fromTo;
        _byType[pc.Type().AsInt()] ^= fromTo;
        _bySide[pc.ColorOf().Side] ^= fromTo;
        _pieces[f] = Piece.EmptyPiece;
        _pieces[t] = pc;
        _index[t] = _index[f];
        _pieceList[pc.AsInt()][_index[t]] = to;
    }

    public Piece MovedPiece(Move m) => PieceAt(m.FromSquare());

    public BitBoard Pieces() => _byType[PieceTypes.AllPieces.AsInt()];

    public BitBoard Pieces(PieceTypes pt) => _byType[pt.AsInt()];

    public BitBoard Pieces(PieceTypes pt1, PieceTypes pt2) => _byType[pt1.AsInt()] | _byType[pt2.AsInt()];

    public BitBoard Pieces(Player p) => _bySide[p.Side];

    public BitBoard Pieces(Player p, PieceTypes pt) => _bySide[p.Side] & _byType[pt.AsInt()];

    public BitBoard Pieces(Player p, PieceTypes pt1, PieceTypes pt2)
        => _bySide[p.Side] & (_byType[pt1.AsInt()] | _byType[pt2.AsInt()]);

    public Square Square(PieceTypes pt, Player p)
    {
        Debug.Assert(_pieceCount[pt.MakePiece(p).AsInt()] == 1);
        return _pieceList[pt.MakePiece(p).AsInt()][0];
    }

    public ReadOnlySpan<Square> Squares(PieceTypes pt, Player c)
    {
        var pcIndex = pt.MakePiece(c).AsInt();
        var pcCount = _pieceCount[pcIndex];

        return pcCount == 0
            ? ReadOnlySpan<Square>.Empty
            : _pieceList[pcIndex].AsSpan()[..pcCount];
    }

    public int PieceCount(Piece pc) => _pieceCount[pc.AsInt()];

    public int PieceCount(PieceTypes pt, Player p) => PieceCount(pt.MakePiece(p));

    public int PieceCount(PieceTypes pt)
        => _pieceCount[pt.MakePiece(Player.White).AsInt()] + _pieceCount[pt.MakePiece(Player.Black).AsInt()];

    public int PieceCount() => PieceCount(PieceTypes.AllPieces);

    public IEnumerator<Piece> GetEnumerator()
        => _pieces.Where(static pc => pc != Piece.EmptyPiece).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}