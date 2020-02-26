using NurseSchedTest;
using System;
using System.Collections.Generic;
using System.Text;
using static NurseSchedTest.Room;

namespace NurseScheduler {
    public class PatientDistanceMatrix {
        List<Room> rooms;
        public PatientDistanceMatrix() {
            rooms = new List<Room>();
        }
        public void LinkRooms(Room r1, Room r2, int dist) {
            r1.AddAdjacency(r2, dist);
            r2.AddAdjacency(r1, dist);
        }
        public int GetDist(Patient target, Patient source) {
            if (DFSr(new Input(target, source, new List<Room>()), Room.GetRoomByPatient(rooms, source), out int distance))
                return distance;
            else
                return -1;
        }

        public bool DFSr(in Input inp, Room cur, out int distance ) {
            if(cur.patients.Contains(inp.targ)) {
                distance = 0;
                return true;
            }
            else
                foreach (RoomRelation r in cur.adjacencies)
                    if ( DFSr(inp, r.room, out distance)) {
                        distance += r.dist;
                        return true;
                    }
            distance = int.MinValue;
            return false;
        }

        public class Input {
            public Patient targ;
            public Patient src;
            public List<Room> visited;
            public Input(Patient targ, Patient src, List<Room> visited) {
                this.targ = new Patient(targ);
                this.src = new Patient(src);
                this.visited = new List<Room>(visited);
            }
        }
    }


   
}
