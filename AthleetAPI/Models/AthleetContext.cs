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
        public DbSet<UserWorkouts> UserWorkouts { get; set; }
        public DbSet<UserWorkoutExercises> UserWorkoutExercises { get; set; }
        public DbSet<Workouts> Workouts { get; set; }
        public DbSet<WorkoutExercises> WorkoutExercises { get; set; }
        public DbSet<Exercises> Exercises { get; set; }
        public DbSet<ViewUserWorkouts> ViewUserWorkouts { get; set; }
        public DbSet<ViewExerciseInWorkout> ViewExerciseInWorkout { get; set; }
    }
}
