using NurseScheduler;
using System;
using System.Collections.Generic;
using System.Text;

namespace NurseSchedTest {
    public class Room {
        public List<Patient> patients;
        public List<RoomRelation> adjacencies;

        public Room(List<Patient> patients) {
            this.patients = patients;
        }
        public void AddAdjacency(Room r, int dist) {
            adjacencies.Add(new RoomRelation(r, dist));
        }
        public class RoomRelation {
            public Room room;
            public int dist;
            public RoomRelation(Room room, int dist) {
                this.room = room;
                this.dist = dist;
            }
        }
        public static Room GetRoomByPatient(List<Room> rooms, Patient patient) {
            foreach (Room r in rooms) {
                if(r.patients.Contains(patient)) {
                    return r;
                }
            }
            return null;
        }
    }
}
