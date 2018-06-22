using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utilities.PriorityQueue {
    public class PriorityQueueNode<T> {
        public int priority;
        public int index;
        public T data;

        public PriorityQueueNode(T data) {
            this.data = data;
        }
    }
}