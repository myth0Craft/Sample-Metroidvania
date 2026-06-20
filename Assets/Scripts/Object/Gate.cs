using UnityEngine;
using System.Collections;


public class Gate : MonoBehaviour
{

    public Vector2 openPosition;
    public Vector2 closedPosition;

    public float closeSpeed = 1.0f;

    //public bool startOpen = false;

    public bool closed = true;

    private Coroutine currentCoroutine;

    [SerializeField] private string id;

    public bool disableOnOpen = false;

    private void Awake()
    {
        if (id == null || id == "")
        {
            Debug.Log("Id of Gate is null!");
        }
        else
        {
            var room = SaveSystem.getRoom(gameObject.scene.name);

            if (room.pickups.TryGetValue(id, out bool closed))
            {
                this.closed = closed;
            }
        }


        if (!closed)
        {
            if (disableOnOpen)
            {
                gameObject.SetActive(false);
            }
            
            gameObject.transform.localPosition = openPosition;
            closed = false;
        } else
        {
            gameObject.SetActive(true);
            gameObject.transform.localPosition = closedPosition;
            closed = true;
        }
    }

    public void TriggerGate()
    {
        if (closed)
        {
            Open();
            closed = false;

        } else
        {
            Close();
            closed = true;
        }

        if (id == null)
        {
            Debug.Log("Id of Gate is null!");
        }
        else
        {
            var room = SaveSystem.getRoom(gameObject.scene.name);
            room.pickups[id] = closed;
        }

        SaveSystem.Save(PlayerData.saveIndex);
    }

    public void Close()
    {
        gameObject.SetActive(true);

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        SetOpenPosition();

        closed = true;
        currentCoroutine = StartCoroutine(MoveCoroutine(openPosition, closedPosition, false));

    }

    public void Open()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        SetClosedPosition();
        closed = false;

        currentCoroutine = StartCoroutine(MoveCoroutine(closedPosition, openPosition, disableOnOpen));
    }

    public void SetClosedPosition()
    {
        transform.localPosition = closedPosition;
    }

    public void SetOpenPosition()
    {
        transform.localPosition = openPosition;
    }

    private IEnumerator MoveCoroutine(Vector2 startPos, Vector2 endPos, bool disableOnEnd)
    {
        float elapsed = 0f;

        while (elapsed < closeSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / closeSpeed;

            transform.localPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        CamShakeSource.instance.AddVerticalScreenShake(0.08f);

        transform.localPosition = endPos;

        if (disableOnEnd)
        {
            gameObject.SetActive(false);
        }
    }
}

