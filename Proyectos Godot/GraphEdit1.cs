using Godot;
using System;
using System.Collections.Generic;

public partial class GraphEdit1 : GraphEdit
{
    public bool mustDraw;
    public List<Figures> figuresList = new List<Figures>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{        
    }

    public void _draw()
    {
        GraphEdit1 graph = GetNode<GraphEdit1>("GraphEdit1");

        graph.DrawCircle(new Vector2(0, 0), 100, Color.Color8(244, 244, 244));
    }
}
