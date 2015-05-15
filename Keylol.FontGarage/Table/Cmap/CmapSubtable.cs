﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Keylol.FontGarage.Table.Cmap
{
    public struct Environment
    {
        public ushort PlatformId { get; set; }
        public ushort EncodingId { get; set; }
    }

    public abstract class CmapSubtable : IOpenTypeFontSerializable, IDeepCopyable
    {
        protected CmapSubtable()
        {
            Environments = new List<Environment>();
            CharGlyphIdMap = new Dictionary<uint, uint>();
        }

        public List<Environment> Environments { get; set; }
        public Dictionary<uint, uint> CharGlyphIdMap { get; set; }
        public abstract ushort Format { get; }

        public object DeepCopy()
        {
            var newTable = (CmapSubtable) MemberwiseClone();
            newTable.CharGlyphIdMap = CharGlyphIdMap.ToDictionary(pair => pair.Key, pair => pair.Value);
            newTable.Environments = Environments.ToList();
            return newTable;
        }

        public abstract void Serialize(BinaryWriter writer, long startOffset, OpenTypeFont font);
    }
}