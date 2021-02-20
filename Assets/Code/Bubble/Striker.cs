using UnityEngine;

namespace Assets.Code.Bubble
{
    public class Striker : MonoBehaviour
    {
        public Vector2 DefaultPosition => new Vector2(2, -10);

        public void DestroyComponent()
        {
            Destroy(this);
        }
    }
}