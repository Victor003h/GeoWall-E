namespace Godot;

public class NumberExpression : Expressions
{
	public override TokenType Type { get => TokenType.NumberExpression; }
	public Token Number { get => number; set => number = value; }

	private Token number;

	public NumberExpression(Token number)
	{
		this.number = number;
	}
}


