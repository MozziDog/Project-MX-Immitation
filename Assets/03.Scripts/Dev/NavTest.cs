using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Navigation.Net;
using TriangleNet.Geometry;

public class NavTest : MonoBehaviour
{
    [SerializeField] private bool drawGizmos;
    public BattleSceneManager battleScene;
    
    private NavMesh _navMesh;
    private List<Mesh> _navMeshConvexes = new();
    private List<(Vector3, Vector3)> _navMeshLinks = new();

    
    // Start is called before the first frame update
    [ContextMenu("Draw NavMesh")]
    void DrawGizmos()
    {
        drawGizmos = true;
        _navMesh = battleScene.BattleLogic.NavigationSystem.NavMesh;
        CalculateGizmos(_navMesh);
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
