using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace SuzuFactory.AozoraMahjong
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class MahjongTopCameraViewButton : UdonSharpBehaviour
    {
        public GameObject view;
        public GameObject cam;

        public override void Interact()
        {
            if (view.activeSelf)
            {
                view.SetActive(false);
                cam.SetActive(false);
            }
            else
            {
                view.SetActive(true);
                cam.SetActive(true);

                view.transform.position = transform.position + new Vector3(0.0f, -0.05f, 0.0f);
                view.transform.rotation = transform.parent.rotation;
            }
        }
    }
}
