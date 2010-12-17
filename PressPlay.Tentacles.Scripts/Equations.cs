using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD;

namespace PressPlay.Tentacles.Scripts
{

public enum Ease {
    Linear = 0,
    EaseQuadIn = 1,
    EaseQuadOut = 2,
    EaseQuadInOut = 3,
    EaseQuadOutIn = 4,
    EaseCubicIn = 5,
    EaseCubicOut = 6,
    EaseCubicInOut = 7,
    EaseCubicOutIn = 8,
    EaseQuartIn = 9,
    EaseQuartOut = 10, 
    EaseQuartInOut = 11, 
    EaseQuartOutIn = 12, 
    EaseQuintIn = 13, 
    EaseQuintOut = 14, 
    EaseQuintInOut = 15, 
    EaseQuintOutIn = 16,
    EaseSineIn = 17,
    EaseSineOut = 18,
    EaseSineInOut = 19,
    EaseSineOutIn = 20,
    EaseExpoIn = 21, 
    EaseExpoOut = 22, 
    EaseExpoInOut = 23, 
    EaseExpoOutIn = 24,
    EaseCircIn = 25, 
    EaseCircOut = 26, 
    EaseCircInOut = 27, 
    EaseCircOutIn = 28, 
    EaseElasticIn = 29, 
    EaseElasticOut = 30, 
    EaseElasticInOut = 31, 
    EaseElasticOutIn = 32,
    EaseBackIn = 33,
    EaseBackOut = 34,
    EaseBackInOut = 35,
    EaseBackOutIn = 36,
    EaseBounceIn = 37,
    EaseBounceOut = 38,
    EaseBounceInOut = 39, 
    EaseBounceOutIn = 40
}
 
public class Equations {

   
    // TWEENING EQUATIONS floats -----------------------------------------------------------------------------------------------------
    // (the original equations are Robert Penner's work as mentioned on the disclaimer)

    /**
     * Easing equation float for a simple linear tweening, with no easing.
     *
     * @param t  Current time (in frames or seconds).
     * @param b  Starting value.
     * @param c  Change needed in value.
     * @param d  Expected easing duration (in frames or seconds).
     * @return    The correct value.
     */
    
    public static float EaseNone (float t, float b, float c, float d) {
        return c * t / d + b;
    }
   
    /**
     * Easing equation float for a quadratic (t^2) easing in: accelerating from zero velocity.
     *
     * @param t  Current time (in frames or seconds).
     * @param b  Starting value.
     * @param c  Change needed in value.
     * @param d  Expected easing duration (in frames or seconds).
     * @return    The correct value.
     */
    public static float EaseQuadIn (float t, float b, float c, float d) {
        return c * (t/=d) * t + b;
    }
   
    /**
     * Easing equation float for a quadratic (t^2) easing out: decelerating to zero velocity.
     *
     * @param t  Current time (in frames or seconds).
     * @param b  Starting value.
     * @param c  Change needed in value.
     * @param d  Expected easing duration (in frames or seconds).
     * @return    The correct value.
     */
    public static float EaseQuadOut (float t, float b, float c, float d) {
        return -c *(t/=d)*(t-2) + b;
    }

    /**
     * Easing equation float for a quadratic (t^2) easing in/out: acceleration until halfway, then deceleration.
     *
     * @param t  Current time (in frames or seconds).
     * @param b  Starting value.
     * @param c  Change needed in value.
     * @param d  Expected easing duration (in frames or seconds).
     * @return    The correct value.
     */
     public static float EaseQuadInOut  (float t, float b, float c, float d) {
       
        if ((t/=d/2) < 1) return c/2*t*t + b;
       
        return -c/2 * ((--t)*(t-2) - 1) + b;
    }
   
    /**
     * Easing equation float for a quadratic (t^2) easing out/in: deceleration until halfway, then acceleration.
     *
     * @param t  Current time (in frames or seconds).
     * @param b  Starting value.
     * @param c  Change needed in value.
     * @param d  Expected easing duration (in frames or seconds).
     * @return    The correct value.
     */
    public static float EaseQuadOutIn (float t, float b, float c, float d) {
        if (t < d/2) return EaseQuadOut (t*2, b, c/2, d);
        return EaseQuadIn((t*2)-d, b+c/2, c/2, d);
    }

    /**
     * Easing equation float for a cubic (t^3) easing in: accelerating from zero velocity.
         *
     * @param t  Current time (in frames or seconds).
     * @param b  Starting value.
     * @param c  Change needed in value.
     * @param d  Expected easing duration (in frames or seconds).
     * @return    The correct value.
     */
    public static float EaseCubicIn (float t, float b, float c, float d) {
        return c*(t/=d)*t*t + b;
    }

    /**
     * Easing equation float for a cubic (t^3) easing out: decelerating from zero velocity.
         *
     * @param t  Current time (in frames or seconds).
     * @param b  Starting value.
     * @param c  Change needed in value.
     * @param d  Expected easing duration (in frames or seconds).
     * @return    The correct value.
     */
    public static float EaseCubicOut (float t, float b, float c, float d) {
        return c*((t=t/d-1)*t*t + 1) + b;
    }

    /**
     * Easing equation float for a cubic (t^3) easing in/out: acceleration until halfway, then deceleration.
         *
     * @param t  Current time (in frames or seconds).
     * @param b  Starting value.
     * @param c  Change needed in value.
     * @param d  Expected easing duration (in frames or seconds).
     * @return    The correct value.
     */
    public static float EaseCubicInOut (float t, float b, float c, float d) {
        if ((t/=d/2) < 1) return c/2*t*t*t + b;
        return c/2*((t-=2)*t*t + 2) + b;
    }

    /**
     * Easing equation float for a cubic (t^3) easing out/in: deceleration until halfway, then acceleration.
         *
     * @param t  Current time (in frames or seconds).
     * @param b  Starting value.
     * @param c  Change needed in value.
     * @param d  Expected easing duration (in frames or seconds).
     * @return    The correct value.
     */
    public static float EaseCubicOutIn (float t, float b, float c, float d) {
        if (t < d/2) return EaseCubicOut (t*2, b, c/2, d);
        return EaseCubicIn((t*2)-d, b+c/2, c/2, d);
    }
   
    /**
         * Easing equation float for a quartic (t^4) easing in: accelerating from zero velocity.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseQuartIn (float t, float b, float c, float d) {
            return c*(t/=d)*t*t*t + b;
        }
   
        /**
         * Easing equation float for a quartic (t^4) easing out: decelerating from zero velocity.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseQuartOut (float t, float b, float c, float d) {
            return -c * ((t=t/d-1)*t*t*t - 1) + b;
        }
   
        /**
         * Easing equation float for a quartic (t^4) easing in/out: acceleration until halfway, then deceleration.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseQuartInOut (float t, float b, float c, float d) {
            if ((t/=d/2) < 1) return c/2*t*t*t*t + b;
            return -c/2 * ((t-=2)*t*t*t - 2) + b;
        }
   
        /**
         * Easing equation float for a quartic (t^4) easing out/in: deceleration until halfway, then acceleration.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseQuartOutIn (float t, float b, float c, float d) {
            if (t < d/2) return EaseQuartOut (t*2, b, c/2, d);
            return EaseQuartIn((t*2)-d, b+c/2, c/2, d);
        }
   
        /**
         * Easing equation float for a quintic (t^5) easing in: accelerating from zero velocity.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseQuintIn (float t, float b, float c, float d) {
            return c*(t/=d)*t*t*t*t + b;
        }
   
        /**
         * Easing equation float for a quintic (t^5) easing out: decelerating from zero velocity.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseQuintOut (float t, float b, float c, float d) {
            return c*((t=t/d-1)*t*t*t*t + 1) + b;
        }
   
        /**
         * Easing equation float for a quintic (t^5) easing in/out: acceleration until halfway, then deceleration.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseQuintInOut (float t, float b, float c, float d) {
            if ((t/=d/2) < 1) return c/2*t*t*t*t*t + b;
            return c/2*((t-=2)*t*t*t*t + 2) + b;
        }
   
        /**
         * Easing equation float for a quintic (t^5) easing out/in: deceleration until halfway, then acceleration.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseQuintOutIn (float t, float b, float c, float d) {
            if (t < d/2) return EaseQuintOut (t*2, b, c/2, d);
            return EaseQuintIn((t*2)-d, b+c/2, c/2, d);
        }
   
        /**
         * Easing equation float for a sinusoidal (sin(t)) easing in: accelerating from zero velocity.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseSineIn (float t, float b, float c, float d) {
            return -c * Mathf.Cos(t/d * (Mathf.PI/2)) + c + b;
        }
   
        /**
         * Easing equation float for a sinusoidal (sin(t)) easing out: decelerating from zero velocity.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseSineOut (float t, float b, float c, float d) {
            return c * Mathf.Sin(t/d * (Mathf.PI/2)) + b;
        }
   
        /**
         * Easing equation float for a sinusoidal (sin(t)) easing in/out: acceleration until halfway, then deceleration.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseSineInOut (float t, float b, float c, float d) {
            return -c/2 * (Mathf.Cos(Mathf.PI*t/d) - 1) + b;
        }
   
        /**
         * Easing equation float for a sinusoidal (sin(t)) easing out/in: deceleration until halfway, then acceleration.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseSineOutIn (float t, float b, float c, float d) {
            if (t < d/2) return EaseSineOut (t*2, b, c/2, d);
            return EaseSineIn((t*2)-d, b+c/2, c/2, d);
        }
   
        /**
         * Easing equation float for an exponential (2^t) easing in: accelerating from zero velocity.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseExpoIn (float t, float b, float c, float d) {
            return (t==0) ? b : c * Mathf.Pow(2, 10 * (t/d - 1)) + b - c * 0.001f;
        }
   
        /**
         * Easing equation float for an exponential (2^t) easing out: decelerating from zero velocity.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseExpoOut (float t, float b, float c, float d) {
            return (t==d) ? b+c : c * 1.001f * (-Mathf.Pow(2, -10 * t/d) + 1) + b;
        }
   
        /**
         * Easing equation float for an exponential (2^t) easing in/out: acceleration until halfway, then deceleration.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseExpoInOut (float t, float b, float c, float d) {
            if (t==0) return b;
            if (t==d) return b+c;
            if ((t/=d/2) < 1) return c/2 * Mathf.Pow(2, 10 * (t - 1)) + b - c * 0.0005f;
            return c/2 * 1.0005f * (-Mathf.Pow(2, -10 * --t) + 2) + b;
        }
   
        /**
         * Easing equation float for an exponential (2^t) easing out/in: deceleration until halfway, then acceleration.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseExpoOutIn (float t, float b, float c, float d) {
            if (t < d/2) return EaseExpoOut (t*2, b, c/2, d);
            return EaseExpoIn((t*2)-d, b+c/2, c/2, d);
        }
   
        /**
         * Easing equation float for a circular (sqrt(1-t^2)) easing in: accelerating from zero velocity.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseCircIn (float t, float b, float c, float d) {
            return -c * (Mathf.Sqrt(1 - (t/=d)*t) - 1) + b;
        }
   
        /**
         * Easing equation float for a circular (sqrt(1-t^2)) easing out: decelerating from zero velocity.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseCircOut (float t, float b, float c, float d) {
            return c * Mathf.Sqrt(1 - (t=t/d-1)*t) + b;
        }
   
        /**
         * Easing equation float for a circular (sqrt(1-t^2)) easing in/out: acceleration until halfway, then deceleration.
        *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseCircInOut (float t, float b, float c, float d) {
            if ((t/=d/2) < 1) return -c/2 * (Mathf.Sqrt(1 - t*t) - 1) + b;
            return c/2 * (Mathf.Sqrt(1 - (t-=2)*t) + 1) + b;
        }
   
        /**
         * Easing equation float for a circular (sqrt(1-t^2)) easing out/in: deceleration until halfway, then acceleration.
         *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseCircOutIn (float t, float b, float c, float d) {
            if (t < d/2) return EaseCircOut (t*2, b, c/2, d);
            return EaseCircIn((t*2)-d, b+c/2, c/2, d);
        }
   
        /**
         * Easing equation float for an elastic (exponentially decaying sine wave) easing in: accelerating from zero velocity.
         *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @param a  Amplitude.
         * @param p  Period.
         * @return    The correct value.
         */
        public static float EaseElasticIn (float t, float b, float c, float d) {
            if (t==0) return b;
            if ((t/=d)==1) return b+c;
            float p =  d *.3f;
            float s = 0;
            float a = 0;
            if (a == 0f || a < Mathf.Abs(c)) {
                a = c;
                s = p/4;
            } else {
                s = p/(2*Mathf.PI) * Mathf.Asin (c/a);
            }
            return -(a*Mathf.Pow(2,10*(t-=1)) * Mathf.Sin( (t*d-s)*(2*Mathf.PI)/p )) + b;
        }
   
        /**
         * Easing equation float for an elastic (exponentially decaying sine wave) easing out: decelerating from zero velocity.
         *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @param a  Amplitude.
         * @param p  Period.
         * @return    The correct value.
         */
        public static float EaseElasticOut (float t, float b, float c, float d) {
            if (t==0) return b;
            if ((t/=d)==1) return b+c;
            float p = d*.3f;
            float s = 0;
            float a = 0;
            if (a == 0f || a < Mathf.Abs(c)) {
                a = c;
                s = p/4;
            } else {
                s = p/(2*Mathf.PI) * Mathf.Asin (c/a);
            }
            return (a*Mathf.Pow(2,-10*t) * Mathf.Sin( (t*d-s)*(2*Mathf.PI)/p ) + c + b);
        }
   
        /**
         * Easing equation float for an elastic (exponentially decaying sine wave) easing in/out: acceleration until halfway, then deceleration.
         *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @param a  Amplitude.
         * @param p  Period.
         * @return    The correct value.
         */
        public static float EaseElasticInOut (float t, float b, float c, float d) {
            if (t==0) return b;
            if ((t/=d/2)==2) return b+c;
            float p =  d*(.3f*1.5f);
            float s = 0;
            float a = 0;
            if (a == 0f || a < Mathf.Abs(c)) {
                a = c;
                s = p/4;
            } else {
                s = p/(2*Mathf.PI) * Mathf.Asin (c/a);
            }
            if (t < 1) return -.5f*(a*Mathf.Pow(2,10*(t-=1)) * Mathf.Sin( (t*d-s)*(2*Mathf.PI)/p )) + b;
            return a*Mathf.Pow(2,-10*(t-=1)) * Mathf.Sin( (t*d-s)*(2*Mathf.PI)/p )*.5f + c + b;
        }
   
        /**
         * Easing equation float for an elastic (exponentially decaying sine wave) easing out/in: deceleration until halfway, then acceleration.
         *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @param a  Amplitude.
         * @param p  Period.
         * @return    The correct value.
         */
        public static float EaseElasticOutIn (float t, float b, float c, float d) {
            if (t < d/2) return EaseElasticOut (t*2, b, c/2, d);
            return EaseElasticIn((t*2)-d, b+c/2, c/2, d);
        }
   
        /**
         * Easing equation float for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: accelerating from zero velocity.
         *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @param s  Overshoot ammount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
         * @return    The correct value.
         */
        public static float EaseBackIn (float t, float b, float c, float d) {
            float s = 1.70158f;
            return c*(t/=d)*t*((s+1)*t - s) + b;
        }
   
        /**
         * Easing equation float for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: decelerating from zero velocity.
         *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @param s  Overshoot ammount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
         * @return    The correct value.
         */
        public static float EaseBackOut (float t, float b, float c, float d) {
            float s = 1.70158f;
            return c*((t=t/d-1)*t*((s+1)*t + s) + 1) + b;
        }
   
        /**
         * Easing equation float for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: acceleration until halfway, then deceleration.
         *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @param s  Overshoot ammount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
         * @return    The correct value.
         */
        public static float EaseBackInOut (float t, float b, float c, float d) {
            float s =  1.70158f;
            if ((t/=d/2) < 1) return c/2*(t*t*(((s*=(1.525f))+1)*t - s)) + b;
            return c/2*((t-=2)*t*(((s*=(1.525f))+1)*t + s) + 2) + b;
        }
   
        /**
         * Easing equation float for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out/in: deceleration until halfway, then acceleration.
         *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @param s  Overshoot ammount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
         * @return    The correct value.
         */
        public static float EaseBackOutIn (float t, float b, float c, float d) {
            if (t < d/2) return EaseBackOut (t*2, b, c/2, d);
            return EaseBackIn((t*2)-d, b+c/2, c/2, d);
        }
   
        /**
         * Easing equation float for a bounce (exponentially decaying parabolic bounce) easing in: accelerating from zero velocity.
         *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseBounceIn (float t, float b, float c, float d) {
            return c - EaseBounceOut (d-t, 0, c, d) + b;
        }
   
        /**
         * Easing equation float for a bounce (exponentially decaying parabolic bounce) easing out: decelerating from zero velocity.
         *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseBounceOut (float t, float b, float c, float d) {
            if ((t/=d) < (1/2.75f)) {
                return c*(7.5625f*t*t) + b;
            } else if (t < (2/2.75f)) {
                return c*(7.5625f*(t-=(1.5f/2.75f))*t + .75f) + b;
            } else if (t < (2.5f/2.75f)) {
                return c*(7.5625f*(t-=(2.25f/2.75f))*t + .9375f) + b;
            } else {
                return c*(7.5625f*(t-=(2.625f/2.75f))*t + .984375f) + b;
            }
        }
   
        /**
         * Easing equation float for a bounce (exponentially decaying parabolic bounce) easing in/out: acceleration until halfway, then deceleration.
         *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseBounceInOut (float t, float b, float c, float d) {
            if (t < d/2) return EaseBounceIn (t*2, 0, c, d) * .5f + b;
            else return EaseBounceOut (t*2-d, 0, c, d) * .5f + c*.5f + b;
        }
   
        /**
         * Easing equation float for a bounce (exponentially decaying parabolic bounce) easing out/in: deceleration until halfway, then acceleration.
         *
         * @param t  Current time (in frames or seconds).
         * @param b  Starting value.
         * @param c  Change needed in value.
         * @param d  Expected easing duration (in frames or seconds).
         * @return    The correct value.
         */
        public static float EaseBounceOutIn (float t, float b, float c, float d) {
            if (t < d/2) return EaseBounceOut (t*2, b, c/2, d);
            return EaseBounceIn((t*2)-d, b+c/2, c/2, d);
        }
   
            
    /// <summary>
    /// This function tweens between two vectors.
    /// </summary>
    /// <param name="t">Time</param>
    /// <param name="b">Begin</param>
    /// <param name="c">Change</param>
    /// <param name="d">Duration</param>
    /// <param name="Ease"></param>
    /// <returns></returns>
    public static Vector3 ChangeVector(float t , Vector3 b , Vector3 c , float d , Ease Ease)
    {
        float x = 0;
        float y = 0;
        float z = 0;
       
        if(Ease == Ease.Linear)
        {
            x = EaseNone (t , b.x , c.x , d);
            y = EaseNone (t , b.y , c.y , d);
            z = EaseNone (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseQuadIn)
        {
            x = EaseQuadIn (t , b.x , c.x , d);
            y = EaseQuadIn (t , b.y , c.y , d);
            z = EaseQuadIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseQuadOut)
        {
            x = EaseQuadOut (t , b.x , c.x , d);
            y = EaseQuadOut (t , b.y , c.y , d);
            z = EaseQuadOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseQuadInOut)
        {
            x = EaseQuadInOut (t , b.x , c.x , d);
            y = EaseQuadInOut (t , b.y , c.y , d);
            z = EaseQuadInOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseQuadOutIn)
        {
            x = EaseQuadOutIn (t , b.x , c.x , d);
            y = EaseQuadOutIn (t , b.y , c.y , d);
            z = EaseQuadOutIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseCubicIn)
        {
            x = EaseCubicIn (t , b.x , c.x , d);
            y = EaseCubicIn (t , b.y , c.y , d);
            z = EaseCubicIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseCubicOut)
        {
            x = EaseCubicOut (t , b.x , c.x , d);
            y = EaseCubicOut (t , b.y , c.y , d);
            z = EaseCubicOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseCubicInOut)
        {
            x = EaseCubicInOut (t , b.x , c.x , d);
            y = EaseCubicInOut (t , b.y , c.y , d);
            z = EaseCubicInOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseCubicOutIn)
        {
            x = EaseCubicOutIn (t , b.x , c.x , d);
            y = EaseCubicOutIn (t , b.y , c.y , d);
            z = EaseCubicOutIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseQuartIn)
        {
            x = EaseQuartIn (t , b.x , c.x , d);
            y = EaseQuartIn (t , b.y , c.y , d);
            z = EaseQuartIn (t , b.z , c.z , d);
        }   
        else if(Ease == Ease.EaseQuartOut) 
        {
            x = EaseQuartOut (t , b.x , c.x , d);
            y = EaseQuartOut (t , b.y , c.y , d);
            z = EaseQuartOut (t , b.z , c.z , d);
        }   
        else if(Ease == Ease.EaseQuartInOut)
        {
            x = EaseQuartInOut (t , b.x , c.x , d);
            y = EaseQuartInOut (t , b.y , c.y , d);
            z = EaseQuartInOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseQuartOutIn)
        {
            x = EaseQuartOutIn (t , b.x , c.x , d);
            y = EaseQuartOutIn (t , b.y , c.y , d);
            z = EaseQuartOutIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseQuintIn)
        {
            x = EaseQuintIn (t , b.x , c.x , d);
            y = EaseQuintIn (t , b.y , c.y , d);
            z = EaseQuintIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseQuintOut)
        {
            x = EaseQuintOut (t , b.x , c.x , d);
            y = EaseQuintOut (t , b.y , c.y , d);
            z = EaseQuintOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseQuintInOut)
        {
            x = EaseQuintInOut (t , b.x , c.x , d);
            y = EaseQuintInOut (t , b.y , c.y , d);
            z = EaseQuintInOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseQuintOutIn)
        {
            x = EaseQuintOutIn (t , b.x , c.x , d);
            y = EaseQuintOutIn (t , b.y , c.y , d);
            z = EaseQuintOutIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseSineIn)
        {
            x = EaseSineIn (t , b.x , c.x , d);
            y = EaseSineIn (t , b.y , c.y , d);
            z = EaseSineIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseSineOut)
        {
            x = EaseSineOut (t , b.x , c.x , d);
            y = EaseSineOut (t , b.y , c.y , d);
            z = EaseSineOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseSineInOut)
        {
            x = EaseSineInOut (t , b.x , c.x , d);
            y = EaseSineInOut (t , b.y , c.y , d);
            z = EaseSineInOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseSineOutIn)
        {
            x = EaseSineOutIn (t , b.x , c.x , d);
            y = EaseSineOutIn (t , b.y , c.y , d);
            z = EaseSineOutIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseExpoIn)
        {
            x = EaseExpoIn (t , b.x , c.x , d);
            y = EaseExpoIn (t , b.y , c.y , d);
            z = EaseExpoIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseExpoOut)
        {
            x = EaseExpoOut (t , b.x , c.x , d);
            y = EaseExpoOut (t , b.y , c.y , d);
            z = EaseExpoOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseExpoInOut)
        {
            x = EaseExpoInOut (t , b.x , c.x , d);
            y = EaseExpoInOut (t , b.y , c.y , d);
            z = EaseExpoInOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseExpoOutIn)
        {
            x = EaseExpoOutIn (t , b.x , c.x , d);
            y = EaseExpoOutIn (t , b.y , c.y , d);
            z = EaseExpoOutIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseCircIn)
        {
            x = EaseCircIn (t , b.x , c.x , d);
            y = EaseCircIn (t , b.y , c.y , d);
            z = EaseCircIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseCircOut)
        {
            x = EaseCircOut (t , b.x , c.x , d);
            y = EaseCircOut (t , b.y , c.y , d);
            z = EaseCircOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseCircInOut)
        {
            x = EaseCircInOut (t , b.x , c.x , d);
            y = EaseCircInOut (t , b.y , c.y , d);
            z = EaseCircInOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseCircOutIn)
        {
            x = EaseCircOutIn (t , b.x , c.x , d);
            y = EaseCircOutIn (t , b.y , c.y , d);
            z = EaseCircOutIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseElasticIn)
        {
            x = EaseElasticIn (t , b.x , c.x , d);
            y = EaseElasticIn (t , b.y , c.y , d);
            z = EaseElasticIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseElasticOut)
        {
            x = EaseElasticOut (t , b.x , c.x , d);
            y = EaseElasticOut (t , b.y , c.y , d);
            z = EaseElasticOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseElasticInOut)
        {
            x = EaseElasticInOut (t , b.x , c.x , d);
            y = EaseElasticInOut (t , b.y , c.y , d);
            z = EaseElasticInOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseElasticOutIn)
        {
            x = EaseElasticOutIn (t , b.x , c.x , d);
            y = EaseElasticOutIn (t , b.y , c.y , d);
            z = EaseElasticOutIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseBackIn)
        {
            x = EaseBackIn (t , b.x , c.x , d);
            y = EaseBackIn (t , b.y , c.y , d);
            z = EaseBackIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseBackOut)
        {
            x = EaseBackOut (t , b.x , c.x , d);
            y = EaseBackOut (t , b.y , c.y , d);
            z = EaseBackOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseBackInOut)
        {
            x = EaseBackInOut (t , b.x , c.x , d);
            y = EaseBackInOut (t , b.y , c.y , d);
            z = EaseBackInOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseBackOutIn)
        {
            x = EaseBackOutIn (t , b.x , c.x , d);
            y = EaseBackOutIn (t , b.y , c.y , d);
            z = EaseBackOutIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseBounceIn)
        {
            x = EaseBounceIn (t , b.x , c.x , d);
            y = EaseBounceIn (t , b.y , c.y , d);
            z = EaseBounceIn (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseBounceOut)
        {
            x = EaseBounceOut (t , b.x , c.x , d);
            y = EaseBounceOut (t , b.y , c.y , d);
            z = EaseBounceOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseBounceInOut)
        {
            x = EaseBounceInOut (t , b.x , c.x , d);
            y = EaseBounceInOut (t , b.y , c.y , d);
            z = EaseBounceInOut (t , b.z , c.z , d);
        }
        else if(Ease == Ease.EaseBounceOutIn)
        {
            x = EaseBounceOutIn (t , b.x , c.x , d);
            y = EaseBounceOutIn (t , b.y , c.y , d);
            z = EaseBounceOutIn (t , b.z , c.z , d);
        }
       

        return new Vector3(x,y,z);
    }

    /// <summary>
    /// This function tweens between two float.
    /// </summary>
    /// <param name="t">Time</param>
    /// <param name="b">Begin</param>
    /// <param name="c">Change</param>
    /// <param name="d">Duration</param>
    /// <param name="Ease"></param>
    /// <returns></returns>    
    public static float ChangeFloat(float t , float b , float c , float d , Ease Ease)
    {
        float value = 0;
       
        if(Ease == Ease.Linear)
            value = EaseNone (t , b , c , d);
        else if(Ease == Ease.EaseQuadIn)
            value = EaseQuadIn (t , b , c , d);
        else if(Ease == Ease.EaseQuadOut)
            value = EaseQuadOut (t , b , c , d);
        else if(Ease == Ease.EaseQuadInOut)
            value = EaseQuadInOut (t , b , c , d);
        else if(Ease == Ease.EaseQuadOutIn)
            value = EaseQuadOutIn (t , b , c , d);
        else if(Ease == Ease.EaseCubicIn)
            value = EaseCubicIn (t , b , c , d);
        else if(Ease == Ease.EaseCubicOut)
            value = EaseCubicOut (t , b , c , d);
        else if(Ease == Ease.EaseCubicInOut)
            value = EaseCubicInOut (t , b , c , d);
        else if(Ease == Ease.EaseCubicOutIn)
            value = EaseCubicOutIn (t , b , c , d);
        else if(Ease == Ease.EaseQuartIn)
            value = EaseQuartIn (t , b , c , d);
        else if(Ease == Ease.EaseQuartOut)
            value = EaseQuartOut (t , b , c , d);
        else if(Ease == Ease.EaseQuartInOut)
            value = EaseQuartInOut (t , b , c , d);
        else if(Ease == Ease.EaseQuartOutIn)
            value = EaseQuartOutIn (t , b , c , d);
        else if(Ease == Ease.EaseQuintIn)
            value = EaseQuintIn (t , b , c , d);
        else if(Ease == Ease.EaseQuintOut)
            value = EaseQuintOut (t , b , c , d);
        else if(Ease == Ease.EaseQuintInOut)
            value = EaseQuintInOut (t , b , c , d);
        else if(Ease == Ease.EaseQuintOutIn)
            value = EaseQuintOutIn (t , b , c , d);
        else if(Ease == Ease.EaseSineIn)
            value = EaseSineIn (t , b , c , d);
        else if(Ease == Ease.EaseSineOut)
            value = EaseSineOut (t , b , c , d);
        else if(Ease == Ease.EaseSineInOut)
            value = EaseSineInOut (t , b , c , d);
        else if(Ease == Ease.EaseSineOutIn)
            value = EaseSineOutIn (t , b , c , d);
        else if(Ease == Ease.EaseExpoIn)
            value = EaseExpoIn (t , b , c , d);
        else if(Ease == Ease.EaseExpoOut)
            value = EaseExpoOut (t , b , c , d);
        else if(Ease == Ease.EaseExpoInOut)
            value = EaseExpoInOut (t , b , c , d);
        else if(Ease == Ease.EaseExpoOutIn)
            value = EaseExpoOutIn (t , b , c , d);
        else if(Ease == Ease.EaseCircIn)
            value = EaseCircIn (t , b , c , d);
        else if(Ease == Ease.EaseCircOut)
            value = EaseCircOut (t , b , c , d);
        else if(Ease == Ease.EaseCircInOut)
            value = EaseCircInOut (t , b , c , d);
        else if(Ease == Ease.EaseCircOutIn)
            value = EaseCircOutIn (t , b , c , d);
        else if(Ease == Ease.EaseElasticIn)
            value = EaseElasticIn (t , b , c , d);
        else if(Ease == Ease.EaseElasticOut)
            value = EaseElasticOut (t , b , c , d);
        else if(Ease == Ease.EaseElasticInOut)
            value = EaseElasticInOut (t , b , c , d);
        else if(Ease == Ease.EaseElasticOutIn)
            value = EaseElasticOutIn (t , b , c , d);
        else if(Ease == Ease.EaseBackIn)
            value = EaseBackIn (t , b , c , d);
        else if(Ease == Ease.EaseBackOut)
            value = EaseBackOut (t , b , c , d);
        else if(Ease == Ease.EaseBackInOut)
            value = EaseBackInOut (t , b , c , d);
        else if(Ease == Ease.EaseBackOutIn)
            value = EaseBackOutIn (t , b , c , d);
        else if(Ease == Ease.EaseBounceIn)
            value = EaseBounceIn (t , b , c , d);
        else if(Ease == Ease.EaseBounceOut)
            value = EaseBounceOut (t , b , c , d);
        else if(Ease == Ease.EaseBounceInOut)
            value = EaseBounceInOut (t , b , c , d);
        else if(Ease == Ease.EaseBounceOutIn)
            value = EaseBounceOutIn (t , b , c , d);
       
        return value;
    }
}}
