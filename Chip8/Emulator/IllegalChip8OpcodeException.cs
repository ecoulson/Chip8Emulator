using System;
namespace Chip8.Desktop.Emulator
{
    public class IllegalChip8OpcodeException : Exception
    {
        public IllegalChip8OpcodeException() : base()
        {
        }

        public IllegalChip8OpcodeException(string name) : base(name)
        {
        }

        public IllegalChip8OpcodeException(string name, Exception innerException) : base(name, innerException)
        {
        }

    }
}
