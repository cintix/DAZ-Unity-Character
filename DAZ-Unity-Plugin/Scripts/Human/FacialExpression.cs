using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FacialExpression {
    public int index;
    public float minimumValue = 0f;
    public float maximumValue = 100f;
    public SkinnedMeshRenderer mesh;
}