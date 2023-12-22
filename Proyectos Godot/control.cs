using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class control : Control
{
	
	public static List<Expressions> figuresList = new List<Expressions>();
	public static List<DrawFunction> drawFunctions=new List<DrawFunction>();
	public static  Node2D node2D;
	public static RichTextLabel richTextLabel1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void _on_graph_edit_draw()
	{
		GraphEdit graph = GetNode<GraphEdit>("GraphEdit");

		base._Draw();
		var canvas = GetCanvasItem();
		Vector2 vector = new Vector2(0, 0);
		graph.DrawCircle(vector, 20, Color.Color8(2, 2, 2));
	}

	public void _on_button_2_pressed()
	{
		FileDialog file = GetNode<FileDialog>("OpenFileDialog");
		file.Popup();
	}

	public void _on_open_file_dialog_file_selected(string path)
	{
		string f = File.ReadAllText(path);
		CodeEdit codeEdit = GetNode<CodeEdit>("CodeEdit");
		codeEdit.Text = f;
	}

	public void _on_button_3_pressed()
	{
		FileDialog file = GetNode<FileDialog>("SaveFileDialog");
		file.Popup();
	}

	public void _on_button_4_pressed()
	{
		if(node2D==null)	return;
		figuresList.Clear();
		node2D.QueueRedraw();
	}

	public void _on_save_file_dialog_file_selected(string path)
	{

		CodeEdit codeEdit = GetNode<CodeEdit>("CodeEdit");
		FileDialog file = GetNode<FileDialog>("SaveFileDialog");
		File.WriteAllText(path, codeEdit.Text);
	}

	public void _on_button_pressed()
	{
		richTextLabel1 = GetNode<RichTextLabel>("RichTextLabel1");
		node2D=GetNode<Node2D>("Control2/ColorRect/Node2D");
		Default();
		CodeEdit codeEdit = GetNode<CodeEdit>("CodeEdit");
		string code = codeEdit.Text;

		if (string.IsNullOrEmpty(code)) return;
		Parser parser = new Parser(code);
		if (!CheckError())
		{
			while (parser.lineNumber < parser.lines.Count)
			{
				var tree = parser.Parse();
				if(CheckError())	return;
				 
				var resul = CheckEvaluator2(tree);
				if(CheckError())	return;
				if(resul==null)
				{
					parser.NextLine();
					continue;					
				}
				if (resul.Resultado is Undefined u)
				{
					if(string.IsNullOrEmpty(richTextLabel1.Text))	richTextLabel1.Text ="undefined";
					else	richTextLabel1.Text += '\n'+"undefined";
				}
				else if(resul.Resultado is DrawFunction df)
				{
					parser.NextLine();
					continue;
				}
				if(CheckError())	return;
				var r = resul.CheckResult();
				// if (r != null)
				// {
				// 	if(string.IsNullOrEmpty(richTextLabel1.Text))	richTextLabel1.Text =r;
				// 	else	richTextLabel1.Text += '\n'+r;
				// }
				parser.NextLine();
			}
			
			DrawFunction s = AllDraw();
			
			Draw(s);
			
		}
		
	}

    private DrawFunction AllDraw()
    {
        List<Expressions> figures= new List<Expressions>();
		foreach (var item in drawFunctions )
		{
			foreach (var f in item.Figures)
			{
				figures.Add(f);
			}
		}
		return new DrawFunction(new Token("",0,0,TokenType.DrawFunction),figures);
    }

    public static void Draw(DrawFunction d)
	{
		
		foreach (var item in d.Figures)
		{
			figuresList.Add(item);
		}
		
		node2D.QueueRedraw();
		CheckError();
	}
	public static bool CheckError()
	{
		if (Lexer.error_list.Count != 0)
		{
			richTextLabel1.Text = Lexer.error_list[0].error_type + " " + $"({Lexer.error_list[0].line},{Lexer.error_list[0].col})";
			return true;
		}

		return false;
	}
	private Result CheckEvaluator2(Expressions tree)
	{
		if (tree is Figures) return null;
		if (tree is Function) return null;
		if(tree is ColorExpression)	return null;
		if(tree is ImportExpression) return null;
		if (tree is RestoreExpression)
		{
			if (Parser.colorsList.Count != 0) Parser.colorsList.Pop();
			return null;
		}
		if (tree is DrawFunction d)
		{
			return Execute.Evaluator(tree);
		}
		return Execute.Evaluator(tree);
	}

	public static void Default()
	{
		figuresList.Clear();
		Parser.colorsList.Clear();
		drawFunctions.Clear();
		richTextLabel1.Text=null;
		Lexer.error_list.Clear();
		Parser.GlobalContext=new Context( new Dictionary<Identifiers,Expressions>(),null,new List<Function>(), new List<MultipleIdentifiers>());
		Parser.contextOfFunction.Clear();
		Parser.comingFromLet=false;
		Parser.comingFromFunctionParam=false;
		Execute.StackOverFlow = 0;
		
	}
}

	
	public class Result
	{
		private Expressions resultado;
		private TokenType type;

		public Result(Expressions resultado, TokenType type)
		{
			this.resultado = resultado;
			this.type = type;
		}

		public string CheckResult()
		{
			if (this.Resultado is NumberExpression num) return num.Number.dato;
			if (this.Resultado is BoolExpression bol) return bol.Nodo.dato;
			if (this.Resultado is StringExpression str) return str.Nodo.dato;

			return null;
		}

		public Expressions Resultado { get => resultado; set => resultado = value; }
		public TokenType Type { get => type; set => type = value; }
	}


	



