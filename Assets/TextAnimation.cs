using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextAnimation : MonoBehaviour
{
    TMP_Text textMeshComp;
    public float speed;

    void Start()
    {
        textMeshComp = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        textMeshComp.ForceMeshUpdate();
        var textInfo = textMeshComp.textInfo;

        for(int i = 0; i< textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible)
                continue;
            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; ++j)
            {
                var orig = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = orig + new Vector3(Mathf.Cos(Time.time * speed + orig.y * 0.01f) * 5f, Mathf.Sin(Time.time * speed + orig.z * 0.01f) * 5f, 0);
            }
        }

        for(int i = 0; i < textInfo.meshInfo.Length; ++i)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textMeshComp.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}
