﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DAZCharacter))]
public class DazCharacterInspector : Editor {
    SerializedProperty SourceRenderIndex;
    SerializedProperty ignornedMeshRenderers;
    SerializedProperty orginalBones;
    SerializedProperty expandIgnore;
    SerializedProperty expandMorphs;

    SkinnedMeshRenderer[] _renders;
    string[] _renderNames;
    string[] _ignoredNames;

    DAZCharacter character;
    private void readAllRenderers(DAZCharacter character) {

    private bool isIgnored(string name) {


    public void InitializeBones() {

}