using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ExpresssionPlayer))]
public class HumanExpression : MonoBehaviour , ExpresssionPlayer.ExpressionListener {
    public enum ExpressionState { Ordered, Random };
    
    [Header("Orintation")]
    public Transform focus;
    
    [Range(0f, 10f)]
    public float distance = 6f;
    public float minViewAngel = 45f;
    public float maxViewAngel = 135f;

    [Header("Eyes")]
    public Expression eyes;

    [Range(0f, 0.1f)]
    public float EyesSpeed = 0.06f;

    [Range(0f, 5f)]
    public float timeBetweenBlinksMin = 0.2f;

    [Range(0f, 5f)]
    public float timeBetweenBlinksMax = 2.3f;

    [Header("Facial Expressions")]
    public ExpressionState state;

    [Range(0f, 0.1f)]
    public float expressionSpeed = 0.01f;

    [Range(0f, 5f)]
    public float timeBetweenExpressionsMin = 0.2f;

    [Range(0f, 5f)]
    public float timeBetweenExpressionsMax = 2.3f;

    public List<Expression> expressions = new List<Expression>();

    [Header("Debug")]
    public bool debug = false;
    public SkinnedMeshRenderer skeleton;

    private ExpresssionPlayer expresssionPlayer;

    private float blinkValue;
    private bool eyesOpen = true;
    private float eyesWait = 0f;
    private bool animateEyes = false;

    private int emotionalIndex = 0;
    private float emotionalWait = 0f;
    private int lastEmotionalIndex = 0;
    private Animator _ani;
    private Vector3 directionToTarget;
    private float angleToTarget;
    private float distanceToTarget;
    private float focusValue;

    void Start() {
        if (debug && skeleton != null) {
            int blendShapeCount = skeleton.sharedMesh.blendShapeCount;
            for (int index = 0; index < blendShapeCount; index++) {
                string name = skeleton.sharedMesh.GetBlendShapeName(index);
                Debug.Log("Blendshape - " + name + " [ index " + index + " ]");
            }
        }

        if (expressions.Count > 0 && state == ExpressionState.Random) {
            emotionalIndex = Random.Range(0, expressions.Count);
        }

        expresssionPlayer = GetComponent<ExpresssionPlayer>();
        expresssionPlayer.SetListener(this);
        expresssionPlayer.SetMeshRender(skeleton);

        _ani = GetComponentInChildren<Animator>();
    }

    void Update() {
        Blink();
        if (expressions != null) {
            Emotional();
        }
    }

    void OnAnimatorIK() {
        HeadOrintation();
    }

    void HeadOrintation() {
        if (_ani != null && focus != null) {

            directionToTarget = focus.position - transform.position;
            angleToTarget = Vector3.Angle(directionToTarget, focus.right);
            
            distanceToTarget = directionToTarget.magnitude;

            if (debug) Debug.Log("angleToTarget: " + angleToTarget);
            if (debug) Debug.Log("distanceToTarget: " + distanceToTarget);

            if (angleToTarget < maxViewAngel && angleToTarget > minViewAngel && distanceToTarget < distance) {
                 focusValue = RemapValues(distanceToTarget, 0f, distance, 1f, 0.2f);
                _ani.SetLookAtWeight(focusValue);
                _ani.SetLookAtPosition(focus.position);
            }

        }
    }

    void Emotional() {        
        if (expressions.Count == 0 || expressions.Count < emotionalIndex || expresssionPlayer.IsPlaying()) return;        
        if (emotionalWait < Time.realtimeSinceStartup) {
            expresssionPlayer.SetSpeed(expressionSpeed);
            expresssionPlayer.PlayExpression(expressions[emotionalIndex]);
        }
    }

    void Blink() {
        if (eyes == null) return;
        if (eyesWait < Time.realtimeSinceStartup && !animateEyes) {
            eyesWait = Time.realtimeSinceStartup + Random.Range(timeBetweenBlinksMin, timeBetweenBlinksMax);
            eyesOpen = false;
            animateEyes = true;
        }

        if (animateEyes) {
            if (!eyesOpen) {
                blinkValue += EyesSpeed;
                if (blinkValue > 1) {
                    blinkValue = 1;
                    eyesOpen = true;
                }
            } else {
                blinkValue -= EyesSpeed;
                if (blinkValue < 0) {
                    blinkValue = 0;
                    animateEyes = false;
                }
            }                
            eyes.SetValue(blinkValue);
        }
        

    }


    public void OnExpressionEnd() {
        if (expressions.Count > 0 && state == ExpressionState.Random) {
            emotionalIndex = Random.Range(0, expressions.Count);
            while (emotionalIndex == lastEmotionalIndex) {
                emotionalIndex = Random.Range(0, expressions.Count);
            }
            lastEmotionalIndex = emotionalIndex;
        } else {
            emotionalIndex++;
            if (emotionalIndex == expressions.Count) emotionalIndex = 0;
        }
        emotionalWait = Time.realtimeSinceStartup + Random.Range(timeBetweenExpressionsMin, timeBetweenExpressionsMax);
    }

    float RemapValues(float value, float in_min, float in_max, float out_min, float out_max) {
        return (value - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

}