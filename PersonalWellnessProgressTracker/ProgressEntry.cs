using System;

namespace PersonalWellnessProgressTracker
{
    public class ProgressEntry
    {
        public int EntryId { get; set; }

        public int ProfileId { get; set; }

        public DateTime EntryDate { get; set; }

        public decimal Weight { get; set; }

        public decimal? BodyFatPercentage { get; set; }

        public string Notes { get; set; }

        public ProgressEntry()
        {
        }

        // UPDATED 06/11/2026:
        // Added profileId so each progress entry belongs to one specific person.
        public ProgressEntry(int entryId, int profileId, decimal weight, decimal? bodyFatPercentage, string notes)
        {
            EntryId = entryId;
            ProfileId = profileId;
            EntryDate = DateTime.Now;
            Weight = weight;
            BodyFatPercentage = bodyFatPercentage;
            Notes = notes;
        }
    }
}