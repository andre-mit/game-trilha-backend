using GameTrilha.API.ViewModels.GameViewModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameTrilha.API.Helpers.JsonConverters;

public class MultidimensionalByteArrayJsonConverter : JsonConverter<byte[,]>
{
    public override byte[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);

        var rowLength = jsonDoc.RootElement.GetArrayLength();
        var columnLength = jsonDoc.RootElement.EnumerateArray().First().GetArrayLength();

        var grid = new byte[rowLength, columnLength];

        var row = 0;
        foreach (var array in jsonDoc.RootElement.EnumerateArray())
        {
            var column = 0;
            foreach (var number in array.EnumerateArray())
            {
                grid[row, column] = number.GetByte();
                column++;
            }
            row++;
        }

        return grid;
    }

    public override void Write(Utf8JsonWriter writer, byte[,] value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        for (var i = 0; i < value.GetLength(0); i++)
        {
            writer.WriteStartArray();
            for (var j = 0; j < value.GetLength(1); j++)
            {
                writer.WriteNumberValue(value[i, j]);
            }
            writer.WriteEndArray();
        }
        writer.WriteEndArray();
    }
}

public class MultidimensionalObjectArrayJsonConverter<T> : JsonConverter<T[,]> where T : notnull
{
    public override T[,]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDoc = JsonDocument.ParseValue(ref reader);

        var rowLength = jsonDoc.RootElement.GetArrayLength();
        var columnLength = jsonDoc.RootElement.EnumerateArray().First().GetArrayLength();

        var grid = new T?[rowLength, columnLength];

        var row = 0;
        foreach (var array in jsonDoc.RootElement.EnumerateArray())
        {
            var column = 0;
            foreach (var element in array.EnumerateArray())
            {
                grid[row, column] = element.Deserialize<T>();
                column++;
            }
            row++;
        }

        return grid;
    }

    public override void Write(Utf8JsonWriter writer, T[,] value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        for (var i = 0; i < value.GetLength(0); i++)
        {
            writer.WriteStartArray();
            for (var j = 0; j < value.GetLength(1); j++)
            {
                writer.WriteRawValue(JsonSerializer.Serialize(value[i,j]));
            }
            writer.WriteEndArray();
        }
        writer.WriteEndArray();
    }
}