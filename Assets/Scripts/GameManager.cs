using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private AudioSource ads;

    public static bool IsWorking { get; private set; }
    public static float CurrentTime { get; private set; }
    public static double ScrollSpeed { get; private set; }
    public static float EndTime { get; set; }
    public static int Combo { get; private set; }
    public static double Score { get; private set; }
    public static double CurrentBeat { get; private set; }
    public static double CurrentBpm { get; private set; }

    public Action OnSheetSelect;
    public Action OnGameStart;
    public Action OnMusicStart;

    public Action OnScrollSpeedChange;
    public Action<int, JUDGES, float> OnJudge;
    public Action<int, JUDGES> OnTickJudge;

    public Action OnGameEnd;

    public AudioClip AudioClip { get; set; }

    public Sheet Now { get; private set; }

    private int dataScore;
    public int[] JudgeCounts { get; private set; } = { 0, 0, 0, 0, 0 };


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        ads = GetComponent<AudioSource>();

        OnGameStart += () =>
        {
            IsWorking = true;
            InputManager.Instance.OnSpeedKeyDown += ChangeSpeed;
        };
    }

    private void Start()
    {
        IsWorking = false;
        CurrentTime = -3;
        ScrollSpeed = 2.5;
        EndTime = 1000f;
        Combo = 0;
        Score = 0;
        CurrentBeat = 0;
        CurrentBpm = 0;

        dataScore = 0;
    }


    private void Update()
    {
        if (!IsWorking)
        {
            return;
        }

        if (CurrentTime >= EndTime)
        {
            OnGameEnd();

            IsWorking = false;
            return;
        }

        CurrentTime += Time.deltaTime;

        //CurrentBeat = CurrentTime * now.bpmMeta.std / 60.0;
        (double, double) bnb = GetCurrentBeatAndBpm();
        CurrentBeat = bnb.Item1;
        CurrentBpm = bnb.Item2;
    }

    private int index = 0;
    private (double, double) GetCurrentBeatAndBpm()
    {
        if (Now.bpmMeta.endTimes[index] <= CurrentTime)
        {
            index++;
        }

        if (index == 0)
        {
            return (CurrentTime * Now.bpmMeta.bpms[0] / 60.0, Now.bpmMeta.bpms[0]);
        }

        return (Now.bpmMeta.beats[index] + (CurrentTime - Now.bpmMeta.endTimes[index - 1]) * Now.bpmMeta.bpms[index] / 60.0, Now.bpmMeta.bpms[index]);
    }

    public void SheetSelect(SerializableDesc desc, SerializableSheet sheet, string audioPath)
    {
        OnSheetSelect?.Invoke();

        Now = new Sheet(desc, sheet)
        {
            bpmMeta = desc.bpms?.Count is int x && x != 0
            ? new BpmMeta(desc.bpms, desc.bpm)
            : new BpmMeta(desc.bpm)
        };

        //print(JsonUtility.ToJson(Now.bpmMeta));

        StartCoroutine(GetComponent<FileExplorer>().GetAudioClip(audioPath, () =>
        {
            StartCoroutine(PlayAudio(0));
            OnGameStart?.Invoke();
        }));
    }

    public IEnumerator PlayAudio(float time)
    {
        while (time > CurrentTime)
        {
            yield return null;
        }
        if (ads.clip != null)
        {
            ads.Play();
        }
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
