using Unity.VisualScripting;
using UnityEngine;

public class RefreshObjectIfArenaFailed : MonoBehaviour
{

    [SerializeField] private ArenaBattleTrigger arenaBattleTrigger;

    private void Start()
    {
        if (arenaBattleTrigger.arenaBattleComplete)
        {
            Destroy(gameObject);
        }
    }
}
