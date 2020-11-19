using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("User")]
public class Student
{
    [Column("UserID")]
    [Key]
    public long UserID { get; set; }

    [Column("FirebaseUID")]
    public string FireBaseUID { get; set; }

    [Column("UserName")]
    public string UserName { get; set; }

    [Column("UserHeadline")]
    public string UserHeadline { get; set; }
}