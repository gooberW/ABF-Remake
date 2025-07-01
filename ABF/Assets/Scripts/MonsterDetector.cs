using UnityEngine;

public class MonsterDetector : MonoBehaviour
{
    [SerializeField] private SanitySystem sanitySystem;
    [SerializeField] private LayerMask monsterLayer;
    [SerializeField] private float maxViewDistance = 10f;

    private void Update()
    {
        RaycastHit hit;
        bool seesMonster = Physics.Raycast(
            transform.position,
            transform.forward,
            out hit,
            maxViewDistance,
            monsterLayer
        );

        sanitySystem.SetLookingAtMonster(seesMonster);
    }
}