using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD.Import
{
    internal class CharacterRange
    {
        public CharacterRange(char from)
        {
            FromChar = ToChar = from;
        }

        private char FromChar;
        private char ToChar;

        public bool Extend(char nextChar)
        {
            if (nextChar == (ToChar + 1))
            {
                ToChar = nextChar;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return (FromChar == ToChar) ? Convert.ToInt32(FromChar).ToString() : Convert.ToInt32(FromChar) + "-" + Convert.ToInt32(ToChar);
        }
    }
}
