/*
 * THIS CODE WAS GENERATED BY SHEET-GENERATOR.
 *
 * CHANGES TO THIS FILE MAY CAUSE INCORRECT BEHAVIOR AND WILL BE LOST IF
 * THE CODE IS REGENERATED.
 */

using System.Text;
using System.Runtime.CompilerServices;
using System.Buffers;
using MessagePack;
using SheetGenerator.IO;
using SheetGenerator.Configuration;

namespace StaticData.Tables
{
    [MessagePackObject]
    public sealed partial class ClientTest4Record : IEquatable<ClientTest4Record>
    {
        #region Fields
        /// <summary>인덱스(필수)</summary>
        [Key(0)]
        public int Index { get; private set; }

        /// <summary>스트링 키</summary>
        [Key(1)]
        public string Key { get; private set; }

        /// <summary>설명</summary>
        [Key(2)]
        public float Description { get; private set; }

        /// <summary>영어</summary>
        [Key(3)]
        public bool English { get; private set; }
        #endregion

        #region Binary IO
        [SerializationConstructor]
        public ClientTest4Record() { }

        public void Read(TableBinaryReader reader, uint[] tags)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            try
            {
                var bytes = reader.GetRemainingBytes();
                if (bytes.Length == 0)
                    throw new InvalidDataException("No data available to read");

                var record = MessagePackSerializer.Deserialize<ClientTest4Record>(bytes, MessagePackConfig.Options);
                CopyFrom(record);
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"Failed to read {nameof(ClientTest4Record)}", ex);
            }
        }

        private void CopyFrom(ClientTest4Record source)
        {
            Index = source.Index;
            Key = source.Key;
            Description = source.Description;
            English = source.English;
        }

        public void Serialize(IBufferWriter<byte> writer)
        {
            MessagePackSerializer.Serialize(writer, this);
        }

        public static ClientTest4Record Deserialize(ReadOnlyMemory<byte> bytes)
        {
            return MessagePackSerializer.Deserialize<ClientTest4Record>(bytes);
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            return ToStringInternal();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ToStringInternal()
        {
            var sb = new StringBuilder(256);
            sb.Append('{');
            sb.Append("\"Index\":");
            StringFormatter.Format(Index, sb);
            sb.Append("\"Key\":");
            StringFormatter.Format(Key, sb);
            sb.Append("\"Description\":");
            StringFormatter.Format(Description, sb);
            sb.Append("\"English\":");
            StringFormatter.Format(English, sb);

            sb.Append('}');
            return sb.ToString();
        }
        #endregion

        #region Equality & GetHashCode
        public bool Equals(ClientTest4Record other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (!Index.Equals(other.Index)) return false;
            if (!string.Equals(Key, other.Key, StringComparison.Ordinal)) return false;
            if (!Description.Equals(other.Description)) return false;
            if (!English.Equals(other.English)) return false;
            return true;

        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || (obj is ClientTest4Record other && Equals(other));
        }

        public override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(Index);
            hash.Add(Key);
            hash.Add(Description);
            hash.Add(English);

            return hash.ToHashCode();
        }

        public static bool operator ==(ClientTest4Record left, ClientTest4Record right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ClientTest4Record left, ClientTest4Record right)
        {
            return !Equals(left, right);
        }
        #endregion

        #region Indexer
        private readonly struct FieldInfo
        {
            public readonly int Index;
            public readonly Func<ClientTest4Record, object> Accessor;

            public FieldInfo(int index, Func<ClientTest4Record, object> accessor)
            {
                Index = index;
                Accessor = accessor;
            }
        }

        private static readonly Dictionary<string, FieldInfo> _fieldInfos;
        private static readonly FieldInfo[] _indexedFields;

        static ClientTest4Record()
        {
            _fieldInfos = new Dictionary<string, FieldInfo>(StringComparer.Ordinal)
            {
                ["Index"] = new FieldInfo(0, record => record.Index),
                ["Key"] = new FieldInfo(1, record => record.Key),
                ["Description"] = new FieldInfo(2, record => record.Description),
                ["English"] = new FieldInfo(3, record => record.English),
            };

            _indexedFields = new FieldInfo[]
            {
                new FieldInfo(0, record => record.Index),
                new FieldInfo(1, record => record.Key),
                new FieldInfo(2, record => record.Description),
                new FieldInfo(3, record => record.English),
            };
        }

        public object this[int fieldIndex]
        {
            get
            {
                if ((uint)fieldIndex >= _indexedFields.Length)
                    throw new ArgumentOutOfRangeException(nameof(fieldIndex));
                return _indexedFields[fieldIndex].Accessor(this);
            }
        }

        public object this[string fieldName]
        {
            get
            {
                if (fieldName == null)
                {
                    throw new ArgumentNullException(nameof(fieldName));
                }

                if (!_fieldInfos.TryGetValue(fieldName, out var fieldInfo))
                    throw new KeyNotFoundException($"Field '{fieldName}' not found.");
                return fieldInfo.Accessor(this);
            }
        }
        #endregion
    }
}
