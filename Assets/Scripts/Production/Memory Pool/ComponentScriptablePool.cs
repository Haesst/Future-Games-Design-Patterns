using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Pools/Component")]
public class ComponentScriptablePool<T> : ScriptableObject, IPool<T> where T : Component
{
    [SerializeField] private T m_Prefab = default;
    [SerializeField] private uint m_InitSize = 1;
    [SerializeField] private uint m_ExpandBy = 1;
    [SerializeField] private bool m_HasParent = false;
    [SerializeField] private bool m_ShareParentIfExist = false;
    [SerializeField] private string m_ParentName = default;

    ComponentPool<T> m_InternalPool;

    public T Rent(bool returnActive)
    {
        if (m_InternalPool == null)
        {
            m_InternalPool = new ComponentPool<T>(
                                    m_InitSize,
                                    m_Prefab,
                                    m_ExpandBy <= 0 ? 1 : m_ExpandBy,
                                    GetParentTransform()
                                   );
        }

        return m_InternalPool.Rent(returnActive);
    }

    private Transform GetParentTransform()
    {
        if (m_HasParent)
        {
            if (m_ShareParentIfExist)
            {
                GameObject parentObject = GameObject.Find(m_ParentName);

                if (parentObject != null)
                {
                    return parentObject.transform;
                }
            }

            return new GameObject(m_ParentName).transform;
        }

        return null;
    }

    public void Dispose()
    {
        m_InternalPool.Dispose();
    }
}