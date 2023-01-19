using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public enum RuneType
{
    Assist, // ����
    Main, // ����
}

public class MagicCircle : MonoBehaviour
{
    private Dictionary<RuneType, List<Card>> _runeDict;

    private const int _mainRuneCnt = 1;

    [SerializeField]
    private float _assistRuneDistance = 3f;

    [SerializeField]
    private float _cardAreaDistance = 5f;

    [SerializeField]
    private Card _runeTemplate;
    [SerializeField]
    private Card _garbageRuneTemplate;

    public Enemy enemy;

    public void Awake()
    {
        _runeDict = new Dictionary<RuneType, List<Card>>();
    }

    public void SortCard()
    {
        if (_runeDict.ContainsKey(RuneType.Assist))
        {
            float angle = -2 * Mathf.PI / _runeDict[RuneType.Assist].Count;

            for (int i = 0; i < _runeDict[RuneType.Assist].Count; i++)
            {
                //_runeDict[RuneType.Assist][i].GetComponent<RectTransform>().transform.rotation = Quaternion.Euler(0, 0, -1 * angle * i + 90);
                //_runeDict[RuneType.Assist][i].GetComponent<RectTransform>().anchoredPosition = new Vector3(_assistRuneDistance, 0, 0);
                _runeDict[RuneType.Assist][i].GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
                float height = Mathf.Sin(angle * i + (90 * Mathf.Deg2Rad)) * _assistRuneDistance;
                float width = Mathf.Cos(angle * i + (90 * Mathf.Deg2Rad)) * _assistRuneDistance;
                _runeDict[RuneType.Assist][i].GetComponent<RectTransform>().anchoredPosition = new Vector3(width, height, 0);
            }
        }

        if (_runeDict.ContainsKey(RuneType.Main))
        {
            _runeDict[RuneType.Main][0].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            _runeDict[RuneType.Main][0].GetComponentInParent<RectTransform>().transform.rotation = Quaternion.identity;
        }
    }

    public bool AddCard(Card card)
    {
        // �̸� ������ ���� �� ��ü���� �հ����� ���� �� �����鿡 ������Ű��
        if (_runeDict.ContainsKey(RuneType.Main) == false || (_runeDict[RuneType.Main].Count == 0))
        {
            if (!DummyCost.Instance.CanUseMainRune(card.Rune.MainRune.Cost))
            {
                Debug.Log("���� ���� ����ϱ� ���� ������ �����մϴ�.");
                return false;
            }
            if (_runeDict.ContainsKey(RuneType.Main))
            {
                if (_runeDict[RuneType.Main].Count >= _mainRuneCnt)
                {
                    Debug.Log("���� ���� ���� �ֽ��ϴ�.");
                    return false;
                }

                GameObject go = Instantiate(_runeTemplate.gameObject, this.transform);
                Card rune = go.GetComponent<Card>();
                rune.SetRune(card.Rune);
                rune.SetIsEquip(true);
                rune.SetCoolTime(card.Rune.MainRune.DelayTurn);
                _runeDict[RuneType.Main].Add(rune);

                for (int i = 0; i < _runeDict[RuneType.Main][0].Rune.AssistRuneCount; i++)
                {
                    GameObject ggo = Instantiate(_runeTemplate.gameObject, this.transform);
                    Card grune = ggo.GetComponent<Card>();
                    grune.SetRune(null);
                    grune.SetIsEquip(true);
                    //grune.CardAnimation();
                    if (_runeDict.ContainsKey(RuneType.Assist))
                    {
                        _runeDict[RuneType.Assist].Add(grune);
                    }
                    else
                    {
                        _runeDict.Add(RuneType.Assist, new List<Card> { grune });
                    }
                }

                SortCard();
                AssistRuneAnimanation();
            }
            else
            {
                GameObject go = Instantiate(_runeTemplate.gameObject, this.transform);
                Card rune = go.GetComponent<Card>();
                rune.SetRune(card.Rune);
                rune.SetIsEquip(true);
                rune.SetCoolTime(card.Rune.MainRune.DelayTurn);
                _runeDict.Add(RuneType.Main, new List<Card>() { rune });

                for (int i = 0; i < _runeDict[RuneType.Main][0].Rune.AssistRuneCount; i++)
                {
                    GameObject ggo = Instantiate(_runeTemplate.gameObject, this.transform);
                    Card grune = ggo.GetComponent<Card>();
                    grune.SetRune(null);
                    grune.SetIsEquip(true);
                    //grune.CardAnimation();
                    if (_runeDict.ContainsKey(RuneType.Assist))
                    {
                        _runeDict[RuneType.Assist].Add(grune);
                    }
                    else
                    {
                        _runeDict.Add(RuneType.Assist, new List<Card> { grune });
                    }
                }
                SortCard();
                AssistRuneAnimanation();
            }
        }
        else
        {
            /*
            if (_runeDict.ContainsKey(RuneType.Assist))
            {
                if (_runeDict[RuneType.Assist].Count >= _assistRuneCnt)
                {
                    Debug.Log("���� ���� ���� �ֽ��ϴ�.");
                    return false;
                }
                GameObject go = Instantiate(_runeTemplate.gameObject, this.transform);
                Card rune = go.GetComponent<Card>();
                rune.SetRune(card.Rune);
                rune.SetCoolTime(card.Rune.AssistRune.DelayTurn);
                _runeDict[RuneType.Assist].Add(rune);
            }
            else
            {
                GameObject go = Instantiate(_runeTemplate.gameObject, this.transform);
                Card rune = go.GetComponent<Card>();
                rune.SetRune(card.Rune);
                rune.SetCoolTime(card.Rune.AssistRune.DelayTurn);
                _runeDict.Add(RuneType.Assist, new List<Card>() { rune });
            }
            */

            // ��ó�� �ִ� ���� ������ ����

            // ��ü Ž�� �� �ڱ���� �Ÿ� �� ���� ����� �ַ� ��ü

            if (!DummyCost.Instance.CanUseSubRune(card.Rune.AssistRune.Cost))
            {
                Debug.Log("���� ���� ����ϱ� ���� ������ �����մϴ�.");
                return false;
            }

            int changeIndex = -1;
            for (int i = 0; i < _runeDict[RuneType.Assist].Count; i++)
            {
                if (_runeDict[RuneType.Assist][i].Rune == null)
                {
                    changeIndex = i;
                    break;
                }
            }

            if (changeIndex == -1) return false;

            // �ִϸ��̼� �ڷ� �̷��
            Sequence seq = DOTween.Sequence();
            seq.AppendCallback(() =>
            {
                GameObject g = Instantiate(_garbageRuneTemplate.gameObject, this.transform);
                card.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
                card.transform.SetParent(this.transform);
                g.GetComponent<RectTransform>().anchoredPosition = card.GetComponent<RectTransform>().anchoredPosition;
                g.GetComponent<RectTransform>().DOAnchorPos(_runeDict[RuneType.Assist][changeIndex].GetComponent<RectTransform>().anchoredPosition, 0.3f).OnComplete(() =>
                {
                    Destroy(g);
                    _runeDict[RuneType.Assist][changeIndex].SetRune(card.Rune);
                });
            });

            SortCard();
        }

        return true;
    }

    public void AssistRuneAnimanation()
    {
        foreach (var r in _runeDict[RuneType.Assist])
        {
            r.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.3f).From();
        }
    }

    public void Damage()
    {
        // �׳� ������ ������´ٴ� ����, �ٸ� �����ٰ� �ű� ����

        // ������ ȸ�� ȿ��
        // ���̿� �߰��� �ٸ� ȿ���� �ְ���
        // �� �Ŀ� ����

        if (_runeDict[RuneType.Main].Count == 0 || _runeDict[RuneType.Main][0].Rune == null)
        {
            // ���� �ȵǴ� ����Ʈ
            // ��.. ī�޶� ��鸲�̶����
            return;
        }

        Sequence seq = DOTween.Sequence();
        seq.Append(this.transform.DORotate(new Vector3(0, 0, 360 * 5), 0.7f, RotateMode.LocalAxisAdd).SetEase(Ease.OutCubic));

        //int damage = 0;
        seq.AppendInterval(0.1f);
        seq.AppendCallback(() =>
        {
            if (_runeDict.ContainsKey(RuneType.Assist))
            {
                for (int i = 0; i < _runeDict[RuneType.Assist].Count; i++)
                {
                    //damage += _runeDict[RuneType.Assist][i]._runeSO.assistRuneValue;
                    //damage += _runeDict[RuneType.Assist][i].Rune.AssistRune.EffectPair
                    Card card = _runeDict[RuneType.Assist][i];
                    if (card.Rune != null)
                    {
                        card.UseAssistEffect();
                    }
                }

                if (_runeDict.ContainsKey(RuneType.Assist))
                {
                    for (int i = 0; i < _runeDict[RuneType.Assist].Count; i++)
                    {
                        Destroy(_runeDict[RuneType.Assist][i].gameObject);
                    }
                }
                //damage += _runeDict[RuneType.Main][i]._runeSO.mainRuneValue;
            }
        });
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() =>
        {
            if (_runeDict.ContainsKey(RuneType.Main))
            {
                for (int i = 0; i < _runeDict[RuneType.Main].Count; i++)
                {
                    //damage += _runeDict[RuneType.Main][i]._runeSO.mainRuneValue;
                    Card card = _runeDict[RuneType.Main][i];
                    if (card.Rune != null)
                    {
                        card.UseMainEffect();
                    }
                }

                if (_runeDict.ContainsKey(RuneType.Main))
                {
                    for (int i = 0; i < _runeDict[RuneType.Main].Count; i++)
                    {
                        Destroy(_runeDict[RuneType.Main][i].gameObject);
                    }
                }
            }
        });
        seq.AppendCallback(() => Debug.Log("Attack Complate"));
        seq.AppendCallback(() =>
        {
            _runeDict.Clear();
        });

        //enemy.Damage(damage);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _cardAreaDistance);
        Gizmos.color = Color.white;
    }
#endif
}
