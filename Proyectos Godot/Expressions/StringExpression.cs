namespace Godot;

public class StringExpression : Expressions
	{
		private Token nodo;
		public StringExpression(Token nodo)
		{
			this.nodo = nodo;
		}

		public Token Nodo { get => nodo; set => nodo = value; }

		public override TokenType Type { get => TokenType.StringExpression; }

	}


