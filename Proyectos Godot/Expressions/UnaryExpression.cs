namespace Godot;

public class UnaryExpression : Expressions
	{
		public override TokenType Type { get => TokenType.BinaryExpression; }
		private Expressions right;
		private Token nodo;

		public UnaryExpression(Token nodo, Expressions right)
		{
			this.right = right;
			this.nodo = nodo;
		}
		public Expressions Right { get => right; set => right = value; }
		public Token Nodo { get => nodo; set => nodo = value; }

	}


