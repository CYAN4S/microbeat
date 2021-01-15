using System;
using System.Collections;
using Events;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    // [SerializeField] private VoidEventChannelSO startExploreI;
    // [SerializeField] private ChartPathEventChannelSO chartSelectF;
    [SerializeField] private ChartEventChannelSO chartChannel;
    [SerializeField] private PlayerSO player;

    private AudioSource audioSource;

    private int currentMetaIndex;
    private GameplayUIManager igm;
    private int noteCount;

    private PlayManager pm;

    private int rawScore;
    private UIManager ui;
    public static GameManager Instance { get; private set; }

    // public int[] JudgeCounts { get; } = {0, 0, 0, 0, 0};
    // public Action OnScrollSpeedChange;

    public BpmMeta Meta { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
        ui = GetComponent<UIManager>();
        pm = GetComponent<PlayManager>();
        igm = GetComponent<GameplayUIManager>();
    }
    
    private void Start()
    {
        // InitVariables();
        // PrepareGame(chartChannel.value);
        player.Reset();
        currentMetaIndex = 0;
        rawScore = 0;
    }


    private void Update()
    {
        //if (!IsWorking) return;
        if (!player.IsWorking) return;
        
        RefreshTime();
    }

    private void OnEnable()
    {
        chartChannel.onEventRaised += PrepareGame;
    }

    private void OnDisable()
    {
        chartChannel.onEventRaised -= PrepareGame;
    }

    // private void InitVariables()
    // {
    //     IsWorking = false;
    //     CurrentTime = -3;
    //     ScrollSpeed = 2.5;
    //     EndTime = 1000f;
    //     Combo = 0;
    //     Score = 0;
    //     CurrentBeat = 0;
    //     CurrentBpm = 0;
    //     currentMetaIndex = 0;
    //     rawScore = 0;
    // }

    private void PrepareGame(Chart chart)
    {
        // ui.selection.SetActive(false);

        var desc = chart.desc;
        var pattern = chart.pattern;

        noteCount = pattern.notes.Count + pattern.longNotes.Count;
        Meta = desc.bpms?.Count is int c && c != 0 ? new BpmMeta(desc.bpms, desc.bpm) : new BpmMeta(desc.bpm);
        player.StdBpm = Meta.std;

        pm.PrepareNotes(desc, pattern);
        
        audioSource.clip = chart.audioClip;
        Debug.Log(audioSource.clip.length);
        if (audioSource.clip != null) StartCoroutine(PlayAudio(0));
        StartGame();
    }

    private void StartGame()
    {
        player.OnGameStart();
        inputReader.speedEvent += ChangeSpeed;
    }

    private void EndGame()
    {
        player.OnGameEnd();
        igm.StopGroove();
        ui.DisplayResult();
    }


    private void RefreshTime()
    {
        if (player.CurrentTime >= player.EndTime)
        {
            EndGame();
            return;
        }

        player.CurrentTime += Time.deltaTime;

        RefreshBeatAndBpm();
    }

    private void RefreshBeatAndBpm()
    {
        if (Meta.endTimes[currentMetaIndex] <= player.CurrentTime) currentMetaIndex++;

        if (currentMetaIndex == 0)
        {
            player.CurrentBeat = player.CurrentTime * Meta.bpms[0] / 60.0;
            player.ChangeBpm(Meta.bpms[0]);
            return;
        }

        player.CurrentBeat = Meta.beats[currentMetaIndex] +
                             (player.CurrentTime - Meta.endTimes[currentMetaIndex - 1]) * Meta.bpms[currentMetaIndex] / 60.0;
        player.ChangeBpm(Meta.bpms[currentMetaIndex]);
    }

    public IEnumerator PlayAudio(float time)
    {
        while (time > player.CurrentTime) yield return null;
        audioSource.Play();
        igm.StartGroove();
    }

    private void ChangeSpeed(int input)
    {
        var value = player.ScrollSpeed + CONST.DELTA_SPEED[input];

        if (value < 0.5)
            value = 0.5;
        else if (value > 9.5) value = 9.5;

        player.ChangeScrollSpeed(value);
        // player.scrollSpeed = value;
        // OnScrollSpeedChange?.Invoke();
    }

    public void ApplyNote(int line, JUDGES judge, float gap)
    {
        player.JudgeCounts[(int) judge]++;

        rawScore += CONST.JUDGE_SCORE[(int) judge];
        player.ChangeScore((double) rawScore / (CONST.JUDGE_SCORE[0] * noteCount) * 300000d);

        if (judge != JUDGES.BAD)
        {
            player.IncreaseCombo(1);
            igm.ShowCombo(player.Combo);
            igm.VisualizeNoteEffect(line);
        }
        else
        {
            player.BreakCombo();
        }

        igm.ShowGap(gap);
        igm.ShowScore(player.Score);
        igm.VisualizeJudge(judge);
    }

    public void ApplyBreak(int line)
    {
        player.JudgeCounts[(int) JUDGES.BREAK]++;
        player.BreakCombo();

        igm.EraseGap();
        igm.VisualizeJudge(JUDGES.BREAK);
    }

    public void ApplyLongNoteStart(int line, JUDGES judge, float gap)
    {
        if (judge == JUDGES.BAD)
        {
            player.BreakCombo();
        }
        else
        {
            player.IncreaseCombo(1);
            igm.ShowCombo(player.Combo);
            igm.VisualizeNoteEffect(line);
        }

        igm.ShowGap(gap);
        igm.VisualizeJudge(judge);
    }

    public void ApplyLongNoteTick(int line, JUDGES judge)
    {
        player.IncreaseCombo(1);
        igm.ShowCombo(player.Combo);
        igm.VisualizeJudge(judge);
        igm.VisualizeNoteEffect(line);
    }

    public void ApplyLongNoteEnd(int line, JUDGES judge, float gap)
    {
        ApplyNote(line, judge, gap);
    }
}