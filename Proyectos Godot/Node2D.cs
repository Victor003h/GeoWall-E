using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public partial class Node2D : Godot.Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	void _draw()
	{
		List<Expressions> figures = control.figuresList;
		System.Console.WriteLine($"hay {figures.Count} figuras para pintar");
		foreach (var figure in figures)
		{

			//var figure = Execute.Evaluator(figure);
			if(control.CheckError())	return;	
				
			if (figure is PointExpression point)
			{
				var X=point.GetX();
				var Y=-point.GetY();
				System.Console.WriteLine($"x es {X} y es {Y} ");
				if(control.CheckError())	return;	
				DrawCircle(new Vector2((int)X,(int)Y), 2, GetColor(point.color));
				if (point.text != "")
                {
                    FontFile fontFile = new FontFile();
                    DrawString(fontFile, new Vector2((float)X, (float)Y), point.text, HorizontalAlignment.Left, -1, 14, Color.Color8(0,0,0));
                }

			}
			
			if (figure is CircunferenceExpression circle && figure is not ArcExpression)
			{
				
				if (circle.center is PointExpression auxpoint && circle.ratio is NumberExpression r )
				{
					var x = auxpoint.GetX();
					var y = -auxpoint.GetY();
					var n = r.Number.dato.ToFloat();
					if (x == null || y == null || n == null)
					{
						control.CheckError();
						System.Console.WriteLine("ss");
						return;
					}
					System.Console.WriteLine("pinto circulp");
					DrawArc(new Vector2((int)x, (int)y), n, 0, (float)(2 * Math.PI), 64, GetColor(circle.color));
					if (circle.text != "")
                    {
                        FontFile fontFile = new FontFile();
						PointExpression point1 = (PointExpression)(Execute.GeneratePointsInLine(circle, 1)[0]);
                        DrawString(fontFile, new Vector2((float)point1.GetX(), (float)point1.GetY()), circle.text, HorizontalAlignment.Left, -1, 14, Color.Color8(0, 0, 0));
                    }
				}
				else return;
			}
			
			if (figure is ArcExpression arc)
			{
				var center = Execute.Evaluator(arc.center);
				var point1 = Execute.Evaluator(arc.point1);
				var point2 = Execute.Evaluator(arc.point2);
				var measure = Execute.Evaluator(arc.ratio);

				if (center.Resultado is PointExpression c && point1.Resultado is PointExpression p1 && point2.Resultado is PointExpression p2 && measure.Resultado is NumberExpression m)
				{
					var x_c = (float)c.GetX();
					var y_c = (float)-c.GetY();

					var x_1 = (float)p1.GetX();
					var y_1 = (float)-p1.GetY();

					var x_2 = (float)p2.GetX();
					var y_2 = (float)-p2.GetY();

					var ratio = m.Number.dato.ToFloat();

					if (x_c == null || y_c == null || x_1 == null || y_1 == null || x_2 == null || y_2 == null || ratio == null) return;

                    Console.WriteLine(y_1);
                    Console.WriteLine(x_1);
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
                    
					if(startAngle < finalAngle)
					{
                        startAngle = 2 * Math.PI - startAngle;
                        finalAngle = 2 * Math.PI - finalAngle;

                        DrawArc(new Vector2(x_c, y_c), m.Number.dato.ToFloat(), (float)(startAngle), (float)(finalAngle), 64, GetColor(arc.color));
                        if (arc.text != "")
                        {
                            FontFile fontFile = new FontFile();
                            PointExpression point3 = (PointExpression)(Execute.GeneratePointsInLine(arc, 1)[0]);
                            DrawString(fontFile, new Vector2((float)point3.GetX(), (float)point3.GetY()), arc.text, HorizontalAlignment.Left, -1, 14, Color.Color8(0, 0, 0));
                        }
                    }
					else
					{
						startAngle = (-startAngle) + (2 * Math.PI);
						finalAngle = (-finalAngle) + (2 * Math.PI);
						DrawArc(new Vector2(x_c, y_c), m.Number.dato.ToFloat(), (float)(finalAngle), (float)(2*Math.PI), 64, GetColor(arc.color));
                        DrawArc(new Vector2(x_c, y_c), m.Number.dato.ToFloat(), (float)(0), (float)(startAngle), 64, GetColor(arc.color));
                        if (arc.text != "")
                        {
                            FontFile fontFile = new FontFile();
                            PointExpression point3 = (PointExpression)(Execute.GeneratePointsInLine(arc, 1)[0]);
                            DrawString(fontFile, new Vector2((float)point3.GetX(), (float)point3.GetY()), arc.text, HorizontalAlignment.Left, -1, 14, Color.Color8(0, 0, 0));
                        }
                    }
                }
				else return;
			}

			if (figure is LineExpression line && figure is not SegmentExpression && figure is not RayExpression)
			{
				var pt1=Execute.Evaluator(line.point1);
				var pt2=Execute.Evaluator(line.point2);
				if(pt1.Resultado is PointExpression p1 && pt2.Resultado is PointExpression p2)
				{
					var x1=p1.GetX();
					var y1=-p1.GetY();
					var x2=p2.GetX();
					var y2=-p2.GetY();

					if (x1 == x2)
					{
						if (x1 == null || y1 == null || x2 == null || y2 == null) return;
						
						DrawLine(new Vector2((float)x1, -900), new Vector2((float)x2, 900), GetColor(line.color));
						return;
					}
					double pendiente = (double)((y2 - y1) / (x2 - x1));
					double traza = (double)(y1 - pendiente * x1);

					var _x1 = -390;
					var _y1 = (float)(pendiente*_x1 + traza);
					var _x2 = 390;
					var _y2 = (float)(pendiente * _x2 + traza);

					if (x1==null || y1==null|| x2==null|| y2==null)	return;	
					DrawLine(new Vector2(_x1, _y1),new Vector2(_x2, _y2), GetColor(line.color));
					if (line.text != "")
                    {
                        FontFile fontFile = new FontFile();
                        PointExpression point3 = (PointExpression)(Execute.GeneratePointsInLine(line, 1)[0]);
                        DrawString(fontFile, new Vector2((float)point3.GetX(), (float)point3.GetY()), line.text, HorizontalAlignment.Left, -1, 14, Color.Color8(0, 0, 0));
                    }
	
				}
				else	return;			
			}
			
			if (figure is SegmentExpression segment)
			{
				var pt1 = Execute.Evaluator(segment.point1);
				var pt2 = Execute.Evaluator(segment.point2);
				if (pt1.Resultado is PointExpression p1 && pt2.Resultado is PointExpression p2)
				{
					var x1 = p1.GetX();
					var y1 =- p1.GetY();
					var x2 = p2.GetX();
					var y2 = -p2.GetY();

					if (x1 == null || y1 == null || x2 == null || y2 == null) return;
					DrawLine(new Vector2((float)x1, (float)y1), new Vector2((float)x2, (float)y2), GetColor(segment.color));
					if (segment.text != "")
                    {
                        FontFile fontFile = new FontFile();
                        PointExpression point3 = (PointExpression)(Execute.GeneratePointsInLine(segment, 1)[0]);
                        DrawString(fontFile, new Vector2((float)point3.GetX(), (float)point3.GetY()), segment.text, HorizontalAlignment.Left, -1, 14, Color.Color8(0, 0, 0));
                    }

				}
				else return;
			}

				
			if (figure is RayExpression ray)
			{
				var pt1 = Execute.Evaluator(ray.point1);
				var pt2 = Execute.Evaluator(ray.point2);
				if (pt1.Resultado is PointExpression p1 && pt2.Resultado is PointExpression p2)
				{
					var x1 = p1.GetX();
					var y1 =- p1.GetY();
					var x2 = p2.GetX();
					var y2 =- p2.GetY();
					if (x1 == x2)
					{
						if (x1 == null || y1 == null || x2 == null || y2 == null) return;
						DrawLine(new Vector2((float)x1, -240), new Vector2((float)x2, 240),GetColor(ray.color));
						return;
					}
					double pendiente = (double)((y2 - y1) / (x2 - x1));
					double traza = (double)(y1 - pendiente * x1);
					var _x1 = x1;
					var _y1 = y1;
					var _x2 = 0;
					if (x1 <= x2)
					{
						_x2 = 390;
					}
					else
					{
						_x2 = -390;
					}
					var _y2 = (float)(pendiente * _x2 + traza);

					if (x1 == null || y1 == null || x2 == null || y2 == null) return;
					
					DrawLine(new Vector2((float)_x1, (float)_y1), new Vector2(_x2, _y2), GetColor(ray.color));
					if (ray.text != "")
                    {
                        FontFile fontFile = new FontFile();
                        PointExpression point3 = (PointExpression)(Execute.GeneratePointsInLine(ray, 1)[0]);
                        DrawString(fontFile, new Vector2((float)point3.GetX(), (float)point3.GetY()), ray.text, HorizontalAlignment.Left, -1, 14, Color.Color8(0, 0, 0));
                    }

				}
				else return;
			}
		}
	}


	private Color GetColor(System.Drawing.Color color)
	{
		var r = color.R; var g = color.G; var b = color.B;
		return  Color.Color8(r, g, b);
	}



}


