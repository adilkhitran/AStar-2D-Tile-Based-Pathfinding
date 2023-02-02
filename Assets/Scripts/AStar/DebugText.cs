using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KHiTrAN.PathFinding
{
    public class DebugText : MonoBehaviour
    {
        [SerializeField]
        private RectTransform arrow;

        [SerializeField]
        private Text g, h, f, p;

        public RectTransform Arrow { get => arrow; set => arrow = value; }

        public Text G { get => g; set => g = value; }
        public Text H { get => h; set => h = value; }
        public Text F { get => f; set => f = value; }
        public Text P { get => p; set => p = value; }
    }
}
