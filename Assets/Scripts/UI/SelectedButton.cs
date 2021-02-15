using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectedButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private bool defaultSelection = false;
    [SerializeField] private Image indicator = null;
    [SerializeField] private Animator animator = null;


    private void Start()
    {
        if (defaultSelection)
        {
            TryGetComponent(out Button thisButton);
            thisButton.Select();
        }
        else
        {
            if (indicator != null)
            {
                indicator.enabled = false;
            }
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (indicator != null)
        {
            if (!indicator.enabled)
                indicator.enabled = true;
        }

        if (animator != null)
        {
            if (!animator.GetBool("Blink"))
                animator.SetBool("Blink", true);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (indicator != null)
        {
            if (indicator.enabled)
                indicator.enabled = false;
        }

        if (animator != null)
        {
            if (animator.GetBool("Blink"))
                animator.SetBool("Blink", false);
        }
    }

    
}
