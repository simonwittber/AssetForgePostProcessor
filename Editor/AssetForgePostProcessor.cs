using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class AssetForgePostProcessor : AssetPostprocessor
{

    void OnPostprocessModel(GameObject gameObject)
    {
        Debug.Log(assetPath);
        if (assetPath.Contains("AssetForge") && !assetPath.EndsWith(".asset"))
            Apply(gameObject);
    }

    void Apply(GameObject gameObject)
    {
        var filters = gameObject.GetComponentsInChildren<MeshFilter>();
        foreach (var f in filters)
            FixScale(f);
        CombineMaterials(gameObject);

    }

    class VertexAttributeCollection
    {
        public int order;
        public List<Vector3> vertex = new List<Vector3>();
        public List<Vector3> normal = new List<Vector3>();
        public List<Color> color = new List<Color>();
        public List<Vector2> uv = new List<Vector2>();
    }

    void CombineMaterials(GameObject go)
    {
        var filters = go.GetComponentsInChildren<MeshFilter>();
        var materialAttributeMap = new Dictionary<Material, VertexAttributeCollection>();

        foreach (var f in filters)
        {
            var mesh = f.sharedMesh;
            var vertices = mesh.vertices;
            var normals = mesh.normals;
            var uvs = mesh.uv;
            var addUV = uvs.Length > 0;
            var addNormal = normals.Length > 0;
            var meshMaterials = f.GetComponent<MeshRenderer>().sharedMaterials;
            for (var submeshIndex = 0; submeshIndex < mesh.subMeshCount; submeshIndex++)
            {
                var currentMaterial = meshMaterials[submeshIndex];
                VertexAttributeCollection vac;

                if (!materialAttributeMap.TryGetValue(currentMaterial, out vac))
                    vac = materialAttributeMap[currentMaterial] = new VertexAttributeCollection() { order = materialAttributeMap.Count };

                var indices = mesh.GetTriangles(submeshIndex);
                for (var i = 0; i < indices.Length; i++)
                {
                    vac.vertex.Add(vertices[indices[i]] + f.transform.localPosition);
                    if (addNormal) vac.normal.Add(normals[indices[i]]);
                    if (addUV) vac.uv.Add(uvs[indices[i]]);
                    vac.color.Add(new Color(vac.order / 255f, 0, 0, 0));
                }
            }
        }
        var newSharedMaterials = materialAttributeMap.Keys.ToArray();
        var allAttributes = new VertexAttributeCollection();
        var allIndices = new List<int>();
        for (var i = 0; i < newSharedMaterials.Length; i++)
        {
            var va = materialAttributeMap[newSharedMaterials[i]];
            allAttributes.vertex.AddRange(va.vertex);
            allAttributes.normal.AddRange(va.normal);
            allAttributes.uv.AddRange(va.uv);
            allAttributes.color.AddRange(va.color);
        }
        var newMesh = new Mesh() { name = "CombinedMesh" };
        newMesh.SetVertices(allAttributes.vertex);
        newMesh.SetNormals(allAttributes.normal);
        newMesh.SetColors(allAttributes.color);
        newMesh.SetUVs(0, allAttributes.uv);
        var triangleIndex = 0;
        for (var i = 0; i < newSharedMaterials.Length; i++)
        {
            var va = materialAttributeMap[newSharedMaterials[i]];
            var vertCount = va.vertex.Count;
            allIndices.AddRange(Enumerable.Range(triangleIndex, vertCount).ToArray());
            triangleIndex += vertCount;
        }
        newMesh.triangles = allIndices.ToArray();
        AssetDatabase.CreateAsset(newMesh, assetPath.Replace(".fbx", ".asset"));
    }

    void FixScale(MeshFilter filter)
    {
        var mesh = filter.sharedMesh;
        var scale = filter.transform.localScale;
        var vertices = mesh.vertices;
        var reverseWinding = scale.x < 0 || scale.y < 0 || scale.z < 0;
        var matrix = Matrix4x4.TRS(Vector3.zero, filter.transform.localRotation, scale);
        var normals = mesh.normals;
        for (var i = 0; i < vertices.Length; i++)
        {
            vertices[i] = matrix * vertices[i];
            normals[i] = Vector3.Scale(scale, normals[i]).normalized;
        }

        for (var submeshIndex = 0; submeshIndex < mesh.subMeshCount; submeshIndex++)
        {
            var indices = mesh.GetTriangles(submeshIndex);
            for (var i = 0; i < indices.Length; i += 3)
            {
                if (reverseWinding)
                {
                    var a = indices[i + 0];
                    var b = indices[i + 1];
                    var c = indices[i + 2];
                    indices[i + 0] = a;
                    indices[i + 1] = c;
                    indices[i + 2] = b;
                }
            }
            mesh.SetTriangles(indices, submeshIndex);
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
        filter.transform.localScale = Vector3.one;
        filter.transform.localRotation = Quaternion.identity;
    }
}
