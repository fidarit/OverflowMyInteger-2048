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

        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        var vector = new Vector2(x, y);
        var intVector = Vector2Int.RoundToInt(vector.normalized);

        if (Mathf.Abs(intVector.magnitude - 1f) > 0.1f)
            return;

        StartCoroutine(Move(intVector));
    }

    private IEnumerator Move(Vector2Int dir)
    {
        isMoving = true;

        CubeController.MoveAll(dir * Mathf.CeilToInt(fieldSize.magnitude * 2f));

        for (int i = 0; i < 4; i++)
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

        int availableCount = maxCountGlobal - CubeController.AllCubes.Count;
        int minCount = Mathf.Max(1, availableCount / 12);
        int maxCount = Mathf.Max(minCount + 1, availableCount / 3);

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

        var busyPoints = CubeController.AllCubes.Select(t => t.transform.position.RoundToHalf());
        var availablePoints = allPoints
            .Except(busyPoints)
            .OrderByDescending(t => busyPoints.Min(x => (float?)Vector3.Distance(t, x)) ?? Random.value)
            .Take(count * 2)
            .Randomize()
            .Take(count);

        foreach (var point in availablePoints)
        {
            Instantiate(prefab, point, Quaternion.identity);
        }
    }
}
