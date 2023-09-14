namespace BattleSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            SimulationEngine SimEng = new SimulationEngine();
            SimEng.RegisterVehicle(new Aircraft(new List<float[]>
                                                        {
                                                         // Waypoints

                                                         new float[] { 0.0f, 0.0f },
                                                         new float[] { 5.0f, 5.0f },
                                                         new float[] { 10.0f, 7.5f },
                                                         new float[] { 15.0f, 5.0f },
                                                         new float[] { 10.0f, 2.5f },
                                                         new float[] { 5.0f, 2.5f },
                                                         new float[] { 0.0f, 0.0f } },

                                                         // Velocities

                                                         1.0f , 10.0f));

            SimEng.RegisterVehicle(new Radar(new List<float[]>
                                                        {new float[] { 10.0f, 5.0f }},
                                                         0.0f, 10.0f));

            SimEng.RegisterVehicle(new Radar(new List<float[]>
                                                        {new float[] { 15.0f, 7.5f }},
                                                         0.0f, 10.0f));

            /*            SimEng.RegisterVehicle(new Aircraft(new List<float[]>
                                                                    {
                                                                     // Waypoints

                                                                     new float[] { -20.0f, 5.0f },
                                                                     new float[] { 50.0f, 5.0f } },

                                                                     // Velocities

                                                                     new float[] { 1.0f }, 10.0f));

                        SimEng.RegisterVehicle(new Aircraft(new List<float[]>
                                                                    {
                                                                     // Waypoints

                                                                     new float[] { -40.0f, 5.0f },
                                                                     new float[] { 50.0f, 5.0f } },

                                                         // Velocities

                                                                     new float[] { 1.0f }, 10.0f));

                        SimEng.RegisterVehicle(new Radar(new List<float[]> { new float[] { 45.0f, 0.0f } }, new float[] { 0.0f }, 10.0f));
                        SimEng.RegisterVehicle(new AntiAir(new List<float[]> { new float[] { 25.0f, 0.0f } }, new float[] { 0.0f }, 10.0f));*/

            float acc_zone_size = 3.0f;
            float att_zone_size = 1.5f;
            while (!SimEng.allVehiclesStopped)
            {

                // Run until all non-stationary vehicles come to a stop

                if (SimEng.EscapeFailed)
                {
                    acc_zone_size += 5.0f;
                }

                Console.WriteLine($"\nPosition after {i} ticks:");
                SimEng.RunSimulationEngine(1.00f, acc_zone_size, att_zone_size);
                Console.WriteLine("Press Enter/Return to display next tick");
                Console.ReadLine();
                i++;
            }
        }
    }
}


abstract class BattleSystemClass
{
    public abstract string Type { get; set; }
    public abstract int VehicleID { get; set; }
    public abstract List<float[]> VehiclePath { get; set; }
    public abstract float[] LegVelocity { get; set; }
    public abstract float Velocities { get; set; }
    public abstract float[] CurrentPosition { get; set; }
    public abstract float[] NewPositionTemp { get; set; }
    public abstract float[] NextWaypoint { get; set; }
    public abstract int CurrWaypointID { get; set; }
    public abstract int InLeg { get; set; }
    public abstract bool VehicleHasStopped { get; set; }
    public abstract bool VelocityChanged { get; set; }
    public abstract float RadarRange { get; set; }
    public abstract float MissileRange { get; set; }
    public abstract float ElapsedTime { get; set; }
    public abstract List<BattleSystemClass> ObjectsVisible { get; set; }
    public abstract float ObjAngle { get; set; }
    public abstract float ObjDist { get; set; }
    public abstract float[] Get();
    public abstract void Set(BattleSystemClass batt_class, string add_rem);
    public abstract void OnTick(float timer);
    public abstract void DecompVelocity();
}

class Aircraft : BattleSystemClass
{
    public override string Type { get; set; }
    public override int VehicleID { get; set; }
    public override List<float[]> VehiclePath { get; set; }
    public override float[] LegVelocity { get; set; }
    public override float Velocities { get; set; }
    public override float[] CurrentPosition { get; set; }
    public override float[] NewPositionTemp { get; set; }
    public override float[] NextWaypoint { get; set; }
    public override int CurrWaypointID { get; set; }
    public override int InLeg { get; set; }
    public override bool VehicleHasStopped { get; set; }
    public override bool VelocityChanged { get; set; }
    public override float RadarRange { get; set; }
    public override float MissileRange { get; set; }
    public override float ElapsedTime { get; set; }
    public override List<BattleSystemClass> ObjectsVisible { get; set; }
    public override float ObjAngle { get; set; }
    public override float ObjDist { get; set; }

    public override float[] Get()
    {
        return CurrentPosition;
    }
    public override void OnTick(float timer)
    {

        // Compute new positions
        if (!VehicleHasStopped)
        {
            NewPositionTemp[0] = CurrentPosition[0] + (LegVelocity[0] * timer);
            NewPositionTemp[1] = CurrentPosition[1] + (LegVelocity[1] * timer);
        }
    }
    public override void Set(BattleSystemClass batt_class, string add_rem)
    {

        // Set current position to new position

        CurrentPosition[0] = NewPositionTemp[0];
        CurrentPosition[1] = NewPositionTemp[1];

        if (add_rem == "add")
        {
            if (!this.ObjectsVisible.Contains(batt_class))
            {
                this.ObjectsVisible.Add(batt_class);
                Console.WriteLine($"\n{batt_class.Type} {batt_class.VehicleID} added to {this.Type} {this.VehicleID}'s range");
            }

        }
        if (add_rem == "remove")
        {
            if (this.ObjectsVisible.Contains(batt_class))
            {
                this.ObjectsVisible.Remove(batt_class);
                Console.WriteLine($"\n{batt_class.Type} {batt_class.VehicleID} removed from {this.Type} {this.VehicleID}'s range");
            }
        }
    }

    public float DistanceCalculator(float[] obj1, float[] obj2)
    {
        float x = (obj1[0] - obj2[0]);
        float y = (obj1[1] - obj2[1]);
        return MathF.Sqrt((x * x) + (y * y));
    }

    public float AngleCalculator(float[] obj1, float[] obj2)
    {
        float x = (obj2[0] - obj1[0]);
        float y = (obj2[1] - obj1[1]);
        float v = MathF.Atan2(y, x);
        return v;
    }

    public override void DecompVelocity()
    {
        this.LegVelocity[0] = this.Velocities * MathF.Cos(AngleCalculator(this.CurrentPosition, this.NextWaypoint));
        this.LegVelocity[1] = this.Velocities * MathF.Sin(AngleCalculator(this.CurrentPosition, this.NextWaypoint));
    }
    public Aircraft(List<float[]> waypoints, float velocities, float radar_range)
    {

        // Object of Aircraft class takes a List of waypoints (float array of size 2), an array of velocities (size = waypoint list size - 1)
        // and a float indicating Radar range.

        NewPositionTemp = waypoints[0];
        CurrentPosition = waypoints[0];
        VehiclePath = waypoints;
        Velocities = velocities;
        VehicleHasStopped = false;
        RadarRange = radar_range;
        NextWaypoint = VehiclePath[1];
        Type = "Aircraft";
        BattleSOS.s_AircraftID++;
        VehicleID = BattleSOS.s_AircraftID;
        ObjectsVisible = new List<BattleSystemClass>();
        ObjAngle = 0.0f;
        ObjDist = 0.0f;
        InLeg = 0;
        CurrWaypointID = 0;
        LegVelocity = new float[2];
        DecompVelocity();
        // Velocities are in direction of any given waypoint leg, decomposing velocities into Vx and Vy
    }
}

class Radar : BattleSystemClass
{
    public override string Type { get; set; }
    public override int VehicleID { get; set; }
    public override List<float[]> VehiclePath { get; set; }
    public override float[] LegVelocity { get; set; }
    public override float Velocities { get; set; }
    public override float[] CurrentPosition { get; set; }
    public override float[] NewPositionTemp { get; set; }
    public override float[] NextWaypoint { get; set; }
    public override int CurrWaypointID { get; set; }
    public override int InLeg { get; set; }
    public override bool VehicleHasStopped { get; set; }
    public override bool VelocityChanged { get; set; }
    public override float RadarRange { get; set; }
    public override float MissileRange { get; set; }
    public override float ElapsedTime { get; set; }
    public override List<BattleSystemClass> ObjectsVisible { get; set; }
    public override float ObjAngle { get; set; }
    public override float ObjDist { get; set; }

    public override float[] Get()
    {
        return CurrentPosition;
    }
    public override void OnTick(float timer)
    {
        // No postitional computation required for stationary objects
    }
    public override void Set(BattleSystemClass batt_class, string add_rem)
    {
        // No postitional computation required for stationary objects
        if (add_rem == "add" && batt_class.Type != "Radar")
        {
            if (!this.ObjectsVisible.Contains(batt_class))
            {
                this.ObjectsVisible.Add(batt_class);
                Console.WriteLine($"\n{batt_class.Type} {batt_class.VehicleID} added to {this.Type} {this.VehicleID}'s range");
            }

        }
        if (add_rem == "remove")
        {
            if (this.ObjectsVisible.Contains(batt_class))
            {
                this.ObjectsVisible.Remove(batt_class);
                Console.WriteLine($"\n{batt_class.Type} {batt_class.VehicleID} removed from {this.Type} {this.VehicleID}'s range");
            }
        }
    }

    public override void DecompVelocity()
    {

    }
    public Radar(List<float[]> waypoints, float velocities, float radar_range)
    {

        // Object of Radar class takes the same arguments as Aircraft, but the List of waypoints only contains one item
        // and the array of velocities has one item with the value 0.0
        // radar_range is the only relevant value in the class.

        NewPositionTemp = waypoints[0];
        CurrentPosition = waypoints[0];
        VehiclePath = waypoints;
        Velocities = velocities;
        VehicleHasStopped = false;
        RadarRange = radar_range;
        NextWaypoint = new float[] { 0.0f, 0.0f };
        LegVelocity = new float[] { 0.0f, 0.0f };
        MissileRange = 0;
        Type = "Radar";
        InLeg = 0;
        CurrWaypointID = 0;
        ObjectsVisible = new List<BattleSystemClass>();
        BattleSOS.s_RadarID++;
        VehicleID = BattleSOS.s_RadarID;
        ObjAngle = 0.0f;
        ObjDist = 0.0f;
    }
}

class AntiAir : BattleSystemClass
{
    public override string Type { get; set; }
    public override int VehicleID { get; set; }
    public override List<float[]> VehiclePath { get; set; }
    public override float[] LegVelocity { get; set; }
    public override float Velocities { get; set; }
    public override float[] CurrentPosition { get; set; }
    public override float[] NewPositionTemp { get; set; }
    public override float[] NextWaypoint { get; set; }
    public override int CurrWaypointID { get; set; }
    public override int InLeg { get; set; }
    public override bool VehicleHasStopped { get; set; }
    public override bool VelocityChanged { get; set; }
    public override float RadarRange { get; set; }
    public override float MissileRange { get; set; }
    public override float ElapsedTime { get; set; }
    public override List<BattleSystemClass> ObjectsVisible { get; set; }
    public override float ObjAngle { get; set; }
    public override float ObjDist { get; set; }

    public override float[] Get()
    {
        return CurrentPosition;
    }
    public override void OnTick(float timer)
    {
        // No postitional computation required for stationary objects
    }
    public override void Set(BattleSystemClass batt_class, string add_rem)
    {
        // No postitional computation required for stationary objects
        if (add_rem == "add" && batt_class.Type != "Radar")
        {
            if (!this.ObjectsVisible.Contains(batt_class))
            {
                this.ObjectsVisible.Add(batt_class);
                Console.WriteLine($"\n{batt_class.Type} {batt_class.VehicleID} added to {this.Type} {this.VehicleID}'s range");
            }

        }
        if (add_rem == "remove")
        {
            if (this.ObjectsVisible.Contains(batt_class))
            {
                this.ObjectsVisible.Remove(batt_class);
                Console.WriteLine($"\n{batt_class.Type} {batt_class.VehicleID} removed from {this.Type} {this.VehicleID}'s range");
            }
        }
    }

    public override void DecompVelocity()
    {

    }
    public AntiAir(List<float[]> waypoints, float velocities, float radar_range)
    {

        // Object of Radar class takes the same arguments as Aircraft, but the List of waypoints only contains one item
        // and the array of velocities has one item with the value 0.0
        // radar_range is the only relevant value in the class.

        NewPositionTemp = waypoints[0];
        CurrentPosition = waypoints[0];
        VehiclePath = waypoints;
        Velocities = velocities;
        VehicleHasStopped = false;
        RadarRange = radar_range;
        MissileRange = radar_range;
        NextWaypoint = new float[] { 0.0f, 0.0f };
        LegVelocity = new float[] { 0.0f, 0.0f };
        Type = "AntiAir";
        InLeg = 0;
        CurrWaypointID = 0;
        ObjectsVisible = new List<BattleSystemClass>();
        BattleSOS.s_AntiAirID++;
        VehicleID = BattleSOS.s_AntiAirID;
        ObjAngle = 0.0f;
        ObjDist = 0.0f;
    }
}

class BattleSOS
{
    public static int s_RadarID = 0;
    public static int s_AircraftID = 0;
    public static int s_AntiAirID = 0;
    public static List<BattleSystemClass> BattleSysList; // Maintains a list of all Vehicles on field
}

class SimulationEngine
{
    public bool allVehiclesStopped = false;
    public bool ThreatDetected = false;
    public bool EscapeFailed = false;
    public float TimeCounter = 0;
    public float FirstVelocity = 0;
    public float[] UnsafePosition = new float[2];
    public float[] FirstUnsafePos = new float[2];
    public SimulationEngine()
    {
        BattleSOS.BattleSysList = new List<BattleSystemClass>();
    }

    public float DistanceCalculator(float[] obj1, float[] obj2)
    {
        float x = (obj1[0] - obj2[0]);
        float y = (obj1[1] - obj2[1]);
        return MathF.Sqrt((x * x) + (y * y));
    }

    public float AngleCalculator(float[] obj1, float[] obj2)
    {
        float x = (obj2[0] - obj1[0]);
        float y = (obj2[1] - obj1[1]);
        float v = MathF.Atan2(y, x);
        return v;
    }

    public void RegisterVehicle(BattleSystemClass newVehicle)
    {
        BattleSOS.BattleSysList.Add(newVehicle);
    }

    public void RunSimulationEngine(float timer, float acc_zone, float att_zone)
    {
        int stoppedVehicles = 0;
        int num_radars = 0;
        int num_aircraft = 0;

        foreach (var vehicle in BattleSOS.BattleSysList)
        {

            if (vehicle.Type == "Radar")
            {
                num_radars++;
            }
            if (vehicle.Type == "Aircraft")
            {
                num_aircraft++;
            }

        }


        // Set size of globalSituationalAwareness based on number of Radars and Aircraft in BattleSOS

        string[,] globalSituationalAwareness = new string[num_radars, num_aircraft];

        for (int i = 0; i < num_radars; i++)
        {
            for (int j = 0; j < num_aircraft; j++)
            {
                globalSituationalAwareness[i, j] = "-";
            }
        }


        // EXECUTE Set() method on every vehicle on field

        foreach (var vehicle in BattleSOS.BattleSysList)
        {
            if (vehicle.Type == "Radar" || vehicle.Type == "Aircraft")
            {
                foreach (var other_vehicles in BattleSOS.BattleSysList)
                {
                    float dist = DistanceCalculator(other_vehicles.CurrentPosition, vehicle.CurrentPosition);
                    float angle = AngleCalculator(other_vehicles.CurrentPosition, vehicle.CurrentPosition);
                    if (vehicle != other_vehicles)
                    {
                        if (dist <= vehicle.RadarRange)
                        {
                            vehicle.Set(other_vehicles, "add");
                        }
                        else if (dist > vehicle.RadarRange)
                        {
                            vehicle.Set(other_vehicles, "remove");
                        }
                    }


                }
                Console.WriteLine($"\nObjects visible to {vehicle.Type} {vehicle.VehicleID}:");
                foreach (var veh in vehicle.ObjectsVisible)
                {
                    float obj_dist = DistanceCalculator(veh.CurrentPosition, vehicle.CurrentPosition);
                    float obj_angle = AngleCalculator(veh.CurrentPosition, vehicle.CurrentPosition);
                    Console.WriteLine($"{veh.Type} {veh.VehicleID} (Distance = {obj_dist}), (Angle = {obj_angle} radians)");
                }
            }

            if (vehicle.Type != "Radar" || vehicle.Type != "AntiAir")
            {
                vehicle.DecompVelocity();

                // Excludes Radar type object from Leg computation to avoid IndexOutOfRange runtime exception.

                for (int i = 0; i < vehicle.VehiclePath.Count - 1; i++)
                {
                    if ((MathF.Abs(DistanceCalculator(vehicle.CurrentPosition, vehicle.NextWaypoint)) <= (vehicle.Velocities * timer)))
                    {
                        if (!vehicle.VehicleHasStopped || vehicle.NextWaypoint != vehicle.VehiclePath.Last())
                        {
                            vehicle.CurrWaypointID++;
                            Console.WriteLine("Waypoint changed");
                            if (vehicle.CurrWaypointID < vehicle.VehiclePath.Count)
                            {
                                vehicle.NextWaypoint = vehicle.VehiclePath[vehicle.CurrWaypointID];
                            }
                            else if (vehicle.CurrWaypointID == vehicle.VehiclePath.Count)
                            {
                                vehicle.VehicleHasStopped = true;
                            }
                            Console.WriteLine($"Next waypoint = ({vehicle.NextWaypoint[0]}, {vehicle.NextWaypoint[1]})");
                        }
                    }

                }
            }
            if (vehicle.Type == "Radar" || vehicle.Type == "AntiAir")
            {
                // Radar is fixed by default

                vehicle.VehicleHasStopped = true;
                Console.WriteLine($"\n{vehicle.Type} {vehicle.VehicleID}");
                Console.WriteLine($"(x, y) = ({vehicle.CurrentPosition[0]},{vehicle.CurrentPosition[1]})");
            }
            if (!vehicle.VehicleHasStopped && (vehicle.Type != "Radar" || vehicle.Type != "AntiAir"))
            {

                // If Vehicle is still on path, execute Set() method set CurrentPosition to the newly computed values

                vehicle.Set(vehicle, "");
                Console.WriteLine($"\n{vehicle.Type} {vehicle.VehicleID}");
                Console.WriteLine($"(x, y) = ({vehicle.CurrentPosition[0]},{vehicle.CurrentPosition[1]})" +
                                  $"\n(Vx, Vy) = {vehicle.LegVelocity[0]},{vehicle.LegVelocity[1]}");
            }
            if (vehicle.VehicleHasStopped)
            {
                stoppedVehicles++;
                if (vehicle.Type == "Aircraft")
                {
                    Console.WriteLine($"\n{vehicle.Type} {vehicle.VehicleID} reached the end of path");
                }
                if (stoppedVehicles == BattleSOS.BattleSysList.Count)
                {
                    allVehiclesStopped = true;
                }
            }
        }
        if (ThreatDetected)
        {
            Console.WriteLine($"({UnsafePosition[0]}, {UnsafePosition[1]})");
        }

        // EXECUTE OnTick() METHOD for each vehicle on field

        foreach (var vehicle in BattleSOS.BattleSysList.ToList())
        {

            // Compute values for new position and objects within Radar range.
            vehicle.OnTick(timer);
            if (vehicle.Type == "AntiAir")
            {
                foreach (var other_vehicle in BattleSOS.BattleSysList.ToList())
                {
                    if (other_vehicle.Type == "Aircraft")
                    {
                        float dist_total = DistanceCalculator(vehicle.CurrentPosition, other_vehicle.CurrentPosition);
                        if (dist_total <= vehicle.MissileRange && !ThreatDetected)
                        {

                            // Logic for the first aircraft seen (and destroyed) by anti-aircraft.

                            Console.WriteLine($"{other_vehicle.Type} {other_vehicle.VehicleID} is in attacking range of {vehicle.Type} {vehicle.VehicleID}");
                            other_vehicle.ElapsedTime += timer;
                            if (other_vehicle.ElapsedTime == 5.0 * timer)
                            {

                                // If aircraft is within range, wait for 5 ticks of the timer to register a hit and record unsafe position

                                Console.WriteLine($"{other_vehicle.Type} {other_vehicle.VehicleID} was hit");
                                ThreatDetected = true;
                                FirstVelocity = other_vehicle.Velocities;
                                UnsafePosition = FirstUnsafePos = other_vehicle.CurrentPosition;
                                BattleSOS.BattleSysList.Remove(other_vehicle);
                            }
                        }

                        else if (ThreatDetected)
                        {

                            // Logic for subsequent aircraft when an unsafe position is known

                            float danger_zone_dist_total = DistanceCalculator(other_vehicle.CurrentPosition, UnsafePosition);
                            Console.WriteLine($"Distance from danger zone = {danger_zone_dist_total}");
                            if (danger_zone_dist_total <= acc_zone)
                            {

                                // When aircraft is within a certain radius of the danger zone, it must change its speed

                                if (other_vehicle.NextWaypoint != other_vehicle.VehiclePath.Last() && !other_vehicle.VelocityChanged)
                                {
                                    float FirstLegVel_x;
                                    float x_to_next_leg = MathF.Abs(other_vehicle.CurrentPosition[0] - other_vehicle.VehiclePath[other_vehicle.InLeg + 1][0]);
                                    float dist_to_next_leg = DistanceCalculator(other_vehicle.CurrentPosition, other_vehicle.VehiclePath[other_vehicle.InLeg + 1]);
                                    float cosine = x_to_next_leg / dist_to_next_leg;
                                    FirstLegVel_x = other_vehicle.Velocities * cosine;
                                    other_vehicle.LegVelocity[0] = (MathF.Abs(UnsafePosition[0]) + danger_zone_dist_total) * (FirstLegVel_x / FirstUnsafePos[0]);
                                    other_vehicle.VelocityChanged = true;
                                }
                                if (dist_total <= vehicle.MissileRange)
                                {

                                    // Condition for Aircraft n when it enters missile range

                                    Console.WriteLine($"{other_vehicle.Type}   {other_vehicle.VehicleID} is in attacking range of {vehicle.Type} {vehicle.VehicleID}");
                                    other_vehicle.ElapsedTime += timer;
                                    Console.WriteLine($"Time since detection: {other_vehicle.ElapsedTime}");
                                    if (other_vehicle.ElapsedTime == 5.0 * timer && danger_zone_dist_total <= att_zone)
                                    {

                                        // Checks if Aircraft n is within "danger zone" at after duration T

                                        Console.WriteLine($"{other_vehicle.Type} {other_vehicle.VehicleID} was hit");
                                        UnsafePosition = other_vehicle.CurrentPosition;
                                        EscapeFailed = true;
                                        BattleSOS.BattleSysList.Remove(other_vehicle);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        Console.WriteLine("Global situational awareness matrix:");
        for (int n = 0; n < num_radars; n++)
        {
            for (int m = 0; m < num_aircraft; m++)
            {
                Console.Write($"{globalSituationalAwareness[n, m]} ");
            }
            Console.WriteLine("");
        }
    }
}
