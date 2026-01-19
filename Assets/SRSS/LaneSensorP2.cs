using UnityEngine;
using System.Collections.Generic;

namespace FCG
{
    public class LaneSensorP2 : MonoBehaviour
    {
        [Header("Accepted Tags")]
        public string[] acceptedTags = { "Car", "IaCar", "Police", "Ambulance" };

        [Header("Debug")]
        public bool logEnterExit = false;

        private readonly HashSet<Transform> vehicles = new HashSet<Transform>();

        private void Reset()
        {
            var bc = GetComponent<BoxCollider>();
            if (bc == null) bc = gameObject.AddComponent<BoxCollider>();
            bc.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            Transform vehicleRoot = FindVehicleRoot(other.transform);
            if (vehicleRoot == null) return;

            if (vehicles.Add(vehicleRoot))
            {
                if (logEnterExit)
                    Debug.Log($"✅ LaneSensor ENTER: {vehicleRoot.name} -> Count={vehicles.Count}", this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Transform vehicleRoot = FindVehicleRoot(other.transform);
            if (vehicleRoot == null) return;

            if (vehicles.Remove(vehicleRoot))
            {
                if (logEnterExit)
                    Debug.Log($"⬅️ LaneSensor EXIT: {vehicleRoot.name} -> Count={vehicles.Count}", this);
            }
        }

        private Transform FindVehicleRoot(Transform t)
        {
            int steps = 0;
            while (t != null && steps < 15)
            {
                if (HasAcceptedTag(t)) return t;

                string n = t.name.ToLower();
                if ((n.Contains("car") || n.Contains("bus") || n.Contains("truck") || n.Contains("vehicle")) &&
                    !n.Contains("sensor") && !n.Contains("road"))
                    return t;

                t = t.parent;
                steps++;
            }
            return null;
        }

        private bool HasAcceptedTag(Transform t)
        {
            foreach (var tag in acceptedTags)
                if (t.CompareTag(tag)) return true;
            return false;
        }

        public int GetCarCount()
        {
            vehicles.RemoveWhere(v => v == null);
            return vehicles.Count;
        }
    }
}
