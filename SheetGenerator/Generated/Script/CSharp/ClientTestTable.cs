/*
* THIS CODE WAS GENERATED BY SHEET-GENERATOR.
*
* CHANGES TO THIS FILE MAY CAUSE INCORRECT BEHAVIOR AND WILL BE LOST IF
* THE CODE IS REGENERATED.
*/

using System.Diagnostics.CodeAnalysis;

namespace StaticData.Tables
{
   /// <summary>테스트 테이블입니다. (개발용)</summary>
   public sealed class ClientTestTable : TableBase<ClientTestRecord>
   {
       public ClientTestRecord GetByIndex(int index)
       {
           if (_recordsByIndex.TryGetValue(index, out var record))
               return record;
           throw new KeyNotFoundException($"Record with index {index} not found.");
       }

       public bool TryGetByIndex(int index, out ClientTestRecord? record)
       {
           return _recordsByIndex.TryGetValue(index, out record);
       }

       public ClientTestRecord GetByKey(string key)
       {
           ArgumentNullException.ThrowIfNull(key);
           if (_recordsByKey.TryGetValue(key, out var record))
               return record;
           throw new KeyNotFoundException($"Record with key {key} not found.");
       }

       public bool TryGetByKey(string key, [NotNullWhen(true)] out ClientTestRecord? record)
       {
           ArgumentNullException.ThrowIfNull(key);
           return _recordsByKey.TryGetValue(key, out record);
       }

       public object[][] BuildObjectValueMap()
       {
          var result = new object[_records.Count][];
          for (int i = 0; i < _records.Count; i++)
          {
              var record = _records[i];
              result[i] = new object[] { record.Index, record.Key, record.Description, record.English, record.Korean, record.Spanish, record.Chinese };
          }
          return result;
       }
   }
}
