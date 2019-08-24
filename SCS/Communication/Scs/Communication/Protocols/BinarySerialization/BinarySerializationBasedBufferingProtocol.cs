using System.IO;

namespace Hik.Communication.Scs.Communication.Protocols.BinarySerialization
{
    /// <summary>
    ///     Default communication protocol between server and clients to send and receive a message.
    ///     It uses .NET binary serialization to write and read messages.
    ///     A Message format:
    ///     [Message Length (4 bytes)][Serialized Message Content]
    ///     If a message is serialized to byte array as N bytes, this protocol
    ///     adds 4 bytes size information to head of the message bytes, so total length is (4 + N) bytes.
    ///     This class can be derived to change serializer (default: BinaryFormatter). To do this,
    ///     SerializeMessage and DeserializeMessage methods must be overrided.
    /// </summary>
    public class BinarySerializationBasedBufferingProtocol : HeaderBasedBufferingProtocol
    {
        /// <summary>
        ///     Writes a int value to a byte array from a starting index.
        /// </summary>
        /// <param name="buffer">Byte array to write int value</param>
        /// <param name="startIndex">Start index of byte array to write</param>
        /// <param name="number">An integer value to write</param>
        protected override void WriteMessageLen(byte[] buffer, int startIndex, int number)
        {
            buffer[startIndex] = (byte) ((number >> 24) & 0xFF);
            buffer[startIndex + 1] = (byte) ((number >> 16) & 0xFF);
            buffer[startIndex + 2] = (byte) ((number >> 8) & 0xFF);
            buffer[startIndex + 3] = (byte) ((number) & 0xFF);
        }

        /// <summary>
        ///     Deserializes and returns a serialized integer.
        /// </summary>
        /// <returns>Deserialized integer</returns>
        protected override int ReadMessageLen(MemoryStream receiveMemoryStream)
        {
            byte[] buffer = ReadByteArray(receiveMemoryStream, 4);
            return ((buffer[0] << 24) |
                    (buffer[1] << 16) |
                    (buffer[2] << 8) |
                    (buffer[3])
                );
        }
    }
}