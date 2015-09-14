using UnityEngine;
using System.Collections;

public class WallColors : MonoBehaviour {

    public float duration = 10.0f;

    private Gradient grad;
    private float lerp;
    private float lerpTime;
    private float timeHolder;
    private bool inRoutine = false;

    void Start () {
        GradientColorKey[] gradColorKey;
        GradientAlphaKey[] gradAlphaKey;

        grad = new Gradient();
        gradColorKey = new GradientColorKey[3];
        gradAlphaKey = new GradientAlphaKey[1];

        gradColorKey[0].color = Color.magenta;
        gradColorKey[0].time = 0.0f;
        gradColorKey[1].color = Color.green;
        gradColorKey[1].time = 0.5f;
        gradColorKey[2].color = Color.magenta;
        gradColorKey[2].time = 1.0f;

        gradAlphaKey[0].alpha = 1.0f;

        grad.SetKeys(gradColorKey,gradAlphaKey);
    }

    IEnumerator gradLoopUp()
    {
        while(inRoutine)
        {
            timeHolder = Time.time % 2;
            lerpTime = (timeHolder / 2);
            lerp = Mathf.Lerp(0.05f,1,lerpTime);
            this.GetComponent<Renderer>().material.color = grad.Evaluate(lerp);
            if(lerp >= 0.95f)
            {
                yield return StartCoroutine(gradLoopDown());
                yield break;
            }
            else
                yield return null;
        }
        yield return StartCoroutine(gradLoopDown());
    }

    IEnumerator gradLoopDown()
    {
        while(inRoutine)
        {
            timeHolder = Time.time % 4;
            lerpTime = (timeHolder - 2) / 2;
            lerp = Mathf.Lerp(0.95f,0,lerpTime);
            this.GetComponent<Renderer>().material.color = grad.Evaluate(lerp);
            if(lerp <= 0.05f)
            {
                inRoutine = false;
                yield break;
            }
            else
                yield return null;
        }
        yield return inRoutine = false;
    }

    void Update () {
        if(!inRoutine)
        {
            inRoutine = true;
            StartCoroutine(gradLoopUp());
        }
    }
}
