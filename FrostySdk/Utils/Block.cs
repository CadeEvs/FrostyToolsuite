using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Frosty.Sdk.Utils;

/// <summary>
/// Manages a sequential area of memory, like <see cref="Span{T}"/> but on the unmanaged heap.
/// </summary>
/// <typeparam name="T">Underlying type.</typeparam>
public unsafe class Block<T> : IDisposable where T : unmanaged
{
    /// <summary>
    /// The absolute start of the block.
    /// </summary>
    public T* BasePtr { get; private set; }
    
    /// <summary>
    /// The current, potentially shifted start of the block.
    /// </summary>
    public T* Ptr { get; private set; }
    
    /// <summary>
    /// Absolute size of the block.
    /// </summary>
    public int BaseSize { get; private set; }
    
    /// <summary>
    /// Shifted size of the block. <see cref="BaseSize"/> - <see cref="ShiftAmount"/> gets the current Size. 
    /// </summary>
    public int ShiftAmount => (int)(Ptr - BasePtr);

    public int Size => BaseSize - ShiftAmount;


    /// <summary>
    /// Allocates a new block of memory on the heap.
    /// </summary>
    /// <param name="inSize">The total amount of items to allocate.</param>
    public Block(int inSize)
    {
        BasePtr = (T*)Marshal.AllocHGlobal(inSize * sizeof(T));
        Ptr = BasePtr;
        BaseSize = inSize;
    }

    /// <summary>
    /// Attempts to set or get the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is negative or larger than the Size of this <see cref="Block{T}"/>.</exception>
    public T this[int index]
    {
        get
        {
            if (index >= 0 && index < Size)
            {
                return Ptr[index];
            }
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        set
        {
            if (index >= 0 && index < Size)
            {
                Ptr[index] = value;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
    }

    /// <summary>
    /// Resizes the block to the specified size.
    /// </summary>
    /// <param name="inNewSize">The new size of the block. Must be at least 1.</param>
    public void Resize(int inNewSize)
    {
        if (inNewSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(inNewSize));
        }
        
        // Get the old offset.
        int currentOffset = ShiftAmount;
        int newSize = inNewSize * sizeof(T);
        BasePtr = (T*)Marshal.ReAllocHGlobal((IntPtr)BasePtr, newSize);
        BaseSize = newSize;
        // Adjust Ptr since BasePtr has changed.
        Ptr = BasePtr + currentOffset;
    }

    /// <summary>
    /// Shifts the beginning of the Block by the specified amount.
    /// </summary>
    /// <param name="inAmount">Amount of elements to shift the <see cref="Block{T}"/> start by.
    /// Can be negative, but attempting to shift before the beginning of the block will result in an exception.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if attempting to shift before the beginning of the block (e.g. BasePtr[-1]).</exception>
    public void Shift(int inAmount)
    {
        if (Ptr + inAmount < BasePtr)
        {
            throw new ArgumentOutOfRangeException(nameof(inAmount), $"Attempted to shift before the beginning of the block!");
        }
        Ptr += inAmount;
    }

    /// <summary>
    /// Resets the Shift back to 0.
    /// </summary>
    public void ResetShift()
    {
        Ptr = BasePtr;
    }

    public T[] ToArray()
    {
        T[] buf = new T[Size];
        for (int i = 0; i < Size; i++)
        {
            buf[i] = this[i];
        }

        return buf;
    }

    public Span<T> ToSpan()
    {
        return new Span<T>(Ptr, Size);
    }

    public List<T> ToList()
    {
        List<T> buf = new(Size);
        for (int i = 0; i < Size; i++)
        {
            buf.Add(this[i]);
        }

        return buf;
    }

    public Stream ToStream()
    {
        return new UnmanagedMemoryStream((byte*)Ptr, Size);
    }

    /// <summary>
    /// Interprets the block as a UTF-8 null terminated string.
    /// </summary>
    /// <returns>A string instance from the data of the block.</returns>
    public static explicit operator string?(Block<T> inBlock)
    {
        return Marshal.PtrToStringUTF8((IntPtr)inBlock.Ptr);
    }
    
    /// <summary>
    /// Copies the data of this <see cref="Block{T}"/> to the specified destination.
    /// </summary>
    /// <param name="inDest">The block to copy to.</param>
    public void CopyTo(Block<T> inDest)
    {
        if (inDest.BaseSize < BaseSize)
        {
            throw new ArgumentOutOfRangeException(nameof(inDest), "The target block must be at least as big as the source block!");
        }
        Buffer.MemoryCopy(BasePtr, inDest.BasePtr, inDest.BaseSize, BaseSize);
    }
    
    public void Dispose()
    {
        Marshal.FreeHGlobal((IntPtr)Ptr);
        GC.SuppressFinalize(this);
    }

    public override string ToString()
    {
        return $"Block<{typeof(T).Name}>[{Size}]";
    }
}