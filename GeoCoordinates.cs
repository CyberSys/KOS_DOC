﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace kOS
{
    public class GeoCoordinates : SpecialValue
    {
        public float Lat;
        public float Lng;
        public Vessel Vessel;
        public CelestialBody Body;

        public GeoCoordinates(Vessel vessel, float lat, float lng)
        {
            this.Lat = lat;
            this.Lng = lng;
            this.Vessel = vessel;

            Body = vessel.mainBody;
        }

        public GeoCoordinates(Vessel vessel, double lat, double lng)
        {
            this.Lat = (float)lat;
            this.Lng = (float)lng;
            this.Vessel = vessel;

            Body = vessel.mainBody;
        }

        public float GetBearing(Vessel vessel)
        {
            return VesselUtils.AngleDelta(VesselUtils.GetHeading(vessel), GetHeadingFromVessel(vessel));
        }
    
        public float GetHeadingFromVessel(Vessel vessel)
        {
            var up = vessel.upAxis;
            var north = VesselUtils.GetNorthVector(vessel);

            var targetWorldCoords = vessel.mainBody.GetWorldSurfacePosition(Lat, Lng, vessel.altitude);
            
            var vector = Vector3d.Exclude(vessel.upAxis, targetWorldCoords - vessel.GetWorldPos3D()).normalized;
            var headingQ = Quaternion.Inverse(Quaternion.Euler(90, 0, 0) * Quaternion.Inverse(Quaternion.LookRotation(vector, up)) * Quaternion.LookRotation(north, up));

            return headingQ.eulerAngles.y;
        }

        public float DistanceFrom(Vessel Vessel)
        {
            return (float)Vector3d.Distance(Vessel.GetWorldPos3D(), Body.GetWorldSurfacePosition(Lat, Lng, Vessel.altitude));
        }

        public override object GetSuffix(string suffixName)
        {
            if (suffixName == "LAT") return (float)Lat;
            if (suffixName == "LNG") return (float)Lng;
            if (suffixName == "DISTANCE") return DistanceFrom(Vessel);
            if (suffixName == "HEADING") return GetHeadingFromVessel(Vessel);
            if (suffixName == "BEARING") return GetBearing(Vessel);
            
            return base.GetSuffix(suffixName);
        }
        
        public override string ToString()
        {
            return "LATLNG(" + Lat + ", " + Lng + ")";
        }
    }
}
