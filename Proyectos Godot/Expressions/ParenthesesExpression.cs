namespace Godot;

public class ParenthesesExpression : Expressions
{
	public override TokenType Type { get => TokenType.ParenthesesExpression; }
	private Expressions corpus;
	public ParenthesesExpression(Token nodo, Expressions corpus)
	{
		this.corpus = corpus;
	}
	public Expressions Corpus { get => corpus; set => corpus = value; }
}


