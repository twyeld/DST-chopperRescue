using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GLGraph : MonoBehaviour
{
	public enum AverageLineType
	{
		None, Straight, EveryPoint
	}
	
	public string description;
	public Material mat;
	public List<LineData> lines;
	public AverageLineType averageLineType;
	
	[Header("Scaling")]
	public float zeroZCoordinate = 0f;
	
	[Header("Bounding Box")] 
	public bool drawBoundingBox;
	public Color32 boxColor = Color.gray;
	public Vector3 min;
	public float width = 20;
	public float height = 10;

	private void OnPostRender()
	{
		DrawBox();
		
		var pointsOfAllLinesOnGraph = new List<List<Vector3>>();
		
		foreach (var line in lines)
		{
			List<Vector3> graphPoints;
			RenderLine(line, out graphPoints);
			pointsOfAllLinesOnGraph.Add(graphPoints);
		}

		if (averageLineType == AverageLineType.EveryPoint)
			RenderAverageLine(pointsOfAllLinesOnGraph);

		if (averageLineType == AverageLineType.Straight)
			RenderAverageLine(pointsOfAllLinesOnGraph, straight: true);
	}

	private void RenderAverageLine(List<List<Vector3>> pointsOfAllLinesOnGraph, bool straight = false)
	{
		var maxCount = pointsOfAllLinesOnGraph.Max(list => list.Count);
		
		if(maxCount < 2)
			return;
		
		var averageLinePoints = new List<Vector3>();
		
		for (var x = 0; x < maxCount; x++)
		{
			var count = 0;
			var sum = Vector3.zero;
			foreach (var list in pointsOfAllLinesOnGraph.Where(list => x < list.Count))
			{
				count++;
				sum += list[x];
			}

			averageLinePoints.Add(sum / count);
		}
		
		GL.Begin(GL.LINES);
		mat.SetPass(0);
		GL.Color(Color.white);

		if (straight)
		{
			GL.Vertex(averageLinePoints[0]);
			GL.Vertex(averageLinePoints[averageLinePoints.Count - 1]);
		}
		else
		{
			for (var i = 0; i < averageLinePoints.Count - 1; i++)
			{
				GL.Vertex(averageLinePoints[i]);
				GL.Vertex(averageLinePoints[i + 1]);
			}
		}
		
		GL.End();
	}

	private void DrawBox()
	{
		if (!drawBoundingBox)
			return;
		
		GL.Begin(GL.LINES);
		mat.SetPass(0);
		GL.Color(boxColor);
		
		GL.Vertex(min); // bottom left
		GL.Vertex(min + new Vector3(width, 0, 0)); // bottom right
		
		GL.Vertex(min + new Vector3(width, 0, 0)); // bottom right
		GL.Vertex(min + new Vector3(width, 0, height)); // top right
		
		GL.Vertex(min + new Vector3(width, 0, height)); // top right
		GL.Vertex(min + new Vector3(0, 0, height)); // top left
		
		GL.Vertex(min + new Vector3(0, 0, height)); // top left
		GL.Vertex(min); // back to bottom left
		
		GL.End();
	}

	public void ResetLines()
	{
		foreach (var line in lines)
			line.points.Clear();
	}

	private void RenderLine(LineData line, out List<Vector3> graphPoints)
	{
		graphPoints = new List<Vector3>();

		if (line.points.Count < 2)
			return;
	    
		GL.Begin(GL.LINES);
		mat.SetPass(0);
		GL.Color(line.color);

		var maxXPointInAllLines = lines.Max(data =>
		{
			return data.points.Count == 0 ? 0f : data.points.Max(point => point.x);
		});

		var minXPointInAllLines = lines.Min(data =>
		{
			return data.points.Count == 0 ? 0f : data.points.Min(point => point.x);
		});

		var maxZPointInAllLines = lines.Max(data =>
		{
			return data.points.Count == 0 ? -zeroZCoordinate + height : data.points.Max(point => point.z);
		});

		var minZPointInAllLines = lines.Min(data =>
		{
			return data.points.Count == 0 ? -zeroZCoordinate : data.points.Min(point => point.z);
		});

		var maxXSpan = maxXPointInAllLines - minXPointInAllLines;
		var maxZSpan = maxZPointInAllLines - minZPointInAllLines;
		
		var multiplierX = maxXSpan < width ? 1f : width / maxXSpan;
		var multiplierZ = maxZSpan < height ? 1f : height / maxZSpan;

		if (minZPointInAllLines < - zeroZCoordinate || maxZPointInAllLines > -zeroZCoordinate + height)
		{
			// recalculate the zCoordinate to map (minXPointInAllLines, maxXPointInAllLines) onto (0, height)
			zeroZCoordinate = -minZPointInAllLines * multiplierZ;
		}

		for (var i = 0; i < line.points.Count; i++)
		{
			var point = min + new Vector3(line.points[i].x * multiplierX, line.points[i].y,
				            zeroZCoordinate + line.points[i].z * multiplierZ);
			graphPoints.Add(point);
			GL.Vertex(point);
			
			if (i + 1 < line.points.Count)
				GL.Vertex(min + new Vector3(line.points[i + 1].x * multiplierX, line.points[i + 1].y,
					          zeroZCoordinate + line.points[i + 1].z * multiplierZ));
		}
        
		GL.End();
	}

	// this is currently not used, instead we are using RenderAverageLine() with straight = true.
	/*
	private void RenderStraightAverageLine(List<List<Vector3>> pointsOfAllLinesOnGraph)
	{
		if(lines.Any(line => line.points.Count < 2))
			return;
		
		GL.Begin(GL.LINES);
		mat.SetPass(0);
		GL.Color(Color.white);

		var maxX = lines.Max(data =>
		{
			return data.points.Count == 0 ? 0f : data.points.Max(point => Mathf.Abs(point.x));
		});
		
		var maxZ = lines.Max(data =>
		{
			return data.points.Count == 0 ? 0f : data.points.Max(point => Mathf.Abs(point.z));
		});
		
		var multiplierX = maxX < xWithoutScaling ? 1f : width / maxX;
		var multiplierZ = maxZ < zWithoutScaling ? 1f : height / maxZ;
		
		var startPoint = min + new Vector3(
			lines.Sum(line => line.points[0].x) / lines.Count * multiplierX,
			lines.Sum(line => line.points[0].y) / lines.Count,
			zeroZCoordinate + lines.Sum(line => line.points[0].z) / lines.Count * multiplierZ);

		var endPoint = min + new Vector3(
			lines.Sum(line => line.points[line.points.Count - 1].x) / lines.Count * multiplierX,
			lines.Sum(line => line.points[line.points.Count - 1].y) / lines.Count,
			zeroZCoordinate + lines.Sum(line => line.points[line.points.Count - 1].z) / lines.Count * multiplierZ);
		
		GL.Vertex(startPoint);
		GL.Vertex(endPoint);
		
		GL.End();
	}*/
}
