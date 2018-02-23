using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HexGameBoard
{
    public interface IField
    {
        /// <summary>
        ///     Poziom dostępności pola:
        ///     0 - niedostępne
        ///     1 - tymczasowo niedostępne
        ///     ...
        ///     n - zawsze dostępne
        /// </summary>
        int AvailabilityLevel { get; set; }

        Vector2Int Position { get; set; }
    }
}
