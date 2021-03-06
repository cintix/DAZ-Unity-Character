﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DAZCharacter))]
public class DazCharacterInspector : Editor {
    // Start is called before the first frame update
    SerializedProperty SourceRenderer;
    SerializedProperty SourceRenderIndex;
    SerializedProperty ignornedMeshRenderers;
    SerializedProperty orginalBones;
    SerializedProperty expandIgnore;
    SerializedProperty expandMorphs;

    SkinnedMeshRenderer[] _renders;
    string[] _renderNames;
    string[] _ignoredNames;

    DAZCharacter character;

    void OnEnable() {
        character = target as DAZCharacter;
        SourceRenderer = serializedObject.FindProperty("sourceRenderer");
        expandIgnore = serializedObject.FindProperty("expandIgnore");
        expandMorphs = serializedObject.FindProperty("expandMorphs");
        SourceRenderIndex = serializedObject.FindProperty("sourceRenderIndex");
        ignornedMeshRenderers = serializedObject.FindProperty("ignornedMeshRenderers");
        orginalBones = serializedObject.FindProperty("orginalBones");
        readAllRenderers(character);
    }

    public override void OnInspectorGUI() {
        //DrawDefaultInspector();
        serializedObject.Update();



        EditorGUILayout.LabelField("DAZ 3D Character Setup");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Figure Shape");
        SourceRenderIndex.intValue = EditorGUILayout.Popup(SourceRenderIndex.intValue, _renderNames);

        if (SourceRenderIndex.intValue > 0) {
            SourceRenderer.objectReferenceValue = _renders[SourceRenderIndex.intValue - 1];
        } else {
            SourceRenderer.objectReferenceValue = null;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        expandIgnore.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(expandIgnore.boolValue, "Shape To Ignore");
        if (expandIgnore.boolValue) {
            EditorGUILayout.Separator();
            for (int index = 0; index < _renders.Length; index++) {
                bool ignored = isIgnored(_renders[index].name);

                bool newIgnored = EditorGUILayout.ToggleLeft(_renders[index].name, ignored);

                if (newIgnored != ignored) {
                    if (ignored == false) {
                        int newIndex = ignornedMeshRenderers.arraySize;
                        ignornedMeshRenderers.arraySize = (ignornedMeshRenderers.arraySize + 1);
                        ignornedMeshRenderers.GetArrayElementAtIndex(newIndex).objectReferenceValue = _renders[index];

                        if (_renders[index].gameObject) {
                            MasterMorph childMorphHandler = _renders[index].gameObject.GetComponent<MasterMorph>();
                            if (childMorphHandler != null) {
                                Debug.Log("DIE Morph on " + _renders[index].name);
                                childMorphHandler.unregister();
                            }
                        }


                    } else {
                        int lastIndex = ignornedMeshRenderers.arraySize - 1;
                        for (int i = 0; i < ignornedMeshRenderers.arraySize; i++) {
                            SkinnedMeshRenderer smr = ignornedMeshRenderers.GetArrayElementAtIndex(i).objectReferenceValue as SkinnedMeshRenderer;
                            if (smr == _renders[index]) {
                                ignornedMeshRenderers.MoveArrayElement(i, lastIndex);
                                ignornedMeshRenderers.DeleteArrayElementAtIndex(lastIndex);
                                ignornedMeshRenderers.arraySize = (ignornedMeshRenderers.arraySize - 1);
                                break;
                            }
                        }
                    }

                    int ignoreCount = ignornedMeshRenderers.arraySize;
                    _ignoredNames = new string[ignoreCount];

                    for (int i = 0; i < ignoreCount; i++) {
                        SkinnedMeshRenderer smr = ignornedMeshRenderers.GetArrayElementAtIndex(i).objectReferenceValue as SkinnedMeshRenderer;
                        _ignoredNames[i] = smr.name;
                    }

                }

            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Separator();

        expandMorphs.boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(expandMorphs.boolValue, "Figure Morphs");
        if (expandMorphs.boolValue) {
            EditorGUILayout.Separator();
            readAllMorphes();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        InitializeBones();
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();

    }

    private void readAllMorphes() {
        if (character.sourceRenderer == null) return;
        int morphsCount = character.sourceRenderer.sharedMesh.blendShapeCount;
        for (int index = 0; index < morphsCount; index++) {

            string label = character.sourceRenderer.sharedMesh.GetBlendShapeName(index);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(index + ".) " + label);
            float currentValue = (character.sourceRenderer.GetBlendShapeWeight(index) / 100);
            float orignal = currentValue;
            currentValue = EditorGUILayout.Slider(currentValue, 0, 1);
            if (orignal != currentValue) {
                character.setMorphValue(index, currentValue * 100);
            }

            EditorGUILayout.EndHorizontal();

        }
    }


    private void readAllRenderers(DAZCharacter character) {
        GameObject gameObject = character.gameObject;
        _renders = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        _renderNames = new string[_renders.Length + 1];
        _renderNames[0] = "None";

        for (int index = 0; index < _renders.Length; index++) {
            SkinnedMeshRenderer smr = _renders[index];
            _renderNames[index + 1] = smr.name.ToString();
        }

        int ignoreCount = ignornedMeshRenderers.arraySize;
        _ignoredNames = new string[ignoreCount];

        for (int index = 0; index < ignoreCount; index++) {
            SkinnedMeshRenderer smr = ignornedMeshRenderers.GetArrayElementAtIndex(index).objectReferenceValue as SkinnedMeshRenderer;
            _ignoredNames[index] = smr.name;
        }

    }

    private bool isIgnored(string name) {
        foreach (string iname in _ignoredNames) {
            if (name == iname) return true;
        }
        return false;
    }


    public void InitializeBones() {

        if (character.sourceRenderer == null) return;

        List<SkinnedMeshRenderer> ignoredRenderers = new List<SkinnedMeshRenderer>();
        if (character.ignornedMeshRenderers != null)
            foreach (SkinnedMeshRenderer smr in character.ignornedMeshRenderers) {
                ignoredRenderers.Add(smr);
            }

        if (SourceRenderer != null)
            foreach (SkinnedMeshRenderer renderer in character.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>()) {

                if (renderer != character.sourceRenderer && !character.orginalBones.ContainsKey(renderer.name)) {
                    character.orginalBones.Add(renderer.name, renderer.bones);
                }
                if (renderer != character.sourceRenderer) {
                    renderer.bones = character.orginalBones[renderer.name];
                }

                if (renderer != character.sourceRenderer && !ignoredRenderers.Contains(renderer)) {
                    Transform[] bones = renderer.bones;
                    List<Transform> newBones = new List<Transform>();
                    foreach (Transform ctf in bones) {
                        foreach (Transform tf in character.sourceRenderer.bones) {
                            string[] nameparts = tf.name.Split(' ');
                            string name = nameparts[0];
                            nameparts = ctf.name.Split(' ');
                            string childname = nameparts[0];
                            if (name.Equals(childname)) {
                                newBones.Add(tf);
                            }
                        }
                    }

                    renderer.bones = newBones.ToArray();

                    MasterMorph masterMorph = renderer.gameObject.GetComponent<MasterMorph>();
                    if (masterMorph == null) {
                        renderer.gameObject.AddComponent<MasterMorph>();
                        masterMorph = renderer.gameObject.GetComponent<MasterMorph>();
                        masterMorph.setDazCharacter(character);
                    } else {
                        masterMorph.enabled = true;
                    }

                }
            }
    }

}