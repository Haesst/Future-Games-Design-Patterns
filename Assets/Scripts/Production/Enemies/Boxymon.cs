using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoxymonType
{
    SmallBoxymon,
    BigBoxymon
}

[SelectionBase]
public class Boxymon : MonoBehaviour
{
    [SerializeField] private ScriptableBoxymon m_SmallBoxymon = default;
    [SerializeField] private ScriptableBoxymon m_BigBoxymon = default;

    [Header("Bodyparts with material")]
    [SerializeField] private MeshRenderer m_Chest = default;
    [SerializeField] private MeshRenderer m_RightArm = default;
    [SerializeField] private MeshRenderer m_LeftArm = default;
    [SerializeField] private MeshRenderer m_RightLeg = default;
    [SerializeField] private MeshRenderer m_LeftLeg = default;
    [SerializeField] private MeshRenderer m_LeftEye = default;
    [SerializeField] private MeshRenderer m_RightEye = default;

    private MapData m_MapData = default;
    private IPathFinder m_PathFinder = default;
    private List<Vector2Int> m_Path = new List<Vector2Int>();
    private Vector3 m_NextPoint = default;
    private bool m_GoingToPoint = false;
    private BoxCollider m_BoxCollider = default;

    private float m_CurrentHealth = default;

    [SerializeField] ScriptableBoxymon currentBoxymon;

    private ScriptableBoxymon CurrentBoxymon { get; set; }


    private void Awake()
    {
        SetBoxymonType(BoxymonType.SmallBoxymon);
        m_BoxCollider = GetComponentInChildren<BoxCollider>();
    }

    void Update()
    {
        if (m_MapData != null)
        {
            if (!m_GoingToPoint)
            {
                if (m_Path.Count <= 0)
                {
                    Vector2Int goalPosition = m_MapData.End.GetValueOrDefault();
                    Vector2Int startPosition = m_MapData.WorldToTilePosition(transform.position);

                    m_Path.AddRange(m_PathFinder.FindPath(startPosition, goalPosition));
                }

                if (m_Path.Count > 0)
                {
                    m_NextPoint = m_MapData.TileToWorldPosition(m_Path[0]);

                    m_NextPoint.y = (m_BoxCollider.bounds.center - m_BoxCollider.bounds.min).y;

                    m_GoingToPoint = true;
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, m_NextPoint) < 0.1f)
                {
                    if (m_MapData.WorldToTilePosition(m_NextPoint) == m_MapData.End)
                    {
                        gameObject.SetActive(false);
                    }
                    m_Path.RemoveAt(0);
                    m_GoingToPoint = false;
                }
            }
        }
    }

    public void FixedUpdate()
    {
        if (m_GoingToPoint)
        {
            Vector3 newRotation = Vector3.RotateTowards(transform.forward, m_NextPoint - transform.position, Time.deltaTime * CurrentBoxymon.RotateAngleStep, 0);
            transform.rotation = Quaternion.LookRotation(newRotation);
            transform.position = Vector3.MoveTowards(transform.position, m_NextPoint, CurrentBoxymon.BaseSpeed * Time.deltaTime);

        }
    }

    public void Init(BoxymonType boxymonType, MapData mapData)
    {
        SetBoxymonType(boxymonType);
        m_MapData = mapData;
        m_Path.Clear();
        m_GoingToPoint = false;
        m_NextPoint = Vector3.zero;
        m_PathFinder = new BreadthFirst(mapData.m_Accessibles);
        m_CurrentHealth = currentBoxymon.Health;
    }

    private void SetBoxymonType(BoxymonType boxymonType, bool updateMaterials = true)
    {
        if(boxymonType == ScriptableBoxymonToBoxymonType(CurrentBoxymon))
        {
            return;
        }

        CurrentBoxymon = boxymonType == BoxymonType.SmallBoxymon ? m_SmallBoxymon : m_BigBoxymon;
        currentBoxymon = CurrentBoxymon;

        UpdateMaterials();
        UpdateScale();
    }

    private void UpdateMaterials()
    {
        UpdateBodyMaterials();
        UpdateEyeMaterials();
    }

    private void UpdateBodyMaterials()
    {
        m_Chest.material = CurrentBoxymon.BodyMaterial;
        m_RightArm.material = CurrentBoxymon.BodyMaterial;
        m_LeftArm.material = CurrentBoxymon.BodyMaterial;
        m_RightLeg.material = CurrentBoxymon.BodyMaterial;
        m_LeftLeg.material = CurrentBoxymon.BodyMaterial;
    }

    private void UpdateEyeMaterials()
    {
        m_LeftEye.material = CurrentBoxymon.EyeMaterial;
        m_RightEye.material = CurrentBoxymon.EyeMaterial;
    }

    private void UpdateScale()
    {
        transform.localScale = CurrentBoxymon.Scale;
    }

    private ScriptableBoxymon BoxymonTypeToScriptableBoxymon(BoxymonType boxymonType)
    {
        return boxymonType == BoxymonType.SmallBoxymon ? m_SmallBoxymon : m_BigBoxymon;
    }

    private BoxymonType ScriptableBoxymonToBoxymonType(ScriptableBoxymon scriptableBoxymon)
    {
        return scriptableBoxymon == m_SmallBoxymon ? BoxymonType.SmallBoxymon : BoxymonType.BigBoxymon;
    }

    public void TakeDamage(float damage)
    {
        m_CurrentHealth -= damage;

        if(m_CurrentHealth <= 0.0f)
        {
            gameObject.SetActive(false);
        }
    }
}
