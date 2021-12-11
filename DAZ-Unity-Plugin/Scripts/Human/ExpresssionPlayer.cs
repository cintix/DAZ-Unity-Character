using UnityEngine;

public class ExpresssionPlayer : MonoBehaviour {

    private SkinnedMeshRenderer _skeleton;
    private float emotionalValue = 0f;

    private bool playingExpression = false;
    private bool forwardEmotional = true;

    private float currentMin = 0f;
    private float currentMax = 0f;

    private float animationSpeed = 0.12f;

    private Expression currentExpression;
    private ExpressionListener expressionListener;

    public void SetMeshRender(SkinnedMeshRenderer _skin) {
        _skeleton = _skin;
    }

    public void SetListener(ExpressionListener _listener) {
        expressionListener = _listener;
    }

    public void SetSpeed(float value) {
        animationSpeed = value;
    }

    public void PlayExpression(Expression expression) {
        currentExpression = expression;
        forwardEmotional = true;
        playingExpression = true;

        currentMin = 0f;
        currentMax = 1f;
    }

    public bool IsPlaying() {
        return playingExpression;
    }

    // Update is called once per frame
    void Update() {
        if (currentExpression != null && playingExpression) {
            if (!forwardEmotional) {
                emotionalValue -= animationSpeed;
                if (emotionalValue < currentMin) {
                    emotionalValue = 0f;
                    playingExpression = false;
                    
                    if (expressionListener != null) expressionListener.OnExpressionEnd();
                    forwardEmotional = true;
                   
                    if (_skeleton != null && currentExpression != null) {
                        currentExpression.SetValue(emotionalValue);
                    }
                    return;
                }
            } else {
                emotionalValue += animationSpeed;
                if (emotionalValue > currentMax) {
                    emotionalValue = currentMax;
                    forwardEmotional = false;
                }
            }
            if (_skeleton != null && currentExpression != null) {
                currentExpression.SetValue(emotionalValue);
            }
        }
    }

    public interface ExpressionListener {
        public void OnExpressionEnd();
    }
}
