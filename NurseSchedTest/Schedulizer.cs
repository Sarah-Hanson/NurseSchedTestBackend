using System;
using System.Collections.Generic;
using System.Text;

namespace NurseScheduler {
    public class Schedulizer {
        // Tuning settings to tweak the speak and priorities for the schedule
        private readonly int maxDisparity;
        private readonly int snipLevel;
        private readonly int preferenceMult;
        private readonly int disparityMult;
        private readonly int distanceMult;

        // Constructor sets all the tuning settings
        public Schedulizer(int maxDisparity, int snipLevel, int preferenceMultiplier, int disparityMultiplier, int distanceMult) {
            this.maxDisparity = maxDisparity;
            this.snipLevel = snipLevel;
            this.preferenceMult = preferenceMultiplier;
            this.disparityMult = disparityMultiplier;
            this.distanceMult = distanceMult;
        }
        // Inout and Output classes conglomerate results to make pass by value easier
        private partial class Result {
            public bool success;
            public List<List<Nurse>> result;
            public int totalOperations;
            public Result() {
                success = false;
                result = new List<List<Nurse>>();
                totalOperations = 0;
            }
            public Result(Result newRes, Result oldRes) {
                success = newRes.success;
                result = oldRes.result;
                result.AddRange(newRes.result);
                totalOperations = oldRes.totalOperations+newRes.totalOperations;
            }
        }
        private partial class Input {
            public List<Nurse> nurses;
            public List<Patient> patients;
            public bool IsDone() {
                return (patients.Count == 0);
            }

            public Input(List<Nurse> n, List<Patient> p) {
                nurses = n;
                patients = p;
            }
            public Input(Input inp) {
                nurses = new List<Nurse>();
                foreach (Nurse n in inp.nurses)
                    nurses.Add(new Nurse(n));
                patients = new List<Patient>();
                patients.AddRange(inp.patients);
            }
        }
        /*
         * The root function, creates a schedule for a team based on the settings made when instantiating the schedulizer.
         * Once all schedules are found they are scored based on:
         *      Total disparity between nurses
         *      Previous patient match preference
         *      Patient clumping
         * The Schedule with the highest score is selected and returned
         */
        public List<Nurse> GetSchedule(Team t) {
            Console.WriteLine("Finding Possible Schedules\n"+
                "===========================================");
            Result res = FindSched(new Input(t.Nurses, t.Patients));
            Console.WriteLine("Total Operations Performed: " + res.totalOperations);
            Console.WriteLine(res.result.Count + " Schedules found, scoring them");
            return CalcBest(res.result, t.Preferences, t.Rooms);
        }
        /*
         * The recursive bit of the algorithm
         * uses a slightly refined brute force algorithm to dig through all permutations of nurses and patients to
         * return all possible lists that meet a base acuity disparity.
         */
        private Result FindSched(Input inp) {
            Result res = new Result();
            res.totalOperations++;
            if (CalcDisparity(inp.nurses) < snipLevel) // Cuts recursion off if its going off on a unintuitive tangent
                if (inp.IsDone()) {
                    if (CalcDisparity(inp.nurses) < maxDisparity) {
                        res.success = true;
                        res.result.Add(inp.nurses);
                    }
                }
                else {
                    for (int i = 0; i < inp.nurses.Count; i++) {
                        Input newInp = new Input(inp);
                        newInp.nurses[i].AddPatient(newInp.patients[0]);
                        newInp.patients.RemoveAt(0);
                        res = new Result(FindSched(newInp), res); //Concats all other found solutions to result and sets to new success state
                    }
                }
            return res;
        }
        // Finds the schedule with the highest score based on the criteria
        private List<Nurse> CalcBest(List<List<Nurse>> resultList, List<Preference> prefs, List<Room> rooms) {
            //Evaluate every result, Higher score is better
            List<Nurse> bestResult = null;
            int bestScore = int.MinValue;
            foreach (List<Nurse> nl in resultList) {
                int curScore = CalcScore(nl, prefs, rooms);
                if (bestScore < curScore) {
                    Console.WriteLine("|" + bestScore + "|Old:New|" + curScore + "|");
                    bestScore = curScore;
                    bestResult = nl;
                }
            }
            Console.WriteLine("");
            return bestResult;
        }
        //Calculates and sums all the criteria scores for a schedule
        private int CalcScore(List<Nurse> nl, List<Preference> prefs, List<Room> rooms) {
            return (CalcMatchScore(nl, prefs) * preferenceMult) 
                - (CalcDisparity(nl) * disparityMult) 
                - (CalcDistanceScore(nl, rooms) * distanceMult);
        }
        //Calculates the difference between the nurse with the greatest acuity and the nurse with the lowest accuity
        private static int CalcDisparity(List<Nurse> nurses) {
            int max = 0;
            int min = int.MaxValue;
            foreach (Nurse n in nurses) {
                if (n.Acuity > max) { max = n.Acuity; }
                if (n.Acuity < min) { min = n.Acuity; }
            }
            return max - min;
        }
        // Scores the schedule based on how many preferences it matches and their weight
        private int CalcMatchScore(List<Nurse> nl, List<Preference> prefs) {
            int i = 0;
            foreach (Preference pr in prefs) {
                foreach (Nurse n in nl) {
                    foreach (Patient p in n.Patients) {
                        if (p.EqualTo(pr.Patient) && n.EqualTo(pr.Nurse)) {
                            i += pr.Weight;
                        }
                    }
                }
            }
            return i;
        }
        // Combines all the patients distance from each other together into a score
        private int CalcDistanceScore(List<Nurse> nl, List<Room> rooms) {
            int totalDist = 0;
            foreach (Nurse n in nl) {
                if (n.Patients.Count > 0) {
                    Room r = Room.GetRoomByPatient(rooms, n.Patients[0]);
                    foreach (Patient p in n.Patients) {
                        Room r2 = Room.GetRoomByPatient(rooms, p);
                        totalDist += Room.FindPathDist(r, r2);
                    }
                }
            }
            return totalDist;
        }
    }
}
