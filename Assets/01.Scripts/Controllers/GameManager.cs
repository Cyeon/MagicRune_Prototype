using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameTurn
{
    Player,
    Monster,
    PlayerWait,
    MonsterWait,
    Unknown
}

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private GameTurn gameTurn = GameTurn.Unknown;
    public GameTurn GameTurn => gameTurn;
    public Player player = null;
    public Enemy enemy = null;
    public Unit currentUnit = null;
    public Unit attackUnit = null;

    public MagicCircle MagicCircle;
    public AudioClip turnChangeSound = null;

    private void Awake()
    {
        DOTween.Init(false, false, LogBehaviour.Default).SetCapacity(500, 50);

        Application.targetFrameRate = 30;
    }

    private void Start()
    {
        enemy = EnemyManager.Instance.SpawnEnemy();
        player = FindObjectOfType<Player>();

        UIManager.instance.HealthbarInit(true, player.HP);

        FeedbackManager.Instance.Init();

        TurnChange();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            StatusManager.Instance.AddStatus(enemy, StatusName.Wound);
        }
    }

    private void OnPlayerTurn()
    {
        EventManager.TriggerEvent(Define.ON_END_MONSTER_TURN);

        gameTurn = GameTurn.Player;
        currentUnit = player;
        attackUnit = enemy;
    }

    public void OnMonsterTurn()
    {
        EventManager.TriggerEvent(Define.ON_START_MONSTER_TURN);
        gameTurn = GameTurn.Monster;
        currentUnit = enemy;
        attackUnit = player;
        enemy.TurnStart();
    }

    # region Debug

    public void TurnChange()
    {
        if (gameTurn == GameTurn.Player || gameTurn == GameTurn.Monster)
        {
            player?.InvokeStatus(StatusInvokeTime.End);
            enemy?.InvokeStatus(StatusInvokeTime.End);

            StatusManager.Instance.StatusUpdate(player);
            StatusManager.Instance.StatusUpdate(enemy);
        }

        switch (gameTurn)
        {
            case GameTurn.Unknown:
                enemy.pattern = PatternManager.Instance.GetPattern();
                enemy.pattern?.Start();
                EventManager<int>.TriggerEvent(Define.ON_START_PLAYER_TURN, 5);
                EventManager.TriggerEvent(Define.ON_START_PLAYER_TURN);
                EventManager<bool>.TriggerEvent(Define.ON_START_PLAYER_TURN, true);

                //SoundManager.instance.PlaySound(turnChangeSound, SoundType.Effect);

                UIManager.Instance.Turn("Player Turn");
                gameTurn = GameTurn.MonsterWait;
                break;

            case GameTurn.Player:
                EventManager<bool>.TriggerEvent(Define.ON_START_MONSTER_TURN, false);

                SoundManager.Instance.PlaySound(turnChangeSound, SoundType.Effect);

                UIManager.Instance.Turn("Enemy Turn");
                gameTurn = GameTurn.PlayerWait;
                break;

            case GameTurn.PlayerWait:
                enemy.StopIdle();
                OnMonsterTurn();
                break;

            case GameTurn.Monster:
                StatusManager.Instance.StatusTurnChange(player);
                StatusManager.Instance.StatusTurnChange(enemy);

                enemy.pattern?.End();
                if (enemy.isSkip)
                    enemy.pattern = PatternManager.Instance.GetCodiPattern("����");
                else
                    enemy.pattern = PatternManager.Instance.GetPattern();
                enemy.pattern?.Start();

                EventManager<int>.TriggerEvent(Define.ON_START_PLAYER_TURN, 5);
                EventManager.TriggerEvent(Define.ON_START_PLAYER_TURN);
                EventManager<bool>.TriggerEvent(Define.ON_START_PLAYER_TURN, true);
                this.MagicCircle.CardCollector.UpdateCardOutline();

                SoundManager.instance.PlaySound(turnChangeSound, SoundType.Effect);

                if (player.Shield > 0)
                {
                    player.Shield = 0;
                    UIManager.Instance.UpdateHealthbar(true);
                }

                UIManager.Instance.Turn("Player Turn");
                gameTurn = GameTurn.MonsterWait;
                break;

            case GameTurn.MonsterWait:
                enemy.Idle();
                OnPlayerTurn();
                break;
        }

        foreach (var rList in this.MagicCircle.RuneDict)
        {
            foreach (var r in rList.Value)
            {
                if (r.Rune == null)
                {
                    Destroy(r.gameObject);
                }
                else
                {
                    r.gameObject.SetActive(false);
                    r.transform.SetParent(this.MagicCircle.CardCollector.transform);
                    r.SetIsEquip(false);
                }
            }
        }
        //Debug.Log(string.Format("Turn Change: {0}", gameTurn));

        if (gameTurn == GameTurn.PlayerWait || gameTurn == GameTurn.MonsterWait)
            currentUnit?.InvokeStatus(StatusInvokeTime.Start);
    }

    public void PlayerTurnEnd()
    {
        if(gameTurn == GameTurn.Player)
        {
            TurnChange();
        }
        this.MagicCircle.RuneDict.Clear();
        this.MagicCircle.EffectDict.Clear();
        this.MagicCircle.EffectContent.Clear();
        //this.MagicCircle.CardCollector.UpdateCardOutline();
        this.MagicCircle.CardCollector.handCardOutline(false);
        this.MagicCircle.CardCollector.IsFront = true;
    }

    #endregion

}