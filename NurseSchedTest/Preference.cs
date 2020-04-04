using System;
using System.Collections.Generic;
using System.Text;

namespace NurseScheduler {
    public class Preference {
        // Constructor
        public Preference(Nurse n, Patient p, int weight) {
            Nurse = n;
            Patient = p;
            this.Weight = weight;
        }

        // Properties
        public Nurse Nurse { get; }
        public Patient Patient { get; }
        public int Weight { get; }

        // Methods
        public override string ToString() {
            return "" + Patient + "->" + Nurse + " |" + Weight + "| ";
        }
    }
}
