using System;
using System.Collections.Generic;

namespace AxonSoft.Assesment
{
    public class TreeNode <T> where T: class
    {
        public List<TreeNode<T>> ChildNodes { get; set; }

        public T Value { get; private set; }

        public TreeNode(T node)
        {
            Value = node;
            ChildNodes = new List<TreeNode<T>>();
        }

        /// <summary>
        /// Useful string representaton of Triangle object for debug.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Value)}:{Value.ToString()}; {nameof(ChildNodes)}:{ChildNodes.Count}";
        }
    }
}
