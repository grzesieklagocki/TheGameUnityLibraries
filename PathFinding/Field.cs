using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HexGameBoard
{
    public class Field
    {
        /// <summary>
        ///     Poziom dostępności pola:
        ///     0 - niedostępne
        ///     1 - tymczasowo niedostępne
        ///     ...
        ///     n - zawsze dostępne
        /// </summary>
        public bool isAvailable;

        public Vector2Int position;
    }
}
