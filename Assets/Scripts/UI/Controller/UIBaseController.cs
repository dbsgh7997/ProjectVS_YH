using UnityEngine;
using UnityEngine.UI;

public class UIBaseController : MonoBehaviour
{
    protected Canvas canvas = null;
    protected GraphicRaycaster raycaster = null;

    private bool isShow = false;

    protected virtual void Awake()
    {
        canvas = GetComponent<Canvas>();
        raycaster = GetComponent<GraphicRaycaster>();

        isShow = canvas.enabled;
    }
    protected virtual void Initialize()
    {

    }

    public virtual void Show()
    {
        if (canvas == null)
        {
            return;
        }

        isShow = true;
        canvas.enabled = true;
        raycaster.enabled = true;
    }

    public virtual void Hide()
    {
        if (canvas == null)
        {
            return;
        }

        isShow = false;
        canvas.enabled = false;
        raycaster.enabled = false;
    }

    public void SetSortingOrder(int _order)
    {
        if (canvas == null)
        {
            return;
        }

        canvas.sortingOrder = _order;
    }

    public virtual bool IsShow()
    {
        if (canvas == null)
        {
            return false;
        }

        return canvas.enabled && isShow;
    }
}
