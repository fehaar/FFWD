
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
            //Debug.Log("CheckLayerMaskContainsMask mask : "+_mask + " contains : "+_contains);
            /*Debug.Log("CheckLayerMaskContainsLayer mask : "+Convert.ToString(_mask.value,2)+
                " contains : "+Convert.ToString(_contains.value,2) +
                " & : "+Convert.ToString((_mask & _contains),2) +
                " result : "+((_mask & _contains) == _contains.value).ToString());*/
            return ((_mask & _contains) == _contains.value);
        }
    }
}
