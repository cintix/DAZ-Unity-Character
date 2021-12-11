using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Expression  {

    [Range(0f,1f)]
    public float value;
    public List<FacialExpression> facialExpressions = new List<FacialExpression>();

    public void SetValue(float _value) {
        if (_value < 0f) value = 0f;
        else if (_value > 1f) value = 1f;
        else value = _value;
        UpdateBlendShapes();
    }

    private void UpdateBlendShapes() {        
        for (int index = 0; index < facialExpressions.Count; index++) {
            FacialExpression facialExpression = facialExpressions[index];
            facialExpression.mesh.SetBlendShapeWeight(facialExpression.index, ProcentValue(facialExpression, value));
        }
    }   

    private float ProcentValue(FacialExpression fe, float procent) {
        float diff = fe.maximumValue - fe.minimumValue;
        return (diff * procent);
    }  
}