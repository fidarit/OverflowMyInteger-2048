using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private CubeController prefab;
    [SerializeField] private Vector2Int fieldSize = new Vector2Int(9, 9);

    private bool isMoving = false;

    private void Start()
    {
        CreateRandomCubes(5);
    }

    private void Update()
    {
        if (isMoving)
            return;

        var x = Mathf.RoundToInt(Input.GetAxis("Horizontal"));
        var y = Mathf.RoundToInt(Input.GetAxis("Vertical"));

        if (x == y)
            return;

        if (x != 0 && y != 0)
            return;

        StartCoroutine(Move(new Vector2Int(x, y)));
    }

    private IEnumerator Move(Vector2Int dir)
    {
        isMoving = true;

        CubeController.MoveAll(dir);

        while (isMoving)
        {
            yield return new WaitForSeconds(0.25f);

            if (CubeController.AllCubes.All(t => t.Rigidbody.IsSleeping()))
                break;
        }

        CreateRandomCubes();

        isMoving = false;
    }

    public void CreateRandomCubes()
    {
        int maxCountGlobal = fieldSize.x * fieldSize.y;

        if (CubeController.AllCubes.Count >= maxCountGlobal)
            return;

        int minCount = 1;
        int maxCount = Mathf.Max(minCount + 1, (maxCountGlobal - CubeController.AllCubes.Count) / 3);

        int count = Random.Range(minCount, maxCount);

        CreateRandomCubes(count);
    }

    public void CreateRandomCubes(int count)
    {
        var allPoints = new List<Vector3>();

        for (int x = 0; x < fieldSize.x; x++)
        {
            for (int y = 0; y < fieldSize.y; y++)
            {
                var dx = x - fieldSize.x * 0.5f;
                var dy = y - fieldSize.y * 0.5f;

                allPoints.Add(new Vector3(dx, 0, dy));
            }
        }

        var busyPoints = CubeController.AllCubes.Select(t => RoundVectorToHalf(t.transform.position));
        var availablePoints = allPoints.Except(busyPoints);

        foreach (var point in Randomize(availablePoints).Take(count))
        {
            Instantiate(prefab, point, Quaternion.identity);
        }
    }

    private static Vector3 RoundVectorToHalf(Vector3 input, float y = 0)
    {
        var x = 0.5f * Mathf.Round(input.x * 2);
        var z = 0.5f * Mathf.Round(input.z * 2);

        return new Vector3(x, y, z);
    }

    private static IEnumerable<T> Randomize<T>(IEnumerable<T> input)
    {
        return input.OrderBy(t => Random.Range(short.MinValue, short.MaxValue));
    }
}
