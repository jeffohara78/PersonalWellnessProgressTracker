using System;

namespace PersonalWellnessProgressTracker
{
    public class WorkoutEntry
    {
        public int WorkoutId { get; set; }

        public int ProfileId { get; set; }

        public DateTime WorkoutDate { get; set; }

        public string WorkoutType { get; set; }

        public int Minutes { get; set; }

        public string Intensity { get; set; }

        public string Notes { get; set; }

        public WorkoutEntry()
        {
        }

        // UPDATED 06/11/2026:
        // Added profileId so each workout entry belongs to one specific person.
        public WorkoutEntry(int workoutId, int profileId, string workoutType, int minutes, string intensity, string notes)
        {
            WorkoutId = workoutId;
            ProfileId = profileId;
            WorkoutDate = DateTime.Now;
            WorkoutType = workoutType;
            Minutes = minutes;
            Intensity = intensity;
            Notes = notes;
        }
    }
}