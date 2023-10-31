using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> cubePrefabs;
    [SerializeField]
    private List<GameObject> planePrefabs;
    [SerializeField]
    private int _difficulty = 1;

    private readonly Vector3 initPlanePos = new Vector3(-15, 0, 0);

    private GameObject cubeInstance;
    private GameObject planeInstance;
    private Transform planeFillerTransform;
    private Transform cubeParentTransform;
    private int cubeLimit;
    private int cubeCount;
    private int sideLength;
    private int sideLength2;

    private int[] dx = { 1, 0, 0, -1, 0, 0 };
    private int[] dy = { 0, 1, 0, 0, -1, 0 };
    private int[] dz = { 0, 0, 1, 0, 0, -1 };

    private bool[,] PlaneInfo = new bool[5, 5];

    private List<Vector3Int> OccupyingPositions = new List<Vector3Int>();
    private List<Vector3Int> PossiblePositions = new List<Vector3Int>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))    //Event type input system: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/manual/index.html
        {
            GenerateCubeAndPlane();
        }
    }

    private void GenerateCubeAndPlane()
    {
        GenerateCube();
        GeneratePlane();
    }

    private void Start()
    {
        GameManager.Instance.SetDifficulty(_difficulty);
        Init();
    }

    public void Init()
    {
        cubeInstance = Instantiate(cubePrefabs[GameManager.Instance.Difficulty]);
        planeInstance = Instantiate(planePrefabs[GameManager.Instance.Difficulty]);
        planeInstance.transform.position = initPlanePos;
        planeFillerTransform = planeInstance.transform.GetChild(0);
        sideLength = GameManager.Instance.Difficulty + 3;
        sideLength2 = sideLength * sideLength;
        cubeLimit = sideLength * sideLength2;
        ClearCube();
        ClearPlaneInfo();
    }

    public void ClearCube()
    {
        cubeParentTransform = cubeInstance.transform;
        foreach (Transform child in cubeParentTransform)
        {
            child.gameObject.SetActive(false);
        }
        OccupyingPositions.Clear();
        PossiblePositions.Clear();
    }

    private void UpdatePosition(Vector3Int iv)
    {
        OccupyingPositions.Add(iv);
        int nx, ny, nz;
        Vector3Int v;
        for (int i = 0; i < 6; ++i)
        {
            nx = iv.x + dx[i];
            ny = iv.y + dy[i];
            nz = iv.z + dz[i];
            if (nx < 0 || ny < 0 || nz < 0 || nx >= sideLength || ny >= sideLength || nz >= sideLength) continue;
            v = new(nx, ny, nz);
            // TODO: List.Contains() has time complexity of O(n) => need to convert this into bool[,,] occupyingPositions = new bool[5,5,5];
            if (OccupyingPositions.Contains(v)) continue;
            PossiblePositions.Add(v);
        }
    }

    public void GenerateCube()
    {
        ClearCube();
        cubeCount = Random.Range(3, cubeLimit - 1); //Number of cubes to be generated
        int startPos_flattened = Random.Range(0, cubeLimit); //First cube position
        int yz = startPos_flattened % sideLength2;
        Vector3Int firstCubePosition = new Vector3Int(startPos_flattened / sideLength2, yz / sideLength, yz % sideLength); //x-axis: front->back, y-axis: bottom->top, z-axis: left->right
        UpdatePosition(firstCubePosition);
        cubeParentTransform.GetChild(startPos_flattened).gameObject.SetActive(true);
        int next;
        int next_flattened;
        while (cubeCount > 0)
        {
            next = Random.Range(0, PossiblePositions.Count);
            UpdatePosition(PossiblePositions[next]);
            next_flattened = PossiblePositions[next].x * sideLength2 + PossiblePositions[next].y * sideLength + PossiblePositions[next].z;
            cubeParentTransform.GetChild(next_flattened).gameObject.SetActive(true);
            PossiblePositions.RemoveAt(next);
            --cubeCount;
        }
    }

    private void ClearPlaneInfo()
    {
        System.Array.Clear(PlaneInfo, 0, PlaneInfo.Length);
        foreach (Transform transform in planeFillerTransform)
        {
            transform.gameObject.SetActive(true);
        }
    }

    private void UpdatePlaneInfo()
    {
        int side = Random.Range(0, 5);
        switch (side) //every reference frame is being converted to x-axis: left->right, y-axis: bottom->top
        {
            case 0: //view from left
                {
                    foreach (var elem in OccupyingPositions) //current reference frame == x-axis: left->right, y-axis: bottom->top
                    {
                        PlaneInfo[elem.x, elem.y] = true;
                    }
                    break;
                }
            case 1: //view from right
                {
                    foreach (var elem in OccupyingPositions) //current reference frame == x-axis: right->left, y-axis: bottom->top
                    {
                        int nx = -elem.x + sideLength - 1;
                        PlaneInfo[nx, elem.y] = true;
                    }
                    break;
                }
            case 2: //view from top
                {
                    foreach (var elem in OccupyingPositions) //current reference frame == x-axis: top->bottom, z-axis: left->right
                    {
                        int nx = elem.z;
                        int ny = -elem.x + sideLength - 1;
                        PlaneInfo[nx, ny] = true;
                    }
                    break;
                }
            case 3: //view from bottom
                {
                    foreach (var elem in OccupyingPositions) //current reference frame == x-axis: bottom->top, z-axis: left->right
                    {
                        int nx = elem.z;
                        int ny = elem.x;
                        PlaneInfo[nx, ny] = true;
                    }
                    break;
                }
            case 4: //view from front
                {
                    foreach (var elem in OccupyingPositions) //current reference frame == y-axis: bottom->top, z-axis: right->left
                    {
                        int nx = -elem.z + sideLength - 1;
                        PlaneInfo[nx, elem.y] = true;
                    }
                    break;
                }
            default:
                Debug.LogError("Undefined Side!");
                break;
        }
    }

    public void GeneratePlane()
    {
        ClearPlaneInfo();
        UpdatePlaneInfo();
        planeInstance.transform.position = initPlanePos;
        for (int i = 0; i < sideLength; ++i)
        {
            for (int j = 0; j < sideLength; ++j)
            {
                if (PlaneInfo[i, j])
                {
                    planeFillerTransform.GetChild(i + j * sideLength).gameObject.SetActive(false);
                }
            }
        }
        // TODO: randomly rotate the planeFiller
        //int rotation = Random.Range(0, 4);
        //switch (rotation)
        //{
        //    case 0:
        //        {

        //        }

        //}
    }
}
