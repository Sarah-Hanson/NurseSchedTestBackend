using System;
using System.Collections.Generic;

namespace NurseScheduler {
    class Program {
        static void Main(string[] args) {
            // Real Example is 45 patients to 12 nurses (or 9 on night shift)
            /*
             * Algorith Settings
             */
            const int maxDisparity = 3;
            const int snipLevel = 6; // Never set lower than max patient disparity
            const int preferenceMultiplier = 3;
            const int disparityMultiplier = 1;
            /*
             * Test data generation settings
             */
            const int teamCount = 4;
            const int patientCount = 12;
            const int nurseCount = 3;
            const int preferenceCount = 6;

            const int patientAcuityMin = 1;
            const int patientAcuityMax = 5;

            const int preferenceWeightMin = 3;
            const int preferenceWeightMax = 7;
            /*
             * Program variables
             */
            Random rand = new Random();
            List<Team> teams = new List<Team>();
            PatientDistanceMatrix pdm = new PatientDistanceMatrix();
            /*
             * Create Test Set
             */
            {

                int n = 0; // nurse# counter
                int p = 0; // patient# counter
                for (int i = 0; i < teamCount; i++) {
                    teams.Add(new Team());
                    teams[i].TeamName = "Team" + i;
                }
                foreach (Team t in teams) {
                    // Fill Team with nurses
                    for (int j = 0; j < nurseCount; j++) {
                        t.Nurses.Add(new Nurse("Nurse" + n++));
                    }
                    // Fill Team with patients
                    for (int j = 0; j < patientCount; j++) {
                        t.Patients.Add(new Patient("Patient" + p++, rand.Next(patientAcuityMin, patientAcuityMax)));
                    }
                    // Set preferences for the team
                    for (int j = 0; j < preferenceCount; j++) {
                        Nurse nurse = t.Nurses[rand.Next(0, t.Nurses.Count)];
                        Patient patient = t.Patients[rand.Next(0, t.Patients.Count)];
                        t.Preferences.Add(new Preference(nurse, patient, rand.Next(preferenceWeightMin, preferenceWeightMax)));
                    }
                    //// Set distances between patients
                    //for (int j = 0; j < t.Patients.Count - 1; j++) {
                    //    pdm.roomGrid.Add(new PatientRelation(t.Patients[j], t.Patients[j + 1], rand.Next(1, 5)));
                    //}
                }
            }
            Schedulizer sched = new Schedulizer(pdm, maxDisparity, snipLevel, preferenceMultiplier, disparityMultiplier);

            /*
             * Run it for each team created
             */
            foreach (Team t in teams) {
                Console.WriteLine("\n");
                t.Assignment = sched.GetSchedule(t);
                Console.WriteLine("Printing Results");
                /*
                 * Results
                 */
                Console.WriteLine("===========================================================" + "\n       Team: " + t.TeamName);
                {
                    Console.Write("     Nurses: ");
                    int i = 1;
                    foreach (Nurse n in t.Nurses) {
                        Console.Write(n.Name);
                        if (i++ < t.Nurses.Count) Console.Write(", ");
                        else Console.Write("\n");
                    }
                    Console.Write("   Patients: ");
                    i = 1;
                    foreach (Patient p in t.Patients) {
                        Console.Write(p.RoomNum);
                        if (i++ < t.Patients.Count) Console.Write(", ");
                        else Console.Write("\n");
                    }
                    Console.WriteLine("\nPREFERENCES");
                    i = 1;
                    foreach (Preference p in t.Preferences) {
                        Console.WriteLine(p);
                        if (i++ < t.Preferences.Count) { }
                        else Console.Write("\n");
                    }
                }
                Console.WriteLine("ASSIGNMENT");
                if (t.Assignment == null) {
                    Console.WriteLine("No possible Schedule");
                }
                else
                    // Print out nurses
                    foreach (Nurse n in t.Assignment) {
                        string s = "|~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n";
                        s += "|Name: " + n.Name + "\n";
                        s += "|Patients: ";
                        int i = 0;
                        foreach (Patient p in n.Patients) {
                            s += p.RoomNum;
                            if (i++ < n.Patients.Count) {
                                s += ", ";
                            }
                        }
                        s += "\n|Total Acuity: " + n.Acuity;
                        Console.WriteLine(s);
                    }
            }
            Console.ReadKey();
        }
    }
}
