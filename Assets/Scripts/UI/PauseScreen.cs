using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class PauseScreen : MonoBehaviour
{
    public GameObject parentObject;
    private GameObject[] childObjects;

    private void Awake()
    {
        childObjects = new GameObject[parentObject.transform.childCount];

        for (int i = 0; i < childObjects.Length; i++)
        {
            childObjects[i] = parentObject.transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < childObjects.Length; i++)
        {
            childObjects[i].SetActive(false);
        }
    }

    public void OnGamePause()
    {
        for (int i = 0; i < childObjects.Length; i++)
        {
            childObjects[i].SetActive(true);
        }
    }

    public void OnGameUnpause()
    {
        for (int i = 0; i < childObjects.Length; i++)
        {
            childObjects[i].SetActive(false);
        }
    }
}
