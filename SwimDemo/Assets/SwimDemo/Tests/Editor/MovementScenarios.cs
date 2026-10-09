using System;
using System.Collections.Generic;
namespace SwimDemo.Tests
{
    // These cases execute the production movement model in both .NET and Unity Edit Mode.
    public static class MovementScenarios
    {
        static void Check(bool condition, string reason)
        { if (!condition) throw new InvalidOperationException(reason); }
        static void Near(float value, float expected, string reason, float tolerance = .001f)
        { Check(Math.Abs(value - expected) <= tolerance, reason + ": " + value + " != " + expected); }
        static void Advance(SwimKinematics state, int frames, float forward = 0, float turn = 0, float rise = 0, float dt = .02f)
        { for (int i = 0; i < frames; i++) state.Step(dt, forward, turn, rise, 2, 1); }
        public static IEnumerable<KeyValuePair<string, Action>> Cases
        {
            get
            {
                yield return Case("W moves forward at configured speed", () =>
                { var s = new SwimKinematics(); Advance(s, 50, forward: 1); Near(s.Z, 2, "Forward distance"); Near(s.X, 0, "No lateral drift"); });
                yield return Case("S reverses without turning the body", () =>
                { var s = new SwimKinematics(); Advance(s, 50, forward: -1); Near(s.Z, -2, "Reverse distance"); Near(s.Yaw, 0, "Heading"); });
                yield return Case("A and D rotate in opposite directions", () =>
                { var a = new SwimKinematics(); var d = new SwimKinematics(); Advance(a, 25, turn: -1); Advance(d, 25, turn: 1); Near(a.Yaw, -55, "Left turn"); Near(d.Yaw, 55, "Right turn"); });
                yield return Case("Forward direction follows heading", () =>
                { var s = new SwimKinematics(); Advance(s, 25, turn: 1); Advance(s, 25, forward: 1); Check(s.X > .8f && s.Z > .5f && s.Z < .6f, "Expected motion along the 55-degree heading"); });
                yield return Case("Space reduces depth", () =>
                { var s = new SwimKinematics(); Advance(s, 10, rise: 1); Near(s.Depth, .5f, "Upward distance"); });
                yield return Case("C increases depth", () =>
                { var s = new SwimKinematics(); Advance(s, 10, rise: -1); Near(s.Depth, .9f, "Downward distance"); });
                yield return Case("Surface limit prevents leaving the water", () =>
                { var s = new SwimKinematics(); Advance(s, 500, rise: 1); Near(s.Depth, SwimKinematics.MinDepth, "Surface limit"); });
                yield return Case("Floor limit preserves clearance", () =>
                { var s = new SwimKinematics(); Advance(s, 500, rise: -1); Near(s.Depth, SwimKinematics.MaxDepth, "Floor limit"); });
                yield return Case("Front and back walls stop movement", () =>
                { var s = new SwimKinematics(); Advance(s, 500, forward: 1); Near(s.Z, SwimKinematics.LimitZ, "Front wall"); Advance(s, 1000, forward: -1); Near(s.Z, -SwimKinematics.LimitZ, "Back wall"); });
                yield return Case("Turning against side walls never escapes pool", () =>
                { var s = new SwimKinematics(); Advance(s, 41, turn: 1); Advance(s, 1000, forward: 1); Check(Math.Abs(s.X) <= SwimKinematics.LimitX && Math.Abs(s.Z) <= SwimKinematics.LimitZ, "Side wall bounds"); });
                yield return Case("Paused or negative time cannot move", () =>
                { var s = new SwimKinematics { Automatic = true }; s.Step(0, 1, 1, 1, 2, 1); s.Step(-1, 1, 1, 1, 2, 1); Near(s.X, 0, "Paused X"); Near(s.Depth, .7f, "Paused depth"); Check(s.Automatic, "Paused input must not toggle mode"); });
                yield return Case("Automatic swim starts without teleporting", () =>
                { var s = new SwimKinematics { Automatic = true }; s.Step(.02f, 0, 0, 0, 2, 1); Check(Math.Sqrt(s.X * s.X + s.Z * s.Z) <= .0401, "First auto step exceeded speed"); });
                yield return Case("Manual input takes priority over automatic swim", () =>
                { var s = new SwimKinematics { Automatic = true }; s.Step(.02f, -1, 0, 0, 2, 1); Check(!s.Automatic && s.Z < 0, "Reverse must take control immediately"); });
                yield return Case("Automatic swim stays within water for ten minutes", () =>
                { var s = new SwimKinematics { Automatic = true }; for (int i = 0; i < 30000; i++) { s.Step(.02f, 0, 0, 0, 4, 1); Check(Math.Abs(s.X) <= SwimKinematics.LimitX && Math.Abs(s.Z) <= SwimKinematics.LimitZ && s.Depth >= SwimKinematics.MinDepth, "Auto bounds"); } });
                yield return Case("Reset clears mode, position, depth and heading", () =>
                { var s = new SwimKinematics(); Advance(s, 50, 1, 1, -1); s.Automatic = true; s.Reset(); Near(s.X, 0, "Reset X"); Near(s.Z, 0, "Reset Z"); Near(s.Yaw, 0, "Reset yaw"); Near(s.Depth, .7f, "Reset depth"); Near(s.LastSpeed, 0, "Reset speed"); Check(!s.Automatic, "Reset mode"); });
                yield return Case("Movement distance is stable across frame rates", () =>
                { foreach (int fps in new[] { 30, 60, 120 }) { var s = new SwimKinematics(); Advance(s, fps, forward: 1, dt: 1f / fps); Near(s.Z, 2, "Distance at " + fps + " fps"); } });
                yield return Case("Frame hitch cannot cause a huge position jump", () =>
                { var s = new SwimKinematics(); s.Step(60, 1, 1, -1, 4, 2); Check(Math.Sqrt(s.X * s.X + s.Z * s.Z) <= .401 && s.Depth <= .901f, "Hitch jump"); });
                yield return Case("Invalid inputs never poison the state", () =>
                { var s = new SwimKinematics(); s.Step(float.NaN, 1, 1, 1, 2, 1); s.Step(.02f, float.NaN, float.PositiveInfinity, float.NegativeInfinity, float.NaN, float.PositiveInfinity); Near(s.X, 0, "Invalid X"); Near(s.Z, 0, "Invalid Z"); Near(s.Depth, .7f, "Invalid depth"); });
                yield return Case("Out-of-range input is bounded", () =>
                { var s = new SwimKinematics(); s.Step(.1f, 100, 0, 0, 100, 1); Near(s.Z, .4f, "Clamped input and speed"); });
                yield return Case("Wave amplitude stays within 5.5 cm", () =>
                { for (int t = 0; t < 1000; t++) Check(Math.Abs(SwimKinematics.WaterHeight(t % 17 - 8, t % 11 - 5, t * .13)) <= .05501f, "Wave amplitude"); });
                yield return Case("Long rotations keep finite heading", () =>
                { var s = new SwimKinematics(); Advance(s, 30000, turn: 1); Check(s.Yaw >= -180 && s.Yaw <= 180, "Wrapped heading"); Near(SwimKinematics.WrapAngle(359), -1, "Shortest-angle wrapping"); });
            }
        }
        static KeyValuePair<string, Action> Case(string name, Action action) => new KeyValuePair<string, Action>(name, action);
    }
}
