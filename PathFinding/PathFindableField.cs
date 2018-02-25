using UnityEngine;

namespace HexGameBoard
{
    /// <summary>
    ///     Pola używane w HexHelper.FindPath(PathFindableField[][], ...) muszą dziedziczyć po tej klasie
    /// </summary>
    public class PathFindableField
    {
        /// <summary>
        ///     Pozycja pola (indeks w tablicy)
        /// </summary>
        public Vector2Int position;

        /// <summary>
        ///     Określa, czy pole jest dostępne (czy można przez niego prowadzić ścieżkę).
        /// </summary>
        public bool isAvailable;


        /// <summary>
        ///     Inicjalizuje nową instację
        /// </summary>
        /// <param name="position">Pozycja pola (indeks w tablicy)</param>
        /// <param name="isAvailable">Określa, czy pole jest dostępne (czy można przez niego prowadzić ścieżkę).</param>
        public PathFindableField(Vector2Int position, bool isAvailable = true)
        {
            this.position = position;
            this.isAvailable = isAvailable;
        }

        /// <summary>
        ///     Inicjalizuje nową instację
        /// </summary>
        /// <param name="positionX">Pozycja pola X (indeks w tablicy)</param>
        /// <param name="positionY">Pozycja pola Y (indeks w tablicy)</param>
        /// <param name="isAvailable">Określa, czy pole jest dostępne (czy można przez niego prowadzić ścieżkę).</param>
        public PathFindableField(int positionX, int positionY, bool isAvailable = true) : this(new Vector2Int(positionX, positionY), isAvailable)
        {

        }
    }
}
