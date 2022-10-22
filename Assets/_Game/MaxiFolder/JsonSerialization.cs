using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Pun;

namespace _Game.MaxiFolder
{
    public class JsonSerialization : MonoBehaviourPun
    {
        public Transform obj;
        public int hp;
        public float speed;
        public bool isAlive;
        public List<string> namesList = new List<string>();
        // diccionario no se puede serializar, se tendria que hacer una clase propia para serializar
        public Dictionary<int, string> PlayerObjs = new Dictionary<int, string>();
        public string path;
        public string filename;

        [ContextMenu("SaveJson")]
        private void Serialization()
        {
            PlayerObjs.Add(1,"test");
            var data = new SerializationInfo
            {
                pos = obj.transform.position,
                hp = hp,
                speed = speed,
                IsAlive = isAlive,
                namesList = namesList,
                PlayerObjs = PlayerObjs,
            };
            var gameFolder = Application.dataPath;
            //para crear una carpeta si no existe ya
            Directory.CreateDirectory(Path.Combine(gameFolder, path));  
            var realPath = Path.Combine(gameFolder, path, filename + ".json");
            var json = JsonUtility.ToJson(data, true);
        
            // estas 3 lineas son lo mismo que el File.wrirtealltext lo dejo por si hay que separar el proceso en partes
            // StreamWriter file = File.CreateText(realPath);
            // file.Write(json);
            // file.Close();
            File.WriteAllText(realPath, json);
        }
    }
}
