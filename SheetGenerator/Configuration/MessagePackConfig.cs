using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using SheetGenerator.MessagePack;

namespace SheetGenerator.Configuration;

public static class MessagePackConfig
{
    public static readonly MessagePackSerializerOptions Options;

    static MessagePackConfig()
    {
        var resolver = CompositeResolver.Create(new IMessagePackFormatter[] { new DynamicObjectFormatter() },
                new[] { StandardResolver.Instance });

        Options = MessagePackSerializerOptions.Standard
                                              .WithResolver(resolver)
                                              .WithCompression(MessagePackCompression.Lz4BlockArray)
                                              .WithSecurity(MessagePackSecurity.TrustedData);
    }
}
