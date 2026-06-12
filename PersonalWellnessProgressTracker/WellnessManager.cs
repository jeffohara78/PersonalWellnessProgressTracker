using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PersonalWellnessProgressTracker
{
    public class WellnessManager
    {
        // UPDATED 06/11/2026:
        // Replaced one single profile with a list of profiles.
        private List<UserProfile> profiles = new List<UserProfile>();

        private int nextProfileId = 101;

        private List<ProgressEntry> progressEntries = new List<ProgressEntry>();

        private List<WorkoutEntry> workoutEntries = new List<WorkoutEntry>();

        private int nextProgressId = 1001;

        private int nextWorkoutId = 5001;

        // UPDATED 06/11/2026:
        // Stores multiple user profiles instead of one single profile.
        private string profileFilePath = "userProfiles.json";

        private string progressFilePath = "progressEntries.json";

        private string workoutFilePath = "workoutEntries.json";

        public WellnessManager()
        {
            LoadDataFromFiles();
        }

        // UPDATED 06/11/2026:
        // This method now creates a new profile instead of replacing one single profile.
        public void CreateOrUpdateProfile()
        {
            Console.WriteLine("\n======================================");
            Console.WriteLine("             ADD PROFILE");
            Console.WriteLine("======================================");
            Console.WriteLine("Use this screen to add a person who wants");
            Console.WriteLine("to track weight, wellness progress, workouts,");
            Console.WriteLine("BMI, and optional body fat percentage.");
            Console.WriteLine();
            Console.WriteLine("Each person receives their own profile ID,");
            Console.WriteLine("so multiple people can track progress separately.");
            Console.WriteLine();
            Console.WriteLine("Enter 0 at any prompt to cancel without saving.");
            Console.WriteLine();

            string fullName = GetTextOrCancel("Name: ");

            if (fullName == "0")
            {
                Console.WriteLine("Profile creation cancelled.");
                return;
            }

            decimal height = GetDecimalOrCancel("Height in inches, such as 73 for 6'1\", or 0 to cancel: ");

            if (height == 0)
            {
                Console.WriteLine("Profile creation cancelled.");
                return;
            }

            decimal startingWeight = GetDecimalOrCancel("Starting weight in pounds, or 0 to cancel: ");

            if (startingWeight == 0)
            {
                Console.WriteLine("Profile creation cancelled.");
                return;
            }

            decimal goalWeight = GetDecimalOrCancel("Goal weight in pounds, or 0 to cancel: ");

            if (goalWeight == 0)
            {
                Console.WriteLine("Profile creation cancelled.");
                return;
            }

            UserProfile profile = new UserProfile(
                nextProfileId,
                fullName,
                height,
                startingWeight,
                goalWeight
            );

            profiles.Add(profile);

            SaveDataToFiles();

            Console.WriteLine("\nProfile saved successfully.");
            Console.WriteLine($"Profile ID: {nextProfileId}");
            Console.WriteLine($"Name: {fullName}");

            nextProfileId++;
        }

        // UPDATED 06/11/2026:
        // Allows the user to choose which profile to view.
        public void ViewProfile()
        {
            Console.WriteLine("\n======================================");
            Console.WriteLine("              VIEW PROFILE");
            Console.WriteLine("======================================");

            if (profiles.Count == 0)
            {
                Console.WriteLine("No profiles have been created yet.");
                return;
            }

            DisplayProfileSummary();

            int profileId = GetIntOrCancel("\nEnter Profile ID to view, or 0 to cancel: ");

            if (profileId == 0)
            {
                Console.WriteLine("View profile cancelled.");
                return;
            }

            UserProfile profile = profiles.Find(item => item.ProfileId == profileId);

            if (profile == null)
            {
                Console.WriteLine("No profile with that ID was found.");
                return;
            }

            Console.WriteLine($"\nName: {profile.FullName}");
            Console.WriteLine($"Height: {profile.HeightInInches} inches");
            Console.WriteLine($"Starting Weight: {profile.StartingWeight} lbs");
            Console.WriteLine($"Goal Weight: {profile.GoalWeight} lbs");

            decimal currentWeight = GetCurrentWeight(profile.ProfileId);

            if (currentWeight > 0)
            {
                decimal bmi = CalculateBmi(currentWeight, profile.HeightInInches);

                Console.WriteLine($"Current Weight: {currentWeight} lbs");
                Console.WriteLine($"Current BMI: {bmi:F1}");
                Console.WriteLine($"BMI Category: {GetBmiCategory(bmi)}");
            }
        }

        // UPDATED 06/11/2026:
        // Progress entries now attach to a selected profile.
        public void AddProgressEntry()
        {
            Console.WriteLine("\n======================================");
            Console.WriteLine("          ADD PROGRESS ENTRY");
            Console.WriteLine("======================================");
            Console.WriteLine("Use this screen to record weight, optional");
            Console.WriteLine("body fat percentage, and progress notes");
            Console.WriteLine("for one saved profile.");
            Console.WriteLine();
            Console.WriteLine("Body fat percentage can come from a smart scale,");
            Console.WriteLine("fitness assessment, body measurement calculator,");
            Console.WriteLine("or professional measurement. If you do not know it,");
            Console.WriteLine("you can press ENTER to skip it.");
            Console.WriteLine();
            Console.WriteLine("Enter 0 at any prompt to cancel without saving.");
            Console.WriteLine();

            if (profiles.Count == 0)
            {
                Console.WriteLine("Create a profile before adding progress entries.");
                return;
            }

            DisplayProfileSummary();

            int profileId = GetIntOrCancel("\nEnter Profile ID for this progress entry, or 0 to cancel: ");

            if (profileId == 0)
            {
                Console.WriteLine("Progress entry cancelled.");
                return;
            }

            UserProfile profile = profiles.Find(item => item.ProfileId == profileId);

            if (profile == null)
            {
                Console.WriteLine("No profile with that ID was found.");
                return;
            }

            decimal weight = GetDecimalOrCancel("Current weight in pounds, or 0 to cancel: ");

            if (weight == 0)
            {
                Console.WriteLine("Progress entry cancelled.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Body fat percentage is optional.");
            Console.WriteLine("Examples of where it may come from:");
            Console.WriteLine("- Smart scale estimate");
            Console.WriteLine("- Gym or trainer assessment");
            Console.WriteLine("- Body measurement calculator");
            Console.WriteLine("- Doctor or health professional measurement");
            Console.WriteLine();

            Console.Write("Body fat percentage, or press ENTER to skip: ");
            string bodyFatInput = Console.ReadLine().Trim();

            decimal? bodyFatPercentage = null;

            if (!string.IsNullOrWhiteSpace(bodyFatInput))
            {
                bool validBodyFat = decimal.TryParse(bodyFatInput, out decimal bodyFat);

                if (validBodyFat && bodyFat >= 0)
                {
                    bodyFatPercentage = bodyFat;
                }
                else
                {
                    Console.WriteLine("Invalid body fat percentage. Entry cancelled.");
                    return;
                }
            }

            string notes = GetTextOrCancel("Notes, or press ENTER to leave blank: ");

            if (notes == "0")
            {
                Console.WriteLine("Progress entry cancelled.");
                return;
            }

            ProgressEntry entry = new ProgressEntry(
                nextProgressId,
                profileId,
                weight,
                bodyFatPercentage,
                notes
            );

            progressEntries.Add(entry);

            SaveDataToFiles();

            Console.WriteLine("\nProgress entry saved successfully.");
            Console.WriteLine($"Profile: {profile.FullName}");
            Console.WriteLine($"Entry ID: {nextProgressId}");
            Console.WriteLine($"Weight: {weight} lbs");

            nextProgressId++;
        }

        // UPDATED 06/11/2026:
        // Workout entries now attach to a selected profile.
        public void AddWorkoutEntry()
        {
            Console.WriteLine("\n======================================");
            Console.WriteLine("           ADD WORKOUT ENTRY");
            Console.WriteLine("======================================");
            Console.WriteLine("Use this screen to record workouts for");
            Console.WriteLine("one saved profile.");
            Console.WriteLine();
            Console.WriteLine("Enter 0 at any prompt to cancel without saving.");
            Console.WriteLine();

            if (profiles.Count == 0)
            {
                Console.WriteLine("Create a profile before adding workout entries.");
                return;
            }

            DisplayProfileSummary();

            int profileId = GetIntOrCancel("\nEnter Profile ID for this workout entry, or 0 to cancel: ");

            if (profileId == 0)
            {
                Console.WriteLine("Workout entry cancelled.");
                return;
            }

            UserProfile profile = profiles.Find(item => item.ProfileId == profileId);

            if (profile == null)
            {
                Console.WriteLine("No profile with that ID was found.");
                return;
            }

            string workoutType = GetTextOrCancel("Workout type, such as Walking or Strength Training: ");

            if (workoutType == "0")
            {
                Console.WriteLine("Workout entry cancelled.");
                return;
            }

            int minutes = GetIntOrCancel("Workout length in minutes, or 0 to cancel: ");

            if (minutes == 0)
            {
                Console.WriteLine("Workout entry cancelled.");
                return;
            }

            string intensity = GetIntensityFromUser();

            if (intensity == "Cancel")
            {
                Console.WriteLine("Workout entry cancelled.");
                return;
            }

            string notes = GetTextOrCancel("Notes, or press ENTER to leave blank: ");

            if (notes == "0")
            {
                Console.WriteLine("Workout entry cancelled.");
                return;
            }

            WorkoutEntry workout = new WorkoutEntry(
                nextWorkoutId,
                profileId,
                workoutType,
                minutes,
                intensity,
                notes
            );

            workoutEntries.Add(workout);

            SaveDataToFiles();

            Console.WriteLine("\nWorkout entry saved successfully.");
            Console.WriteLine($"Profile: {profile.FullName}");
            Console.WriteLine($"Workout ID: {nextWorkoutId}");
            Console.WriteLine($"Workout: {workoutType}");
            Console.WriteLine($"Minutes: {minutes}");

            nextWorkoutId++;
        }

        public void ViewProgressEntries()
        {
            Console.WriteLine("\n======================================");
            Console.WriteLine("          PROGRESS ENTRIES");
            Console.WriteLine("======================================");

            if (progressEntries.Count == 0)
            {
                Console.WriteLine("No progress entries have been added yet.");
                return;
            }

            foreach (ProgressEntry entry in progressEntries)
            {
                Console.WriteLine("\n------------------------------");
                Console.WriteLine($"Entry ID: {entry.EntryId}");
                Console.WriteLine($"Date: {entry.EntryDate}");
                Console.WriteLine($"Weight: {entry.Weight} lbs");

                if (entry.BodyFatPercentage.HasValue)
                {
                    Console.WriteLine($"Body Fat: {entry.BodyFatPercentage.Value}%");
                }

                Console.WriteLine($"Notes: {entry.Notes}");
            }
        }

        public void ViewWorkoutEntries()
        {
            Console.WriteLine("\n======================================");
            Console.WriteLine("           WORKOUT ENTRIES");
            Console.WriteLine("======================================");

            if (workoutEntries.Count == 0)
            {
                Console.WriteLine("No workout entries have been added yet.");
                return;
            }

            foreach (WorkoutEntry workout in workoutEntries)
            {
                Console.WriteLine("\n------------------------------");
                Console.WriteLine($"Workout ID: {workout.WorkoutId}");
                Console.WriteLine($"Date: {workout.WorkoutDate}");
                Console.WriteLine($"Workout Type: {workout.WorkoutType}");
                Console.WriteLine($"Minutes: {workout.Minutes}");
                Console.WriteLine($"Intensity: {workout.Intensity}");
                Console.WriteLine($"Notes: {workout.Notes}");
            }
        }

        // UPDATED 06/11/2026:
        // Updated dashboard to work with multiple profiles.
        // The user now chooses which profile should be analyzed.
        public void ViewWellnessDashboard()
        {
            Console.WriteLine("\n======================================");
            Console.WriteLine("          WELLNESS DASHBOARD");
            Console.WriteLine("======================================");

            if (profiles.Count == 0)
            {
                Console.WriteLine("No profiles have been created yet.");
                return;
            }

            DisplayProfileSummary();

            int profileId = GetIntOrCancel("\nEnter Profile ID to view dashboard, or 0 to cancel: ");

            if (profileId == 0)
            {
                Console.WriteLine("Dashboard view cancelled.");
                return;
            }

            UserProfile profile = profiles.Find(item => item.ProfileId == profileId);

            if (profile == null)
            {
                Console.WriteLine("No profile with that ID was found.");
                return;
            }

            decimal currentWeight = GetCurrentWeight(profile.ProfileId);

            if (currentWeight == 0)
            {
                Console.WriteLine("No progress entries available yet for this profile.");
                return;
            }

            decimal bmi = CalculateBmi(currentWeight, profile.HeightInInches);

            decimal weightChange = currentWeight - profile.StartingWeight;

            decimal poundsToGoal = currentWeight - profile.GoalWeight;

            int totalWorkoutMinutes = 0;

            foreach (WorkoutEntry workout in workoutEntries)
            {
                if (workout.ProfileId == profile.ProfileId)
                {
                    totalWorkoutMinutes += workout.Minutes;
                }
            }

            Console.WriteLine($"\nName: {profile.FullName}");
            Console.WriteLine($"Starting Weight: {profile.StartingWeight} lbs");
            Console.WriteLine($"Current Weight: {currentWeight} lbs");
            Console.WriteLine($"Goal Weight: {profile.GoalWeight} lbs");
            Console.WriteLine();

            Console.WriteLine("--- Weight Progress ---");

            if (weightChange < 0)
            {
                Console.WriteLine($"Weight Change: Lost {Math.Abs(weightChange)} lbs");
            }
            else if (weightChange > 0)
            {
                Console.WriteLine($"Weight Change: Gained {weightChange} lbs");
            }
            else
            {
                Console.WriteLine("Weight Change: No change yet");
            }

            if (poundsToGoal > 0)
            {
                Console.WriteLine($"Remaining to Goal: {poundsToGoal} lbs");
            }
            else
            {
                Console.WriteLine($"Goal Status: Goal met or exceeded by {Math.Abs(poundsToGoal)} lbs");
            }

            Console.WriteLine();

            Console.WriteLine("--- BMI Summary ---");
            Console.WriteLine($"Current BMI: {bmi:F1}");
            Console.WriteLine($"BMI Category: {GetBmiCategory(bmi)}");
            Console.WriteLine();

            Console.WriteLine("--- Workout Summary ---");
            Console.WriteLine($"Total Workout Minutes: {totalWorkoutMinutes}");

            decimal latestBodyFat = GetLatestBodyFat(profile.ProfileId);

            if (latestBodyFat > 0)
            {
                Console.WriteLine();
                Console.WriteLine("--- Body Fat Summary ---");
                Console.WriteLine($"Latest Body Fat Percentage: {latestBodyFat}%");
            }
        }

        // NEW 06/11/2026:
        // Shows all saved profiles so the user can choose who they are working with.
        private void DisplayProfileSummary()
        {
            Console.WriteLine("\n--- Current Profiles ---");

            foreach (UserProfile profile in profiles)
            {
                Console.WriteLine($"Profile ID: {profile.ProfileId} | Name: {profile.FullName} | Goal Weight: {profile.GoalWeight} lbs");
            }
        }

        // UPDATED 06/11/2026:
        // Gets the latest weight for one selected profile.
        private decimal GetCurrentWeight(int profileId)
        {
            ProgressEntry latestEntry = null;

            foreach (ProgressEntry entry in progressEntries)
            {
                if (entry.ProfileId == profileId)
                {
                    if (latestEntry == null || entry.EntryDate > latestEntry.EntryDate)
                    {
                        latestEntry = entry;
                    }
                }
            }

            if (latestEntry == null)
            {
                return 0;
            }

            return latestEntry.Weight;
        }

        // UPDATED 06/11/2026:
        // Gets the latest body fat percentage for one selected profile.
        private decimal GetLatestBodyFat(int profileId)
        {
            decimal latestBodyFat = 0;

            DateTime latestDate = DateTime.MinValue;

            foreach (ProgressEntry entry in progressEntries)
            {
                if (entry.ProfileId == profileId &&
                    entry.BodyFatPercentage.HasValue &&
                    entry.EntryDate > latestDate)
                {
                    latestBodyFat = entry.BodyFatPercentage.Value;
                    latestDate = entry.EntryDate;
                }
            }

            return latestBodyFat;
        }

        private decimal CalculateBmi(decimal weight, decimal heightInInches)
        {
            if (heightInInches <= 0)
            {
                return 0;
            }

            return (weight / (heightInInches * heightInInches)) * 703;
        }

        private string GetBmiCategory(decimal bmi)
        {
            if (bmi < 18.5m)
            {
                return "Underweight";
            }
            else if (bmi < 25)
            {
                return "Normal weight";
            }
            else if (bmi < 30)
            {
                return "Overweight";
            }
            else
            {
                return "Obese";
            }
        }

        private string GetIntensityFromUser()
        {
            while (true)
            {
                Console.WriteLine("\nChoose workout intensity:");
                Console.WriteLine("1. Light - easy movement, low effort");
                Console.WriteLine("2. Moderate - noticeable effort, but manageable");
                Console.WriteLine("3. Hard - challenging effort");
                Console.WriteLine("0. Cancel and return to main menu");
                Console.Write("Choose option 0 through 3: ");

                string choice = Console.ReadLine();

                if (choice == "1") return "Light";
                if (choice == "2") return "Moderate";
                if (choice == "3") return "Hard";
                if (choice == "0") return "Cancel";

                Console.WriteLine("Invalid option. Please choose 0 through 3.");
            }
        }

        private string GetTextOrCancel(string prompt)
        { 
            Console.Write(prompt);

            return Console.ReadLine().Trim();
        }

        private decimal GetDecimalOrCancel(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                string input = Console.ReadLine().Trim();

                if (input == "0")
                {
                    return 0;
                }

                bool isValidDecimal = decimal.TryParse(input, out decimal value);

                if (isValidDecimal && value > 0)
                { 
                    return value;
                }

                Console.WriteLine("Invalid number, enter a positive number, or 0 to cancel.");
            }
        }

        private int GetIntOrCancel(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);

                string input = Console.ReadLine().Trim();

                if (input == "0")
                {
                    return 0;
                }

                bool isValidNumber = int.TryParse(input, out int value);

                if (isValidNumber && value > 0)
                {
                    return value;
                }

                Console.WriteLine("Invalid number, enter a positive whole number, or 0 to cancel.");
            }
        }

        // UPDATED 06/11/2026:
        // Saves multiple profiles, progress entries, and workout entries.
        private void SaveDataToFiles()
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string profileJson = JsonSerializer.Serialize(profiles, options);

            string progressJson = JsonSerializer.Serialize(progressEntries, options);

            string workoutJson = JsonSerializer.Serialize(workoutEntries, options);

            File.WriteAllText(profileFilePath, profileJson);

            File.WriteAllText(progressFilePath, progressJson);

            File.WriteAllText(workoutFilePath, workoutJson);
        }

        private void LoadDataFromFiles()
        {
            // UPDATED 06/11/2026:
            // Loads multiple profiles from file.
            if (File.Exists(profileFilePath))
            {
                string profileJson = File.ReadAllText(profileFilePath);

                if (!string.IsNullOrWhiteSpace(profileJson))
                {
                    profiles = JsonSerializer.Deserialize<List<UserProfile>>(profileJson);

                    if (profiles == null)
                    {
                        profiles = new List<UserProfile>();
                    }
                }
            }

            if (File.Exists(progressFilePath))
            {
                string progressJson = File.ReadAllText(progressFilePath);

                if (!string.IsNullOrWhiteSpace(progressJson))
                {
                    progressEntries = JsonSerializer.Deserialize<List<ProgressEntry>>(progressJson);

                    if (progressEntries == null)
                    {
                        progressEntries = new List<ProgressEntry>();
                    }
                }
            }

            if (File.Exists(workoutFilePath))
            {
                string workoutJson = File.ReadAllText(workoutFilePath);

                if (!string.IsNullOrWhiteSpace(workoutJson))
                {
                    workoutEntries = JsonSerializer.Deserialize<List<WorkoutEntry>>(workoutJson);

                    if (workoutEntries == null)
                    {
                        workoutEntries = new List<WorkoutEntry>();
                    }
                }
            }

            // UPDATED 06/11/2026:
            // Prevents duplicate profile IDs after loading saved profiles.
            foreach (UserProfile profile in profiles)
            {
                if (profile.ProfileId >= nextProfileId)
                {
                    nextProfileId = profile.ProfileId + 1;
                }
            }

            foreach (ProgressEntry entry in progressEntries)
            {
                if (entry.EntryId >= nextProgressId)
                {
                    nextProgressId = entry.EntryId + 1;
                }
            }

            foreach (WorkoutEntry workout in workoutEntries)
            {
                if (workout.WorkoutId >= nextWorkoutId)
                {
                    nextWorkoutId = workout.WorkoutId + 1;
                }
            }
            
        }

    }

}