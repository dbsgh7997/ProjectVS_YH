using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<Type, UIBaseController> cachedPanelDict = null;

    private Stack<UIBaseController> panelStack = null;

    public event Action OnHideCallback;

    private const int POPUP_SORTING_ORDER = 100;

    public UIManager()
    {
        cachedPanelDict = new Dictionary<Type, UIBaseController>();
        cachedPanelDict.Clear();

        panelStack = new Stack<UIBaseController>();
        panelStack.Clear();
    }

    ~UIManager()
    {
        ClearPanels();

        cachedPanelDict = null;
        panelStack = null;
    }

    public async UniTask<T> Show<T>(string _panelName = "") where T : UIBaseController
    {
        var panel = await GetCachedPanel<T>(_panelName);
        if (!panel.IsShow())
        {
            panel.Show();
        }
        panelStack.Push(panel);
        panel.SetSortingOrder(POPUP_SORTING_ORDER + panelStack.Count);

        return panel;
    }

    public void Hide()
    {
        if (panelStack.Count > 0)
        {
            var panel = panelStack.Pop();
            panel.Hide();
            OnHideCallback?.Invoke();
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("panelStack Count 0!!!!");
#endif
        }
    }

    public async UniTask<T> GetCachedPanel<T>(string _panelName = "") where T : UIBaseController
    {
        cachedPanelDict.TryGetValue(typeof(T), out var panel);
        if (panel == null)
        {
            panel = await AddCachePanel<T>(_panelName);
        }

        return (T)panel;
    }

    private async UniTask<T> AddCachePanel<T>(string _panelName = "") where T : UIBaseController
    {
        var panel = FindPanelInHierarchy<T>(_panelName);
        if (panel == null)
        {
            panel = await CreatePanel<T>(_panelName);
        }

        cachedPanelDict.Add(typeof(T), panel);

        return panel;
    }

    private async UniTask<T> CreatePanel<T>(string _prefabPath = "") where T : UIBaseController
    {
        var prefab = await Resources.LoadAsync($"Prefabs/Popup/{_prefabPath}", typeof(GameObject)) as GameObject;
        var panelObject = GameObject.Instantiate(prefab);

        return panelObject.GetComponent<T>();
    }

    private GameObject Find(GameObject _item, string _objName)
    {
        if (_item.name == _objName)
            return _item;

        var t = _item.transform;
        for (int i = 0; i < t.childCount; i++)
        {
            var found = Find(t.GetChild(i).gameObject, _objName);
            if (found != null)
                return found;
        }

        return null;
    }

    private GameObject FindGameObject(string _objName)
    {
        GameObject result = null;
        foreach (GameObject root in UnityEngine.Object.FindObjectsByType(typeof(GameObject), 0))
        {            
            if (root.transform.parent == null)
            {
                result = Find(root, _objName);
                if (result != null)
                {
                    break;
                }
            }
        }

        return result;
    }

    private T FindPanelInHierarchy<T>(string _panelName) where T : UIBaseController
    {
        GameObject panelObj = FindGameObject(_panelName);
        if (panelObj == null)
        {
            return null;
        }

        return panelObj.GetComponent<T>();
    }

    public void ClearAllCachedPanel()
    {
        cachedPanelDict.Clear();
    }

    public void ClearAllPanelStack()
    {
        panelStack.Clear();
    }

    public void ClearPanels()
    {
        cachedPanelDict.Clear();
        panelStack.Clear();
    }
}
