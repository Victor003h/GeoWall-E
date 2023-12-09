using System;
using System.Collections.Generic;
using System.IO;

namespace Godot;

	public abstract class Figures : Expressions
	{
		public virtual IdentifierExpression Id { get; set; }
		public string text = "";
		
		private new TokenType type = TokenType.Figure;
		public override TokenType Type { get => type; }
	
	}


	public class PointExpression : Figures
	{
		public Expressions x { get; set; }
		public Expressions y { get; set; }
		public  System.Drawing.Color color {get;set;}
		
		public PointExpression(Expressions x, Expressions y, IdentifierExpression id,System.Drawing.Color color)
		{
			this.y = y;
			this.x = x;
			this.Id = id;
			this.color=color;
		}

		public double?  GetX()
		{
			var x1= Execute.Evaluator(this.x);
			if(x1==null)    return null;
			if(x1.Resultado is NumberExpression X)
			{
				return double.Parse(X.Number.dato);
			}
			Errors Err = new Errors($"! SEMANTIC ERROR: Cannot convert from '{x1.Resultado}' to 'NumberExpression'.", 0, 0);
			Lexer.error_list.Add(Err);
			return null;
		}
		public double?  GetY()
		{
			var x1= Execute.Evaluator(this.y);
			if(x1==null)    return null;
			if(x1.Resultado is NumberExpression X)
			{
				return double.Parse(X.Number.dato);
			}
			Errors Err = new Errors($"! SEMANTIC ERROR: Cannot convert from '{x1.Resultado}' to 'NumberExpression'.", 0, 0);
			Lexer.error_list.Add(Err);
			return null;
		}
	}
	public class LineExpression : Figures
	{
		public Expressions point1 { get; set; }
		public Expressions point2 { get; set; }
		public  System.Drawing.Color color {get; set ;}
		
		public LineExpression(Expressions point1, Expressions point2,System.Drawing.Color color)
		{
			this.point1 = point1;
			this.point2 = point2;
			this.color=color;
		}
		public bool Vertical()
		{
			double x1 = (double)(((PointExpression)(point1)).GetX());
			double x2 = (double)(((PointExpression)(point2)).GetX());

			if (x1==x2)
			{
				return true;
			}
			return false;
		}
		public RectEcuation GetRectEcuation()
		{
			var x1 = ((PointExpression)(this.point1)).GetX();
			var y1 = ((PointExpression)(this.point1)).GetY();
			var x2 = ((PointExpression)(this.point2)).GetX();
			var y2 = ((PointExpression)(this.point2)).GetY();
			double pendiente = 0;
			double traza = 0;

			if (x1 == x2)
			{
				pendiente = int.MaxValue;
				traza = int.MaxValue;
				return new RectEcuation(pendiente, traza);
			}

			pendiente = (double)((y2 - y1) / (x2 - x1));
			traza = (double)(y1 - pendiente * x1);
			return new RectEcuation(pendiente, traza);
		}


	}
	public class SegmentExpression : LineExpression
	{
		public SegmentExpression(Expressions point1, Expressions point2, System.Drawing.Color color) : base(point1, point2, color)
		{
		}       
	}

	public class RayExpression : LineExpression
	{
		public RayExpression(Expressions point1, Expressions point2, System.Drawing.Color color) : base(point1, point2, color)
		{
		}

	}
	public class RectEcuation
	{
		public double pendiente;
		public double traza;

		public RectEcuation(double pendiente, double traza)
		{
			this.pendiente = pendiente;
			this.traza = traza;
		}
	}

	public class CircleEcuation
	{
		public double x;
		public double y;
		public double radio;


		public CircleEcuation(double x, double y, double radio)
		{
			this.x = x;
			this.y = y;
			this.radio = radio;
		}
	}

	public class CircunferenceExpression : Figures
	{
		public Expressions center { get; set; }
		public Expressions ratio { get; set; }
		public  System.Drawing.Color color {get;set;}

		public CircunferenceExpression(Expressions point, Expressions ratio, System.Drawing.Color color)
		{
			this.center = point;
			this.ratio = ratio;
			this.color=color;

		}

		public CircleEcuation GetCircleEcuation()
		{
			var x = (double)((PointExpression)(this.center)).GetX();
			var y = (double)((PointExpression)(this.center)).GetY();
			var ratio = Execute.Evaluator(this.ratio).Resultado;

			if (ratio is not NumberExpression n)
			{  
				Errors Error = new Errors($"! SEMANTIC ERROR: Unexpected expression for ratio parameter, Number token expected.",0, 0);
				Lexer.error_list.Add(Error);
				return null;
			}
			else
			{
				double r = double.Parse(n.Number.dato);

				return new CircleEcuation(x, y, r);
			}
		}
	}


	public class ArcExpression : CircunferenceExpression
	{
		public Expressions point1 { get; set; }
		public Expressions point2 { get; set; }


		public ArcExpression(Expressions center, Expressions point1, Expressions point2, Expressions ratio, System.Drawing.Color color) : base(center, ratio, color)
		{
			this.point1 = point1;
			this.point2 = point2;
		
		}
	}
