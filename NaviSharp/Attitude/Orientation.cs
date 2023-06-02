namespace NaviSharp;

public record class Orientation
{
    public EulerAngles EulerAngles { get; }
    public Matrix<double> Matrix { get; }
    public Quaternion<double> Quaternion { get; }

    public Orientation(EulerAngles eulerAngles)
    {
        EulerAngles = eulerAngles;
        Matrix = eulerAngles.ToRotationMatrix<double>();
        Quaternion = eulerAngles.ToQuaternion<double>();
    }

    public Orientation(Matrix<double> rotationMatrix)
    {
        if (!rotationMatrix.IsSizeOf(3, 3))
            throw new ArgumentException("The rotation matrix must be a square matrix of order 3.", nameof(rotationMatrix));
        Matrix = rotationMatrix;
        EulerAngles = rotationMatrix.ToEulerAngles();
        Quaternion = EulerAngles.ToQuaternion<double>();
    }

    public Orientation(Quaternion<double> rotationQuaternion)
    {
        Quaternion = rotationQuaternion;
        Matrix = rotationQuaternion.ToRotationMatrix();
        EulerAngles = Matrix.ToEulerAngles();
    }
}
