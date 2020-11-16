using Microsoft.EntityFrameworkCore;

namespace AthleetAPI.Models
{
    public class AthleetContext : DbContext
    {
        public AthleetContext(DbContextOptions<AthleetContext> options) 
            : base(options)
        {

        }

        public DbSet<UserModel> User { get; set; }
        public DbSet<UserWorkoutModel> UserWorkoutModels { get; set; }
        public DbSet<UserWorkoutExercisesModel> UserWorkoutExercisesModels { get; set; }
        public DbSet<WorkoutsModel> WorkoutsModels { get; set; }
        public DbSet<WorkoutExercisesModel> WorkoutExerciseModels { get; set; }
        public DbSet<ExercisesModel> ExercisesModels { get; set; }
    }
}
