﻿using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cintix/DAZ 3D/Character")]
public class DAZCharacter : MonoBehaviour {
    public SkinnedMeshRenderer[] ignornedMeshRenderers;
    public SkinnedMeshRenderer sourceRenderer;

    public int sourceRenderIndex;
    public bool expandIgnore = false;
        
    }

    // Update is called once per frame
    void Update() {

    }


    public void setMorphValue(int index, float value) {

}