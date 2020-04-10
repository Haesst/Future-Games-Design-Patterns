using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Pools/GameObject")]
public class GameObjectScriptablePool : ScriptableObject, IPool<GameObject>
{
    [SerializeField] private GameObject m_Prefab = default;
    [SerializeField] private uint m_InitSize = 1;
    [SerializeField] private uint m_ExpandBy = 1;
    [SerializeField] private bool m_HasParent = false;
    [SerializeField] private bool m_ShareParentIfExist = false;
    [SerializeField] private string m_ParentName = default;

    private GameObjectPool m_InternalPool;

    public GameObject Rent(bool returnActive)
    {
        InitPool();

        return m_InternalPool.Rent(returnActive);
    }

    public void InitPool()
    {
        if (m_InternalPool == null)
        {
            m_InternalPool = new GameObjectPool(
                                    m_InitSize,
                                    m_Prefab,
                                    m_ExpandBy <= 0 ? 1 : m_ExpandBy,
                                    GetParentTransform()
                                   );
        }
    }

    private Transform GetParentTransform()
    {
        if(m_HasParent)
        {
            if(m_ShareParentIfExist)
            {
                GameObject parentObject = GameObject.Find(m_ParentName);

                if(parentObject != null)
                {
                    return parentObject.transform;
                }
            }

            return new GameObject(m_ParentName).transform;
        }

        return null;
    }

    private void OnDestroy()
    {
        m_InternalPool.Dispose();
    }
}
