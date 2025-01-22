using MessagePack;

namespace SheetGenerator.IO;

public class TableBinaryReader : IDisposable
{
    private readonly byte[] _data;
    private readonly MemoryStream _stream;

    public TableBinaryReader(byte[] data)
    {
        _data = data;
        _stream = new MemoryStream(_data);
    }

    public long Position
    {
        get => _stream.Position;
    }

    public long Length
    {
        get => _stream.Length;
    }

    public void Dispose()
    {
        _stream?.Dispose();
    }

    public T ReadMessagePackData<T>(MessagePackSerializerOptions options)
    {
        try
        {
            var currentPosition = _stream.Position;
            var bytes = GetRemainingBytes();

            if (bytes.Length == 0)
            {
                throw new InvalidDataException("No remaining data to read");
            }

            var reader = new MessagePackReader(bytes);
            var type = reader.NextMessagePackType;

            Console.WriteLine($"Current Position: {currentPosition}, Remaining Bytes: {bytes.Length}");
            Console.WriteLine($"Next MessagePack Type: {type}");

            if (bytes.Length >= 4)
            {
                Console.WriteLine($"First 4 bytes: {BitConverter.ToString(bytes.Slice(0, 4).ToArray())}");
            }

            var result = MessagePackSerializer.Deserialize<T>(bytes, options);

            var consumed = reader.Consumed;
            _stream.Position = currentPosition + consumed;

            return result;
        }
        catch (Exception ex)
        {
            var position = _stream.Position;
            var bytesRead = Math.Min(16, _stream.Length - position);
            var errorContext = new byte[bytesRead];
            _stream.Position = position;
            _stream.Read(errorContext, 0, (int)bytesRead);

            throw new InvalidDataException($"Failed to read MessagePack data of type {typeof(T).Name} at position {position}. " +
                                           $"Next bytes: {BitConverter.ToString(errorContext)}", ex);
        }
    }

    public ReadOnlyMemory<byte> GetRemainingBytes()
    {
        var remaining = _stream.Length - _stream.Position;
        if (remaining <= 0)
        {
            return ReadOnlyMemory<byte>.Empty;
        }

        var buffer = new byte[(int)remaining];
        var bytesRead = _stream.Read(buffer, 0, (int)remaining);
        return new ReadOnlyMemory<byte>(buffer, 0, bytesRead);
    }
}
