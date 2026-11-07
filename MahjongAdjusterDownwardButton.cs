using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace SuzuFactory.AozoraMahjong
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class MahjongAdjusterDownwardButton : UdonSharpBehaviour
    {
        public Transform col;

        public override void Interact()
        {
            var y = Mathf.Max(col.localPosition.y - 0.05f, 0.0f);
            col.localPosition = new Vector3(0.0f, y, 0.0f);
        }
    }
}
