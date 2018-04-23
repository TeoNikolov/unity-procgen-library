using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Utilities.PriorityQueue {
    public sealed class PriorityQueue<T> : IPriorityQueue<T, int>
        where T : PriorityQueueNode {

        // Index 0 of array is not used in this implementation
        private T[] nodes;
        private int maxNodes;

        private int currentIndex = 1;
        private int nodeCount = 0;


        public PriorityQueue(int maxNodes) {
            this.maxNodes = maxNodes;
            nodes = new T[maxNodes + 1];
        }

        public T First { get { return nodes[1]; } }

        public int Count { get { return nodeCount; } }

        public void Clear() {
            nodes = new T[maxNodes + 1];
        }

        public bool Contains(T node) {
            if (nodes[node.index] == node) {
                return true;
            } else {
                for (int i = 0; i < currentIndex; i++) {
                    if (nodes[i] == node) {
                        return true;
                    }
                }
            }

            return false;
        }

        public T Dequeue() {
            if (IsEmpty()) {
                throw new InvalidOperationException("The heap is empty. Cannot dequeue any nodes.");
            }

            T dequeuedNode = nodes[1];
            T swapperNode = nodes[currentIndex - 1];
            nodes[1] = swapperNode;
            nodes[currentIndex - 1] = null;
            swapperNode.index = 1;

            T left = GetLeft(swapperNode);
            T right = GetRight(swapperNode);

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

            currentIndex--;
            nodeCount--;
            return dequeuedNode;
        }

        public void Enqueue(T node, int priority) {
            if (currentIndex >= nodes.Length) {
                throw new InvalidOperationException("The heap has reached its maximum capacity. Cannot enqueue further nodes.");
            }

            if (priority < 0) {
                throw new ArgumentOutOfRangeException("Priority cannot be less than 0!");
            }

            // Place the new element in the next available position in the array
            node.index = currentIndex;
            node.priority = priority;
            nodes[currentIndex] = node;

            // Compare the new element with its parent - if smaller, then swap with parent
            T parent;
            while ((parent = GetParent(node)) != null && parent.priority > node.priority) {
                Swap(node, parent);
            }
            
            currentIndex++;
            nodeCount++;
        }

        public IEnumerator<T> GetEnumerator() {
            throw new System.NotImplementedException();
        }

        public void Remove(T node) {
            if (nodes[node.index] != node) {
                throw new InvalidOperationException("The node being removed has already been removed (index mismatch).");
            }

            T swapperNode = nodes[currentIndex - 1];
            nodes[node.index] = swapperNode;
            nodes[currentIndex - 1] = null;
            swapperNode.index = node.index;

            T left = GetLeft(swapperNode);
            T right = GetRight(swapperNode);

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

            currentIndex--;
            nodeCount--;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new System.NotImplementedException();
        }

        public void UpdatePriority(T node, int priority) {
            Remove(node);
            Enqueue(node, priority);
        }

        public void Swap(T first, T second) {
            int temp = first.index;
            first.index = second.index;
            second.index = temp;
            nodes[first.index] = first;
            nodes[second.index] = second;
        }

        public T GetLeft(T node) {
            int index = 2 * node.index;
            if (index >= maxNodes) {
                return null;
            }

            return nodes[index];
        }

        public T GetRight(T node) {
            int index = 2 * node.index + 1;
            if (index >= maxNodes) {
                return null;    
            }

            return nodes[index];
        }

        public T GetParent(T node) {
            if (node.index < 2) {
                return null;
            }

            return nodes[node.index / 2];
        }

        public bool IsEmpty() {
            for (int i = 0; i < nodes.Length; i++) {
                if (nodes[i] != null) {
                    return false;
                }
            }

            return true;
        }

        public T GetElementAt(int index) {
            if (index < 0 || index >= nodes.Length) {
                throw new ArgumentOutOfRangeException();
            }

            return nodes[index];
        }
    }
}
