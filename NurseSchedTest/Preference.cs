using System;
using System.Collections.Generic;
using System.Text;

namespace NurseScheduler {
    public class Preference {
        public Nurse nurse { get; }
        public Patient patient { get; }
        public int weight { get; }

        public Preference( Nurse n, Patient p, int weight) {
            nurse = n;
            patient = p;
            this.weight = weight;
        }

        public override string ToString() {
            return "" + patient + "->" + nurse + " |" + weight + "| ";
        }
    }
}
