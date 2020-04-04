using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NurseScheduler {
    public class Patient : IEquatable<Patient> {
        // Constructors
        public Patient(string roomNum, int acuity) {
            RoomNum = roomNum;
            Acuity = acuity;
        }
        public Patient(Patient p) {
            RoomNum = p.RoomNum;
             Acuity = p.Acuity;
        }

        // Properties
        public int Acuity { get; set; }
        public string RoomNum { get; set; }

        // Methods
        override public string ToString() {
            return RoomNum;
        }
        public bool EqualTo(Patient p) {
            return p.RoomNum.Equals(this.RoomNum);
        }
        public bool Equals([AllowNull] Patient other) {
            return other.EqualTo(this);
        }
    }
}
