using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Frosty.Sdk.Utils;

/// <summary>
/// Manages a sequential area of memory like <see cref="Span{T}"/>, but on the unmanaged heap.
/// If used on stack memory, make sure to 
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
    /// Indicates if the underlying memory is assumed to be already disposed before <see cref="Block{T}.Dispose()"/> is called.
    /// This is the case with stack allocated memory, pointers from <see langword="fixed"/> scopes and Spans.
    /// </summary>
    private bool m_fragileMemory = false;

    /// <summary>
    /// Indicates if the underlying memory is usable. Any operation with this disabled will cause the <see cref="Block{T}"/> to destroy itself.
    /// </summary>
    private bool m_usable = true;

    
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
    /// Creates a <see cref="Block{T}"/> around a pre-allocated memory region.
    /// </summary>
    /// <param name="inPtr">The pointer to the start of the region.</param>
    /// <param name="inSize">The amount of elements (not bytes!) in the region.</param>
    public Block(T* inPtr, int inSize)
    {
        BasePtr = inPtr;
        Ptr = BasePtr;
        BaseSize = inSize;
    }

    /// <summary>
    /// Creates a <see cref="Block{T}"/> around a pre-allocated memory region.
    /// </summary>
    /// <param name="inStartPtr">The pointer to the start of the region.</param>
    /// <param name="inEndPtr">The pointer to the end of the region.</param>
    public Block(T* inStartPtr, T* inEndPtr)
    {
        BasePtr = inStartPtr;
        Ptr = BasePtr; 
        // If the diff between end and start pointers is 0, then the size is 1.
        // => size = diff + 1
        BaseSize = ((int)(inEndPtr - inStartPtr) / sizeof(T)) + 1;
    }

    /// <summary>
    /// Creates a <see cref="Block{T}"/> around a <see cref="Span{T}"/>.
    /// Also does not free the memory
    /// </summary>
    /// <param name="inSpan"></param>
    public Block(Span<T> inSpan)
    {
        // Note: According to the .NET 7 source code the pointer to a Span should not change over its lifetime.
        //       While it's not guaranteed to be like this on every platform, once a span goes out of scope,
        //       its associated memory will too and will be automatically freed. Attempting to access data through a
        //       Block in this state will cause a segmentation fault.
        //       If you want to avoid this, convert the Span to an Array first and use that constructor, which will copy
        //       all elements to a newly allocated region in memory (which might be slower).
        fixed (T* ptr = inSpan)
        {
            BasePtr = ptr;
            Ptr = BasePtr;
            BaseSize = inSpan.Length;
            m_fragileMemory = true;
        }
    }

    public Block(T[] inArray)
    {
        fixed (T* ptr = inArray)
        {
            int size = inArray.Length * sizeof(T);
            BasePtr = (T*)Marshal.AllocHGlobal(size);
            Ptr = BasePtr;
            BaseSize = inArray.Length;
            Buffer.MemoryCopy(ptr, BasePtr, size, size);
        }
    }

    /// <summary>
    /// Attempts to set or get the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is negative or larger than the size of this <see cref="Block{T}"/>.</exception>
    public T this[int index]
    {
        get
        {
            if (!m_usable)
            {
                throw new ObjectDisposedException(ToString());
            }
            if (index >= 0 && index < Size)
            {
                return Ptr[index];
            }
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        set
        {
            if (!m_usable)
            {
                throw new ObjectDisposedException(ToString());
            }
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
        if (!m_usable)
        {
            throw new ObjectDisposedException(ToString());
        }
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
        if (!m_usable)
        {
            throw new ObjectDisposedException(ToString());
        }
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
        if (!m_usable)
        {
            throw new ObjectDisposedException(ToString());
        }
        Ptr = BasePtr;
    }

    public T[] ToArray()
    {
        if (!m_usable)
        {
            throw new ObjectDisposedException(ToString());
        }
        T[] buf = new T[Size];
        for (int i = 0; i < Size; i++)
        {
            buf[i] = this[i];
        }

        return buf;
    }

    public Span<T> ToSpan()
    {
        if (!m_usable)
        {
            throw new ObjectDisposedException(ToString());
        }
        return new Span<T>(Ptr, Size);
    }

    public List<T> ToList()
    {
        if (!m_usable)
        {
            throw new ObjectDisposedException(ToString());
        }
        List<T> buf = new(Size);
        for (int i = 0; i < Size; i++)
        {
            buf.Add(this[i]);
        }

        return buf;
    }

    public Stream ToStream()
    {
        if (!m_usable)
        {
            throw new ObjectDisposedException(ToString());
        }
        return new UnmanagedMemoryStream((byte*)Ptr, Size);
    }

    /// <summary>
    /// Interprets the block as a UTF-8 null terminated string.
    /// </summary>
    /// <returns>A string instance from the data of the block.</returns>
    public static explicit operator string?(Block<T> inBlock)
    {
        if (!inBlock.m_usable)
        {
            throw new ObjectDisposedException(inBlock.ToString());
        }
        return Marshal.PtrToStringUTF8((IntPtr)inBlock.Ptr);
    }
    
    /// <summary>
    /// Copies the data of this <see cref="Block{T}"/> to the specified destination.
    /// </summary>
    /// <param name="inDest">The block to copy to.</param>
    public void CopyTo(Block<T> inDest)
    {
        if (!m_usable)
        {
            throw new ObjectDisposedException(ToString());
        }
        if (inDest.BaseSize < BaseSize)
        {
            throw new ArgumentOutOfRangeException(nameof(inDest), "The target block must be at least as big as the source block!");
        }
        Buffer.MemoryCopy(BasePtr, inDest.BasePtr, inDest.BaseSize, BaseSize);
    }

    /// <summary>
    /// Signals to the <see cref="Block{T}"/> that its underlying memory might already not exist anymore when <see cref="Dispose"/> is called.
    /// </summary>
    public void MarkMemoryAsFragile()
    {
        m_fragileMemory = true;
    }

    /// <summary>
    /// Signals to the <see cref="Block{T}"/> that its underlying memory has been destroyed.
    /// </summary>
    public void MarkMemoryAsUnusable()
    {
        m_usable = false;
    }
    
    public void Dispose()
    {
        if (m_usable)
        {
            if (!m_fragileMemory)
            {
                Marshal.FreeHGlobal((IntPtr)Ptr);
            }
            BasePtr = (T*)0;
            Ptr = (T*)0;
            m_usable = false;
        }
        GC.SuppressFinalize(this);
    }

    public override string ToString()
    {
        return $"Block<{typeof(T).Name}>[{Size}]";
    }
}