using System;

namespace Assets.Scripts
{
    // Struct for storing attributes of the body nodes linerenderer
    [Serializable]
    public struct BodyLineRendererSettings
    {
        public int bodySortingOrder, limbPositionCount;
        public float bodyStartWidth, bodyEndWidth, limbStartWidth, limbEndWidth;
    }   
}
