using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace SuzuFactory.AozoraMahjong
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class MahjongAdjusterUpwardButton : UdonSharpBehaviour
    {
        public Transform col;

        public override void Interact()
        {
            var y = Mathf.Min(col.localPosition.y + 0.05f, 0.9f);
            col.localPosition = new Vector3(0.0f, y, 0.0f);
        }
    }
}
