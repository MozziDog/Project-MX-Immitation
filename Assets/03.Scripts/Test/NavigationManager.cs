using System;
using System.Collections.Generic;
using UnityEngine;
using Navigation.Net;
using TriangleNet.Geometry;
using Unity.VisualScripting;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private bool drawGizmos = false;
    
    private NavMesh _navMesh = new();
    private List<Mesh> meshForGizmos = new();
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<Point> surface = new List<Point>();
        surface.Add(new Point(-15, -15));
        surface.Add(new Point(-15, 15));
        surface.Add(new Point(15, 15));
        surface.Add(new Point(15, -15));
        _navMesh.AddSurface(surface);
        _navMesh.BuildNavMesh();
        
        CalculateGizmos(_navMesh);
    }

    public List<Vector2> GetPath(Vector2 start, Vector2 end)
    {
        var pointList = Pathfind.Run(ToPoint(start), ToPoint(end), _navMesh);
        
        var vectorList = new List<Vector2>();
        foreach (var point in pointList)
            vectorList.Add(ToVector2(point));
        
        return vectorList;
    }

    public static Point ToPoint(Vector2 var)
    {
        return new Point(var.x, var.y);
    }

    public static Vector2 ToVector2(Point var)
    {
        return new Vector2((float)var.x, (float)var.y);
    }

    public static Vector3 ToVector3(Point var)
    {
        return new Vector3((float) var.x, 0, (float) var.y);
    }

    private void CalculateGizmos(NavMesh navMesh)
    {
        meshForGizmos.Clear();
        foreach (var poly in navMesh.MergedPolygons)
        {
            int count = poly.Count;
            
            var vertices = new Vector3[count];
            for (int i=0; i<count; i++)
            {
                vertices[i] = ToVector3(poly.Points[i]);
            }

            var normals = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                normals[i] = new Vector3(0, 1, 0);
            }
            
            var triangles = new int[3 * (count - 2)];
            for (int i = 0; i < count - 2; i++)
            {
                triangles[3 * i] = 0;
                triangles[3 * i + 1] = i + 1;
                triangles[3 * i + 2] = i + 2;
            }

            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            meshForGizmos.Add(mesh);
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        
        Gizmos.color = Color.blue;
        foreach (var mesh in meshForGizmos)
        {
            Gizmos.DrawWireMesh(mesh, Vector3.up * 0.1f);
        }
    }
}
