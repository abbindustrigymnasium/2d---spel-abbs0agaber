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
        mesh  = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void LateUpdate()
    {

        float angle = StartingAngle + 90;
        float angleIncrese = fov/rayCount;

        Vector3[] vertices = new Vector3[rayCount + 1 +1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];
        
        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, UtilsClass.GetVectorFromAngle(angle), viewDistance, layerMask);
            if(raycastHit2D.collider == null){
                vertex = origin +  UtilsClass.GetVectorFromAngle(angle) * viewDistance;
            } else {
                vertex = raycastHit2D.point;
            }
            vertices[vertexIndex] = vertex;

            if (i>0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            vertexIndex++;
            angle -= angleIncrese;
        }

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
