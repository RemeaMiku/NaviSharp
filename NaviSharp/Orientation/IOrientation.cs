// RemeaMiku(Wuhan University)
//  Email:2020302142257@whu.edu.cn

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviSharp.Orientation;

public interface IOrientation
{
    #region Public Methods

    public EulerAngles ToEulerAngles();
    public RotationMatrix ToRotationMatrix();
    public RotationVector ToRotationVector();
    public Quaternion<double> ToQuaternion();

    #endregion Public Methods
}
