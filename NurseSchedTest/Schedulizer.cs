using System;
using System.Collections.Generic;
using System.Text;

namespace NurseScheduler {
    public class Schedulizer {
        private PatientDistanceMatrix pdm;
        private readonly int maxDisparity;
        private readonly int snipLevel;
        private readonly int preferenceMultiplier;
        private readonly int disparityMultiplier;

        public Schedulizer(PatientDistanceMatrix patientDist, int maxDisparity, int snipLevel, int preferenceMultiplier, int disparityMultiplier) {
            this.pdm = patientDist;
            this.maxDisparity = maxDisparity;
            this.snipLevel = snipLevel;
            this.preferenceMultiplier = preferenceMultiplier;
            this.disparityMultiplier = disparityMultiplier;
        }
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
        public List<Nurse> GetSchedule(Team t) {
            Console.WriteLine("Finding Possible Schedules");
            Result res = FindSched(new Input(t.Nurses, t.Patients));
            Console.WriteLine("Total Operations Performed: " + res.totalOperations);
            Console.WriteLine(res.result.Count + " Schedules found, scoring them");
            return CalcBest(res.result, t.Preferences);
        }
        private Result FindSched(Input inp) {
            Result res = new Result();
            res.totalOperations++;
            if (CalcDisparity(inp.nurses) < snipLevel)
                if (inp.IsDone()) {
                    if (CalcDisparity(inp.nurses) < maxDisparity) {
                        res.success = true;
                        res.result.Add(inp.nurses);
                        //Console.WriteLine("Ding! Result Found");
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
            else {
            }
            return res;
        }
        // Calculates the schedule with the best score (most nurses assigned to preferential patients and lowest disparity)
        private List<Nurse> CalcBest(List<List<Nurse>> resultList, List<Preference> prefs) {
            //Evaluate every result, Higher score is better
            List<Nurse> bestResult = null;
            int bestScore = int.MinValue;
            foreach (List<Nurse> nl in resultList) {
                int curScore = CalcScore(nl, prefs);
                if (bestScore < curScore) {
                    Console.WriteLine("|" + bestScore + "|Old:New|" + curScore + "|");
                    bestScore = curScore;
                    bestResult = nl;
                }
            }
            Console.WriteLine("");
            return bestResult;
        }
        //Calculates all the combines scores for a given result
        private int CalcScore(List<Nurse> nl, List<Preference> prefs) {
            int score = 0;
            score -= CalcDisparity(nl)*disparityMultiplier;
            score += CalcMatchScore(nl, prefs);
            //score -= DistanceScore(nl);


            return score;
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
        private int CalcMatchScore(List<Nurse> nl, List<Preference> prefs) {
            int i = 0;
            foreach (Preference pr in prefs) {
                foreach (Nurse n in nl) {
                    foreach (Patient p in n.Patients) {
                        if (p.EqualTo(pr.patient) && n.EqualTo(pr.nurse)) {
                            i += pr.weight*2;
                        }
                    }
                }
            }
            return i;
        }
        // Combines all the patients distance from each other together into a score
        private int CalcDistanceScore(List<Nurse> nl) {
            int i = 0;
            foreach (Nurse n in nl)
                foreach (Patient p1 in n.Patients) {
                    int j = pdm.GetDist(p1, n.Patients[0]);
                    if (j >= 0)
                        i += j;
                }
            return i;
        }
    }
}
