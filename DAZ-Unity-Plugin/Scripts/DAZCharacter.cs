using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cintix/DAZ 3D/Character")]
public class DAZCharacter : MonoBehaviour {    public delegate void MorphChange(string name, float value);    public event MorphChange OnMorphChange;    public Dictionary<string, Transform[]> orginalBones = new Dictionary<string, Transform[]>();
    public SkinnedMeshRenderer[] ignornedMeshRenderers;
    public SkinnedMeshRenderer sourceRenderer;

    public int sourceRenderIndex;
    public bool expandIgnore = false;    public bool expandMorphs = false;    // Start is called before the first frame update    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

    }


    public void setMorphValue(int index, float value) {        if (sourceRenderer != null) {            string name = sourceRenderer.sharedMesh.GetBlendShapeName(index);            if (name.StartsWith(sourceRenderer.name.Substring(0, sourceRenderer.name.IndexOf(".Shape")))) {                name = name.Substring(name.IndexOf("__"));            }            sourceRenderer.SetBlendShapeWeight(index, value);            if (OnMorphChange != null) OnMorphChange(name, value);        }    }

}