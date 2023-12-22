using System.Collections.Generic;
namespace Godot;

public class Sequence:Expressions , ISequence
{
	private int position=-1;
	private List<Expressions> _sequence;
	private TokenType elementsType;
	private bool TakeNextSequence;
	private ISequence nextSequence;
	
	public List<Expressions> sequence { get => _sequence; set => _sequence= value; }

	public TokenType ElementsType { get => elementsType; set => elementsType= value; }

	public   Expressions Current
	{
		get
		{
			if(!TakeNextSequence)
				return sequence[position];
			return	NextSequence.Current;

		}
			
	}

    public ISequence NextSequence { get => nextSequence; set => nextSequence = value ;}

    public Sequence(List<Expressions> sequence)
	{
		this.sequence = sequence;
	}

	public  bool MoveNext()
	{
		if(position+1<sequence.Count)
		{
			position++;
			return true;
		}
		if(nextSequence==null)
			return false;


		if(nextSequence.MoveNext())
		{
			TakeNextSequence=true;
			return true;
		}
		return false;
	}

	public  Expressions Take()
	{
		List<Expressions> new_secuence= new List<Expressions>();
		new_secuence.Add(Current);
		while(MoveNext())
		{
			new_secuence.Add(Current);
		}
		return new Sequence(new_secuence);
	}

}

public class InfineSequence : Expressions, ISequence
{
	private int position=-1;
	public int stard, end;
	public bool hasEnd;
	private bool TakeNextSequence;
	
	private List<Expressions> range;
	private TokenType elementsType;
	private ISequence nextSequence;
	public List<Expressions> sequence { get => range; set => range=value; }

	public TokenType ElementsType { get => elementsType; set => elementsType= value; }
	public  Expressions Current 
	{
		get
		{
			if(!TakeNextSequence)
				return  new NumberExpression(new Token((stard+position).ToString(),0,0,TokenType.NumberExpression));
			return NextSequence.Current;
		}
	}

    public ISequence NextSequence { get => nextSequence; set => nextSequence=value; }
    

    public InfineSequence(List<Expressions> range,bool hasEnd)
	{
		this.range=range;
		this.hasEnd=hasEnd;
	}
	public void GetLimit(bool first)
	{
		int index;
		if(first)   index=0;
		else    index=1;
		var x=Execute.Evaluator(range[index]);
		if(x.Resultado is NumberExpression n)
		{
			bool isInt=int.TryParse(n.Number.dato,out int f);
			if(isInt && f>=0)
			{
				if(first)   stard=f;
				else    end=f;
				return;
			}    
		}
		Errors Error = new Errors($"! SEMANTIC ERROR: Sequence range must be between positive integers.", 0, 0);
		Lexer.error_list.Add(Error);
	}

	public bool CheckDefined()
	{
		return stard<=end;
	}	

	public  Expressions Take()  
	{
		List<Expressions> range= new List<Expressions>();
		range.Add(Current);
		if(!hasEnd)				
			return new InfineSequence(range,false);
		
		while(MoveNext())
		{
			range.Add(Current);
		}
		return new Sequence(range);
	}

	public  bool MoveNext()               
	{
		GetLimit(true);
		if(hasEnd)	GetLimit(false);
		
		if(!hasEnd)
		{
			position++;
			return  true;
		}
		if(position+1<end-stard+1)
		{
			position++;
			return true;
		}
		if(nextSequence==null)
			return false;


		if(nextSequence.MoveNext())
		{
			TakeNextSequence=true;
			return true;
		}
		return false;
	}
}

public interface ISequence
{	
	public TokenType ElementsType {get;set;}
	public Expressions Current{get ;}
	public List<Expressions> sequence{get;set;}
	public bool MoveNext();

	public ISequence NextSequence{get;set;}
	public Expressions Take();

}














