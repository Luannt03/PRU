using System;
namespace SwimDemo
{
    // Positions/depth are in metres. This is bounded kinematic movement, not a fluid solver.
    public sealed class SwimKinematics
    {
        public const float LimitX = 7.3f, LimitZ = 4.3f, MinDepth = .35f, MaxDepth = 2.5f;
        public float X { get; private set; }
        public float Z { get; private set; }
        public float Depth { get; private set; } = .7f;
        public float Yaw { get; private set; }
        public float LastSpeed { get; private set; }
        public bool Automatic { get; set; }
        double pathTime;
        static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
        static float Clamp(float value, float min, float max) => Math.Max(min, Math.Min(max, value));
        static float Axis(float value) => Finite(value) ? Clamp(value, -1, 1) : 0;
        public static float WrapAngle(float degrees)
        {
            if (!Finite(degrees)) return 0;
            float value = degrees % 360;
            return value > 180 ? value - 360 : value < -180 ? value + 360 : value;
        }
        public static float WaterHeight(float x, float z, double time) =>
            (float)(.035 * Math.Sin(x * .8 + time * 1.2) + .020 * Math.Sin(z * 1.3 + time * 1.9));
        public void Reset()
        {
            X = Z = Yaw = LastSpeed = 0; Depth = .7f; pathTime = 0; Automatic = false;
        }
        public void Step(float seconds, float forward, float turn, float rise, float speed, float verticalSpeed)
        {
            if (!Finite(seconds) || seconds <= 0) return;
            float dt = Math.Min(seconds, .1f);
            forward = Axis(forward); turn = Axis(turn); rise = Axis(rise);
            speed = Finite(speed) ? Clamp(speed, .1f, 4f) : 1.8f;
            verticalSpeed = Finite(verticalSpeed) ? Clamp(verticalSpeed, .1f, 2f) : .8f;
            if (forward != 0 || turn != 0 || rise != 0) Automatic = false;
            float oldX = X, oldZ = Z, oldDepth = Depth;
            if (Automatic)
            {
                pathTime += dt * .32;
                float targetX = (float)Math.Sin(pathTime) * 5f;
                float targetZ = (float)Math.Cos(pathTime) * 2.5f;
                float dx = targetX - X, dz = targetZ - Z;
                float distance = (float)Math.Sqrt(dx * dx + dz * dz);
                if (distance > .0001f)
                {
                    float step = Math.Min(speed * dt, distance);
                    X += dx / distance * step; Z += dz / distance * step;
                    float targetYaw = (float)(Math.Atan2(dx, dz) * 180 / Math.PI);
                    Yaw = WrapAngle(Yaw + Clamp(WrapAngle(targetYaw - Yaw), -110 * dt, 110 * dt));
                }
            }
            else
            {
                Yaw = WrapAngle(Yaw + turn * 110 * dt);
                double radians = Yaw * Math.PI / 180;
                X += (float)Math.Sin(radians) * forward * speed * dt;
                Z += (float)Math.Cos(radians) * forward * speed * dt;
                Depth -= rise * verticalSpeed * dt;
            }
            X = Clamp(X, -LimitX, LimitX); Z = Clamp(Z, -LimitZ, LimitZ);
            Depth = Clamp(Depth, MinDepth, MaxDepth);
            LastSpeed = (float)Math.Sqrt((X - oldX) * (X - oldX) + (Z - oldZ) * (Z - oldZ) + (Depth - oldDepth) * (Depth - oldDepth)) / dt;
        }
    }
}
