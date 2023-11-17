using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public int speed;
    private readonly Vector3 destination = Vector3.zero;
    private readonly Vector3 initPlanePos = new Vector3(-15, 0, 0);
    public float elapsedTime;
    private CubeGenerator cubeGenerator;
    private Transform planeFillerParentObjectTransform;

    public bool collisionHappened = false;

    private void Awake()
    {
        speed = 2;
        cubeGenerator = GameManager.Instance.CubeGenerator;
        planeFillerParentObjectTransform = transform.GetChild(0);
        GameManager.Instance.PlaneController = this;
    }
    private void Update()
    {
        elapsedTime += Time.deltaTime;
        transform.position = Vector3.Lerp(initPlanePos, destination, elapsedTime * speed / 15);
        if (elapsedTime * speed > 15f)
        {
            elapsedTime = 0f;
            if (!Determine())
            {
                GameManager.Instance.GameOver();
            }
            cubeGenerator.GenerateCubeAndPlane();
        }
        if (collisionHappened)
        {
            GameManager.Instance.GameOver();
        }
    }

    private bool Determine()
    {
        RaycastHit hit;
        bool ret = false;
        int sideLength = GameManager.Instance.Difficulty + 3;
        float delta = 3f / sideLength;
        Vector3 leftbottom = new Vector3(-1.6f, -1.5f + delta / 2f, -1.5f + delta / 2f);
        for (int i = 0; i < sideLength; ++i)
        {
            for (int j = 0; j < sideLength; ++j)
            {
                Vector3 curPos = leftbottom + new Vector3(0f, i * delta, j * delta);
                ret = Physics.Raycast(curPos, Vector3.right, out hit, 10f);
                if (!ret) return ret;
            }
        }
        return ret;
    }

}
