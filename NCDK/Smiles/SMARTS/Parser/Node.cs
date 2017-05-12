/* Generated By:JJTree: Do not edit this line. Node.java Version 4.3 */
/* JavaCCOptions:MULTI=true,NODE_USES_PARSER=false,VISITOR=true,TRACK_TOKENS=false,NODE_PREFIX=AST,NODE_EXTENDS=,NODE_FACTORY=,SUPPORT_CLASS_VISIBILITY_PUBLIC=true */
namespace NCDK.Smiles.SMARTS.Parser
{
    /* All AST nodes must implement this interface.  It provides basic
       machinery for constructing the parent and child relationships
       between nodes. */

    public
    interface Node
    {

        /// <summary> This method is called after the node has been made the current node.  It indicates that child nodes can now be added to it.</summary>
        void JJTOpen();

        /// <summary> This method is called after all the child nodes have been added.</summary>
        void JJTClose();

        /// <summary> This pair of methods are used to inform the node of its parent.</summary>
        void JJTSetParent(Node n);
        Node JJTGetParent();

        /// <summary> This method tells the node to add its argument to the node's list of children. </summary>
        void JJTAddChild(Node n, int i);

        /// <summary> This method returns a child node.  The children are numbered from zero, left to right.</summary>
        Node JJTGetChild(int i);

        /// <summary>Return the number of children the node has.</summary>
        int JJTGetNumChildren();

        /// <summary>Accept the visitor. </summary>
        object JJTAccept(SMARTSParserVisitor visitor, object data);

        /// <summary>
        /// Removes a child from this node
        /// </summary>
        /// <param name="i"></param>
        void JJTRemoveChild(int i);
    }
}