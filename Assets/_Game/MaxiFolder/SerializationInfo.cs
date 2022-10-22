using System.Collections.Generic;
using UnityEngine;

namespace _Game.MaxiFolder
{
    [System.Serializable]
    public class SerializationInfo
    {
        public Vector3 pos;
        public int hp;
        public float speed;
        [SerializeField] private bool isAlive;
        public List<string> namesList = new List<string>();
        // diccionario no se puede serializar, se tendria que hacer una clase propia para serializar
        public Dictionary<int, string> PlayerObjs = new Dictionary<int, string>();

        public bool IsAlive
        {
            get => isAlive;
            set => isAlive = value;
        }
    }
}
