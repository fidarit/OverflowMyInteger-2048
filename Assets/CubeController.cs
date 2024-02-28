using System.Collections;
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

    public static void MoveAll(Vector2Int impulse)
    {
        AllCubes.ForEach(t => t.StartCoroutine(t.Move(impulse)));
    }

    private IEnumerator Move(Vector2Int impulse)
    {
        if (impulse.x != 0)
            Rigidbody.constraints = RigidbodyConstraints.FreezeAll & ~RigidbodyConstraints.FreezePositionX;

        else if(impulse.y != 0)
            Rigidbody.constraints = RigidbodyConstraints.FreezeAll & ~RigidbodyConstraints.FreezePositionZ;

        Rigidbody.velocity = Vector3.right * impulse.x + Vector3.forward * impulse.y;

        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.1f);

            if (Rigidbody.IsSleeping())
                break;
        }

        transform.position = transform.position.RoundToHalf();

        Rigidbody.Sleep();
        Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
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
