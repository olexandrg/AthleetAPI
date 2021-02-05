using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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
        public DbSet<Workout> Workout { get; set; }
        public DbSet<WorkoutExercises> WorkoutExercises { get; set; }
        public DbSet<Exercises> Exercises { get; set; }
        public DbSet<ViewUserWorkouts> ViewUserWorkouts { get; set; }
        public DbSet<ViewExerciseInWorkout> ViewExerciseInWorkout { get; set; }
        public DbSet<TeamListItem> TeamListItem { get; set; }
        public DbSet<Team> Team { get; set; }
        public DbSet<TeamUserNames> TeamUserNames { get; set; }
        public DbSet<TeamWorkoutNames> TeamWorkoutNames { get; set; }
        public DbSet<ExerciseID> ExerciseID { get; set; }
        //public DbSet<List<int>> ExerciseID { get; set; }
    }
}
