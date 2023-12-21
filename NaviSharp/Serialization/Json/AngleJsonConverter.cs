// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System.Text.Json;
using System.Text.Json.Serialization;

namespace NaviSharp.Serialization.Json;

public class AngleJsonConverter : JsonConverter<Angle>
{
    public override Angle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return FromDegrees(reader.GetDouble());
    }

    public override void Write(Utf8JsonWriter writer, Angle value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Degrees);
    }
}
