using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> prefabs;

    GameObject cubeInstance;
    Transform parent;
    int cubeLimit;
    int cubeCount;
    int sideLength;
    int sideLength2;

    int[] dx = { 1, 0, 0, -1, 0, 0 };
    int[] dy = { 0, 1, 0, 0, -1, 0 };
    int[] dz = { 0, 0, 1, 0, 0, -1 };

    List<Vector3Int> OccupyingPositions = new List<Vector3Int>();
    List<Vector3Int> PossiblePositions = new List<Vector3Int>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))    //Event type input system: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/manual/index.html
        {
            GenerateCube();
        }
    }

    private void Start()
    {
        GameManager.Instance.SetDifficulty(1);
        Init();
    }

    public void Init()
    {
        cubeInstance = Instantiate(prefabs[GameManager.Instance.Difficulty - 1]);
        sideLength = GameManager.Instance.Difficulty + 2;
        sideLength2 = sideLength * sideLength;
        cubeLimit = sideLength * sideLength2;
        ClearCube();
    }

    public void ClearCube()
    {
        parent = cubeInstance.transform;
        foreach (Transform child in parent)
        {
            child.gameObject.SetActive(false);
        }
        ClearPosition();
    }

    public void GenerateCube()
    {
        ClearCube();
        cubeCount = Random.Range(3, cubeLimit - 1); //Number of cubes to be generated
        int startPos_flattened = Random.Range(0, cubeLimit); //First cube position
        int yz = startPos_flattened % sideLength2;
        Vector3Int firstCubePosition = new Vector3Int(startPos_flattened / sideLength2, yz / sideLength, yz % sideLength);
        UpdatePosition(firstCubePosition);
        parent.GetChild(startPos_flattened).gameObject.SetActive(true);
        int next;
        int next_flattened;
        while (cubeCount > 0)
        {
            next = Random.Range(0, PossiblePositions.Count);
            UpdatePosition(PossiblePositions[next]);
            next_flattened = PossiblePositions[next].x * sideLength2 + PossiblePositions[next].y * sideLength + PossiblePositions[next].z;
            parent.GetChild(next_flattened).gameObject.SetActive(true);
            PossiblePositions.RemoveAt(next);
            --cubeCount;
        }
    }

    private void ClearPosition()
    {
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
            if (OccupyingPositions.Contains(v)) continue;
            PossiblePositions.Add(v);
        }
    }
}
