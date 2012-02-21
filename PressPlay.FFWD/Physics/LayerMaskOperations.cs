
using System.Collections;
using System;

namespace PressPlay.FFWD
{
    public class LayerMaskOperations
    {

        public static bool CheckLayerMaskContainsLayer(LayerMask _mask, int _layer)
        {
            LayerMask _contains = 1 << _layer;



            return LayerMaskOperations.CheckLayerMaskOverlap(_mask, _contains);
        }

        public static bool CheckLayerMaskOverlap(LayerMask _mask, LayerMask _contains)
        {
            return ((_mask & _contains) == _contains.value);
        }
    }
}
