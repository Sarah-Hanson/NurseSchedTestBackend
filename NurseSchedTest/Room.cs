using NurseScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NurseScheduler {
    /*
     * Each room can have several patients in it
     * each room is a certain distance from the rooms next to it
     */
    public class Room {
        //=============
        // Constructors
        //=============
        public Room(List<Patient> patients, string roomNum) {
            this.adjacencies = new List<RoomRelation>();
            this.patients = patients;
            this.roomNum = roomNum;
        }
        public Room(string roomNum) {
            this.roomNum = roomNum;
            this.adjacencies = new List<RoomRelation>();
            this.patients = new List<Patient>();
        }

        //===========
        // Properties
        //===========
        public string roomNum;
        public List<Patient> patients;
        public List<RoomRelation> adjacencies;

        //===============
        // Methods
        //===============
        public void AddAdjacency(Room r, int dist) {
            adjacencies.Add(new RoomRelation(r, dist));
        }
        public static Room GetRoomByPatient(List<Room> rooms, Patient patient) {
            Room room = null;
            //var room = rooms.Single(r => r.patients.Contains(patient));
            foreach (Room r in rooms)
                if (r.patients.Contains(patient))
                    room = r;
            return room;
        }
        public override string ToString() {
            return roomNum;
        }
        /*
         * Combined with the PathDistRecurser method this method find the shortest distance
         * between two rooms using a simple brute force recursion algorithm. A more fine
         * tuned algorithm isn't super nescessary since there is a guarentee that there
         * will be under 50 rooms max in the system.
         */
        public static int FindPathDist(Room start, Room targ) {
            List<Room> visited = new List<Room>();
            Output o = PathDistRecurser(new Input(targ, start, visited));
            if (o.success)
                return o.result;
            else
                return int.MinValue;
        }
        private static Output PathDistRecurser(Input inp) {
            // Exit case, if the target is the current node you made it!
            if (inp.targ.Equals(inp.curr)) {
                return new Output(true);
            }
            else {
                int shortestDist = int.MaxValue; // Any path found "should" have a distance that is shorter than then overflow of an int
                int nodeDist = -1;
                bool success = false;
                // Check each adjacent room for a path
                foreach (RoomRelation r in inp.curr.adjacencies) {
                    Input newInput = new Input(inp);
                    newInput.curr = r.room;
                    newInput.visited.Add(inp.curr);
                    Output o = PathDistRecurser(newInput);
                    // If the path is found and is shorter than current path it is the new path
                    if (o.success && o.result < shortestDist) {
                        shortestDist = o.result;
                        nodeDist = r.dist;
                        success = true;
                    }
                }
                return (new Output(success, shortestDist + nodeDist));
            }
        }

        // ==============
        // Helper Classes
        // ==============
        // Represents an adjacent room and the distance to it
        public class RoomRelation {
            public Room room;
            public int dist;
            public RoomRelation(Room room, int dist) {
                this.room = room;
                this.dist = dist;
            }
        }
        // All the input values for the recursive algorithm, with some build in copy
        // constructors to avoid changes bleeding backwards as it recurses
        private class Input {
            public Room targ;
            public Room curr;
            public List<Room> visited;
            public bool IsDone() {
                return (curr.Equals(targ));
            }
            public Input(Room targ, Room curr, List<Room> visited) {
                this.targ = targ;
                this.curr = curr;
                this.visited = visited;
            }
            public Input(Input inp) {
                targ = inp.targ;
                curr = inp.curr;
                visited = new List<Room>(inp.visited);
            }
        }
        // The output for the algorithm so that I don't need to mess with out fields
        // also as copy constructor to prevent bleed between method calls
        private class Output {
            public bool success; // Path distance
            public int result;   // Wether a path was found
            public Output(bool success) {
                this.success = success;
                result = 0;
            }
            public Output(Output newOut, Output oldOut) {
                success = newOut.success;
                result = oldOut.result;
            }
            public Output(bool success, int result) {
                this.success = success;
                this.result = result;
            }
        }
    }
}
