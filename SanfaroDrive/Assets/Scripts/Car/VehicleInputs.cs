using UnityEngine;

namespace VehicleBehaviour 
{
    [CreateAssetMenu(fileName = "VehicleInputs", menuName = "VehicleBehaviour/Inputs", order = 1)]
    public class VehicleInputs : ScriptableObject
    {
        [Header("Wheelvehicle")]
        public string ThrottleInput = "Vertical";
        public string BrakeInput = "Brake";
        public string TurnInput = "Horizontal";
        public string JumpInput = "Jump";
        public string DriftInput = "Drift";
        public string BoostInput = "Boost";
        public string RotateInput = "Rotate";
    }
}
