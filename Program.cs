using System;
using System.Linq;
using System.Collections.Generic;

class Student
{
    public string Name { get; private set; }
    public List<double> Grades { get; private set; }

    public Student(string name, List<double> grades)
    {
        Name = name;
        Grades = grades;
    }
    
    public void ModifyGrades(List<double> newGrades)  // method to modify the grades of the student
    {
        Grades = newGrades;
    }
}

class StudentManager
{
    private Dictionary<string, Student> StudentData { get; set; }
    private SortedList<double, List<string>> Leaderboard { get; set; }
    private HashSet<string> ModifiedStudents { get; set; }

    private Queue<string> PendingOperations { get; set; }

    public StudentManager()
    {
        StudentData = [];
        Leaderboard = [];
        ModifiedStudents = [];
        PendingOperations = new Queue<string>();
    }

    public void AddStudent(string name, List<double> grades)
    {
        // create a new student
        Student newStudent = new(name, grades);

        // add the student to the dictionary
        if (StudentData.ContainsKey(name))
        {
            Console.WriteLine($"Student, {name}, already exists. Cannot add duplicate student.");
            return;
        }
        StudentData[name] = newStudent;

        ModifiedStudents.Add(name); // add the name of the student to the modified students set
        PendingOperations.Enqueue($"New student added: {name}"); // add the name of the student to the pending operations queue
        UpdateLeaderboard(); // update the leaderboard
        return;
    }

    public void UpdateGrades(string name, List<double> newGrades)
    {
        // Check the name of the student and if the value of that student is the same as the argument
        if (StudentData.TryGetValue(name, out Student? value))
        {
            Console.WriteLine($"Student, {name}, found.");
            value.ModifyGrades(newGrades); // update grades of the student
            Console.WriteLine($"{name}'s grades modified successfully");
            ModifiedStudents.Add(name); // add the name of the student to the modified students set
            PendingOperations.Enqueue($"Modified {name}"); // add the name of the student to the pending operations queue
            UpdateLeaderboard(); // update the leaderboard
            return;
        }
        else
        {
            Console.WriteLine($"{name} not found.");
        }
    }

    public void RemoveStudent(string name)
    {
        // check if student exists
        if (StudentData.ContainsKey(name))
        {
            StudentData.Remove(name);  // Remove from dictionary
            ModifiedStudents.Remove(name);  // Remove from modified students
            PendingOperations.Enqueue($"{name} has been removed.");
            UpdateLeaderboard();
            return;
        }
        else
        {
            Console.WriteLine($"{name} not found.");
        }
    }

    public string SearchStudentByName(string name)
    {
        if (StudentData.Values.FirstOrDefault(s => s.Name == name) != null)
        {
            return $"{name} found";
        }

        return $"{name} not found";
    }

    public string GetTopStudent()
    {
        return StudentData.Values.OrderByDescending(s => s.Grades.Average()).FirstOrDefault()?.Name ?? "No students found";
    }

    public void ViewStudentRecord()
    {
        for (int i = Leaderboard.Count - 1; i >= 0; i--)
        {
            
            Console.WriteLine($"{Leaderboard.Keys[i].ToString("F2")} : {string.Join(", ", Leaderboard.Values[i])}");
        }
    }

    private void UpdateLeaderboard()
    {
        Leaderboard.Clear(); // First clear the leaderboard to recalculate it based on the current student data

        foreach (var student in StudentData.Values)
        {
            double average = student.Grades.Average(); // calculate the average grade of the student

            // check if average already exists in leaderboard
            if (Leaderboard.TryGetValue(average, out List<string>? value))
            {
                value.Add(student.Name);
            }
            else
            {
                Leaderboard.Add(average, [student.Name]);
            }
        }
    }
}


public class Program
{
    public static void Main(string[] args)
    {
        StudentManager manager = new();

        manager.AddStudent("Alice", [85, 90, 92]);
        manager.AddStudent("Bob", [78, 82, 88]);
        manager.AddStudent("Charlie", [90, 95, 93]);

        Console.WriteLine("Initial Leaderboard:");
        manager.ViewStudentRecord();

        manager.UpdateGrades("Bob", [80, 85, 90]);
        Console.WriteLine("\nLeaderboard after updating Bob's grades:");
        manager.ViewStudentRecord();

        manager.RemoveStudent("Alice");
        Console.WriteLine("\nLeaderboard after removing Alice:");
        manager.ViewStudentRecord();

        string message = manager.SearchStudentByName("Bob");
        string message2 = manager.SearchStudentByName("Brian");
        string topStudent = manager.GetTopStudent();

        Console.WriteLine(message);
        Console.WriteLine(message2);
        Console.WriteLine(topStudent);
        
    }
}