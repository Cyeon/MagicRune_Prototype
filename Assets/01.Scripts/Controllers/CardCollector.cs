using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NaughtyAttributes;

public class CardCollector : MonoBehaviour
{
    [SerializeField]
    private MagicCircle _magicCircle;

    [SerializeField, MinValue(0f), MaxValue(1440f)]
    private float _cardAreaDistance;
    [SerializeField]
    private Vector2 _offset;

    [SerializeField]
    private CardsViewUI _deckViewUI = null;

    [SerializeField]
    private CardsViewUI _restViewUI = null;

    [SerializeField]
    private TMP_Text _restAmountText = null;

    [SerializeField]
    private int _cardCnt;

    [SerializeField]
    private CardListSO _deck = null;

    [SerializeField]
    private List<Card> _deckCards = null;

    [SerializeField]
    private List<Card> _handCards = null;

    [SerializeField]
    private List<Card> _restCards = null;

    private Vector2 _cardOriginPos;
    private int _uiIndex;

    private Card _selectCard;
    public Card SelectCard
    {
        get => _selectCard;
        private set
        {
            if (value != null)
            {
                _selectCard = value;
                _cardOriginPos = _selectCard.GetComponent<RectTransform>().anchoredPosition;
                _magicCircle.IsBig = true;
            }
            else
            {
                // ๋ง์ฝ ? ํ ์นด๋๊ฐ ๋ง๋ฒ์ง??์ ?๋ค๋ฉ?
                if (Vector2.Distance(Input.GetTouch(0).position, _magicCircle.GetComponent<RectTransform>().anchoredPosition)
                <= _magicCircle.CardAreaDistance)
                {
                    if((_magicCircle.RuneDict.ContainsKey(RuneType.Main) == true && _isFront == true) || (_magicCircle.RuneDict.ContainsKey(RuneType.Main) == false && _isFront == false))
                    {
                        Card isAdd = _magicCircle.AddCard(SelectCard);
                        if (isAdd != null)
                        {
                            _handCards.Remove(isAdd);
                            //SelectCard.IsRest = true;
                            _restCards.Add(isAdd);

                            //SelectCard.gameObject.SetActive(false);
                        }
                    }
                }
                // YES : ๋ง๋ฒ์ง??์ ?ฃ๊ธฐ, ๋ฆฌ์ค???์ ์นด๋ ์ง?ฐ๊ธฐ
                _selectCard.GetComponent<RectTransform>().anchoredPosition = _cardOriginPos;
                _selectCard = value;
                _magicCircle.SortCard();
                CardSort();
            }
        }
    }

    public IReadOnlyList<Card> DeckCards => _deckCards;
    public IReadOnlyList<Card> RestCards => _restCards;

    private bool _isFront = true;

    private void Awake()
    {
        //_cardList = new List<Card>();
        EventManager.StartListening(Define.ON_END_MONSTER_TURN, CoolTimeDecrease);
    }

    // 2960 * 1440
    private void Start()
    {
        for (int i = 0; i < _deck.cards.Count; i++)
        {
            GameObject go = Instantiate(_deck.cards[i], this.transform);
            _deckCards.Add(go.GetComponent<Card>());
            go.SetActive(false);
            go.name = $"Card_{i + 1}";
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
        }

        CardDraw(_cardCnt);
        UIUpdate();

        _isFront = true;
    }

    private void Update()
    {
        if (SelectCard != null)
        {
            if (Input.touchCount <= 0) return;
            SelectCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y - this.GetComponent<RectTransform>().anchoredPosition.y);

            if (_magicCircle.IsBig == true)
            {
                if (Vector2.Distance(Input.GetTouch(0).position, _magicCircle.GetComponent<RectTransform>().anchoredPosition)
                    <= _magicCircle.CardAreaDistance)
                {
                    SelectCard.RuneAreaParent.gameObject.SetActive(true);
                    SelectCard.CardAreaParent.gameObject.SetActive(false);
                    //SelectCard.GetComponent<RectTransform>().sizeDelta = new Vector2(128, 128);
                }
                else
                {
                    SelectCard.RuneAreaParent.gameObject.SetActive(false);
                    SelectCard.CardAreaParent.gameObject.SetActive(true);
                    //SelectCard.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 500);
                }
            }
        }
    }

    private void CardDraw(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int idx = Random.Range(0, _deckCards.Count);
            Card card = _deckCards[idx];
            _deckCards.Remove(card);
            _handCards.Add(card);
            card.gameObject.SetActive(true);
        }
        CardSort();
        UIUpdate();
    }

    private void CardSort()
    {
        _handCards.Sort(delegate (Card a, Card b)
        {
            int aName = int.Parse(a.gameObject.name.ToString().Substring(5, a.gameObject.name.Length - 5));
            int bName = int.Parse(b.gameObject.name.ToString().Substring(5, b.gameObject.name.Length - 5));
            if (aName > bName) { return 1; }
            return -1;
        });

        for (int i = 0; i < _handCards.Count; i++)
        {
            //?ด๊ฑธ ?ด์ค??Animation???ํด MagicCircle???์?ผ๋ก ?ฃ์??๊ฒ๋ ?ค์ ???จ์ ?์?ผ๋ก ?์? ?์?์ผ๋ก?Sort๊ฐ ?๋??๊ทธ๋ฌ๋ฉ?Damage๋ถ๋ถ์???ค๋ฅ๊ฐ ??๋ช?๋ฃ?
            //_handCards[i].transform.SetParent(this.transform); 
            RectTransform rect = _handCards[i].GetComponent<RectTransform>();
            float xDelta = 1440f / _handCards.Count;
            rect.anchoredPosition = new Vector3(i * xDelta + rect.sizeDelta.x / 2 + 150 + _offset.x, rect.sizeDelta.y / 2 + _offset.y, 0);
        }
    }

    public void CardSelect(Card card)
    {
        if (card == null)
        {
            SelectCard.transform.SetSiblingIndex(_uiIndex);
            _uiIndex = -1;
            SelectCard.RuneAreaParent.gameObject.SetActive(false);
            SelectCard.CardAreaParent.gameObject.SetActive(true);
        }
        else
        {
            _uiIndex = card.transform.GetSiblingIndex();
            card.transform.SetAsLastSibling();
        }
        SelectCard = card;
    }

    private void CoolTimeDecrease()
    {
        for (int i = _restCards.Count - 1; i >= 0; i--)
        {
            Card card = _restCards[i];
            card.CoolTime--;
            if (card.CoolTime <= 0)
            {
                _deckCards.Add(card);
                _restCards.Remove(card);
            }
        }
        UIUpdate();
    }

    public void CardRotate()
    {
        Sequence seq = DOTween.Sequence();
        foreach(var card in _handCards)
        {
            seq.Join(card.transform.DORotate(new Vector3(0, 360, 0), 0.3f, RotateMode.FastBeyond360));
        }

        foreach (var card in _handCards)
        {
            seq.InsertCallback(0.15f, () =>
            {
                card.transform.rotation = Quaternion.Euler(0, 0, 0);
                card.IsFront = !card.IsFront;
            });
        }

        _isFront = _handCards[0].IsFront;
    }

    private void OnDestroy()
    {
        EventManager.StopListening(Define.ON_END_MONSTER_TURN, CoolTimeDecrease);
    }

    private void UIUpdate()
    {
        _deckViewUI?.UITextUpdate();
        _restViewUI?.UITextUpdate();
    }
}