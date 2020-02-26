using System;
using System.Collections.Generic;
using System.Text;

namespace NurseScheduler {
    public class Nurse {
        public Nurse(string name) {
            Name = name;
            Patients = new List<Patient>();
            Acuity = 0;
        }
        public Nurse(Nurse n) {
            Name = n.Name;
            Patients = new List<Patient>();
            Patients.AddRange(n.Patients);
            Acuity = n.Acuity;
        }
        public List<Patient> Patients { get; }
        public string Name { get; set; }
        public int Acuity { get; set; }
        public void AddPatient(Patient p) {
            Patients.Add(p);
            Acuity += p.Acuity;
        }
        public void RemovePatient(Patient p) {
            Patients.Remove(p);
            Acuity -= p.Acuity;
        }
        override public string ToString() {
            return Name;
        }
        public bool EqualTo(Nurse n) {
            return n.Name.Equals(this.Name);
        }
    }
}
