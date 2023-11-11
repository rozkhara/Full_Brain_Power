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
    private void Awake()
    {
        speed = 2;
        cubeGenerator = GameManager.Instance.CubeGenerator;
    }
    private void Update()
    {
        elapsedTime += Time.deltaTime;
        transform.position = Vector3.Lerp(initPlanePos, destination, elapsedTime * speed / 15);
        if (elapsedTime * speed > 15f)
        {
            //TODO: 정답 판정
            cubeGenerator.GenerateCubeAndPlane();
            elapsedTime = 0f;
        }
    }
}
