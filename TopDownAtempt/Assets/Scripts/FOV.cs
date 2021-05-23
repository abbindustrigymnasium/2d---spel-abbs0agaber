using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class FOV : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private Mesh mesh;
    public float fov = 90f;
    public int rayCount = 50;
    public float viewDistance = 100f;
    public Vector3 origin;
    private float StartingAngle;

    void Start()
    {
        // Creates the mesh for the FOV effect
        mesh  = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void LateUpdate()
    {
        // Sets starting angle corrected for player rotation
        float angle = StartingAngle + 90;
        float angleIncrese = fov/rayCount;

        // Creates array for the vertacies , uv, and points for the triangles  
        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];
        
        // sets first vertex to the origin
        vertices[0] = origin;

        // Creates Index for vertex and triangle
        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <= rayCount; i++)
        {
            // Sets vertex to hitpoint of the raycast if it hit something, otherwise to the max viewdistance
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, UtilsClass.GetVectorFromAngle(angle), viewDistance, layerMask);
            if(raycastHit2D.collider == null){
                vertex = origin +  UtilsClass.GetVectorFromAngle(angle) * viewDistance;
            } else {
                vertex = raycastHit2D.point;
            }
            // sets the vertex in the array to the created vertex
            vertices[vertexIndex] = vertex;

            // Sets each triangels points to origin, previous vertex and current vertex
            if (i>0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            // Goes to next angel and next vertex
            vertexIndex++;
            angle -= angleIncrese;
        }

        // Gives the mesh its properties
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

        public void SetOrigin(Vector3 origin){
        this.origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection)
    {
        StartingAngle = UtilsClass.GetAngleFromVectorFloat(aimDirection) - fov / 2f;
    }
}
