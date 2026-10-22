using UnityEngine;

public class MixColorFromMat : MonoBehaviour
{
    [SerializeField] Material MatA;
    [SerializeField] Material MatB;
    public Material MixMat;

    private void OnEnable()
    {
        MixMat = Resources.Load<Material>("DefaultMixMat");
        if (MatA)
        {
            ChangeFirst(MatA);
        }
        if (MatB)
        {
            ChangeSecond(MatB);
        }
    }

    private SlimeMat AddSlimeMatIfNotExists()
    {
        if (!gameObject.TryGetComponent<SlimeMat>(out SlimeMat sm))
        {
            return gameObject.AddComponent<SlimeMat>();
        }
        return sm;
    }

    private Material AddMixMaterialIfNotExists()
    {
        SlimeMat sm = AddSlimeMatIfNotExists();
        Material mm = sm.GetMat();
        if (mm != null && !mm.shader.name.EndsWith("MixMat"))
        {
            sm.SetMat(MixMat);
            mm = sm.GetMat();
            return mm;
        }
        return mm;
    }

    private void ChangeMat(Material mat, string varName)
    {
        Material mm = AddMixMaterialIfNotExists();
        if (mm == null)
        {
            Debug.LogWarning("Mischung auf " + gameObject.name + " nicht moeglich, da kein Schleimmaterial vorhanden ist.");
            return;
        }
        Color color = mat.GetColor("_Color");
        if(color != null)
        {
            mm.SetColor(varName, color);
        }
    }

    public void ChangeFirst(Material mat)
    {
        ChangeMat(mat, "_ColorA");
    }

    public void ChangeSecond(Material mat)
    {
        ChangeMat(mat, "_ColorB");
    }
}
