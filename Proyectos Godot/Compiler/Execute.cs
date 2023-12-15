using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
namespace Godot;
public static class Execute
	{
		public static List<Function> functionList = new List<Function>();
		public static int StackOverFlow = 0;
		

		public static Result Evaluator(Expressions tree)
		{

			Token generico = new Token("",0,0,TokenType.Error);
			
			if(tree is ISequence s)
			{
				bool first=true;
				TokenType fi= TokenType.NumberExpression;
				List<Expressions> new_secuen= new List<Expressions>();
				foreach (var item in s.sequence)
				{
					var x= Evaluator(item);
					if(x==null) return new Result(null, TokenType.Error);
					if(first)
					{
						 fi=x.Type; first=false;
					}
					else
					{
						if(x.Type!=fi)
						{
							Errors Err = new Errors($"! SEMANTIC ERROR: Unexpected {x.Type} , expected a {fi} ,Sequence elements must be  of the same type", 0, 0);
							Lexer.error_list.Add(Err);
							return new Result(null, TokenType.Error);

						}
					}
					new_secuen.Add(x.Resultado);
				}
				
				if(s is InfineSequence i)   
				{
					var r= new InfineSequence(new_secuen,i.hasEnd);
					r.ElementsType=fi;
					i.GetLimit(true);
					if(i.hasEnd)	i.GetLimit(false);
					return	new Result(r,TokenType.InfinitiSequence);
				}
				var r2= new Sequence(new_secuen);
				r2.ElementsType=fi;
				return new Result(r2,TokenType.Sequence);
			}
		   
			if(tree is Undefined u)
			{
				return new Result(u,TokenType.UndefinedExpression);
			}
			
			if (tree is PointExpression point )
			{
				var x=Evaluator(point.x);
				var y=Evaluator(point.y);
				control.CheckError();
				return new Result(new PointExpression(x.Resultado,y.Resultado,point.Id,point.color), TokenType.Point);
			}
			
			if (tree is LineExpression line && tree is not SegmentExpression && tree is not RayExpression)
			{
				var p1 = Evaluator(line.point1);
				var p2 = Evaluator(line.point2);

				if (p1.Resultado is not PointExpression pt1)
				{
					Errors Error = new Errors($"! SEMANTIC ERROR: Expected PointEsxpression ", 0, 0);
					Lexer.error_list.Add(Error);
					return new Result(null, TokenType.Error);
					
				}
				else if (p2.Resultado is not PointExpression pt2)
				{
					Errors Error = new Errors($"! SEMANTIC ERROR: Expected PointEsxpression ", 0, 0);
					Lexer.error_list.Add(Error);
					return new Result(null, TokenType.Error);
				}

				return new Result(new LineExpression(p1.Resultado,p2.Resultado,line.color), TokenType.Line);

			}
			
			if (tree is SegmentExpression segment)
			{
				var p1 = Evaluator(segment.point1);
				var p2 = Evaluator(segment.point2);
				
				if (p1.Resultado is not PointExpression pt1)
				{
					Errors Error = new Errors($"! SEMANTIC ERROR: Expected PointEsxpression ", 0, 0);
					Lexer.error_list.Add(Error);
					return new Result(null, TokenType.Error);
				}
				else if (p2.Resultado is not PointExpression pt2)
				{
					Errors Error = new Errors($"! SEMANTIC ERROR: Expected PointEsxpression ", 0, 0);
					Lexer.error_list.Add(Error);
					return new Result(null, TokenType.Error);
				}

				return new Result(new SegmentExpression(p1.Resultado, p2.Resultado,segment.color), TokenType.Segment);

			}
		   
			if (tree is RayExpression ray)
			{
				var p1 = Evaluator(ray.point1);
				var p2 = Evaluator(ray.point2);

				if (p1.Resultado is not PointExpression pt1)
				{
					Errors Error = new Errors($"! SEMANTIC ERROR: Expected PointEsxpression ", 0, 0);
					Lexer.error_list.Add(Error);
					return new Result(null, TokenType.Error);
				}
				else if (p2.Resultado is not PointExpression pt2)
				{
					Errors Error = new Errors($"! SEMANTIC ERROR: Expected PointEsxpression ", 0, 0);
					Lexer.error_list.Add(Error);
					return new Result(null, TokenType.Error);
				}

				return new Result(new RayExpression(p1.Resultado, p2.Resultado,ray.color), TokenType.Ray);

			}
		   
			if (tree is CircunferenceExpression circle && tree is not ArcExpression)
			{
				var auxpoint = Evaluator(circle.center);
				var c = Evaluator(circle.ratio);

				if (auxpoint.Resultado is not PointExpression pt)
				{
					Errors Error = new Errors($"! SEMANTIC ERROR: Expected PointEsxpression ", 0, 0);
					Lexer.error_list.Add(Error);
					return new Result(null, TokenType.Error);


				}
				else if (c.Resultado is not NumberExpression n)
				{
					Errors Error = new Errors($"! SEMANTIC ERROR: Expected NumberExpression ", 0, 0);
					Lexer.error_list.Add(Error);
					return new Result(null, TokenType.Error);
				}

				return new Result(new CircunferenceExpression(auxpoint.Resultado, c.Resultado,circle.color), TokenType.Circunference);

			}

			if (tree is ArcExpression arc)
			{
				var center = Evaluator(arc.center);
				var point1 = Evaluator(arc.point1);
				var point2 = Evaluator(arc.point2);
				var rad = Evaluator(arc.ratio);

				if (center.Resultado is not PointExpression c)
				{
					Errors Error = new Errors($"! SEMANTIC ERROR: Expected PointEsxpression ", 0, 0);
					Lexer.error_list.Add(Error);
					return new Result(null, TokenType.Error);


				}
				else if (point1.Resultado is not PointExpression pt1)
				{
					Errors Error = new Errors($"! SEMANTIC ERROR: Expected PointEsxpression ", 0, 0);
					Lexer.error_list.Add(Error);
					return new Result(null, TokenType.Error);


				}
				else if (point2.Resultado is not PointExpression pt2)
				{
					Errors Error = new Errors($"! SEMANTIC ERROR: Expected PointEsxpression ", 0, 0);
					Lexer.error_list.Add(Error);
					return new Result(null, TokenType.Error);


				}
				
				else if (rad.Resultado is not NumberExpression m)
				{
					Errors Error = new Errors($"! SEMANTIC ERROR: Expected NumberEsxpression ", 0, 0);
					Lexer.error_list.Add(Error);
					return new Result(null, TokenType.Error);
				}
				
				return new Result(new ArcExpression(center.Resultado, point1.Resultado, point2.Resultado, rad.Resultado,arc.color), TokenType.Arc);

			}

			if (tree is MeasureExpression measure)
			{
				var p1 = Evaluator(measure.point1);
				var p2 = Evaluator(measure.point2);

				PointExpression aux1;
				PointExpression aux2;

				if (p1.Resultado is PointExpression pt1)
				{
					aux1 = pt1;
				}
				else
				{
					Errors Error = new Errors($"! SEMANTIC ERROR: Expected PointEsxpression ", 0, 0);
					Lexer.error_list.Add(Error);
					return new Result(null, TokenType.Error);
				}

				if (p2.Resultado is PointExpression pt2)
				{
					aux2 = pt2;
				}
				else
				{
					Errors Error = new Errors($"! SEMANTIC ERROR: Expected PointEsxpression ", 0, 0);
					Lexer.error_list.Add(Error);
					return new Result(null, TokenType.Error);
				}

				double auxMeasure = GetMeasure(aux1, aux2);

				return new Result(new NumberExpression(new Token(auxMeasure.ToString(), 0, 0, TokenType.Measure)), TokenType.NumberToken);

			}

			if (tree is NumberExpression number)
			{
				return new Result(number, TokenType.NumberExpression);
			}

			if (tree is ParenthesesExpression p)
			{
				return Evaluator(p.Corpus);
			}

			if (tree is BinaryExpression binary)
			{
				if (binary.Nodo.type == TokenType.PlusToken || binary.Nodo.type == TokenType.MinusToken || binary.Nodo.type == TokenType.MultiplicationToken
				|| binary.Nodo.type == TokenType.DivisionToken || binary.Nodo.type == TokenType.RestoToken || binary.Nodo.type == TokenType.ExponentToken)
				{
					var res = BinaryOperatorEvaluator(binary);
					generico.dato= res.ToString();
					return new Result(res, TokenType.NumberExpression);
				}
				bool resul = BoolEvaluator(binary);
				generico.dato=resul.ToString();
				return new Result(new BoolExpression(generico), TokenType.NumberExpression);
			}

			if (tree is BoolExpression boolExpr)
			{
				bool res = bool.Parse(boolExpr.Nodo.dato);
				generico.dato=res.ToString();
				return new Result(new BoolExpression(generico), TokenType.NumberExpression);
			}

			if (tree is UnaryExpression unary)
			{
				var right = Evaluator(unary.Right);
				
				if (unary.Nodo.type == TokenType.PlusToken || unary.Nodo.type == TokenType.MinusToken)
				{
					if (right.Resultado is NumberExpression num)
					{
						if (unary.Nodo.type == TokenType.PlusToken)  return right;
						else
						{
							double aux = double.Parse(num.Number.dato);
							aux = -aux;
							generico.dato= aux.ToString();
							return new Result(new NumberExpression(generico), TokenType.NumberExpression);
						}
					}
 
					Errors error = new Errors($"! SEMANTIC ERROR: Unary Operator '{unary.Nodo.dato}' cannot be applied to operands of type  '{right.Type}'.", unary.Nodo.line, unary.Nodo.col);
					Lexer.error_list.Add(error);
					return new Result(null, TokenType.Error);
				}

				else if (unary.Nodo.type == TokenType.NotOperatorToken)
				{
					if (right.Resultado is BoolExpression b)
					{
						bool aux = bool.Parse(b.Nodo.dato);
						aux = !aux;
						generico.dato= aux.ToString();
						return new Result(new BoolExpression(generico), TokenType.BoolExpression);
						
					}
					
					Errors error = new Errors($"! SEMANTIC ERROR: Unary Operator '{unary.Nodo.dato}' cannot be applied to operands of type  '{right.Type}'.", unary.Nodo.line, unary.Nodo.col);
					Lexer.error_list.Add(error);
					return new Result(null, TokenType.Error);

				}
			}

			if (tree is StringExpression str)
			{
				return new Result(str, TokenType.StringExpression);
			}

			if (tree is ReservedFunction fun)
			{
				return FuntionEvaluator(fun);
			}
			
			if (tree is Identifiers I)
			{
				if(I is IdentifierExpression id)
				{
					var value = Parser.GlobalContext.GetValue(id);
					if (value == null) return new Result(null, TokenType.Error);
					var x = Evaluator(value);
					return x;
				}
				if(I is MultipleIdentifiers mult)
				{
					Parser.GlobalContext.AddIdentifiers(mult,mult.sequence);
					return new Result(mult,TokenType.MultipleIdentifiersExpression);										

				}
			}
			
			
			if (tree is LetExpression let)
			{
				//   mul             v 			ii=        
				Dictionary<Identifiers,Expressions> nuevo= new Dictionary<Identifiers, Expressions>();
				Parser.GlobalContext = new Context(nuevo,Parser.GlobalContext,let.context.functionsList,let.context.multipleIdentifiersList);
				if(let.context.Variables.Count!=0)
				{

					foreach (var item in let.context.Variables)
					{
						if(item.Key is IdentifierExpression i)
						{
							var value=Evaluator(item.Value);
							nuevo.Add(item.Key,value.Resultado);
						}
						else
						{
							Parser.GlobalContext.AddIdentifiers((MultipleIdentifiers)item.Key, item.Value);
						}
					}
				}

				foreach (var fEx in let.listReserverdFunction)
				{
					Evaluator(fEx);
				}
				var res = Evaluator(let.Scope);

				Parser.GlobalContext = Parser.GlobalContext.Father;				
				return res;

			}

			if(tree is DrawFunction d)
			{
				List<Expressions> figures = new List<Expressions>();
			   foreach (var fig in d.Figures)
			   {
					var r= Evaluator(fig);
					if(r==null) return null;
					if (r.Resultado is Figures figur)
					{
						figur.text = d.text;
						figures.Add(figur);
					}
					else
					{
						Errors Error = new Errors($"! SEMANTIC ERROR: Function draw receives a figure argument, not {r.Type} token.", 0, 0);
						Lexer.error_list.Add(Error);
						return null;
					}
			   }

				var ss=new DrawFunction(d.Function,figures);
				ss.text = d.text;
				control.drawFunctions.Add(ss);
				return new  Result(ss,TokenType.DrawFunction);
			}
		   
			if (tree is FunctionCall v)
			{
				Function function=Parser.GlobalContext.Getfunction(v.function);
				if(function==null)  return null;

				if (v.arg.Count != function.Context.Variables.Count)                       // chequeo si recibe la cantidad correcta de parametros 
				{
					Errors Error = new Errors($"! SEMANTIC ERROR: Function '{function.NameToken.dato}' receives {function.Context.Variables.Count} argument(s), but {v.arg.Count} were given.", v.function.line, v.function.col);
					Lexer.error_list.Add(Error);
					return null;
				}     
				
				Dictionary<Identifiers,Expressions> identifiers= new Dictionary<Identifiers, Expressions>();
				StackOverFlow++;
				// if (StackOverFlow > 150)
				// {
				// 	Errors Error = new Errors($"! SEMANTIC ERROR: StackOverflow error", 0, 0);
				// 	Lexer.error_list.Add(Error);
				// 	return new Result(null, TokenType.Error);
				// }

				for (int i = 0; i < v.arg.Count; i++)
				{
					var aux = Evaluator(v.arg[i]);
					if(aux==null)   return new Result(null, TokenType.Error); 

					identifiers.Add(function.args[i],aux.Resultado);         
				}
				Parser.GlobalContext = new Context(identifiers,Parser.GlobalContext,function.Context.functionsList,function.Context.multipleIdentifiersList);

				var a = Evaluator(function.Corpus);
				Parser.GlobalContext = Parser.GlobalContext.Father;
				StackOverFlow--;

				return a;

			}

			if (tree is Function f)
			{
				Errors e = new Errors("! SEMANTIC ERROR : Unexpected function declaration.", f.NameToken.line, f.NameToken.col);
				Lexer.error_list.Add(e);
				Lexer.function.Remove(f.NameToken.dato);
				functionList.RemoveAt(functionList.Count - 1);
				return new Result(null, TokenType.Error);
			}

			Errors er = new Errors($"! SEMANTIC ERROR. Unexpected expression of type {tree.type}", 0, 0);
			Lexer.error_list.Add(er);
			return new Result(null, TokenType.Error);
		}

		private static double GetMeasure(PointExpression p1, PointExpression p2)
		{
			var x1 = p1.GetX();
			var y1 = p1.GetY();
			var x2 = p2.GetX();
			var y2 = p2.GetY();

			int measure = (int)Math.Sqrt(Math.Pow((double)(x1 - x2), 2) + Math.Pow((double)(y1 - y2), 2));

			return measure;
		}
		private static bool BoolEvaluator(BinaryExpression tree)
		{
			var left = Evaluator(tree.Left);
			var right = Evaluator(tree.Right);
			if(left==null || right==null )  return false;
			if (tree.Nodo.dato == "&" || tree.Nodo.dato == "|")
			{
				if (left.Resultado is BoolExpression l && right.Resultado is BoolExpression r)
				{
					if (tree.Nodo.dato == "&")  return bool.Parse(l.Nodo.dato) && bool.Parse(r.Nodo.dato);
					return bool.Parse(l.Nodo.dato) || bool.Parse(r.Nodo.dato);
				}
				string error_type = $"! SEMANTIC ERROR: Operator '{tree.Nodo.dato}' cannot be applied to operands of type '{tree.Left.Type}' and '{tree.Right.Type}.'";
				Errors Error = new Errors(error_type, tree.Nodo.line, tree.Nodo.col);
				Lexer.error_list.Add(Error);
				return false;
			}
		   
			if (tree.Nodo.dato == ">" || tree.Nodo.dato == ">=" || tree.Nodo.dato == "<" || tree.Nodo.dato == "<=")
			{
				if (left.Resultado is NumberExpression l && right.Resultado is NumberExpression r)
				{
					
					if (tree.Nodo.dato == ">")  return double.Parse( l.Number.dato) > double.Parse(r.Number.dato);
					if (tree.Nodo.dato == ">=") return double.Parse( l.Number.dato) >= double.Parse(r.Number.dato);
					if (tree.Nodo.dato == "<")  return double.Parse( l.Number.dato) < double.Parse(r.Number.dato);
					if (tree.Nodo.dato == "<=") return double.Parse( l.Number.dato) <= double.Parse(r.Number.dato);
				}
				string error_type = $"! SEMANTIC ERROR: Operator '{tree.Nodo.dato}' cannot be applied to operands of type '{tree.Left.Type}' and '{tree.Right.Type}'.";
				Errors Error = new Errors(error_type, tree.Nodo.line, tree.Nodo.col);
				Lexer.error_list.Add(Error);
				return false;
			}       
			else if (tree.Nodo.dato == "==" || tree.Nodo.dato == "!=")
			{
				if (left.Type != right.Type)
				{
					string error_type = $"! SEMANTIC ERROR: Operator '{tree.Nodo.dato}' cannot be applied to operands of type '{tree.Left.Type}' and '{tree.Right.Type}'.";
					Errors Error = new Errors(error_type, tree.Nodo.line, tree.Nodo.col);
					Lexer.error_list.Add(Error);
					return false;
				}
				if (left.Resultado is NumberExpression l && right.Resultado is NumberExpression r)
				{
					if (tree.Nodo.dato == "==") return double.Parse(l.Number.dato) == double.Parse(r.Number.dato);
					return double.Parse(l.Number.dato) != double.Parse(r.Number.dato);
				}
				if (left.Resultado is BoolExpression l2 && right.Resultado is BoolExpression r2)
				{    
					if (tree.Nodo.dato == "==") return bool.Parse(l2.Nodo.dato) == bool.Parse(r2.Nodo.dato);
					return bool.Parse(l2.Nodo.dato) != bool.Parse(r2.Nodo.dato);
				}
				if (left.Resultado is StringExpression l3 && right.Resultado is StringExpression r3)
				{   
					if (tree.Nodo.dato == "==") return l3.Nodo.dato == r3.Nodo.dato;
					return l3.Nodo.dato != r3.Nodo.dato;
				}
			}

			return false;

		}
		
		public static Expressions BinaryOperatorEvaluator(BinaryExpression tree)
		{
			var left = Evaluator(tree.Left);
			var right = Evaluator(tree.Right);
			if(left.Resultado==null || right.Resultado==null )  return new NumberExpression(new Token("0", 0, 0, TokenType.NumberToken));
			
			if (left.Resultado is NumberExpression l && right.Resultado is NumberExpression r)
			{
				if (l.Number.type == TokenType.Measure || r.Number.type == TokenType.Measure)
				{
					if (tree.Nodo.type == TokenType.PlusToken || tree.Nodo.type == TokenType.MinusToken)
					{
						if(l.Number.type == TokenType.Measure && r.Number.type == TokenType.Measure)
						{
							if(tree.Nodo.type == TokenType.PlusToken)
								return new NumberExpression(new Token(Math.Abs(double.Parse(l.Number.dato) + double.Parse(r.Number.dato)).ToString(), 0, 0, TokenType.Measure));
							
							return new NumberExpression(new Token(Math.Abs(double.Parse(l.Number.dato) - double.Parse(r.Number.dato)).ToString(), 0, 0, TokenType.Measure));
						}
						Errors Err = new Errors("SINTAX ERROR: A number and a measure can not be sumed or rested.", tree.Nodo.line, tree.Nodo.col);
						Lexer.error_list.Add(Err);
						return new NumberExpression(new Token("0", 0, 0, TokenType.NumberToken));
                	}

					if (tree.Nodo.type == TokenType.MultiplicationToken)
					{	
						if(l.Number.type == TokenType.Measure && r.Number.type == TokenType.Measure)
						{
							
							Errors Err = new Errors("SINTAX ERROR: Measures can not be multiplied, only scaled by a number.", tree.Nodo.line, tree.Nodo.col);
							Lexer.error_list.Add(Err);
							return new NumberExpression(new Token("0", 0, 0, TokenType.NumberToken));
                    	}
						return new NumberExpression(new Token(Math.Truncate(Math.Abs(double.Parse(l.Number.dato) * double.Parse(r.Number.dato))).ToString(), 0, 0, TokenType.Measure));
					}
					if (tree.Nodo.type == TokenType.DivisionToken)
					{
						if (double.Parse(r.Number.dato) == 0)
						{
							string error_typ = $" ! MATH ERROR: You can not divide by 0.";
							Errors err = new Errors(error_typ, tree.Nodo.line, tree.Nodo.col);
							Lexer.error_list.Add(err);
						return new NumberExpression(new Token("0", 0, 0, TokenType.NumberToken));

                        }
						if (l.Number.type == TokenType.Measure && r.Number.type == TokenType.Measure)
						{
							return new NumberExpression(new Token(Math.Truncate(double.Parse(l.Number.dato) / double.Parse(r.Number.dato)).ToString(), 0, 0, TokenType.NumberToken));
						}
						Errors Err = new Errors("SINTAX ERROR: Measures can not be divided by a number, only by another measure.", tree.Nodo.line, tree.Nodo.col);
						Lexer.error_list.Add(Err);
						return new NumberExpression(new Token("0", 0, 0, TokenType.NumberToken));

                    }
				}

				if (tree.Nodo.type == TokenType.PlusToken)  return new NumberExpression(new Token((double.Parse(l.Number.dato) + double.Parse(r.Number.dato)).ToString(), 0, 0, TokenType.NumberToken));
				if (tree.Nodo.type == TokenType.MinusToken) return new NumberExpression(new Token((double.Parse(l.Number.dato) - double.Parse(r.Number.dato)).ToString(), 0, 0, TokenType.NumberToken));
				if (tree.Nodo.type == TokenType.MultiplicationToken)    return new NumberExpression(new Token((double.Parse(l.Number.dato) * double.Parse(r.Number.dato)).ToString(), 0, 0, TokenType.NumberToken));
				if (tree.Nodo.type == TokenType.ExponentToken)  return new NumberExpression(new Token((Math.Pow(double.Parse(l.Number.dato), double.Parse(r.Number.dato))).ToString(), 0, 0, TokenType.NumberToken));
				if (tree.Nodo.type == TokenType.RestoToken) return new NumberExpression(new Token((double.Parse(l.Number.dato) % double.Parse(r.Number.dato)).ToString(), 0, 0, TokenType.NumberToken));
				
				if (tree.Nodo.type == TokenType.DivisionToken)
				{
					if (double.Parse(r.Number.dato) == 0)
					{
						string error_typ = $" ! MATH ERROR: You can not divide by 0.";
						Errors Err = new Errors(error_typ, tree.Nodo.line, tree.Nodo.col);
						Lexer.error_list.Add(Err);
						return new NumberExpression(new Token("0", 0, 0, TokenType.NumberToken));
					}

					return new NumberExpression(new Token((double.Parse(l.Number.dato) / double.Parse(r.Number.dato)).ToString(), 0, 0, TokenType.NumberToken));

				}                
			}

			if(left.Resultado is ISequence seq)	//                               
			{
				
				if(seq is Sequence s1 && right.Resultado is ISequence s2)
				{
					var Concatenation= s1;
					Concatenation.NextSequence=s2;
					return Concatenation;
				}
				if(seq is InfineSequence i1 && right.Resultado is ISequence s3)
				{
					var Concatenation= i1;
					Concatenation.NextSequence=s3;
					return Concatenation;
				}
				if(right.Resultado is Undefined )
					return left.Resultado;	
			}
			if(left.Resultado is Undefined)
				return new Undefined();
			

			string error_type = $"! SEMANTIC ERROR: Operator '{tree.Nodo.dato}' cannot be applied to operands of type '{left.Type}' and '{right.Type}'.";
			Errors Error = new Errors(error_type, tree.Nodo.line, tree.Nodo.col);
			Lexer.error_list.Add(Error);
			return new NumberExpression(new Token("0", 0, 0, TokenType.NumberToken));

    }
		public static Result FuntionEvaluator(ReservedFunction tree)
		{
			switch (tree.Function.dato)
			{
				case "rand":
					{
						if (tree.Child.Count != 0)
						{
							Errors Error = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives 0 argument, but {tree.Child.Count} were given.", tree.Function.line, tree.Function.col);
							Lexer.error_list.Add(Error);
							return new Result(null, TokenType.Error);
						}
						Random r = new Random();
						Token resultToken= new Token(r.NextDouble().ToString(),0,0,TokenType.NumberToken);
						return new Result(new NumberExpression(resultToken), TokenType.NumberExpression);
					}
				case "print":
					{
						tree.Type = TokenType.PrintExpression;
						var x=Evaluator(tree.Child[0]);
						var e=x.CheckResult();
						if(e!=null)
						{
							if(string.IsNullOrEmpty(control.richTextLabel1.Text))	control.richTextLabel1.Text =e;
							else	control.richTextLabel1.Text += '\n'+e;
						}
						return x;
					}
				case "cos":
				case "sin":
				case "sqrt":
				case "count":
					{
						if (tree.Child.Count != 1)
						{
							Errors Err = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives 1 argument, but {tree.Child.Count} were given.", tree.Function.line, tree.Function.col);
							Lexer.error_list.Add(Err);
							return new Result(null, TokenType.Error);
						}
						var arg = Evaluator(tree.Child[0]); if(arg==null)   return new Result(null,TokenType.Error);
						if(tree.Function.dato=="count")
						{
							if(arg.Resultado is ISequence sequence)
							{
							   if(sequence is InfineSequence inf)
							   {
									if(!inf.hasEnd) return new Result(new Undefined(),TokenType.UndefinedExpression);
									inf.GetLimit(true);inf.GetLimit(false);
					
		
									Token resultToken = new Token((inf.end - inf.stard+1).ToString(), 0, 0, TokenType.NumberToken);
									return new Result(new NumberExpression(resultToken), TokenType.NumberExpression);
							   }

							   Token t= new Token(sequence.sequence.Count.ToString(),0,0,TokenType.NumberToken);
							   return new Result(new NumberExpression(t), TokenType.NumberExpression);
							}

							Errors Err = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives 'Sequence ' not '{arg.Type}'.", tree.Function.line, tree.Function.col);
							Lexer.error_list.Add(Err);
							return new Result(null, TokenType.Error);        

						}
						if (arg.Resultado is NumberExpression number)
						{
							double res = 0;
							double num = double.Parse(number.Number.dato); 
							if (tree.Function.dato == "cos") res = Math.Cos(num);
							else if (tree.Function.dato == "sin") res = Math.Sin(num);
							else
							{
								if (num < 0)
								{
									Errors Err = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' must receive a positive argument.", tree.Function.line, tree.Function.col);
									Lexer.error_list.Add(Err);
									return new Result(null, TokenType.Error);
								}
								res = Math.Sqrt(num);
							}
							if (res > 0 && res < 0.0000001 || (res < 0 && res > 0.0000001)) res = 0;
							else if (res - Math.Truncate(res) > 0.99999999) res = Math.Truncate(res) + 1;
							Token resultToken= new Token(res.ToString(),0,0,TokenType.NumberToken);
							return new Result(new NumberExpression(resultToken), TokenType.NumberExpression);
						}
						
						Errors Error = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives 'NumberExpression' not '{arg.Type}'.", tree.Function.line, tree.Function.col);
						Lexer.error_list.Add(Error);
						return new Result(null, TokenType.Error);
					}
				case "pow":
				case "log":
					{

						if (tree.Child.Count != 2)
						{
							Errors Error = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives 2 arguments, but {tree.Child.Count} were given.", tree.Function.line, tree.Function.col);
							Lexer.error_list.Add(Error);
							return new Result(null, TokenType.Error);
						}
						var arg1 = Evaluator(tree.Child[0]);
						var arg2 = Evaluator(tree.Child[1]);
						if(arg1==null || arg2==null )      return new Result(null,TokenType.Error);
						if(arg1.Resultado is NumberExpression num && arg2.Resultado is NumberExpression num2)
						{
							double firstArgument = double.Parse(num.Number.dato);
							double secondArgument = double.Parse(num2.Number.dato);
							double res = 0;
							if (tree.Function.dato == "pow")
							{
								res = Math.Pow(firstArgument, secondArgument);
								Token resultToken0= new Token(res.ToString(),0,0,TokenType.NumberToken);
								return new Result(new NumberExpression(resultToken0), TokenType.NumberExpression);
							}

							if (firstArgument <= 0 || secondArgument <= 0)
							{
								Errors Error = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' must receives positive arguments.", tree.Function.line, tree.Function.col);
								Lexer.error_list.Add(Error);
								return new Result(null, TokenType.Error);
							}
							if (secondArgument == 1)
							{
								Errors Error = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' cannot receive a base equal 1.", tree.Function.line, tree.Function.col);
								Lexer.error_list.Add(Error);
								return new Result(null, TokenType.Error);
							}

							res = Math.Log(firstArgument, secondArgument);
							Token resultToken= new Token(res.ToString(),0,0,TokenType.NumberToken);
							return new Result(new NumberExpression(resultToken), TokenType.NumberExpression);                                
						}
						Errors Err = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives 'NumberExpression'.", tree.Function.line, tree.Function.col);
						Lexer.error_list.Add(Err);
						return new Result(null, TokenType.Error);

						
					}

				case "if":
					{
						var conditon = Evaluator(tree.Child[0]); if(conditon==null) return new Result(null,TokenType.Error);
						if(conditon.Resultado is NumberExpression number)
						{
							var num=double.Parse(number.Number.dato);
							if(num==0)  return Evaluator(tree.Child[2]);
							return Evaluator(tree.Child[1]);    
						}
						if(conditon.Resultado is Sequence sequence)
						{
							if(sequence.sequence.Count==0)   return Evaluator(tree.Child[2]); 
							return Evaluator(tree.Child[1]);
						}
						if(conditon.Resultado is Undefined)
							return Evaluator(tree.Child[2]);
						if (conditon.Resultado is BoolExpression cond)
						{
							bool aux = bool.Parse(cond.Nodo.dato);
							if (aux) return Evaluator(tree.Child[1]);
							else return Evaluator(tree.Child[2]);
						}
						return  Evaluator(tree.Child[1]);
					}

				
				case "samples":
					{
						if (tree.Child.Count != 1)
						{
							Errors Error = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives 1 argument, but {tree.Child.Count} were given.", tree.Function.line, tree.Function.col);
							Lexer.error_list.Add(Error);
							return new Result(null, TokenType.Error);
						}

						var n = Evaluator(tree.Child[0]).Resultado;
						if (n is not NumberExpression number)
						{
							Errors Error = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives a number argument, {tree.Child[0]} is not a number.", tree.Function.line, tree.Function.col);
							Lexer.error_list.Add(Error);
							return new Result(null, TokenType.Error);
						}
						
						
						List<Expressions> points = new List<Expressions>();

						for (int i = 0; i < int.Parse(number.Number.dato); i++)
						{
							PointExpression point = Parser.GeneratePoint();
							points.Add(point);
						}
						Sequence sequence = new Sequence(points);

						return new Result(sequence, TokenType.Sequence);
						
					}
				case "points":
					{
						if (tree.Child.Count != 2)
						{
							Errors Error = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives 2 arguments, but {tree.Child.Count} were given.", tree.Function.line, tree.Function.col);
							Lexer.error_list.Add(Error);
							return new Result(null, TokenType.Error);
						}

						var figure = Evaluator(tree.Child[0]).Resultado;
						if (figure is not Figures fig)
						{
							Errors Error = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives a figure as the first argument, {tree.Child[0]} is not a figure.", tree.Function.line, tree.Function.col);
							Lexer.error_list.Add(Error);
							return new Result(null, TokenType.Error);
						}
						else if (tree.Child[1] is not NumberExpression num)
						{
							Errors Error = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives a number as the second argument, {tree.Child[1]} is not a number.", tree.Function.line, tree.Function.col);
							Lexer.error_list.Add(Error);
							return new Result(null, TokenType.Error);
						}
						else
						{
							List<Expressions> points = GeneratePointsInLine(fig, int.Parse(num.Number.dato));
							Sequence sequence = new Sequence(points);

							return new Result(sequence, TokenType.Sequence);
						}
					}
				case "intersect":
					if (tree.Child.Count != 2)
					{
						Errors Error = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives 2 arguments, but {tree.Child.Count} were given.", tree.Function.line, tree.Function.col);
						Lexer.error_list.Add(Error);
						return new Result(null, TokenType.Error);
					}

					var figure1 = Evaluator(tree.Child[0]).Resultado;
					var figure2 = Evaluator(tree.Child[1]).Resultado;

					if (figure1 is not Figures fig1)
					{
						Errors Error = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives a figure as the first argument, {tree.Child[0]} is not a figure.", tree.Function.line, tree.Function.col);
						Lexer.error_list.Add(Error);
						return new Result(null, TokenType.Error);
					}
					else if (figure2 is not Figures fig2)
					{
						Errors Error = new Errors($"! SEMANTIC ERROR: Function '{tree.Function.dato}' receives a number as the second argument, {tree.Child[1]} is not a number.", tree.Function.line, tree.Function.col);
						Lexer.error_list.Add(Error);
						return new Result(null, TokenType.Error);
					}
					else
					{
						List<Expressions> points = GetIntersect(fig1, fig2);
						Sequence sequence = new Sequence(points);

						return new Result(sequence, TokenType.Sequence);
					}

				default:
					return new Result(null, TokenType.Error);
			}
		}

		public static bool belongsToLine(LineExpression line, PointExpression point)
		{
			double abscisa = (double)point.GetX();
			double ordenada = (double)point.GetY();

			var start = ((PointExpression)(line.point1)).GetX();
			var start2 = ((PointExpression)(line.point1)).GetY();
			var end = ((PointExpression)(line.point2)).GetX();
			var end2 = ((PointExpression)(line.point2)).GetY();

			if (start == end)
			{
				if (abscisa != start)
				{
					return false;
				}
			}

			if (line is SegmentExpression segment)
			{
				if (start <= end)
				{
					if (abscisa > end || abscisa < start)
					{
						return false;
					}
				}
				else
				{
					if (abscisa > start || abscisa < end)
					{
						return false;
					}
				}
				if (start2 <= end2)
				{
					if (ordenada > end2 || ordenada < start2)
					{
						return false;
					}
				}
				else
				{
					if (ordenada > start2 || ordenada < end2)
					{
						return false;
					}
				}
			}
			if (line is RayExpression ray)
			{
				if (start <= end)
				{
					if (abscisa < start)
					{
						return false;
					}
				}
				else
				{
					if (abscisa > start)
					{
						return false;
					}
				}
				if (start2 <= end2)
				{
					if (ordenada < start2)
					{
						return false;
					}
				}
				else
				{
					if (ordenada > start2)
					{
						return false;
					}
				}
			}
			return true;
		}

		private static List<Expressions> IntersectLines(LineExpression line, LineExpression line2)
		{
			List<Expressions> points = new List<Expressions>();

			RectEcuation ecuation = line.GetRectEcuation();

			var start = ((PointExpression)(line.point1)).GetX();
			var end = ((PointExpression)(line.point2)).GetX();

			double pendiente = ecuation.pendiente;
			double traza = ecuation.traza;


			RectEcuation ecuation2 = line2.GetRectEcuation();

			var start2 = ((PointExpression)(line2.point1)).GetX();
			var end2 = ((PointExpression)(line2.point2)).GetX();

			double pendiente2 = ecuation2.pendiente;
			double traza2 = ecuation2.traza;


			if (pendiente == pendiente2)
			{
				return points;
			}
			
			double abscisa = 0;
			double ordenada = 0;

			if (line.Vertical())
			{
				abscisa = (double)start;
				ordenada = (double)((pendiente2 * abscisa) + traza2);
			}
			else if (line2.Vertical())
			{
				abscisa = (double)start2;
				ordenada = (double)((pendiente * abscisa) + traza);

			}
			else
			{
				abscisa = (traza2 - traza) / (pendiente - pendiente2);
				ordenada = (pendiente * abscisa) + traza;

			}

			NumberExpression X = new NumberExpression(new Token(abscisa.ToString(), 0, 0, TokenType.NumberToken));
			NumberExpression Y = new NumberExpression(new Token(ordenada.ToString(), 0, 0, TokenType.NumberToken));
			PointExpression point = new PointExpression(X, Y, new IdentifierExpression(new Token("", 0, 0, TokenType.Point)), System.Drawing.Color.Black);
			if (!belongsToLine(line, point) || !belongsToLine(line2, point))
			{
				return points;
			}
			points.Add(point);
			return points;
		}

		private static List<Expressions> IntersectLineCircle(LineExpression line, CircunferenceExpression circle3)
		{
			List<Expressions> points = new List<Expressions>();

			RectEcuation ecuation = line.GetRectEcuation();

			var start = ((PointExpression)(line.point1)).GetX();
			var end = ((PointExpression)(line.point2)).GetX();
			double pendiente = ecuation.pendiente;
			double traza = ecuation.traza;

			var x = ((PointExpression)(circle3.center)).GetX();
			var y = ((PointExpression)(circle3.center)).GetY();
			var ratioAux = Execute.Evaluator(circle3.ratio).Resultado;

			if (ratioAux is not NumberExpression n)
			{ 
				Errors Error = new Errors($"! SEMANTIC ERROR: Unexpected expression for ratio parameter, Number token expected.", 0, 0);
				Lexer.error_list.Add(Error);
				return null;
			}
			else
			{

				double ratio = double.Parse(n.Number.dato);

				double x1 = 0;
				double x2 = 0;
				double y1 = 0;
				double y2 = 0;


				if (line.Vertical())
				{
					if (start < x - ratio || start > x + ratio)
					{
						return points;
					}
					x1 = (double)start;
					x2 = (double)start;
					y1 = (double)(Math.Sqrt((double)((ratio * ratio) - ((start - x) * (start - x)))) + y);
					y2 = (double)(-Math.Sqrt((double)((ratio * ratio) - ((start - x) * (start - x)))) + y);

					NumberExpression X1 = new NumberExpression(new Token(x1.ToString(), 0, 0, TokenType.NumberToken));
					NumberExpression Y1 = new NumberExpression(new Token(y1.ToString(), 0, 0, TokenType.NumberToken));
					NumberExpression X2 = new NumberExpression(new Token(x2.ToString(), 0, 0, TokenType.NumberToken));
					NumberExpression Y2 = new NumberExpression(new Token(y2.ToString(), 0, 0, TokenType.NumberToken));

					bool aux1 = true;
					bool aux2 = true;
					if (circle3 is ArcExpression arc3)
					{
						if (aux1)
						{
							aux1 = belongsToArc(arc3, x1, y1);
						}
						if (aux2)
						{
							aux2 = belongsToArc(arc3, x2, y2);
						}
					}

					PointExpression point1 = new PointExpression(X1, Y1, new IdentifierExpression(new Token("", 0, 0, TokenType.Point)), System.Drawing.Color.Black);
					if (belongsToLine(line, point1))
					{

						if (aux1)
						{
							points.Add(point1);

						}
					}
					PointExpression point2 = new PointExpression(X2, Y2, new IdentifierExpression(new Token("", 0, 0, TokenType.Point)), System.Drawing.Color.Black);
					if (belongsToLine(line, point2))
					{

						if (aux2)
						{
							points.Add(point2);

						}
					}
					return points;
				}


				double A = 1 + pendiente * pendiente;
				double B = (double)((-2 * x) + (2 * pendiente * (traza - y)));
				double C = (double)((x * x) + ((traza - y) * (traza - y)) - (ratio * ratio));

				double discriminant = (B * B) - (4 * A * C);
				if (discriminant < 0)
				{
					return points;
				}
				else
				{


					x1 = (-B + Math.Sqrt(discriminant)) / (2 * A);
					x2 = (-B - Math.Sqrt(discriminant)) / (2 * A);
					y1 = pendiente * x1 + traza;
					y2 = pendiente * x2 + traza;

					NumberExpression X1 = new NumberExpression(new Token(x1.ToString(), 0, 0, TokenType.NumberToken));
					NumberExpression Y1 = new NumberExpression(new Token(y1.ToString(), 0, 0, TokenType.NumberToken));
					NumberExpression X2 = new NumberExpression(new Token(x2.ToString(), 0, 0, TokenType.NumberToken));
					NumberExpression Y2 = new NumberExpression(new Token(y2.ToString(), 0, 0, TokenType.NumberToken));

					bool aux1 = true;
					bool aux2 = true;
					if (circle3 is ArcExpression arc3)
					{
						if (aux1)
						{
							aux1 = belongsToArc(arc3, x1, y1);
						}
						if (aux2)
						{
							aux2 = belongsToArc(arc3, x2, y2);
						}
					}
					PointExpression point1 = new PointExpression(X1, Y1, new IdentifierExpression(new Token("", 0, 0, TokenType.Point)), System.Drawing.Color.Black);
					PointExpression point2 = new PointExpression(X2, Y2, new IdentifierExpression(new Token("", 0, 0, TokenType.Point)), System.Drawing.Color.Black);

					if (belongsToLine(line, point1))
					{
						if (aux1)
						{
							points.Add(point1);
						}
					}

					if (discriminant == 0) return points;   

					if (belongsToLine(line, point2))
					{
						if (aux2)
						{
							points.Add(point2);
						}
					}

					return points;
				}
			}
		}

		private static List<Expressions> IntersectCircles(CircunferenceExpression circle, CircunferenceExpression circle2)
		{
			List<Expressions> points = new List<Expressions>();

			var x = ((PointExpression)(circle.center)).GetX();
			var y = ((PointExpression)(circle.center)).GetY();
			var ratioAux = Execute.Evaluator(circle.ratio).Resultado;

			if (ratioAux is not NumberExpression n)
			{   
				Errors Error = new Errors($"! SEMANTIC ERROR: Unexpected expression for ratio parameter, Number token expected.", 0, 0);
				Lexer.error_list.Add(Error);
				return null;
			}

			double ratio = double.Parse(n.Number.dato);

			var _x = ((PointExpression)(circle2.center)).GetX();
			var _y = ((PointExpression)(circle2.center)).GetY();
			var _ratioAux = Execute.Evaluator(circle2.ratio).Resultado;

			if (_ratioAux is not NumberExpression _r)
			{ 
				Errors Error = new Errors($"! SEMANTIC ERROR: Unexpected expression for ratio parameter, Number token expected.", 0, 0);
				Lexer.error_list.Add(Error);
				return null;
			}

			double _ratio = double.Parse(_r.Number.dato);

			double distance = Math.Sqrt(Math.Pow((double)(_x - x), 2) + Math.Pow((double)(_y - y), 2));
			if (distance > ratio + _ratio || distance < Math.Abs(ratio - _ratio))
			{
				return points;
			}

			double a = (Math.Pow(ratio, 2) - Math.Pow(_ratio, 2) + Math.Pow(distance, 2)) / (2 * distance);
			double h = Math.Sqrt(Math.Pow(ratio, 2) - Math.Pow(a, 2));

			double x3 = (double)(x + a * (_x - x) / distance + h * (_y - y) / distance);
			double y3 = (double)(y + a * (_y - y) / distance - h * (_x - x) / distance);
			double x4 = (double)(x + a * (_x - x) / distance - h * (_y - y) / distance);
			double y4 = (double)(y + a * (_y - y) / distance + h * (_x - x) / distance);

			NumberExpression X1 = new NumberExpression(new Token(x3.ToString(), 0, 0, TokenType.NumberToken));
			NumberExpression Y1 = new NumberExpression(new Token(y3.ToString(), 0, 0, TokenType.NumberToken));
			NumberExpression X2 = new NumberExpression(new Token(x4.ToString(), 0, 0, TokenType.NumberToken));
			NumberExpression Y2 = new NumberExpression(new Token(y4.ToString(), 0, 0, TokenType.NumberToken));


			bool aux1 = true;
			bool aux2 = true;
			if (circle is ArcExpression arc)
			{
				if(aux1)
				{
					aux1 = belongsToArc(arc, x3, y3);
				}
				if (aux2)
				{
					aux2 = belongsToArc(arc, x4, y4);
				}
			}

			if (circle2 is ArcExpression arc2)
			{
				if (aux1)
				{
					aux1 = belongsToArc(arc2, x3, y3);
				}
				if (aux2)
				{
					aux2 = belongsToArc(arc2, x4, y4);
				}
			}

			if (aux1)
			{
				PointExpression pointAux1 = new PointExpression(X1, Y1, new IdentifierExpression(new Token("", 0, 0, TokenType.Point)), System.Drawing.Color.Black);
				points.Add(pointAux1);
			}
			if (aux2)
			{
				PointExpression pointAux2 = new PointExpression(X2, Y2, new IdentifierExpression(new Token("", 0, 0, TokenType.Point)), System.Drawing.Color.Black);
				points.Add(pointAux2);
			}
			return points;
		}

		private static bool belongsToArc(ArcExpression arc, double x, double y)
		{
			var definitionAngles = GetAngles(arc.point1, arc.point2, arc.center, arc.ratio);
			NumberExpression X = new NumberExpression(new Token(x.ToString(), 0, 0, TokenType.NumberToken));
			NumberExpression Y = new NumberExpression(new Token(y.ToString(), 0, 0, TokenType.NumberToken));
			var anglesIntersect1 = GetAngles(arc.point1,  new PointExpression(X, Y, new IdentifierExpression(new Token("", 0, 0, TokenType.Point)), System.Drawing.Color.Black), arc.center, arc.ratio);

			if (definitionAngles[0] <= definitionAngles[1])
			{
				return (definitionAngles[1] >= anglesIntersect1[1]) && (anglesIntersect1[1] >= definitionAngles[0]);
			}

			return ((definitionAngles[1] >= anglesIntersect1[1]) || (anglesIntersect1[1] >= definitionAngles[0]));

		}
		public static List<Expressions> GetIntersect(Figures fig1, Figures fig2)
		{
			List<Expressions> points = new List<Expressions>();

			if (fig1 is PointExpression p)
			{
				if(fig2 is LineExpression l1)
				{
					return GetIntersect(l1, p);
				}
				if (fig2 is CircunferenceExpression c1)
				{
					return GetIntersect(c1, p);
				}
				if(fig2 is PointExpression p2)
				{
					double x1 = (double)p.GetX();
					double y1 = (double)p.GetY();

					double x2 = (double)p2.GetX();
					double y2 = (double)p2.GetY();
					if(x1 == x2 && y1 == y2)
					{
						points.Add(p);
						return points;
					}
					return points;
				}
			}
				
			if (fig1 is LineExpression line)
			{
				RectEcuation ecuation = line.GetRectEcuation();

				var start = ((PointExpression)(line.point1)).GetX();
				var end = ((PointExpression)(line.point2)).GetX();

				double pendiente = ecuation.pendiente;
				double traza = ecuation.traza;

				if (fig2 is PointExpression point4)
				{
					double x = (double)point4.GetX();
					double y = (double)point4.GetY();

					if (line.Vertical())
					{
						if (x != start)
						{
							return points;
						}
						points.Add(point4);
						return points;
					}

					if (y != (x*pendiente + traza))
					{
						return points;
					}
					points.Add(point4);
					return points;
				}

				if (fig2 is LineExpression line2)
				{
					return IntersectLines(line, line2);
				}

				// Figure2 es una circunferencia   
				return IntersectLineCircle(line, (CircunferenceExpression)(fig2));
		 
			}

			if (fig1 is CircunferenceExpression circle)
			{
				var x = ((PointExpression)(circle.center)).GetX();
				var y = ((PointExpression)(circle.center)).GetY();
				var ratioAux = Execute.Evaluator(circle.ratio).Resultado;
				
				if (ratioAux is not NumberExpression n)
				{
				    Errors Error = new Errors($"! SEMANTIC ERROR: Unexpected expression for ratio parameter, Number token expected.", 0, 0);
					Lexer.error_list.Add(Error);
					return null;
				}
				
				double ratio = double.Parse(n.Number.dato);

				if (fig2 is PointExpression point)
				{
					double x4 = (double)point.GetX();
					double y4 = (double)point.GetY();

					if ((ratio*ratio) != (x4*x4 + y4*y4))
					{
						return points;
					}
					if(circle is ArcExpression arcExp)
					{
						if (!belongsToArc(arcExp, x4, y4))
						{
							return points;
						}
					}
					points.Add(point);
					return points;
				}
				if (fig2 is CircunferenceExpression circle2)
				{
					return IntersectCircles(circle, circle2);
				}
				if (fig2 is LineExpression line3)
				{
					return GetIntersect(line3, circle);
				}

					return null;
			}

			return null;
		}
		
		public static double[] GetAngles(Expressions point1, Expressions point2, Expressions center, Expressions measure)
		{
			if (!(point1 is PointExpression p1 && point2 is PointExpression p2 && center is PointExpression c && measure is NumberExpression m))
			{
				return null; //Lanzar error aqui
			}
			else
			{
				var x_c = (float)c.GetX();
				var y_c = (float)-c.GetY();

				var x_1 = (float)p1.GetX();
				var y_1 = (float)-p1.GetY();

				var x_2 = (float)p2.GetX();
				var y_2 = (float)-p2.GetY();

    			double ratio = double.Parse(m.Number.dato);

                double startAngle = (double)Math.Atan2(y_1 - y_c, x_1 - x_c);
				double finalAngle = (double)Math.Atan2(y_2 - y_c, x_2 - x_c);


                if (startAngle < 0 && startAngle > -Math.PI)
				{
					startAngle = -startAngle;
				}
                else
                {
                    if(startAngle > 0)
					{ 
                        startAngle = 2 * Math.PI - startAngle;
                    }
                }
				if (finalAngle < 0 && finalAngle > -Math.PI)
				{
					finalAngle = -finalAngle;
				}
				else
				{
                    if (finalAngle > 0)
                    {
                        finalAngle = 2 * Math.PI - finalAngle;
                    }
                }
				
				return new double[] { startAngle, finalAngle };
			}
		}
		public static  List<Expressions> GeneratePointsInLine(Expressions fig, int num)
		{
			if (fig is LineExpression line && fig is not SegmentExpression && fig is not RayExpression)
			{
				List<Expressions> points = new List<Expressions>();
				RectEcuation ecuation = line.GetRectEcuation();

				double pendiente = ecuation.pendiente;
				double traza = ecuation.traza;

				for (int i = 0; i < num; i++)
				{
					Random random1 = new Random(Guid.NewGuid().GetHashCode());
					double abscisa = random1.NextDouble() * 780 - 390;
					double imagen = pendiente * abscisa + traza;

					NumberExpression X = new NumberExpression(new Token(abscisa.ToString(), 0, 0, TokenType.NumberToken));
					NumberExpression Y = new NumberExpression(new Token(imagen.ToString(), 0, 0, TokenType.NumberToken));
					PointExpression point = new PointExpression(X, Y, new IdentifierExpression(new Token("", 0, 0, TokenType.Point)), System.Drawing.Color.Black);
					points.Add(point);
				}
				return points;
			}
			else if (fig is SegmentExpression segment)
			{
				List<Expressions> points = new List<Expressions>();
				RectEcuation ecuation = segment.GetRectEcuation();
				var x1 = ((PointExpression)(segment.point1)).GetX();
				var x2 = ((PointExpression)(segment.point2)).GetX();
				var y1 = ((PointExpression)(segment.point1)).GetY();
				var y2 = ((PointExpression)(segment.point2)).GetY();

				double pendiente = ecuation.pendiente;
				double traza = ecuation.traza;

				if (segment.Vertical())
				{
					for (int i = 0; i<num; i++)
					{
						Random random1 = new Random(Guid.NewGuid().GetHashCode());
						double imagen = 0;
						if(y1<y2)
						{
							imagen = (double)(random1.NextDouble()*(240-y1) + y1);
							
						}
						else
						{
							imagen = (double)(random1.NextDouble()*(y1+240) - 240);
							
						}
						NumberExpression X = new NumberExpression(new Token(x1.ToString(), 0, 0, TokenType.NumberToken));
						NumberExpression Y = new NumberExpression(new Token(imagen.ToString(), 0, 0, TokenType.NumberToken));
						PointExpression point = new PointExpression(X, Y, new IdentifierExpression(new Token("", 0, 0, TokenType.Point)), System.Drawing.Color.Black);
						points.Add(point);
					}
					return points;
				}

				for (int i = 0; i < num; i++)
				{
					double abscisa = 0;
					Random random1 = new Random(Guid.NewGuid().GetHashCode());
					if (x1 < x2)
					{
						abscisa = (double)(random1.NextDouble() * (x2 - x1) + x1);
					}
					else
					{
						abscisa = (double)(random1.NextDouble() * (x1 - x2) + x2);
					}
					double imagen = pendiente * abscisa + traza;

					NumberExpression X = new NumberExpression(new Token(abscisa.ToString(), 0, 0, TokenType.NumberToken));
					NumberExpression Y = new NumberExpression(new Token(imagen.ToString(), 0, 0, TokenType.NumberToken));
					PointExpression point = new PointExpression(X, Y, new IdentifierExpression(new Token("", 0, 0, TokenType.Point)), System.Drawing.Color.Black);
					points.Add(point);
				}
				return points;
			}
			else if (fig is RayExpression ray)
			{
				List<Expressions> points = new List<Expressions>();

				var x1 = ((PointExpression)(ray.point1)).GetX();
				var x2 = ((PointExpression)(ray.point2)).GetX();
				var y1 = ((PointExpression)(ray.point1)).GetY();
				var y2 = ((PointExpression)(ray.point2)).GetY();
				RectEcuation ecuation = ray.GetRectEcuation();

				double pendiente = ecuation.pendiente;
				double traza = ecuation.traza;

				if (ray.Vertical())
				{
					for (int i = 0; i<num; i++)
					{
						Random random1 = new Random(Guid.NewGuid().GetHashCode());
						double imagen = 0;
						if(y1<y2)
						{
							imagen = (double)(random1.NextDouble()*(240-y1) + y1);
							
						}
						else
						{
							imagen = (double)(random1.NextDouble()*(y1+240) - 240);
							
						}
						NumberExpression X = new NumberExpression(new Token(x1.ToString(), 0, 0, TokenType.NumberToken));
						NumberExpression Y = new NumberExpression(new Token(imagen.ToString(), 0, 0, TokenType.NumberToken));
						PointExpression point = new PointExpression(X, Y, new IdentifierExpression(new Token("", 0, 0, TokenType.Point)), System.Drawing.Color.Black);
						points.Add(point);
					}
					return points;
				}

				for (int i = 0; i < num; i++)
				{
					Random random1 = new Random(Guid.NewGuid().GetHashCode());
					double abscisa = 0;
					if (x1 < x2)
					{
						abscisa = (double)(random1.NextDouble() * (390 - x1) + x1);
					}
					else
					{
						abscisa = (double)(random1.NextDouble() * (x1 + 390) - 390);
					}
					double imagen = pendiente * abscisa + traza;

					NumberExpression X = new NumberExpression(new Token(abscisa.ToString(), 0, 0, TokenType.NumberToken));
					NumberExpression Y = new NumberExpression(new Token(imagen.ToString(), 0, 0, TokenType.NumberToken));
					PointExpression point = new PointExpression(X, Y, new IdentifierExpression(new Token("", 0, 0, TokenType.Point)), System.Drawing.Color.Black);
					points.Add(point);
				}
				return points;
			}
			else if (fig is CircunferenceExpression circle && fig is not ArcExpression)
			{
				List<Expressions> points = new List<Expressions>();
				CircleEcuation ecuation = circle.GetCircleEcuation();
				double x = ecuation.x;
				double y = ecuation.y;
				double r = ecuation.radio;

				for (int i = 0; i < num; i++)
				{
					
					var random = new Random(Guid.NewGuid().GetHashCode());
					double angle = (double)(random.NextDouble() * 2 * Math.PI);

					double _x = (double)(Math.Cos(angle) * r + x);
					double _y = (double)(Math.Sin(angle) * r + y);


					PointExpression point = new PointExpression(new NumberExpression(new Token(_x.ToString(), 0, 0, TokenType.NumberToken)),
						new NumberExpression(new Token(_y.ToString(), 0, 0, TokenType.NumberToken)), new IdentifierExpression(new Token("", 0, 0, TokenType.Point)), System.Drawing.Color.Black);
					points.Add(point);
				}
				return points;
			}
			else if (fig is ArcExpression arc)
			{
				List<Expressions> points = new List<Expressions>();

				var x_c = ((PointExpression)(arc.center)).GetX();
				var y_c = -((PointExpression)(arc.center)).GetY();
				var ratio = Execute.Evaluator(arc.ratio).Resultado;

				var x_1 = ((PointExpression)(arc.point1)).GetX();
				var y_1 = -((PointExpression)(arc.point1)).GetY();
				var x_2 = ((PointExpression)(arc.point2)).GetX();
				var y_2 = -((PointExpression)(arc.point2)).GetY();


				double startAngle = (double)Math.Atan2((double)(y_1 - y_c),(double)( x_1 - x_c));
				double finalAngle = (double)Math.Atan2((double)(y_2 - y_c), (double)(x_2 - x_c));

				if (startAngle < 0 && startAngle > -Math.PI)
				{
					startAngle = -startAngle;
				}
				else
				{
					if (startAngle > 0)
					{
						startAngle = 2 * Math.PI - startAngle;
					}
				}
				if (finalAngle < 0 && finalAngle > -Math.PI)
				{
					finalAngle = -finalAngle;
				}
				else
				{
					if (finalAngle > 0)
					{
						finalAngle = 2 * Math.PI - finalAngle;
					}
				}

				if (ratio is not NumberExpression n)
				{ 
					Errors Error = new Errors($"! SEMANTIC ERROR: Unexpected expression for ratio parameter, Number token expected.", 0, 0);
					Lexer.error_list.Add(Error);
					return null;
				}
				else
				{
					double r = double.Parse(n.Number.dato);
					for (int i = 0; i < num; i++)
					{
						var random = new Random(Guid.NewGuid().GetHashCode());
						double angle = 0;
						if(finalAngle >= startAngle)
						{
							angle = (double)((random.NextDouble() * (finalAngle - startAngle)) + startAngle);
						}
						else
						{
							angle = (double)(random.NextDouble() * ((finalAngle + (2 * Math.PI)) - startAngle) + startAngle);
							if (angle >= 2*Math.PI)
							{
								angle -= 2*Math.PI;
							}

						}

						double _x = (double)((Math.Cos(angle) * r) + x_c);
						double _y = (double)((Math.Sin(angle) * r) - y_c);

						PointExpression point = new PointExpression(new NumberExpression(new Token(_x.ToString(), 0, 0, TokenType.NumberToken)),
							new NumberExpression(new Token(_y.ToString(), 0, 0, TokenType.NumberToken)), new IdentifierExpression(new Token("", 0, 0, TokenType.Point)), System.Drawing.Color.Black);
						points.Add(point);
					}
				}
				return points;
			}
			return null;
		}

	}
