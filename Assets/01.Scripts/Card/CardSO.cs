using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

/// <summary>
/// 속성 Enum
/// </summary>
public enum AttributeType
{
    Fire, // 불
    Ice, // 얼음
    Wind, // 바람
    None, // 무속성
}

public enum EffectType
{
    Attack, // 공격하는거 예) 5데미지
    Defence, // 방어하는거 예) 5방어
    Etc, // 기타 효과 예) 1장 드로우, 화상효과 부여 등...
}

[Serializable]
public class Pair
{
    [Tooltip("효과 간단 속성?")]
    public EffectType EffectType;
    [ResizableTextArea, Tooltip("카드 효과 텍스트")]
    public string Effect;
}

/// <summary>
/// 룬 정보 클래스
/// </summary>
[Serializable]
public class RuneProperty
{
    [Tooltip("카드 이름")]
    public string Name;
    // 유형 Enum ? 필요한가?
    public List<Pair> EffectDescription;
    [Tooltip("속성")]
    public AttributeType Attribute;
    [Min(0), Tooltip("코스트")]
    public int Cost; // 코스트
    [Min(0), Tooltip("재사용 가능 턴")]
    public int DelayTurn; // 재사용 가능 턴
}

[CreateAssetMenu(menuName = "SO/Rune")]
public class CardSO : ScriptableObject
{
    [ShowAssetPreview(32, 32), Tooltip("카드에 들어갈 이미지")]
    public Sprite CardImage;
    [ShowAssetPreview(32, 32), Tooltip("마법진에 들어갈 룬 이미지")]
    public Sprite RuneImage;
    [Tooltip("카드 설명"), ResizableTextArea]
    public string CardDescription;
    [Min(1), Tooltip("보조 룬 개수")]
    public int AssistRuneCount = 5;

    [Tooltip("메인 룬의 속성")]
    public RuneProperty MainRune;
    [Tooltip("보조 룬의 속성")]
    public RuneProperty AssistRune;
}
