using NurseScheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NurseScheduler {
    class Program {
        // Real Example is 45 patients to 12 nurses on 4 teams (or 9 on night shift)
        /*
         * Test data generation settings
         */
        const int TOTAL_PATIENTS = 45;
        const int TOTAL_NURSES = 12;

        const int teamCount = 4;
        const int patientCount = TOTAL_PATIENTS / teamCount;
        const int nurseCount = TOTAL_NURSES / teamCount;
        const int preferenceCount = TOTAL_NURSES / 2;

        const int patientAcuityMin = 1;
        const int patientAcuityMax = 5;

        const int preferenceWeightMin = -5;
        const int preferenceWeightMax = 5;
        /*
         * Scheduler Algorithm Settings
         */
        const int maxDisparity = 3;
        const int snipLevel = 1 + patientAcuityMax; //Must be greater than max patient acuity
        const int preferenceMult = 1;
        const int disparityMult = 1;
        const int distanceMult = 5;


        static void Main() {
            /*
             * Make Test Data, and a schedulizer object
             */
            List<Team> teams = MakeTestData();
            Schedulizer sched = new Schedulizer(maxDisparity, snipLevel, preferenceMult, disparityMult, distanceMult);
            /*
             * Run schedulizer on test data
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
                    Console.WriteLine(">No possible Schedule to meet constraints!<");
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
                        s += "\n|Rooms:";
                        List<Room> rooms = new List<Room>();
                        foreach (Patient p in n.Patients) {
                            Room r = Room.GetRoomByPatient(t.Rooms, p);
                            if (!rooms.Contains(r)) {
                                rooms.Add(r);
                            }
                        }
                        i = 0;
                        foreach (Room r in rooms) {
                            s += r;
                            if (i < rooms.Count)
                                s += ",";
                        }
                        Console.WriteLine(s);
                    }
            }
            Console.WriteLine("\n\nPress Any Key To Exit");
            Console.ReadKey();
        }

        static public List<Team> MakeTestData() {
            Random rand = new Random();
            Dictionary<string, Room> rooms = new Dictionary<string, Room>();
            List<Team> teams = new List<Team>();
            /*
             * Making and setting up rooms
             */
            void makeRoom(int i) {
                String s = "Room" + i;
                rooms.Add(s, new Room(s));
            }
            //Create all the rooms for the unit
            for (int i = 1; i <= 29; i++) {
                makeRoom(i);
            }
            makeRoom(49);
            makeRoom(50);
            makeRoom(51);
            // Mapping rooms to each other
            void MakeAdjacent(int i, int j, int dist) {
                rooms["Room" + i].AddAdjacency(rooms["Room" + j], dist);
            }
            void AdjacentRange(int startNum, int endNum) {
                for (int i = startNum; i < endNum; i++)
                    MakeAdjacent(i, i + 1, 2);
            }
            void CrossHallwayLink(int sideAstart, int sideBStart, int range, int dist) {
                int max = sideAstart + range;
                while (sideAstart < max) {
                    MakeAdjacent(sideAstart++, sideBStart--, dist);
                }
            }
            // Link all next-door rooms
            AdjacentRange(1, 9);
            AdjacentRange(10, 16);
            AdjacentRange(17, 23);
            AdjacentRange(24, 26);
            AdjacentRange(27, 29);
            // The "special" rooms
            MakeAdjacent(49, 50, 2);
            MakeAdjacent(24, 50, 2);
            // Hallway->Hallway Links
            MakeAdjacent(9, 10, 5);
            MakeAdjacent(16, 17, 5);
            MakeAdjacent(23, 49, 5);
            MakeAdjacent(1, 29, 10);
            // Link rooms Across Hallways
            CrossHallwayLink(1, 8, 4, 3);
            MakeAdjacent(29, 49, 3);
            MakeAdjacent(28, 50, 3);
            MakeAdjacent(27, 24, 3);

            /*
             * Generate Teams
             */
            int n = 0; // nurse# counter
            int p = 0; // patient# counter
            List<Nurse> MakeNurses(int r) {
                List<Nurse> nurses = new List<Nurse>();
                for (int i = 0; i < r; i++)
                    nurses.Add(new Nurse("Nurse" + n++));
                return nurses;
            }
            List<Patient> MakePatients(int r) {
                List<Patient> patients = new List<Patient>();
                for (int i = 0; i < r; i++)
                    patients.Add(new Patient("Patient" + p++, rand.Next(patientAcuityMin, patientAcuityMax)));
                return patients;
            }
            // Make Teams
            for (int i = 0; i < teamCount; i++) {
                teams.Add(new Team());
                teams[i].TeamName = "Team" + i;
                teams[i].Rooms = rooms.Values.ToList(); ;
            }
            // Populate teams
            foreach (Team t in teams) {
                t.Nurses = MakeNurses(nurseCount);
                t.Patients = MakePatients(patientCount);
                // Set preferences for the team
                for (int j = 0; j < preferenceCount; j++) {
                    Nurse nurse = t.Nurses[rand.Next(0, t.Nurses.Count)];
                    Patient patient = t.Patients[rand.Next(0, t.Patients.Count)];
                    t.Preferences.Add(new Preference(nurse, patient, rand.Next(preferenceWeightMin, preferenceWeightMax)));
                }
            }
            {
                //Adding Patients to rooms (obviously in the most readable format ever conceived)
                int j = 1;
                int i = 0;                // 1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 49 50 51
                int[] roomCaps = new int[] { 2, 2, 2, 2, 1, 2, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 2, 3, 1, 1, 1 };
                foreach (Team t in teams) {
                    i = 0;
                    while (i < t.Patients.Count && j < roomCaps.Length) {
                        for (int k = 0; k < roomCaps[i]; k++) {
                            rooms["Room" + j].patients.Add(t.Patients[i]);
                            i++;
                        }
                        j++;
                    } 
                }
                //Team lastTeam = teams[teams.Count - 1];
                //rooms["Room49"].patients.Add(lastTeam.Patients[i++]);
                //rooms["Room50"].patients.Add(lastTeam.Patients[i++]);
            }
            return teams;
        }
    }
}
