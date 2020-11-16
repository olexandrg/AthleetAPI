using System;

namespace AthleetAPI.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserHeadline { get; set; }
    }

    public class UserWorkoutModel
    {
        public int UserWorkoutId { get; set; }
        public int UserId { get; set; }
        public int WorkoutId { get; set; }
        public DateTime WorkoutDate { get; set; }
    }

    public class UserWorkoutExercisesModel
    {
        public int UserWorkoutExerciseId { get; set; }
        public int UserWorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public int NumberOfReps { get; set; }
    }

    public class WorkoutsModel
    {
        public int WorkoutId { get; set; }
        public string WorkoutName { get; set; }
        public string Description { get; set; }
    }

    public class WorkoutExercisesModel
    {
        public int WorkoutExerciseId { get; set; }
        public int WorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public int DefaultReps { get; set; }
    }

    public class ExercisesModel
    {
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public string Description { get; set; }
        public int DefaultReps { get; set; }
    }
}
