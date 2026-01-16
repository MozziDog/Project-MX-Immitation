using System;
using System.Collections.Generic;
using _03.Scripts.Test;
using UnityEngine;
using Navigation.Net;
using TriangleNet.Geometry;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] private bool drawGizmos = false;
    
    private NavMesh _navMesh = new();
    private List<Mesh> _navMeshConvexes = new();
    private List<(Vector3, Vector3)> _navMeshLinks = new();
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<Point> surface = new List<Point>();
        surface.Add(new Point(-15, -15));
        surface.Add(new Point(-15, 15));
        surface.Add(new Point(15, 15));
        surface.Add(new Point(15, -15));
        _navMesh.AddSurface(surface);

        var obstacle1 = new Obstacle(new Vector2(5, 4), new Vector2(4, 2), 60.0f);
        _navMesh.AddObstacle(obstacle1.Vertices);
        _navMesh.AddLink(obstacle1.Link[0], obstacle1.Link[1], 3f);
        
        var obstacle2 = new Obstacle(new Vector2(-3, -8), new Vector2(4, 2), 0.0f);
        _navMesh.AddObstacle(obstacle2.Vertices);
        _navMesh.AddLink(obstacle2.Link[0], obstacle2.Link[1], 3f);
        
        var obstacle3 = new Obstacle(new Vector2(0, 10), new Vector2(4, 2), -30.0f);
        _navMesh.AddObstacle(obstacle3.Vertices);
        _navMesh.AddLink(obstacle3.Link[0], obstacle3.Link[1], 3f);
        
        _navMesh.BuildNavMesh();
        CalculateGizmos(_navMesh);
    }

    public List<Vector2> GetPath(Vector2 start, Vector2 end)
    {
        (var pointList, _) = Pathfind.Run(ToPoint(start), ToPoint(end), _navMesh);
        
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
        _navMeshConvexes.Clear();
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
            _navMeshConvexes.Add(mesh);
        }
        
        _navMeshLinks.Clear();
        foreach (var link in navMesh.Links)
        {
            (var src, var dst, _, _) = link;
            Vector3 v0 = ToVector3(src);
            Vector3 v1 = ToVector3(dst);
            _navMeshLinks.Add((v0, v1));
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        
        Gizmos.color = Color.blue;
        foreach (var mesh in _navMeshConvexes)
        {
            Gizmos.DrawWireMesh(mesh, Vector3.up * 0.1f);
        }

        Gizmos.color = Color.green;
        foreach ((var src, var dst) in _navMeshLinks)
        {
            Gizmos.DrawLine(src, dst);
        }
    }
}
