using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Data;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] private Transform _canvas;
    public Keyword word;

    [Header("Enemy UI")]
    [SerializeField] private Image _enemyPatternIcon;
    [SerializeField] private TextMeshProUGUI _enemyPatternValueText;
    [SerializeField] private TextMeshProUGUI _enemyShieldText;
    [SerializeField] private Transform _enemySlideBar;
    private Slider _enemyHealthSlider;
    private Slider _enemyShieldSlider;
    private Slider _enemyHealthFeedbackSlider;
    private TextMeshProUGUI _enemyHealthText;
    public Transform enemyIcon;
    public Image enemyImage;

    [Header("Player UI")]
    [SerializeField] private Transform _playerSlideBar;
    [SerializeField] private TextMeshProUGUI _playerShieldText;
    private Slider _playerHealthSlider;
    private Slider _playerShieldSlider;
    private Slider _playerHealthFeedbackSlider;
    private TextMeshProUGUI _playerHealthText;

    [Header("?????̻? UI")]
    [SerializeField] private GameObject _statusPrefab;
    [SerializeField] private GameObject _statusPopup;
    [SerializeField] private GameObject _statusTextPopup;
    [SerializeField] private Transform _statusPlayerPanel;
    [SerializeField] private Transform _statusEnemyPanel;

    [Header("Panel")]
    [SerializeField] private GameObject _turnPanel;
    private Image _turnBackground;
    private TextMeshProUGUI _turnText;

    [SerializeField] private GameObject _cardDescPanel;
    private TextMeshProUGUI _cardDescName;
    private Image _cardDescSkillIcon;
    private TextMeshProUGUI _cardDescInfo;
    private TextMeshProUGUI _cardDescMana;
    private TextMeshProUGUI _cardDescCool;
    private Transform _cardDescKeyword;

    [SerializeField] private GameObject _statusDescPanel;
    private TextMeshPro _statusDescName;
    private TextMeshPro _statusDescInfo;

    public GameObject cardAssistPanel;
 
    [Header("ETC")]
    [SerializeField] private GameObject _damagePopup;
    [SerializeField] private GameObject _infoMessage;

    private Slider hSlider = null;
    private Slider sSlider = null;
    private Slider hfSlider = null;
    private TextMeshProUGUI hText = null;

    private GraphicRaycaster _gr;
    public GraphicRaycaster GR => _gr;

    private void Awake()
    {
        _turnBackground = _turnPanel?.GetComponent<Image>();
        _turnText = _turnPanel?.GetComponentInChildren<TextMeshProUGUI>();

        _enemyHealthSlider = _enemySlideBar.Find("HealthBar").GetComponent<Slider>();
        _enemyHealthFeedbackSlider = _enemySlideBar.Find("HealthFeedbackBar").GetComponent<Slider>();
        _enemyShieldSlider = _enemySlideBar.Find("ShieldBar").GetComponent<Slider>();
        _enemyHealthText = _enemyHealthSlider.transform.Find("HealthText").GetComponent<TextMeshProUGUI>();

        _playerHealthSlider = _playerSlideBar.Find("HealthBar").GetComponent<Slider>();
        _playerHealthFeedbackSlider = _playerSlideBar.Find("HealthFeedbackBar").GetComponent<Slider>();
        _playerShieldSlider = _playerSlideBar.Find("ShieldBar").GetComponent<Slider>();
        _playerHealthText = _playerHealthSlider.transform.Find("HealthText").GetComponent<TextMeshProUGUI>();

        _cardDescName = _cardDescPanel.transform.Find("Desc_Name_Text").GetComponent<TextMeshProUGUI>();
        _cardDescSkillIcon = _cardDescPanel.transform.Find("Desc_Skill_Image").GetComponent<Image>();
        _cardDescInfo = _cardDescPanel.transform.Find("Desc_Description").GetComponent<TextMeshProUGUI>();
        _cardDescMana = _cardDescPanel.transform.Find("Desc_Mana").Find("Mana_Text").GetComponent<TextMeshProUGUI>();
        _cardDescCool = _cardDescPanel.transform.Find("Desc_CoolTime").Find("CoolTime_Text").GetComponent<TextMeshProUGUI>();
        _cardDescKeyword = _cardDescPanel.transform.Find("Keyword");

        _statusDescName = _statusDescPanel.transform.Find("Name").GetComponent<TextMeshPro>();
        _statusDescInfo = _statusDescPanel.transform.Find("Infomation").GetComponent<TextMeshPro>();

        _gr = _canvas.GetComponent<GraphicRaycaster>();

        enemyImage = enemyIcon.GetComponent<Image>();
    }

    public void OnDestroy()
    {
        transform.DOKill();
    }

    public void UpdateShieldText(bool isPlayer, float shield)
    {
        if (isPlayer)
        {
            _playerShieldText.text = shield.ToString();

            Sequence seq = DOTween.Sequence();
            seq.Append(_playerShieldText.transform.parent.DOScale(1.4f, 0.1f));
            seq.Append(_playerShieldText.transform.parent.DOScale(1f, 0.1f));
        }
        else
        {
            _enemyShieldText.text = shield.ToString();

            Sequence seq = DOTween.Sequence();
            seq.Append(_enemyShieldText.transform.parent.DOScale(1.2f, 0.1f));
            seq.Append(_enemyShieldText.transform.parent.DOScale(1f, 0.1f));
        }

        UpdateHealthbar(isPlayer);
    }

    public void Turn(string text)
    {
        _turnPanel.SetActive(true);
        _turnText.text = text;

        Sequence seq = DOTween.Sequence();
        seq.Append(_turnBackground.DOFade(0.5f, 0.5f));
        seq.Join(_turnText.DOFade(1f, 0.5f));
        seq.AppendInterval(0.5f);
        seq.Append(_turnBackground.DOFade(0, 0.5f));
        seq.Join(_turnText.DOFade(0, 0.5f));
        seq.AppendCallback(() => _turnPanel.SetActive(false));
        seq.AppendCallback(() => BattleManager.Instance.TurnChange());
    }

    #region Health Bar

    public void SliderInit(bool isPlayer)
    {
        if (isPlayer)
        {
            hSlider = _playerHealthSlider;
            sSlider = _playerShieldSlider;
            hText = _playerHealthText;
            hfSlider = _playerHealthFeedbackSlider;
        }
        else
        {
            hSlider = _enemyHealthSlider;
            sSlider = _enemyShieldSlider;
            hText = _enemyHealthText;
            hfSlider = _enemyHealthFeedbackSlider;
        }
    }

    public void HealthbarInit(bool isPlayer, float health, float maxHealth = 0)
    {
        SliderInit(isPlayer);

        if (maxHealth == 0)
            maxHealth = health;

        hSlider.maxValue = maxHealth;
        hSlider.value = health;
        hText.text = string.Format("{0} / {1}", health, maxHealth);

        sSlider.maxValue = maxHealth;
        sSlider.value = 0;

        hfSlider.maxValue = maxHealth;
        hfSlider.value = 0;
    }

    public void UpdateHealthbar(bool isPlayer)
    {
        SliderInit(isPlayer);
        Unit unit = isPlayer ? BattleManager.Instance.player : BattleManager.Instance.enemy;

        if (unit.Shield > 0)
        {
            if (unit.HP + unit.Shield > unit.MaxHealth)
                hfSlider.value = hSlider.maxValue = sSlider.maxValue = hfSlider.maxValue = unit.HP + unit.Shield;
            else
                hSlider.maxValue = sSlider.maxValue = hfSlider.maxValue = unit.MaxHealth;

            hfSlider.value = hSlider.value;
            hSlider.value = unit.HP;
            sSlider.value = unit.HP + unit.Shield;
            hText.text = string.Format("{0} / {1}", hSlider.value, unit.MaxHealth);
        }
        else
        {
            hSlider.maxValue = sSlider.maxValue = hfSlider.maxValue = unit.MaxHealth;
            sSlider.value = 0;
            hSlider.value = unit.HP;
            hText.text = string.Format("{0} / {1}", hSlider.value, unit.MaxHealth);
        }

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.Append(hfSlider.DOValue(unit.HP, 0.2f));

        Sequence vibrateSeq = DOTween.Sequence();
        vibrateSeq.Append(hSlider.transform.parent.DOShakeScale(0.1f));
        vibrateSeq.Append(hSlider.transform.parent.DOScale(1f, 0));
    }

    /*public void UpdateEnemyHealthbar()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_enemyHealthSlider.DOValue(GameManager.Instance.enemy.HP, 0.2f));
        seq.AppendCallback(() => _enemyHealthText.text = string.Format("{0} / {1}", _enemyHealthSlider.value, _enemyHealthSlider.maxValue));
    }

    public void UpdatePlayerHealthbar()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_playerHealthSlider.DOValue(GameManager.Instance.player.HP, 0.2f));
        seq.AppendCallback(() => _playerHealthText.text = string.Format("{0} / {1}", _playerHealthSlider.value, _playerHealthSlider.maxValue));
    }*/
    #endregion

    #region Status

    public StatusPanel GetStatusPanel(Status status, Transform parent, bool isPopup = false)
    {
        //StatusPanel statusPanel = Instantiate(isPopup ? _statusPopup : _statusPrefab).GetComponent<StatusPanel>();
        StatusPanel statusPanel = Instantiate(_statusPrefab).GetComponent<StatusPanel>();
        statusPanel.status = status;

        statusPanel.image.sprite = status.icon;
        statusPanel.image.color = status.color;
        statusPanel.duration.text = status.typeValue.ToString();
        statusPanel.statusName = status.statusName;
        statusPanel.transform.SetParent(parent);
        statusPanel.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);

        return statusPanel;
    }

    public void AddStatus(Unit unit, Status status)
    {
        Transform trm = unit == BattleManager.Instance.player ? _statusPlayerPanel : _statusEnemyPanel;
        GetStatusPanel(status, trm);
    }

    public GameObject GetStatusPanelStatusObj(Unit unit, StatusName name)
    {
        Transform trm = unit == BattleManager.Instance.player ? _statusPlayerPanel : _statusEnemyPanel;

        GameObject obj = null;
        for (int i = 0; i < trm.childCount; i++)
        {
            if (trm.GetChild(i).GetComponent<StatusPanel>().statusName == name)
            {
                obj = trm.GetChild(i).gameObject;
            }
        }

        return obj;
    }

    public void ReloadStatusPanel(Unit unit, StatusName name, int duration)
    {
        GameObject obj = GetStatusPanelStatusObj(unit, name);

        if (obj == null)
            return;

        if (duration <= 0)
        {
            RemoveStatusPanel(unit, name);
            return;
        }

        obj.GetComponent<StatusPanel>().duration.text = duration.ToString();
    }

    public void RemoveStatusPanel(Unit unit, StatusName name)
    {
        GameObject obj = GetStatusPanelStatusObj(unit, name);

        if (obj == null)
        {
            return;
        }

        Destroy(obj);
    }

    #endregion

    #region Popup
    public void DamageUIPopup(float amount, Vector3 pos, Status status = null)
    {
        DamagePopup popup = Instantiate(_damagePopup, _canvas).GetComponent<DamagePopup>();
        popup.Setup(amount, pos, status);
    }

    /*public void StatusPopup(Status status, Vector3 pos)
    {
        StatusPanel panel = GetStatusPanel(status, _canvas, true);
        panel.transform.position = pos;
        panel.transform.localScale = Vector3.one * 2f;

        CanvasGroup group = panel.GetComponent<CanvasGroup>();

        Sequence seq = DOTween.Sequence();
        seq.Append(panel.transform.DOJump(new Vector3(pos.x + Random.Range(-2f, 2f), pos.y, pos.x), 0.8f, 1, 1f));
        seq.Join(group.DOFade(0, 1f).SetEase(Ease.InQuart));
        seq.AppendCallback(() =>
        {
            Destroy(group.gameObject);
        });
    }*/

    public void StatusPopup(Status status)
    {
        GameObject obj = Instantiate(_statusPopup, enemyIcon);
        Image img = obj.GetComponent<Image>();
        img.sprite = status.icon;
        obj.transform.localScale = Vector3.one * 8f;

        Sequence seq = DOTween.Sequence();
        seq.Append(obj.transform.DOScale(9f, 0.7f).SetEase(Ease.InQuart));
        seq.Join(img.DOFade(0, 0.7f).SetEase(Ease.InQuart));
        seq.AppendCallback(() =>
        {
            Destroy(obj);
        });

        GameObject textPopup = Instantiate(_statusTextPopup, enemyIcon);
        TextMeshProUGUI text = textPopup.GetComponent<TextMeshProUGUI>();
        text.text = status.debugName;
        textPopup.transform.localPosition = new Vector3(0, 300, 0);

        Sequence seq1 = DOTween.Sequence();
        seq1.Append(textPopup.transform.DOLocalMoveY(400f, 0.5f));
        seq1.Join(text.DOColor(Color.red, 0.5f));
        seq1.AppendInterval(0.3f);
        seq1.Join(text.DOFade(0, 0.5f).SetEase(Ease.InQuart));
        seq1.AppendCallback(() =>
        {
            Destroy(textPopup);
        });
    }

    public void InfoMessagePopup(string message, Vector3 pos)
    {
        InfoMessage popup = Instantiate(_infoMessage, _canvas).GetComponent<InfoMessage>();
        pos.z = 0;
        pos.y += 1;
        popup.Setup(message, pos);
    }
    #endregion

    #region Pattern
    public void ReloadPattern(Sprite sprite, string value = "")
    {
        _enemyPatternIcon.sprite = sprite;
        _enemyPatternValueText.text = value;
    }

    public IEnumerator PatternIconAnimationCoroutine()
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject obj = Instantiate(_enemyPatternIcon.gameObject, _enemyPatternIcon.transform.parent);
            obj.transform.position = _enemyPatternIcon.transform.position;
            PatternIconAnimation(obj);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void PatternIconAnimation(GameObject obj)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(obj.transform.DOScale(2f, 0.5f));
        seq.Join(obj.GetComponent<Image>().DOFade(0, 0.5f));
        seq.AppendCallback(() => Destroy(obj));
    }
    #endregion

    #region Description

    public void CardDescPopup(Card card)
    {
        _cardDescPanel.SetActive(true);
        //_cardDescPanel.transform.position = pos + new Vector3(0, 700, 0);

        RuneProperty rune = card.IsFront ? card.Rune.MainRune : card.Rune.AssistRune;
        
        _cardDescName.text = rune.Name;
        _cardDescSkillIcon.sprite = rune.CardImage;
        _cardDescInfo.text = rune.CardDescription;
        _cardDescMana.text = rune.Cost.ToString();
        _cardDescCool.text = rune.CoolTime.ToString();

        for(int i = 0; i < _cardDescKeyword.childCount; i++)
        {
            Destroy(_cardDescKeyword.GetChild(i).gameObject);
        }

        foreach(var keyword in card.Rune.keywordList)
        {
            GameObject panel = word.KeywordInit(keyword);
            panel.transform.SetParent(_cardDescKeyword);
            panel.transform.localScale = Vector3.one;
        }
    }

    public void CardDescPopup(TestCard card)
    {
        if (card == null)
        {
            _cardDescPanel.SetActive(false);
        }
        else
        {
            RuneProperty magic = card.Magic.MainRune;

            _cardDescName.text = magic.Name;
            _cardDescSkillIcon.sprite = magic.CardImage;
            _cardDescInfo.text = magic.CardDescription;
            _cardDescPanel.SetActive(true);
        }
    }

    public void CardDescDown()
    {
        _cardDescPanel.SetActive(false);
    }

    public void StatusDescPopup(Status status, Vector3 pos, bool isDown = true)
    {
        if (isDown == false)
        {
            _statusDescPanel.SetActive(false);
            return;
        }

        _statusDescPanel.SetActive(true);
        _statusDescPanel.transform.position = pos + new Vector3(0, 2, 0);
        _statusDescPanel.transform.DOMoveZ(-1, 0);

        _statusDescName.text = status.debugName;
        _statusDescInfo.text = status.information;
    }

    #endregion
}
