using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    public static List<CubeController> AllCubes { get; } = new List<CubeController>();

    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private TMP_Text m_text;
    
    private long number = 1;

    public Rigidbody Rigidbody => m_rigidbody;

    public long Number
    {
        get => number;
        private set
        {
            m_text.text = FormatNumber(value);

            number = value;
        }
    }

    private void Awake()
    {
        AllCubes.Add(this);

        if (m_rigidbody == null)
            m_rigidbody = GetComponent<Rigidbody>();

        Number = GetRandomNumber();
    }

    private void OnDestroy()
    {
        AllCubes.Remove(this);
    }

    private static int GetRandomNumber()
    {
        var power = Random.Range(1, 5);

        return Mathf.RoundToInt(Mathf.Pow(2, power));
    }

    public static string FormatNumber(long number)
    {
        string[] suffixes = { "", "K", "M", "G", "T", "P", "E", "Z", "Y" };
        int suffixIndex = 0;

        while (number >= 1024 && suffixIndex < suffixes.Length - 1)
        {
            number /= 1024;
            suffixIndex++;
        }

        return $"{number}{suffixes[suffixIndex]}";
    }

    public static void MoveAll(Vector2Int dir)
    {
        AllCubes.ForEach(t => t.Move(dir));
    }

    public void Move(Vector2Int dir)
    {

    }

    public void Destroy()
    {
        this.enabled = false;
        this.gameObject.SetActive(false);

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var otherCubeController = collision.collider.GetComponent<CubeController>();

        if (otherCubeController == null)
            return;

        if (otherCubeController.Rigidbody.IsSleeping())
            return;

        var impulseDir = collision.impulse.normalized;
        var dirToOther = (collision.transform.position - transform.position).normalized;

        var angle = Vector3.Angle(dirToOther, impulseDir);

        if (angle > 5f) //Это должен быть объект, в которого прилетели
            return;

        if (Number != otherCubeController.Number)
            return;

        otherCubeController.Destroy();

        Number *= 2;
    }
}
