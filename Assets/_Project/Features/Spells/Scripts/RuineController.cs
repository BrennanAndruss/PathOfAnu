// ...existing code...
using UnityEngine;
using _Project.Features.Spells.Scripts;

public class RuineController : MonoBehaviour
{
    [Header("Rune Requirement")]
    [SerializeField] private SpellType requiredSpell;

    [Header("State")]
    [SerializeField] private bool activated;

    [Header("VFX")]
    [SerializeField] private GameObject correctSpellVFX;
    [SerializeField] private GameObject incorrectSpellVFX;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        // use the Spell base type and its accessor
        Spell spell = other.GetComponent<Spell>();
        if (spell == null) return;

        if (spell.GetSpellType() == requiredSpell)
        {
            if (correctSpellVFX != null)
                Instantiate(correctSpellVFX, transform.position, Quaternion.identity);

            activated = true;
        }
        else
        {
            if (incorrectSpellVFX != null)
                Instantiate(incorrectSpellVFX, transform.position, Quaternion.identity);
        }
    }
}
