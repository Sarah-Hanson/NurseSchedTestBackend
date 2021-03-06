﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NurseScheduler {
    public class Team {
        // Constructor
        public Team() {
            TeamName = "";
            Nurses = new List<Nurse>();
            Patients = new List<Patient>();
            Preferences = new List<Preference>();
            Assignment = new List<Nurse>();
            Rooms = new List<Room>();
        }

        // Properties
        public string TeamName { get; set; }
        public List<Nurse> Nurses { get; set; }
        public List<Patient> Patients { get; set; }
        public List<Preference> Preferences { get; set; }
        public List<Nurse> Assignment { get; set; }
        public List<Room> Rooms { get; set; }


    }
}
