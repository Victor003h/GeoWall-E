using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace Godot;

public abstract class Expressions
{
	public virtual TokenType Type { get => type; set => type = value; }
	public TokenType type;

}


	
public class ErrorExpression : Expressions
{
	public int position { get; set; }
	public Token token { get; set; }

	public ErrorExpression(int position, Token token)
	{
		this.token = token;
		this.position = position;
	}
}


	
public class ColorExpression : Expressions
{
	public System.Drawing.Color color;

	public ColorExpression(System.Drawing.Color color)
	{
		this.color = color;
	}
}
public class RestoreExpression : Expressions
{
}
public class MeasureExpression : Expressions
{
	public Expressions point1 { get; set; }
	public Expressions point2 { get; set; }

	public MeasureExpression(Expressions point1, Expressions point2)
	{
		this.point1 = point1;
		this.point2 = point2;

	}
}

public class Undefined :Expressions
{
}

public class ImportExpression:Expressions
{
	public List<Expressions> expressions { get; set; }

	public ImportExpression(List<Expressions> expressions)
	{
		this.expressions = expressions;
	}
}


	


