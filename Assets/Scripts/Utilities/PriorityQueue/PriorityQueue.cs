using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Utilities.PriorityQueue {
    public sealed class PriorityQueue<TData> : IPriorityQueue<PriorityQueueNode<TData>, int> {

        // Index 0 of array is not used in this implementation
        private PriorityQueueNode<TData>[] nodes;
        private int maxNodes;

        private int _currentIndex = 1;
        private int _nodeCount = 0;


        public PriorityQueue(int maxNodes) {
            this.maxNodes = maxNodes;
            nodes = new PriorityQueueNode<TData>[maxNodes + 1];
        }

        public PriorityQueueNode<TData> First { get { return nodes[1]; } }

        public int Count { get { return _nodeCount; } }

        public void Clear() {
            Array.Clear(nodes, 0, nodes.Length);
            _currentIndex = 1;
            _nodeCount = 0;
        }

        public bool Contains(PriorityQueueNode<TData> node) {
            if (nodes[node.index] == node) {
                return true;
            } else {
                for (int i = 0; i < _currentIndex; i++) {
                    if (nodes[i] == node) {
                        return true;
                    }
                }
            }

            return false;
        }

        public PriorityQueueNode<TData> Dequeue() {
            if (IsEmpty()) {
                return null;
            }

            PriorityQueueNode<TData> dequeuedNode = nodes[1];
            PriorityQueueNode<TData> swapperNode = nodes[_currentIndex - 1];
            nodes[1] = swapperNode;
            nodes[_currentIndex - 1] = null;
            swapperNode.index = 1;

            PriorityQueueNode<TData> left = GetLeft(swapperNode);
            PriorityQueueNode<TData> right = GetRight(swapperNode);

            while ((left != null && swapperNode.priority > left.priority) || (right != null && swapperNode.priority > right.priority)) {
                if (left == null) {
                    Swap(swapperNode, right);
                } else if (right == null) {
                    Swap(swapperNode, left);
                } else {
                    if (left.priority <= right.priority) {
                        Swap(swapperNode, left);
                    } else {
                        Swap(swapperNode, right);
                    }
                }
                
                left = GetLeft(swapperNode);
                right = GetRight(swapperNode);
            }

            _currentIndex--;
            _nodeCount--;
            return dequeuedNode;
        }

        public void Enqueue(PriorityQueueNode<TData> node, int priority) {
            if (_currentIndex >= nodes.Length) {
                throw new InvalidOperationException("The heap has reached its maximum capacity. Cannot enqueue further nodes.");
            }

            if (priority < 0) {
                throw new ArgumentOutOfRangeException("Priority cannot be less than 0!");
            }

            // Place the new element in the next available position in the array
            node.index = _currentIndex;
            node.priority = priority;
            nodes[_currentIndex] = node;

            // Compare the new element with its parent - if smaller, then swap with parent
            PriorityQueueNode<TData> parent;
            while ((parent = GetParent(node)) != null && parent.priority > node.priority) {
                Swap(node, parent);
            }
            
            _currentIndex++;
            _nodeCount++;
        }

        public IEnumerator<PriorityQueueNode<TData>> GetEnumerator() {
            throw new System.NotImplementedException();
        }

        public void Remove(PriorityQueueNode<TData> node) {
            if (nodes[node.index] != node) {
                throw new InvalidOperationException("The node being removed has already been removed (index mismatch).");
            }

            PriorityQueueNode<TData> swapperNode = nodes[_currentIndex - 1];
            nodes[node.index] = swapperNode;
            nodes[_currentIndex - 1] = null;
            swapperNode.index = node.index;

            PriorityQueueNode<TData> left = GetLeft(swapperNode);
            PriorityQueueNode<TData> right = GetRight(swapperNode);

            while ((left != null && swapperNode.priority > left.priority) || (right != null && swapperNode.priority > right.priority)) {
                if (left == null) {
                    Swap(swapperNode, right);
                } else if (right == null) {
                    Swap(swapperNode, left);
                } else {
                    if (left.priority <= right.priority) {
                        Swap(swapperNode, left);
                    } else {
                        Swap(swapperNode, right);
                    }
                }

                left = GetLeft(swapperNode);
                right = GetRight(swapperNode);
            }

            _currentIndex--;
            _nodeCount--;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new System.NotImplementedException();
        }

        public void UpdatePriority(PriorityQueueNode<TData> node, int priority) {
            Remove(node);
            Enqueue(node, priority);
        }

        public void Swap(PriorityQueueNode<TData> first, PriorityQueueNode<TData> second) {
            int temp = first.index;
            first.index = second.index;
            second.index = temp;
            nodes[first.index] = first;
            nodes[second.index] = second;
        }

        public PriorityQueueNode<TData> GetLeft(PriorityQueueNode<TData> node) {
            int index = 2 * node.index;
            if (index >= maxNodes) {
                return null;
            }

            return nodes[index];
        }

        public PriorityQueueNode<TData> GetRight(PriorityQueueNode<TData> node) {
            int index = 2 * node.index + 1;
            if (index >= maxNodes) {
                return null;    
            }

            return nodes[index];
        }

        public PriorityQueueNode<TData> GetParent(PriorityQueueNode<TData> node) {
            if (node.index < 2) {
                return null;
            }

            return nodes[node.index / 2];
        }

        public bool IsEmpty() {
            return First == null;
        }

        public PriorityQueueNode<TData> GetElementAt(int index) {
            if (index < 0 || index >= nodes.Length) {
                throw new ArgumentOutOfRangeException();
            }

            return nodes[index];
        }

        public bool IsFull() {
            return _currentIndex >= nodes.Length;
        }
    }
}
