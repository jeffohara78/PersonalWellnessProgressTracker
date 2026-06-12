namespace PersonalWellnessProgressTracker
{
    public class UserProfile
    {
        public int ProfileId { get; set; }

        public string FullName { get; set; }

        public decimal HeightInInches { get; set; }

        public decimal StartingWeight { get; set; }

        public decimal GoalWeight { get; set; }

        public UserProfile()
        {
        }

        // UPDATED 06/11/2026:
        // Added profileId so the program can support multiple saved profiles.
        public UserProfile(int profileId, string fullName, decimal heightInInches, decimal startingWeight, decimal goalWeight)
        {
            ProfileId = profileId;
            FullName = fullName;
            HeightInInches = heightInInches;
            StartingWeight = startingWeight;
            GoalWeight = goalWeight;
        }
    }
}