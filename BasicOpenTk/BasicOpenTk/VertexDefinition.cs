using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
namespace BasicOpenTk
{
    public readonly struct VertexAttribute
    {
        public readonly string Name;
        public readonly int Index;
        public readonly int ComponentCount;
        public readonly int Offset;

        public VertexAttribute(string name, int index, int componentCount, int offset)
        {
            Name = name;
            Index = index;
            ComponentCount = componentCount;
            Offset = offset;
        }
    }

    public sealed class VertexInfo
    {
        public readonly Type Type;
        public readonly int SizeInBytes;
        public VertexAttribute[] VertexAttributes;

        public VertexInfo(Type type, params VertexAttribute[] vertexAttributes)
        {
            Type = type;
            VertexAttributes = vertexAttributes;
            SizeInBytes = 0;

            for (int i = 0; i < vertexAttributes.Length; i++)
            {
                var attribute = vertexAttributes[i];
                SizeInBytes += attribute.ComponentCount * sizeof(float);
            }
        }
    }
    public readonly struct VertexPositionColor
    {
        public readonly Vector2 Position;
        public readonly Color4 Color;

        public static readonly VertexInfo VertexInfo = new VertexInfo(
            typeof(VertexPositionColor),
            new VertexAttribute("Position", 0, 2, 0),
            new VertexAttribute("Color", 1, 4, 2 * sizeof(float))
            );
        public VertexPositionColor(Vector2 position, Color4 color)
        {
            Position = position;
            Color = color;
        }
    }

    public readonly struct VertexPositionTexture
    {
        public readonly Vector2 Position;
        public readonly Vector2 TexCoord;

        public VertexPositionTexture(Vector2 position, Vector2 texCoord)
        {
            Position = position;
            TexCoord = texCoord;
        }

        public static readonly VertexInfo vertexInfo = new VertexInfo(
            typeof(VertexPositionTexture),
            new VertexAttribute("Postion", 0, 2, 0),
            new VertexAttribute("TexCoord", 1, 2, 2 * sizeof(float))
            );
    }
}
