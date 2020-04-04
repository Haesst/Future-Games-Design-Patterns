using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoxymonType
{
    SmallBoxymon,
    BigBoxymon
}

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

    private BoxymonType m_BoxymonType = default;
    private MapData m_MapData = default;
    private IPathFinder m_PathFinder = default;
    private List<Vector2Int> m_Path = new List<Vector2Int>();
    private Vector3 m_NextPoint = default;
    private bool m_GoingToPoint = true;
    [SerializeField] ScriptableBoxymon currentBoxymon;

    private ScriptableBoxymon CurrentBoxymon { get; set; }


    private void Awake()
    {
        SetBoxymonType(BoxymonType.SmallBoxymon);
    }

    void Update()
    {
        if (m_MapData != null)
        {
            if (!m_GoingToPoint)
            {
                Vector2Int goalPosition = m_MapData.End.GetValueOrDefault();
                Vector2Int startPosition = m_MapData.WorldToTilePosition(transform.position);

                m_Path.AddRange(m_PathFinder.FindPath(startPosition, goalPosition));


                if (m_Path.Count > 0)
                {
                    m_NextPoint = m_MapData.TileToWorldPosition(m_Path[0]);
                    m_NextPoint.y = 0.75f;

                    m_GoingToPoint = true;
                }
            }

            if (m_GoingToPoint)
            {
                Vector3 newRotation = Vector3.RotateTowards(transform.forward, m_NextPoint - transform.position, Time.deltaTime * CurrentBoxymon.RotateAngleStep, 0);
                transform.rotation = Quaternion.LookRotation(newRotation);
                transform.position = Vector3.MoveTowards(transform.position, m_NextPoint, CurrentBoxymon.BaseSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, m_NextPoint) < 0.1f)
                {
                    m_GoingToPoint = false;
                }
            }
        }
        //if (m_MapData != null)
        //{
        //    if(m_GoingToPoint)
        //    {
        //        Vector3 newRotation = Vector3.RotateTowards(transform.forward, m_NextPoint - transform.position, Time.deltaTime * CurrentBoxymon.RotateAngleStep, 0);
        //        transform.rotation = Quaternion.LookRotation(newRotation);
        //        transform.position = Vector3.MoveTowards(transform.position, m_NextPoint, CurrentBoxymon.BaseSpeed * Time.deltaTime);

        //        if (Vector3.Distance(transform.position, m_NextPoint) < 0.1f)
        //        {
        //            m_GoingToPoint = false;
        //        }
        //    }
        //    else
        //    {
        //        if (m_Path.Count > 0)
        //        {
        //            m_NextPoint = m_MapData.TileToWorldPosition(m_Path.Dequeue());
        //            m_NextPoint.y = 0.75f;

        //            m_GoingToPoint = true;
        //        }
        //        else
        //        {
        //            Vector2Int goalPosition = m_MapData.End.GetValueOrDefault();
        //            Vector2Int startPosition = m_MapData.WorldToTilePosition(transform.position);

        //            foreach (Vector2Int point in m_PathFinder.FindPath(startPosition, goalPosition))
        //            {
        //                m_Path.Enqueue(point);
        //            }
        //        }
        //    }
        //}
    }

    public void Init(BoxymonType boxymonType, MapData mapData)
    {
        SetBoxymonType(boxymonType);
        m_MapData = mapData;
        m_PathFinder = new BreadthFirst(mapData.accessibles);
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

        Debug.Log(CurrentBoxymon.BodyMaterial);
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
}
