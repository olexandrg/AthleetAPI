using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AthleetAPI.Models
{

    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirebaseUID { get; set; }
        public string UserHeadline { get; set; }
    }

    public class UserWorkouts
    {
        [Key]
        public int UserWorkoutId { get; set; }
        public int UserId { get; set; }
        public int WorkoutId { get; set; }
        public DateTime WorkoutDate { get; set; }
    }

    public class UserWorkoutExercises
    {
        [Key]
        public int UserWorkoutExerciseId { get; set; }
        public int UserWorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public int NumberOfReps { get; set; }
    }

    public class Workouts
    {
        [Key]
        public int WorkoutId { get; set; }
        public string WorkoutName { get; set; }
        public string Description { get; set; }
    }

    public class ViewUserWorkouts
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        //key must be set to a value that is unique for the entry
        //if not then the same record will be duplicated weather or not the records are identical
        //example: key was set to userId when ran there were two different records both with UserId
        //as 1 the first record was used in place of the second one
        [Key]
        public int WorkoutId { get; set; }
        public DateTime WorkoutDate { get; set; }
        public string WorkoutName { get; set; }
        public string Description { get; set; }
    }

    [Keyless]
    public class ViewExerciseInWorkout
    {
        public string UserName { get; set; }
        public string WorkoutName { get; set; }
        public string ExerciseName { get; set; }
        public string Description { get; set; }
        public int DefaultReps { get; set; }
    }

    public class WorkoutExercises
    {
        [Key]
        public int WorkoutExerciseId { get; set; }
        public int WorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public int DefaultReps { get; set; }
    }

    public class Exercises
    {
        [Key]
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; }
        public string Description { get; set; }
        public int DefaultReps { get; set; }
    }

    public class TeamWorkouts
    {
        [Key]
        public int TeamWorkoutID { get; set; }
        public int TeamID { get; set; }
        public int WorkoutID { get; set; }
        public DateTime WorkoutDate { get; set; }
    }

    public class TeamUser
    {
        [Key]
        public int TeamUserID { get; set; }
        public int TeamID { get; set; }
        public int UserID { get; set; }
        public bool isAdmin { get; set; }
    }

    public class Team
    {
        [Key]
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public string Description { get; set; }
    }
}
