using System;
using System.Collections.Generic;

namespace Gameplay
{
    public class MusicData
    {
        public string path;

        public SerializableDesc desc;

        // audio path, line, level, diff
        public List<Tuple<string, int, int, int>> patternData;

        public MusicData(SerializableDesc desc, string path)
        {
            this.desc = desc;
            this.path = path;
            this.patternData = new List<Tuple<string, int, int, int>>();
        }

        public void AddTuple(string path, int line, int level, int diff)
        {
            patternData.Add(new Tuple<string, int, int, int>(path, line, level, diff));
        }
    }
}