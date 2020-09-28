using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private AudioSource audioSource;
    private InputManager im;
    private FileExplorer fe;
    private UIManager ui;
    private PlayManager pm;
    private IngameGraphicsManager igm;

    public static bool IsWorking { get; private set; }
    public static float CurrentTime { get; private set; }
    public static double ScrollSpeed { get; private set; }
    public static float EndTime { get; set; }
    public static int Combo { get; private set; }
    public static double Score { get; private set; }
    public static double CurrentBeat { get; private set; }
    public static double CurrentBpm { get; private set; }
    public int[] JudgeCounts { get; private set; } = { 0, 0, 0, 0, 0 };

    public Action<SheetData> OnSheetSelect;
    public Action OnGameStart;
    public Action OnMusicStart;

    public Action OnScrollSpeedChange;
    public Action<int, JUDGES, float> OnJudge;
    public Action<int, JUDGES> OnTickJudge;

    public AudioClip AudioClip { get; set; }

    public Sheet Now { get; private set; }

    private int dataScore;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        im = GetComponent<InputManager>();
        fe = GetComponent<FileExplorer>();
        ui = GetComponent<UIManager>();
        pm = GetComponent<PlayManager>();
        igm = GetComponent<IngameGraphicsManager>();

        OnSheetSelect += _ => PrepareGame(_);
    }


    private void Start()
    {
        InitVariables();
        PrepareSongList();
    }


    private void Update()
    {
        if (!IsWorking)
        {
            return;
        }

        RefreshTime();
    }

    private void PrepareGame(SheetData sheetData)
    {
        ui.selection.SetActive(false);

        SerializableDesc desc = sheetData.desc;
        SerializableSheet sheet = sheetData.sheet;

        Now = new Sheet(desc, sheet)
        {
            bpmMeta = desc.bpms?.Count is int x && x != 0 ? new BpmMeta(desc.bpms, desc.bpm) : new BpmMeta(desc.bpm)
        };

        pm.PrepareNotes();

        StartCoroutine
        (
            fe.GetAudioClip(sheetData.audioPath, () =>
            {
                audioSource.clip = fe.streamAudio;
                if (audioSource.clip != null)
                {
                    StartCoroutine(PlayAudio(0));
                }
                StartGame();
                OnGameStart?.Invoke();
            })
        );
    }

    private void StartGame()
    {
        IsWorking = true;
        im.OnSpeedKeyDown += ChangeSpeed;
    }

    private void EndGame()
    {
        igm.StopGroove();
        ui.DisplayResult();
    }

    private void InitVariables()
    {
        IsWorking = false;
        CurrentTime = -3;
        ScrollSpeed = 2.5;
        EndTime = 1000f;
        Combo = 0;
        Score = 0;
        CurrentBeat = 0;
        CurrentBpm = 0;
        currentMetaIndex = 0;
        dataScore = 0;
    }

    private void PrepareSongList()
    {
        StartCoroutine(fe.ExploreAsync(GetComponent<UIManager>().DisplayMusics));
    }

    private void RefreshTime()
    {
        if (CurrentTime >= EndTime)
        {
            EndGame();
            IsWorking = false;
            return;
        }

        CurrentTime += Time.deltaTime;

        RefreshBeatAndBpm();
    }

    private int currentMetaIndex = 0;
    private void RefreshBeatAndBpm()
    {
        if (Now.bpmMeta.endTimes[currentMetaIndex] <= CurrentTime)
        {
            currentMetaIndex++;
        }

        if (currentMetaIndex == 0)
        {
            CurrentBeat = CurrentTime * Now.bpmMeta.bpms[0] / 60.0;
            CurrentBpm = Now.bpmMeta.bpms[0];
            return;
        }

        CurrentBeat = Now.bpmMeta.beats[currentMetaIndex] + (CurrentTime - Now.bpmMeta.endTimes[currentMetaIndex - 1]) * Now.bpmMeta.bpms[currentMetaIndex] / 60.0;
        CurrentBpm = Now.bpmMeta.bpms[currentMetaIndex];
    }

    public IEnumerator PlayAudio(float time)
    {
        while (time > CurrentTime)
        {
            yield return null;
        }
        audioSource.Play();
        OnMusicStart?.Invoke();
    }

    private void ChangeSpeed(int input)
    {
        double value = ScrollSpeed + CONST.DELTASPEED[input];

        if (value < 0.5)
        {
            value = 0.5;
        }
        else if (value > 9.5)
        {
            value = 9.5;
        }

        ScrollSpeed = value;
        OnScrollSpeedChange?.Invoke();
    }

    public void HandleJudge(int line, JUDGES judge, float gap)
    {
        JudgeCounts[(int)judge]++;

        if (judge == JUDGES.BREAK)
        {
            Combo = 0;
        }
        else
        {
            dataScore += CONST.JUDGESCORE[(int)judge];
            Score = (double)dataScore / (CONST.JUDGESCORE[0] * (Now.notes.Count + Now.longNotes.Count)) * 300000d;
            Combo = (judge != JUDGES.BAD) ? (Combo + 1) : 0;
        }

        OnJudge?.Invoke(line, judge, gap);
    }

    public void HandleFirstTickJudge(int line, JUDGES judge, float gap)
    {
        Combo++;
        OnJudge?.Invoke(line, judge, gap);
    }

    public void HandleTickJudge(int line, JUDGES judge)
    {
        Combo++;
        OnTickJudge?.Invoke(line, judge);
    }
}
