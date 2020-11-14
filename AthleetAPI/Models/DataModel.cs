using System;

namespace AthleetAPI.Models
{
    public class UserModel
    {
        public int userId { get; set; }
        public string userName { get; set; }
        public string userHeadline { get; set; }
    }

    public class UserWorkoutModel
    {
        public int userWorkoutId { get; set; }
        public int userId { get; set; }
        public int workoutId { get; set; }
        public DateTime workoutDate { get; set; }
    }

    public class UserWorkoutExercisesModel
    {
        public int userWorkoutExerciseId { get; set; }
        public int userWorkoutId { get; set; }
        public int exerciseId { get; set; }
        public int numberOfReps { get; set; }
    }

    public class WorkoutsModel
    {
        public int workoutId { get; set; }
        public string workoutName { get; set; }
        public string description { get; set; }
    }

    public class WorkoutExercisesModel
    {
        public int workoutExerciseId { get; set; }
        public int workoutId { get; set; }
        public int exerciseId { get; set; }
        public int defaultReps { get; set; }
    }

    public class ExercisesModel
    {
        public int exerciseId { get; set; }
        public string exerciseName { get; set; }
        public string description { get; set; }
        public int DefaultReps { get; set; }
    }
}
