using UnityEngine;
using System.Collections;

namespace FCG
{
    public class AdaptiveIntersectionController : MonoBehaviour
    {
        [Header("FCG Traffic Lights (4 directions)")]
        public TrafficLight lightN;
        public TrafficLight lightS;
        public TrafficLight lightE;
        public TrafficLight lightW;

        [Header("Lane Sensors (Queue Counters)")]
        public LaneSensorP2 laneN;
        public LaneSensorP2 laneS;
        public LaneSensorP2 laneE;
        public LaneSensorP2 laneW;

        [Header("Timing")]
        public float minGreen = 8f;
        public float maxGreen = 25f;
        public float yellowTime = 3f;
        public float allRedTime = 1.5f;
        public int switchDiffThreshold = 2;

        [Header("Fairness")]
        public float maxWaitForOther = 30f;

        private enum Phase { NS_Green, EW_Green }
        private Phase currentPhase = Phase.NS_Green;

        private float phaseStartTime;
        private float otherPhaseWaitingTime;

        void Start()
        {
            SetAllRed();
            StartCoroutine(MainLoop());
        }

        IEnumerator MainLoop()
        {
            currentPhase = Phase.NS_Green;
            yield return StartCoroutine(SwitchTo(currentPhase));

            while (true)
            {
                int ns = GetCount(laneN) + GetCount(laneS);
                int ew = GetCount(laneE) + GetCount(laneW);

                float elapsed = Time.time - phaseStartTime;
                otherPhaseWaitingTime += Time.deltaTime;

                if (elapsed < minGreen)
                {
                    yield return null;
                    continue;
                }

                if (elapsed >= maxGreen)
                {
                    currentPhase = (currentPhase == Phase.NS_Green) ? Phase.EW_Green : Phase.NS_Green;
                    yield return StartCoroutine(SwitchTo(currentPhase));
                    continue;
                }

                if (otherPhaseWaitingTime >= maxWaitForOther)
                {
                    currentPhase = (currentPhase == Phase.NS_Green) ? Phase.EW_Green : Phase.NS_Green;
                    yield return StartCoroutine(SwitchTo(currentPhase));
                    continue;
                }

                if (currentPhase == Phase.NS_Green)
                {
                    if (ew >= ns + switchDiffThreshold && ew > 0)
                    {
                        currentPhase = Phase.EW_Green;
                        yield return StartCoroutine(SwitchTo(currentPhase));
                    }
                }
                else
                {
                    if (ns >= ew + switchDiffThreshold && ns > 0)
                    {
                        currentPhase = Phase.NS_Green;
                        yield return StartCoroutine(SwitchTo(currentPhase));
                    }
                }

                yield return null;
            }
        }

        IEnumerator SwitchTo(Phase target)
        {
            // أصفر للجهة اللي كانت خضراء
            if (target == Phase.NS_Green) SetEWYellow();
            else SetNSYellow();

            yield return new WaitForSeconds(yellowTime);

            SetAllRed();
            yield return new WaitForSeconds(allRedTime);

            if (target == Phase.NS_Green) SetNSGreen();
            else SetEWGreen();

            phaseStartTime = Time.time;
            otherPhaseWaitingTime = 0f;
        }

        // ===== Light Helpers =====
        void SetAllRed()
        {
            SetLight(lightN, "1");
            SetLight(lightS, "1");
            SetLight(lightE, "1");
            SetLight(lightW, "1");
        }

        void SetNSGreen()
        {
            SetLight(lightN, "3");
            SetLight(lightS, "3");
            SetLight(lightE, "1");
            SetLight(lightW, "1");
        }

        void SetEWGreen()
        {
            SetLight(lightN, "1");
            SetLight(lightS, "1");
            SetLight(lightE, "3");
            SetLight(lightW, "3");
        }

        void SetNSYellow()
        {
            SetLight(lightN, "2");
            SetLight(lightS, "2");
            SetLight(lightE, "1");
            SetLight(lightW, "1");
        }

        void SetEWYellow()
        {
            SetLight(lightN, "1");
            SetLight(lightS, "1");
            SetLight(lightE, "2");
            SetLight(lightW, "2");
        }

        void SetLight(TrafficLight tl, string status)
        {
            if (tl != null) tl.SetStatus(status);
        }

        // ✅ أهم تعديل: صار LaneSensorP2 بدل LaneSensor
        int GetCount(LaneSensorP2 s)
        {
            if (s == null) return 0;
            return s.GetCarCount();
        }
    }
}
