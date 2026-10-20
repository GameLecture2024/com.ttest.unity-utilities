using UnityEngine;

namespace TTest.UnityUtilities.Samples.PoolingDemo
{
    /// <summary>Draws a simple square, triangle, or circle without requiring art assets.</summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public sealed class TopViewShapeVisual : MonoBehaviour
    {
        public enum Shape { Square, Triangle, Circle }

        [SerializeField] private Shape shape;
        [SerializeField] private Color color = Color.white;
        [SerializeField, Min(.1f)] private float size = 1f;

        private Mesh _mesh;
        private Material _material;

        private void Awake()
        {
            _mesh = CreateMesh();
            GetComponent<MeshFilter>().sharedMesh = _mesh;
            var shader = Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Color");
            _material = new Material(shader) { color = color };
            GetComponent<MeshRenderer>().sharedMaterial = _material;
        }

        private void OnDestroy()
        {
            Destroy(_mesh);
            Destroy(_material);
        }

        private Mesh CreateMesh()
        {
            var points = shape == Shape.Square ? 4 : shape == Shape.Triangle ? 3 : 24;
            var vertices = new Vector3[points + 1];
            vertices[0] = Vector3.zero;
            for (var index = 0; index < points; index++)
            {
                var angle = shape == Shape.Square ? Mathf.PI * .25f + index * Mathf.PI * .5f
                    : shape == Shape.Triangle ? Mathf.PI * .5f + index * Mathf.PI * 2f / 3f
                    : Mathf.PI * .5f + index * Mathf.PI * 2f / points;
                vertices[index + 1] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * (size * .5f);
            }

            var triangles = new int[points * 3];
            for (var index = 0; index < points; index++)
            {
                var triangle = index * 3;
                triangles[triangle] = 0;
                triangles[triangle + 1] = index + 1;
                triangles[triangle + 2] = index == points - 1 ? 1 : index + 2;
            }
            return new Mesh { vertices = vertices, triangles = triangles };
        }
    }
}
