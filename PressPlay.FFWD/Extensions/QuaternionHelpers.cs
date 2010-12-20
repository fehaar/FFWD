using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Extensions
{
    public static class QuaternionHelpers
    {
        //public static Microsoft.Xna.Framework.Vector3 eulerAngles(this Quaternion q1)
        //{
        //    //Heading = rotation about y axis
        //    //Attitude = rotation about z axis
        //    //Bank = rotation about x axis
        //    //heading = atan2(2*y*w-2*x*z , 1 - 2*y2 - 2*z2)
        //    //attitude = asin(2*x*y + 2*z*w) 
        //    //bank = atan2(2*x*w-2*y*z , 1 - 2*x2 - 2*z2)
        //    //except when x*y + z*w = 0.5 (north pole)
        //    //  which gives:
        //    //      heading = 2 * atan2(x,w)
        //    //      bank = 0
        //    //and when x*y + z*w = -0.5 (south pole)
        //    //  which gives:
        //    //      heading = -2 * atan2(x,w)
        //    //      bank = 0
        //    Microsoft.Xna.Framework.Vector3 radAngles = new Microsoft.Xna.Framework.Vector3();
        //    Microsoft.Xna.Framework.Vector3 angles = new Microsoft.Xna.Framework.Vector3();
        //    double test = q1.X * q1.Y + q1.Z * q1.W;
        //    if (test > 0.499)// singularity at north pole
        //    {
        //        radAngles.Y = 2 * (float)Math.Atan2(q1.X, q1.W); //heading
        //        radAngles.Z = (float)Math.PI / 2; //attitude
        //        radAngles.X = 0; //bank
        //        angles.X = (float)Math.Round((decimal)MathHelper.ToDegrees(radAngles.X), 3);
        //        angles.Y = (float)Math.Round((decimal)MathHelper.ToDegrees(radAngles.Y), 3);
        //        angles.Z = (float)Math.Round((decimal)MathHelper.ToDegrees(radAngles.Z), 3);
        //        return angles;
        //    }
        //    if (test < -0.499)// singularity at south pole
        //    {
        //        radAngles.Y = -2 * (float)Math.Atan2(q1.X, q1.W); //heading
        //        radAngles.Z = (float)-Math.PI / 2; //attitude
        //        radAngles.X = 0; //bank
        //        angles.X = (float)Math.Round((decimal)MathHelper.ToDegrees(radAngles.X), 3);
        //        angles.Y = (float)Math.Round((decimal)MathHelper.ToDegrees(radAngles.Y), 3);
        //        angles.Z = (float)Math.Round((decimal)MathHelper.ToDegrees(radAngles.Z), 3);
        //        return angles;
        //    }
        //    double sqx = q1.X * q1.X;
        //    double sqy = q1.Y * q1.Y;
        //    double sqz = q1.Z * q1.Z;
        //    radAngles.Y = (float)Math.Atan2(2 * q1.Y * q1.W - 2 * q1.X * q1.Z, 1 - 2 * sqy - 2 * sqz);//heading
        //    radAngles.Z = (float)Math.Asin(2 * test);//attitude
        //    radAngles.X = (float)Math.Atan2(2 * q1.X * q1.W - 2 * q1.Y * q1.Z, 1 - 2 * sqx - 2 * sqz);//bank
        //    angles.X = (float)Math.Round((decimal)MathHelper.ToDegrees(radAngles.X), 3);
        //    angles.Y = (float)Math.Round((decimal)MathHelper.ToDegrees(radAngles.Y), 3);
        //    angles.Z = (float)Math.Round((decimal)MathHelper.ToDegrees(radAngles.Z), 3);
        //    return angles;
        //}

        //public static Quaternion Euler(this Quaternion q, float x, float y, float z)
        //{
        //    return Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(y), MathHelper.ToRadians(x), MathHelper.ToRadians(z));
        //}

        //public static Quaternion Euler(this Quaternion q, Microsoft.Xna.Framework.Vector3 vec)
        //{
        //    return q.Euler(vec.X, vec.Y, vec.Z);
        //}
    }
}
