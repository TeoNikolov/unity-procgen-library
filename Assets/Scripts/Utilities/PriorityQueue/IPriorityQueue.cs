using System;
using System.Collections.Generic;

namespace Assets.Scripts.Utilities.PriorityQueue {
    /// <summary>
    /// The IPriorityQueue interface.  This is mainly here for purists, and in case I decide to add more implementations later.
    /// For speed purposes, it is actually recommended that you *don't* access the priority queue through this interface, since the JIT can
    /// (theoretically?) optimize method calls from concrete-types slightly better.
    /// </summary>
    public interface IPriorityQueue<TItem, in TPriority> : IEnumerable<TItem>
        where TPriority : IComparable<TPriority> {
        /// <summary>
        /// Enqueue a node to the priority queue.  Lower values are placed in front. Ties are broken by first-in-first-out.
        /// See implementation for how duplicates are handled.
        /// </summary>
        void Enqueue(TItem node, TPriority priority);

        /// <summary>
        /// Removes the head of the queue (node with minimum priority; ties are broken by order of insertion), and returns it.
        /// </summary>
        TItem Dequeue();

        /// <summary>
        /// Removes every node from the queue.
        /// </summary>
        void Clear();

        /// <summary>
        /// Returns whether the given node is in the queue.
        /// </summary>
        bool Contains(TItem node);

        /// <summary>
        /// Removes a node from the queue.  The node does not need to be the head of the queue.  
        /// </summary>
        void Remove(TItem node);

        /// <summary>
        /// Call this method to change the priority of a node.  
        /// </summary>
        void UpdatePriority(TItem node, TPriority priority);
        
        /// <summary>
        /// Swaps two nodes inside the priority queue.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        void Swap(TItem first, TItem second);

        /// <summary>
        /// Gets the element located in the provided index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        TItem GetElementAt(int index);

        /// <summary>
        /// Get the left node for the provided node (if any).
        /// </summary>
        /// <param name="node"></param>
        TItem GetLeft(TItem node);

        /// <summary>
        /// Get the right node for the provided node (if any).
        /// </summary>
        /// <param name="node"></param>
        TItem GetRight(TItem node);

        /// <summary>
        /// Get the parent node for the provided node (if any).
        /// </summary>
        /// <param name="node"></param>
        TItem GetParent(TItem node);

        /// <summary>
        /// Determine if the heap is empty.
        /// </summary>
        /// <returns></returns>
        bool IsEmpty();

        /// <summary>
        /// Returns the head of the queue, without removing it (use Dequeue() for that).
        /// </summary>
        TItem First { get; }

        /// <summary>
        /// Returns the number of nodes in the queue.
        /// </summary>
        int Count { get; }
    }
}