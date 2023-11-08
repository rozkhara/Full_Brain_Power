using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 endPos;
    private bool isRotating = false;
    private float elapsedTime = 0f;
    public float speed = 1f;

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began)
            {
                startPos = touch.position;
            }
            if (touch.phase == TouchPhase.Ended)
            {
                endPos = touch.position;
                int rotDir = DetermineRotation(startPos, endPos);
                if (rotDir != -1)
                {
                    if (!isRotating)
                    {
                        isRotating = true;
                        StartCoroutine(RotateCube(rotDir));
                    }
                }
            }
        }
    }

    private int DetermineRotation(Vector2 _startPos, Vector2 _endPos)
    {
        Vector2 diff = (_endPos - _startPos).normalized;
        if (diff.sqrMagnitude < 10f) return -1;
        float angle = Vector2.Angle(new Vector2(1, 0), diff);
        if (angle < 45)  return 0; //right;
        if (angle < 135) return 1; //up;
        if (angle < 225) return 2; //left;
        if (angle < 315) return 3; //down;
                         return 0; //right;
    }

    private IEnumerator RotateCube(int direction)
    {
        //while(elapsedTime<)
        isRotating = false;
        yield return null;
    }
}
