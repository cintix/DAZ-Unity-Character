using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MasterMorph : MonoBehaviour {
    private Dictionary<string, int> _morphIndexes = new Dictionary<string, int>();
    private SkinnedMeshRenderer _sourceRenderer;

    public DAZCharacter character;

    public void setDazCharacter(DAZCharacter daz) {        character = daz;        _sourceRenderer = GetComponent<SkinnedMeshRenderer>();        setupMorphes();    }

    private void OnEnable() {        setupMorphes();    }

    private void Awake() {        setupMorphes();    }    private void Start() {        setupMorphes();    }

    private void setupMorphes() {        _sourceRenderer = GetComponent<SkinnedMeshRenderer>();        if (_sourceRenderer != null && character != null) {            int blendShapeCounts = _sourceRenderer.sharedMesh.blendShapeCount;            for (int index = 0; index < blendShapeCounts; index++) {                string name = _sourceRenderer.sharedMesh.GetBlendShapeName(index);                if (!_morphIndexes.ContainsKey(name)) {                    _morphIndexes.Add(name, index);                }            }            character.OnMorphChange += OnMorphChanged;        }    }

    public void unregister() {        character.OnMorphChange -= OnMorphChanged;        if (Application.isEditor) DestroyImmediate(this);        else Destroy(this,0);    }

    private void OnMorphChanged(string name, float value) {        if (name.StartsWith("__")) {            string newName = _sourceRenderer.name.Substring(0, _sourceRenderer.name.IndexOf(".Shape")) + name;            name = newName;        }        if (_morphIndexes.ContainsKey(name)) {            int index = _morphIndexes[name];            _sourceRenderer.SetBlendShapeWeight(index, value);        }    }

}