using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 endPos;
    private bool isRotating = false;
    private float elapsedTime = 0f;
    private float speed;
    private float middleX;

    public Transform CubeInstanceTransform;

    private void Awake()
    {
        speed = 8f;
        middleX = Screen.width / 2;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
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
                        elapsedTime = 0;
                        StartCoroutine(RotateCube(rotDir));
                    }
                }
            }
        }
    }

    private int DetermineRotation(Vector2 _startPos, Vector2 _endPos)
    {
        Vector2 diff = (_endPos - _startPos);
        if (diff.sqrMagnitude < 3f)
        {
            if (_endPos.x < middleX)
            {
                return 4; // CCW;
            }
            else return 5; //CW;
        }
        float angle = Vector2.SignedAngle(Vector2.right, diff);
        angle = (angle + 360) % 360;
        if (angle < 45) return 0; //right;
        if (angle < 135) return 1; //up;
        if (angle < 225) return 2; //left;
        if (angle < 315) return 3; //down;
        return 0; //right;
    }

    private IEnumerator RotateCube(int direction)
    {
        Vector3 towardDirection = direction switch
        {
            0 => Vector3.forward,
            1 => Vector3.up,
            2 => Vector3.back,
            3 => Vector3.down,
            4 => Vector3.back,
            5 => Vector3.forward,
            _ => Vector3.zero,
        };

        Quaternion initQ = CubeInstanceTransform.rotation;
        Quaternion destQ;
        if (direction < 4) destQ = Quaternion.FromToRotation(Vector3.right, towardDirection) * initQ;
        else destQ = Quaternion.FromToRotation(Vector3.up, towardDirection) * initQ;

        while (elapsedTime * speed < 1f)
        {
            elapsedTime += Time.deltaTime;
            CubeInstanceTransform.rotation = Quaternion.Slerp(initQ, destQ, elapsedTime * speed);
            yield return null;
        }
        CubeInstanceTransform.rotation = destQ;
        isRotating = false;
        yield return null;
    }
}
