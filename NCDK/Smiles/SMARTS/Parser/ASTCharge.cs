/* Generated By:JJTree: Do not edit this line. ASTCharge.java Version 4.3 */
/* JavaCCOptions:MULTI=true,NODE_USES_PARSER=false,VISITOR=true,TRACK_TOKENS=false,NODE_PREFIX=AST,NODE_EXTENDS=,NODE_FACTORY=,SUPPORT_CLASS_VISIBILITY_PUBLIC=true */
namespace NCDK.Smiles.SMARTS.Parser
{

    public
    class ASTCharge : SimpleNode
    {
        /// <summary>
        /// The charge value.
        /// </summary>
        public int Charge { get; set; }

        /// <summary>
        /// true if charge is positive.
        /// </summary>
        public bool IsPositive { get; set; }

        public ASTCharge(int id)
          : base(id)
        {
        }

        public ASTCharge(SMARTSParser p, int id)
          : base(p, id)
        {
        }

        /// <summary>Accept the visitor. </summary>
        public override object JjtAccept(SMARTSParserVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }
    }
}
