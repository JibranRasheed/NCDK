/* Generated By:JJTree: Do not edit this line. ASTSimpleBond.java Version 4.3 */
/* JavaCCOptions:MULTI=true,NODE_USES_PARSER=false,VISITOR=true,TRACK_TOKENS=false,NODE_PREFIX=AST,NODE_EXTENDS=,NODE_FACTORY=,SUPPORT_CLASS_VISIBILITY_PUBLIC=true */
namespace NCDK.Smiles.SMARTS.Parser
{
    [System.Obsolete]
    internal class ASTSimpleBond : SimpleNode
    {
        public ASTSimpleBond(int id)
          : base(id)
        {
        }

        public ASTSimpleBond(SMARTSParser p, int id)
          : base(p, id)
        {
        }

        /// <summary>
        /// Type of bond.
        /// </summary>
        public int BondType { get; set; }

        /// <summary>Accept the visitor. </summary>
        public override object JjtAccept(ISMARTSParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }
    }
}
