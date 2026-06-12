/* Jeff O'Hara
 * 6/11/2026
 * 
 * Helps users monitor their health and fitness journey by tracking weight, BMI, body fat percentage, workout activity, 
 * and progress toward personal wellness goals. The application provides progress reporting and dashboard summaries to 
 * help users evaluate trends, measure results, and stay focused on achieving their health objectives.
 */

/* UPDATES as of 6/11/2026 1700 hours
 * 
 * Added support for multiple user profiles, allowing several individuals to track their weight, workouts, BMI, body fat percentage, 
 * and wellness progress independently within the same application. Also improved the user experience by providing clearer guidance 
 * on body fat percentage measurements and linking progress and workout records to specific profiles for more accurate reporting 
 * and dashboard analysis.
 */

using System;

namespace PersonalWellnessProgressTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            WellnessManager manager = new WellnessManager();

            bool running = true;

            while (running)
            {
                Console.WriteLine("\n==========================================");
                Console.WriteLine("     PERSONAL WELLNESS PROGRESS TRACKER");
                Console.WriteLine("==========================================");
                Console.WriteLine("Track weight, BMI, body fat percentage,");
                Console.WriteLine("workouts, goals, and wellness progress.");
                Console.WriteLine();
                Console.WriteLine("1. Create or update profile");
                Console.WriteLine("2. View profile");
                Console.WriteLine("3. Add progress entry");
                Console.WriteLine("4. Add workout entry");
                Console.WriteLine("5. View progress entries");
                Console.WriteLine("6. View workout entries");
                Console.WriteLine("7. View wellness dashboard");
                Console.WriteLine("8. Exit");
                Console.Write("\nChoose an option 1 through 8: ");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    manager.CreateOrUpdateProfile();
                }
                else if (choice == "2")
                {
                    manager.ViewProfile();
                }
                else if (choice == "3")
                {
                    manager.AddProgressEntry();
                }
                else if (choice == "4")
                {
                    manager.AddWorkoutEntry();
                }
                else if (choice == "5")
                {
                    manager.ViewProgressEntries();
                }
                else if (choice == "6")
                {
                    manager.ViewWorkoutEntries();
                }
                else if (choice == "7")
                {
                    manager.ViewWellnessDashboard();
                }
                else if (choice == "8")
                {
                    running = false;
                    Console.WriteLine("Exiting Personal Wellness Progress Tracker.");
                }
                else
                {
                    Console.WriteLine("Invalid option. Please choose 1 through 8.");
                }
            }
        }
    }
}