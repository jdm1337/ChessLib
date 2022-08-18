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

// ReSharper disable MemberCanBeInternal
namespace Rudz.Chess.Types;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// Bitboard struct, wraps an unsigned long with some nifty helper functionality and operators.
/// Enumeration will yield each set bit as a Square struct.
/// <para>For more information - please see https://github.com/rudzen/ChessLib/wiki/BitBoard</para>
/// </summary>
public readonly struct BitBoard : IEnumerable<Square>, IEquatable<BitBoard>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBoard(ulong value)
        : this() => Value = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBoard(BitBoard value)
        : this(value.Value) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBoard(Square square)
        : this(square.AsBb()) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBoard(int value)
        : this((ulong)value) { }

    public readonly ulong Value;

    public int Count => BitBoards.PopCount(in this);

    public bool IsEmpty => Value == 0;

    public static readonly BitBoard Empty = BitBoards.EmptyBitBoard;

    public string String => Convert.ToString((long)Value, 2).PadLeft(64, '0');

    /// <summary>
    /// [] overload :&gt;
    /// </summary>
    /// <param name="index">the damn index</param>
    /// <returns>the Bit object if assigning</returns>
    public BitBoard this[int index] => this.Get(index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator BitBoard(ulong value)
        => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator BitBoard(int value)
        => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator BitBoard(Square square)
        => new(square.AsBb());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard operator *(BitBoard left, ulong right)
        => left.Value * right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard operator *(ulong left, BitBoard right)
        => left * right.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard operator -(BitBoard left, int right)
        => left.Value - (ulong)right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard operator >>(BitBoard left, int right)
        => left.Value >> right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard operator <<(BitBoard left, int right)
        => left.Value << right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard operator |(BitBoard left, Square right)
        => left.Value | right.AsBb();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard operator |(BitBoard left, BitBoard right)
        => left.Value | right.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard operator ^(BitBoard left, BitBoard right)
        => left.Value ^ right.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard operator &(BitBoard left, BitBoard right)
        => left.Value & right.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard operator &(ulong left, BitBoard right)
        => left & right.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard operator &(BitBoard left, ulong right)
        => left.Value & right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard operator &(BitBoard left, Square right)
        => left.Value & right.AsBb();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard operator &(BitBoard left, File right)
        => left.Value & right.BitBoardFile();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard operator &(Square left, BitBoard right)
        => left.AsBb() & right.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard operator ~(BitBoard bitBoard)
        => ~bitBoard.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BitBoard operator --(BitBoard bitBoard)
    {
        BitBoards.ResetLsb(ref bitBoard);
        return bitBoard;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(BitBoard left, BitBoard right)
        => left.Count == right.Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <(BitBoard left, BitBoard right)
        => left.Count < right.Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >(BitBoard left, BitBoard right)
        => left.Count > right.Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator >=(BitBoard left, BitBoard right)
        => left.Count >= right.Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator <=(BitBoard left, BitBoard right)
        => left.Count <= right.Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(BitBoard left, BitBoard right)
        => left.Value != right.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(BitBoard bitBoard)
        => bitBoard.Value != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(BitBoard bitBoard)
        => bitBoard.Value == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Square FirstOrDefault()
        => IsEmpty ? Square.None : this.Lsb();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBoard Xor(int pos)
        => Value ^ (uint)pos;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBoard And(BitBoard other)
        => Value & other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBoard Or(BitBoard other)
        => Value | other;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBoard OrAll(params BitBoard[] bbs)
        => bbs.Aggregate(Value, (current, b) => current | b.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBoard OrAll(IEnumerable<BitBoard> bbs)
        => bbs.Aggregate(Value, (current, bb) => current | bb.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BitBoard OrAll(IEnumerable<Square> sqs)
    {
        var b = this;
        return sqs.Aggregate(b, (current, sq) => current | sq);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoreThanOne()
        => (Value & (Value - 1)) > 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<Square> GetEnumerator()
    {
        if (IsEmpty)
            yield break;

        BitBoard bb = Value;
        while (bb)
            yield return BitBoards.PopLsb(ref bb);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string ToString()
        => BitBoards.PrintBitBoard(this, Value.ToString());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(BitBoard other)
        => Value == other.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool Equals(object obj)
        => obj is BitBoard board && Equals(board);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode()
        => (int)(Value >> 32);
}
