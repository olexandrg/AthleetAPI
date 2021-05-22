using Microsoft.EntityFrameworkCore;

namespace AthleetAPI.Models
{
    public class AthleetContext : DbContext
    {
        
        public AthleetContext(DbContextOptions<AthleetContext> options) 
            : base(options)
        {

        }

        public DbSet<User> User { get; set; }
        public DbSet<BlockedUser> BlockedUsers { get; set; }
        public DbSet<UserWorkouts> UserWorkouts { get; set; }
        public DbSet<UserWorkoutExercises> UserWorkoutExercises { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutExercises> WorkoutExercises { get; set; }
        public DbSet<Exercises> Exercises { get; set; }
        public DbSet<ViewUserWorkouts> ViewUserWorkouts { get; set; }
        public DbSet<ViewExerciseInWorkout> ViewExerciseInWorkout { get; set; }
        public DbSet<TeamListItem> TeamListItem { get; set; }
        public DbSet<Team> Team { get; set; }
        public DbSet<TeamUser> TeamUser { get; set; }
        public DbSet<TeamWorkoutNames> TeamWorkoutNames { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<Conversations> Conversations { get; set; }
        public DbSet<UserConversations> UserConversations { get; set; }
        public DbSet<ConvList> ConvList { get; set; }
    }
}
