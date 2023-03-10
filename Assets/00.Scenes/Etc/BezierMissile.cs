using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BezierMissile : MonoBehaviour
{
    #region Trail Color
    [SerializeField]
    private Color _attackColor;
    [SerializeField]
    private Color _defenceColor;
    [SerializeField]
    private Color _statusColor;
    [SerializeField]
    private Color _utillColor;
    #endregion

    private Vector2[] m_points = new Vector2[4];

    private float m_timerMax = 0;
    private float m_timerCurrent = 0;
    private float m_speed;

    private RectTransform _rect;

    private Action _endAction;

    private TrailRenderer _trail;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _trail = GetComponent<TrailRenderer>();
    }

    public void Init(RectTransform _startTr, RectTransform _endTr, float _speed, float _newPointDistanceFromStartTr, float _newPointDistanceFromEndTr, Action action = null)
    {
        m_speed = _speed;

        // 끝에 도착할 시간을 랜덤으로 줌.
        m_timerMax = Random.Range(0.8f, 1.0f);

        // 시작 지점.
        m_points[0] = Camera.main.ScreenToWorldPoint(_startTr.anchoredPosition);

        // 시작 지점을 기준으로 랜덤 포인트 지정.
        m_points[1] = Camera.main.ScreenToWorldPoint(_startTr.anchoredPosition3D) +
            (_newPointDistanceFromStartTr * Random.Range(-1.0f, 1.0f) * _startTr.right) + // X (좌, 우 전체)
            (_newPointDistanceFromStartTr * Random.Range(-0.15f, 1.0f) * _startTr.up);  // Y (아래쪽 조금, 위쪽 전체)
            //(_newPointDistanceFromStartTr * Random.Range(-1.0f, -0.8f) * _startTr.forward); // Z (뒤 쪽만)

        // 도착 지점을 기준으로 랜덤 포인트 지정.
        m_points[2] = _endTr.anchoredPosition3D +
            (_newPointDistanceFromEndTr * Random.Range(-1.0f, 1.0f) * _endTr.right) + // X (좌, 우 전체)
            (_newPointDistanceFromEndTr * Random.Range(-1.0f, 1.0f) * _endTr.up); // Y (위, 아래 전체)
            //(_newPointDistanceFromEndTr * Random.Range(0.8f, 1.0f) * _endTr.forward); // Z (앞 쪽만)

        // 도착 지점.
        m_points[3] = _endTr.anchoredPosition;

        _rect.anchoredPosition = _startTr.anchoredPosition;

        _endAction = action;
    }

    public void Init(Vector2 _startTr, Vector2 _endTr, float _speed, float _newPointDistanceFromStartTr, float _newPointDistanceFromEndTr, Action action = null)
    {
        m_speed = _speed;

        // 끝에 도착할 시간을 랜덤으로 줌.
        m_timerMax = Random.Range(0.8f, 1.0f);

        // 시작 지점.
        m_points[0] = _startTr;

        // 시작 지점을 기준으로 랜덤 포인트 지정.
        m_points[1] = _startTr +
            (_newPointDistanceFromStartTr * Random.Range(-1.0f, 1.0f) * new Vector2(_startTr.x, 0)) + // X (좌, 우 전체)
            (_newPointDistanceFromStartTr * Random.Range(-0.15f, 1.0f) * new Vector2(0, _startTr.y));  // Y (아래쪽 조금, 위쪽 전체)
                                                                                        //(_newPointDistanceFromStartTr * Random.Range(-1.0f, -0.8f) * _startTr.forward); // Z (뒤 쪽만)

        // 도착 지점을 기준으로 랜덤 포인트 지정.
        m_points[2] = _endTr +
            (_newPointDistanceFromEndTr * Random.Range(-1.0f, 1.0f) * new Vector2(_endTr.x, 0)) + // X (좌, 우 전체)
            (_newPointDistanceFromEndTr * Random.Range(-1.0f, 1.0f) * new Vector2(0, _endTr.y)); // Y (위, 아래 전체)
                                                                                  //(_newPointDistanceFromEndTr * Random.Range(0.8f, 1.0f) * _endTr.forward); // Z (앞 쪽만)

        // 도착 지점.
        m_points[3] = _endTr;

        _rect.anchoredPosition = _startTr;

        _endAction = action;
    }

    public void SetTrailColor(EffectType type)
    {
        switch (type)
        {
            case EffectType.Attack:
                _trail.startColor = _attackColor;
                _trail.endColor = _attackColor;
                break;
            case EffectType.Defence:
                _trail.startColor = _defenceColor;
                _trail.endColor = _defenceColor;
                break;
            case EffectType.Status:
                _trail.startColor = _statusColor;
                _trail.endColor = _statusColor;
                break;
            case EffectType.Destroy:
                _trail.startColor = _attackColor;
                _trail.endColor = _attackColor;
                break;
            case EffectType.Draw:
                _trail.startColor = _utillColor;
                _trail.endColor = _utillColor;
                break;
            case EffectType.Etc:
                break;
        }
    }

    public void SetEffect(GameObject go)
    {
        GameObject g = Instantiate(go, this.transform);
        g.transform.localScale *= 3f;
        //g.transform.DOScale(Vector3.one * 200f, 0.2f);
    }

    void Update()
    {
        if (m_timerCurrent > m_timerMax)
        {
            return;
        }

        // 경과 시간 계산.
        m_timerCurrent += Time.deltaTime * m_speed;

        // 베지어 곡선으로 X,Y,Z 좌표 얻기.
        _rect.anchoredPosition = new Vector2(
            CubicBezierCurve(m_points[0].x, m_points[1].x, m_points[2].x, m_points[3].x),
            CubicBezierCurve(m_points[0].y, m_points[1].y, m_points[2].y, m_points[3].y)
            //CubicBezierCurve(m_points[0].z, m_points[1].z, m_points[2].z, m_points[3].z)
        );

        if(_rect.anchoredPosition == m_points[3])
        {
            // 도착한거임
            _endAction?.Invoke();

            // 뭔가하고
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 3차 베지어 곡선.
    /// </summary>
    /// <param name="start">시작 위치</param>
    /// <param name="startOffset">시작 위치에서 얼마나 꺾일 지 정하는 위치</param>
    /// <param name="endOffset">도착 위치에서 얼마나 꺾일 지 정하는 위치</param>
    /// <param name="end">도착 위치</param>
    /// <returns></returns>
    private float CubicBezierCurve(float start, float startOffset, float endOffset, float end)
    {
        // (0~1)의 값에 따라 베지어 곡선 값을 구하기 때문에, 비율에 따른 시간을 구했다.
        float t = m_timerCurrent / m_timerMax; // (현재 경과 시간 / 최대 시간)

        // 방정식.
        /*
        return Mathf.Pow((1 - t), 3) * a
            + Mathf.Pow((1 - t), 2) * 3 * t * b
            + Mathf.Pow(t, 2) * 3 * (1 - t) * c
            + Mathf.Pow(t, 3) * d;
        */

        // 이해한대로 편하게 쓰면.
        float startLerp = Mathf.Lerp(start, startOffset, t);
        float offsetLerp = Mathf.Lerp(startOffset, endOffset, t);
        float endLerp = Mathf.Lerp(endOffset, end, t);

        float startOffsetLerp = Mathf.Lerp(startLerp, offsetLerp, t);
        float endOffsetLerp = Mathf.Lerp(offsetLerp, endLerp, t);

        return Mathf.Lerp(startOffsetLerp, endOffsetLerp, t);
    }
}
