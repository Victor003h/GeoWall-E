using System.Collections.Generic;

namespace Godot;

public abstract class Identifiers : Expressions
{


}

public class IdentifierExpression : Identifiers
{
	private Token identifier;

	public IdentifierExpression(Token identifier)
	{
		this.identifier = identifier;
	}

	public new TokenType type = TokenType.Identifier;
	public override TokenType Type { get => this.type; set => this.type = value; }
	public Token Identifier { get => identifier; set => identifier = value; }
	

	public override bool Equals(object obj)
	{
		if(obj is IdentifierExpression id)
			return id.Identifier.dato==identifier.dato;
		
		return false;
	}

	public override int GetHashCode()
	{
		return identifier.dato.GetHashCode();
	}
}

public class MultipleIdentifiers : Identifiers
{
	public Expressions sequence;
	public List<IdentifierExpression> identifiers;
	public bool underscore;

	public MultipleIdentifiers(List<IdentifierExpression> identifiers)
	{
		this.identifiers = identifiers;
	}

	
}

