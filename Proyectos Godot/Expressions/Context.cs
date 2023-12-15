using System.Collections.Generic;
namespace Godot;
//  a=2   b=2   c,d,g={1,2,3}
//
//
//	a    b     c,d,g   c    d    g
//

public class Context
{
	public Dictionary<Identifiers, Expressions> Variables;
	public List<MultipleIdentifiers> multipleIdentifiersList;
	public List<Function> functionsList;
	public Context Father;
	public Context(Dictionary<Identifiers, Expressions> variables, Context  father,List<Function> functionsList,List<MultipleIdentifiers> multipleIdentifiersList)
	{
		this.Variables = variables;
		this.Father = father;
		this.multipleIdentifiersList=multipleIdentifiersList;
		this.functionsList=functionsList;
	}
	

	public Expressions  GetValue(IdentifierExpression identifier)
	{	
		if(Variables.ContainsKey(identifier))   return Variables[identifier];

		if (this.Father == null)
		{
			Errors Error = new Errors($"! SEMANTIC ERROR: The name '{identifier.Identifier.dato}' does not exist in the current context.",identifier.Identifier.line,identifier.Identifier.col);
			Lexer.error_list.Add(Error);
			return null;
		}
		return this.Father.GetValue(identifier);
	}
	
	internal Function Getfunction(Token dato)
	{
		foreach (var funcion in functionsList)
		{
			if(funcion.NameToken.dato==dato.dato)    return funcion;
		}
		if(Father==null)
		{
			Errors Error = new Errors($"! SEMANTIC ERROR: The name '{dato.dato}' does not exist in the current context.",dato.line,dato.col);
			Lexer.error_list.Add(Error);
			return null;
		}
		return Father.Getfunction(dato);
	}

	public void AddIdentifiers(MultipleIdentifiers mult,Expressions s)
	{
		var sequence= Execute.Evaluator(s);
		bool defined=true;
		if(Lexer.error_list.Count==0 &&	sequence.Resultado is Undefined)
		{
			for (int j = 0; j < mult.identifiers.Count; j++)
			{
				Variables.Add(mult.identifiers[j],new Undefined());
			}
			return;
		}
		
		if(sequence.Resultado is not ISequence seq)
		{
			Errors Error = new Errors($"! SEMANTIC ERROR: Unexepected {sequence.Type}	Expected Sequence .",0,0);
			Lexer.error_list.Add(Error);
			return;
		}
		
		for (int j = 0; j < mult.identifiers.Count; j++)
		{
			Expressions current= new Undefined();
			if(seq.MoveNext()&& defined) current=seq.Current;
			if(j==mult.identifiers.Count-1)
			{
				if (mult.identifiers[j].Identifier.dato=="_")continue;   
				if(current is Undefined)
				{
					if(defined)
						Variables.Add(mult.identifiers[j],new Sequence(new List<Expressions>()));
					else Variables.Add(mult.identifiers[j],current);
				}
				else
				{		
					Variables.Add(mult.identifiers[j],seq.Take());
				}
				continue;
			}
			if (mult.identifiers[j].Identifier.dato=="_")continue;
			Variables.Add(mult.identifiers[j],current);
		} 


	}
}


