using System;
using System.Collections.Generic;
using System.Linq;
using FileIO;

namespace Core
{
    [Serializable]
    public class BpmMeta
    {
        public List<double> beats;
        public List<double> bpms;
        public List<double> lengths;
        public List<double> endTimes;
        public double std;
        
        public BpmMeta(List<List<double>> bpms, double stdBpm)
        {
            GetMeta(bpms);
            std = stdBpm;
        }

        public BpmMeta(List<SerializableBpm> bpms, double stdBpm)
        {
            GetMeta(bpms);
            std = stdBpm;
        }

        public BpmMeta(double stdBpm)
        {
            GetMeta(new List<SerializableBpm>
            {
                new SerializableBpm
                {
                    beat = 0,
                    bpm = stdBpm
                }
            });
            std = stdBpm;
        }

        public void GetMeta(List<SerializableBpm> bpmData)
        {
            // Init
            beats = new List<double>();
            bpms = new List<double>();
            lengths = new List<double>();
            endTimes = new List<double>();

            // Set beat, bpm
            beats.AddRange(from item in bpmData select item.beat);
            bpms.AddRange(from item in bpmData select item.bpm);

            // Calc length
            for (var i = 0; i < beats.Count - 1; i++) lengths.Add(60.0 * (beats[i + 1] - beats[i]) / bpms[i]);
            lengths.Add(double.MaxValue); // PLZ Don't rely on this value.

            // Calc endTime
            endTimes.Add(lengths[0]);
            for (var i = 1; i < beats.Count - 1; i++) endTimes.Add(lengths[i] + endTimes[i - 1]);
            endTimes.Add(double.MaxValue); // PLZ Don't rely on this value too.
        }
        
        public void GetMeta(List<List<double>> bpmData)
        {
            // Init
            beats = new List<double>();
            bpms = new List<double>();
            lengths = new List<double>();
            endTimes = new List<double>();

            // Set beat, bpm
            beats.AddRange(from item in bpmData select item[0]);
            bpms.AddRange(from item in bpmData select item[1]);

            // Calc length
            for (var i = 0; i < beats.Count - 1; i++) lengths.Add(60.0 * (beats[i + 1] - beats[i]) / bpms[i]);
            lengths.Add(double.MaxValue); // PLZ Don't rely on this value.

            // Calc endTime
            endTimes.Add(lengths[0]);
            for (var i = 1; i < beats.Count - 1; i++) endTimes.Add(lengths[i] + endTimes[i - 1]);
            endTimes.Add(double.MaxValue); // PLZ Don't rely on this value too.
        }

        // This function works slowly. Don't use it too often.
        // Use GetTimesFromList function for better performance.
        public float GetTime(double beat)
        {
            int index;
            for (index = 0; index < beats.Count - 1; index++)
                if (beats[index + 1] >= beat)
                    break;

            if (index == 0) return (float) ((beat - beats[index]) * 60.0 / bpms[index]);

            return (float) (endTimes[index - 1] + (beat - beats[index]) * 60.0 / bpms[index]);
        }

        // beats List MUST BE SORTED.
        // public Dictionary<double, float> GetTimesFromList(List<double> beatList)
        // {
        //     var result = new Dictionary<double, float>();
        //
        //     int index, bt = 0;
        //     for (index = 0; index < beats.Count - 1; index++)
        //     {
        //         if (bt <= beatList.Count)
        //         {
        //             break;
        //         }
        //
        //         for (int i = bt; i < beatList.Count; i++, bt++)
        //         {
        //             if (beats[index + 1] >= beatList[i])
        //                 break;
        //
        //             var t = index == 0
        //                 ? (float) ((beatList[i] - beats[index]) * 60.0 / bpms[index])
        //                 : (float) (endTimes[index - 1] + (beatList[i] - beats[index]) * 60.0 / bpms[index]);
        //
        //             result.Add(beatList[i], t);
        //         }
        //     }
        //
        //     return result;
        // }
    }
}