using NCDK.Common.Collections;
using System;

namespace FaulonSignatures
{
    public class Invariants : ICloneable
    {
      /// <summary>
        /// The colors assigned to vertices of the input graph
        /// </summary>
        public int[] colors;

      /// <summary>
        /// The invariants of the nodes of the DAG
        /// </summary>
        public int[] nodeInvariants;

      /// <summary>
        /// The invariants of the vertices of the input graph
        /// </summary>
        public int[] vertexInvariants;

        public Invariants(int vertexCount, int nodeCount)
        {
            this.colors = new int[vertexCount];
            Arrays.Fill(colors, -1);
            this.nodeInvariants = new int[nodeCount];
            this.vertexInvariants = new int[vertexCount];
        }

        public int GetColor(int vertexIndex)
        {
            return colors[vertexIndex];
        }

        public void SetColor(int vertexIndex, int color)
        {
            colors[vertexIndex] = color;
        }

        public int GetVertexInvariant(int vertexIndex)
        {
            return vertexInvariants[vertexIndex];
        }

        public int[] GetVertexInvariants()
        {
            return vertexInvariants;
        }

        public int[] GetVertexInvariantCopy()
        {
            return (int[])vertexInvariants.Clone();
        }

        public void SetVertexInvariant(int vertexIndex, int value)
        {
            vertexInvariants[vertexIndex] = value;
        }

        public int GetNodeInvariant(int nodeIndex)
        {
            return nodeInvariants[nodeIndex];
        }

        public void SetNodeInvariant(int nodeIndex, int value)
        {
            nodeInvariants[nodeIndex] = value;
        }

        public object Clone()
        {
            Invariants copy = new Invariants(colors.Length, vertexInvariants.Length);
            copy.colors = (int[])colors.Clone();
            copy.nodeInvariants = (int[])nodeInvariants.Clone();
            copy.vertexInvariants = (int[])vertexInvariants.Clone();
            return copy;
        }

        public override string ToString()
        {
            return "colors: " + Arrays.ToJavaString(colors) + ", "
                 + "node inv" + Arrays.ToJavaString(nodeInvariants)
                 + ", vertex inv : " + Arrays.ToJavaString(vertexInvariants);
        }
    }
}
