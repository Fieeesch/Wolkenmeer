using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlimeMat : MonoBehaviour
{
    private string[] shaders = new string[]{ "SlimeMat","MixMat" };
    private Dictionary<Transform, int> objectsWithSlimeMatPos = new Dictionary<Transform, int>();

    public Material GetMat()
    {
        Scan();
        if (objectsWithSlimeMatPos.Count > 0)
        {
            KeyValuePair<Transform, int> kvp = objectsWithSlimeMatPos.First();
            if (kvp.Key.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
            {
                return mr.materials[kvp.Value];
            }
        }
        Debug.LogWarning("Kein gueltiges Schleimmaterial auf " + gameObject.name + " vorhanden.");
        return null;
    }

    public void SetMat(Material newMat)
    {
        Scan();
        foreach (KeyValuePair<Transform, int> kvp in objectsWithSlimeMatPos)
        {
            if (kvp.Key.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
            {
                Material[] matCopies = mr.materials;
                matCopies[kvp.Value] = newMat;
                mr.materials = matCopies;
            }
        }
    }

    public void Scan()
    {
        if (objectsWithSlimeMatPos.Count <= 0)
        {
            objectsWithSlimeMatPos = FindChildWithMat(gameObject.transform, objectsWithSlimeMatPos);
        }
    }

    private void OnEnable()
    {
        Scan();
    }

    private Dictionary<Transform, int> FindChildWithMat(Transform parent, Dictionary<Transform, int> result)
    {
        if (parent.TryGetComponent<MeshRenderer>(out MeshRenderer mr))
        {
            for (int matIndex = 0; matIndex < mr.materials.Length; matIndex++)
            {
                foreach (string shader in shaders)
                {
                    if (mr.materials[matIndex].shader.name.EndsWith(shader))
                    {
                        result.Add(parent, matIndex);
                    }
                }
            }
        }
        foreach (Transform child in parent)
        {
            FindChildWithMat(child, result);
        }
        return result;
    }
}
