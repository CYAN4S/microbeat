using System.Collections;
using Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private ChartEventChannelSO chartChannel;
    [SerializeField] private PlayerSO player;

    private int currentMetaIndex;
    private int noteCount;
    private int rawScore;

    private PlayManager pm;
    private AudioSource audioSource;
    
    private void Awake()
    {
        pm = GetComponent<PlayManager>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        player.Reset();
        currentMetaIndex = 0;
        rawScore = 0;
    }


    private void Update()
    {
        if (player.IsWorking)
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

    private void PrepareGame(Chart chart)
    {
        var desc = chart.desc;
        var pattern = chart.pattern;

        noteCount = pattern.notes.Count + pattern.longNotes.Count;
        var meta = desc.bpms?.Count is int c && c != 0 ? new BpmMeta(desc.bpms, desc.bpm) : new BpmMeta(desc.bpm);
        player.StdBpm = meta.std;
        player.Meta = meta;

        pm.PrepareNotes(desc, pattern);

        audioSource.clip = chart.audioClip;
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
        SceneManager.LoadScene(3);
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
        if (player.Meta.endTimes[currentMetaIndex] <= player.CurrentTime) currentMetaIndex++;

        if (currentMetaIndex == 0)
        {
            player.CurrentBeat = player.CurrentTime * player.Meta.bpms[0] / 60.0;
            player.ChangeBpm(player.Meta.bpms[0]);
            return;
        }

        player.CurrentBeat = player.Meta.beats[currentMetaIndex] +
                             (player.CurrentTime - player.Meta.endTimes[currentMetaIndex - 1]) *
                             player.Meta.bpms[currentMetaIndex] / 60.0;
        player.ChangeBpm(player.Meta.bpms[currentMetaIndex]);
    }

    public IEnumerator PlayAudio(float time)
    {
        while (time > player.CurrentTime) yield return null;
        audioSource.Play();
        player.OnZero();
    }

    private void ChangeSpeed(int input)
    {
        var value = player.ScrollSpeed + CONST.DELTA_SPEED[input];

        if (value < 0.5)
            value = 0.5;
        else if (value > 9.5) value = 9.5;

        player.ChangeScrollSpeed(value);
    }

    public void ApplyNote(int line, JUDGES judge, float gap)
    {
        player.JudgeCounts[(int) judge]++;

        rawScore += CONST.JUDGE_SCORE[(int) judge];
        player.ChangeScore((double) rawScore / (CONST.JUDGE_SCORE[0] * noteCount) * 300000d);

        if (judge != JUDGES.BAD)
        {
            player.IncreaseCombo(1);
            player.OnNoteEffect(line);
        }
        else
        {
            player.BreakCombo();
        }

        player.OnJudge(judge);
        player.OnGap(gap);
    }

    public void ApplyBreak(int line)
    {
        player.JudgeCounts[(int) JUDGES.BREAK]++;
        player.BreakCombo();
        player.OnJudge(JUDGES.BREAK);
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
            player.OnNoteEffect(line);
        }

        player.OnGap(gap);
        player.OnJudge(judge);
    }

    public void ApplyLongNoteTick(int line, JUDGES judge)
    {
        player.IncreaseCombo(1);
        player.OnJudge(judge);
        player.OnNoteEffect(line);
    }

    public void ApplyLongNoteEnd(int line, JUDGES judge, float gap)
    {
        ApplyNote(line, judge, gap);
    }
}