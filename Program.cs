using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Text.Json;

namespace todo
{
    static class Program
    {
        public static List<Task> taskList = new List<Task>();

        private static readonly string _saveFile = @"..\..\..\temp\activeTasks.bin";
        public static bool appRunning = true;

        static void Main(string[] args)
        {
            LoadData();
            Console.Clear();

            while (appRunning)
            {
                Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
                {
                    e.Cancel = true;
                    Program.appRunning = false;
                };

                Console.Clear();
                Console.WriteLine("Tasks:\n");

                for (int i = 0; i < taskList.Count; i++)
                {
                    Console.WriteLine((i + 1) + ". " + taskList[i].Entry);
                }

                Console.WriteLine("\n--------------");
                Console.WriteLine("[e]dit, [r]emove, [o]ld, [q]uit, or start typing to add a task:");
                string entry = Console.ReadLine();
                if (string.IsNullOrEmpty(entry))
                {
                    Console.WriteLine("Nothing was entered.");
                }
                else if (string.Equals(entry.ToLower().Trim(), "e"))
                {
                    Console.Clear();
                    Console.WriteLine("Edit which task? [Integer]\n");
                    for (int i = 0; i < taskList.Count; i++)
                    {
                        Console.WriteLine("(" + taskList[i].ID + "). " + taskList[i].Entry);
                    }
                    string edit = Console.ReadLine();
                    if (string.IsNullOrEmpty(edit))
                    {
                        Console.WriteLine("Nothing was entered.");
                    }
                    else
                    {
                        int num = int.Parse(edit); //add parse to not break if not a number
                        for (int i = 0; i < taskList.Count; i++)
                        {
                            if (num == taskList[i].ID)
                            {
                                Task selected = taskList[i];
                                Console.Clear();
                                Console.WriteLine("Task selected:\n");
                                Console.WriteLine(selected.ID + ". " + selected.Entry);
                                Console.WriteLine("\nNotes:\n");
                                Console.WriteLine("- " + selected.Notes);
                                if (selected.isComplete)
                                {
                                    Console.WriteLine("\n[Active]");
                                }
                                else
                                {
                                    Console.WriteLine("\n[Complete]");
                                }

                                Console.WriteLine("\n--------------");
                                Console.WriteLine("[done] to complete task, or start typing to add notes.\n");
                                string notes = Console.ReadLine();
                                if (string.IsNullOrEmpty(notes))
                                {
                                    Console.WriteLine("Nothing was entered. Press [Enter] again to go back.");
                                }
                                else if (string.Equals(notes.ToLower().Trim(), "done"))
                                {
                                    selected.CompleteTask();
                                    Console.WriteLine("Task completed.");
                                }
                                else
                                {
                                    selected.Notes = notes;
                                }
                            }
                        }
                        Console.ReadLine();
                    }
                }
                else if (string.Equals(entry.ToLower().Trim(), "o"))//clean
                {
                    List<Task> oldTasks = new List<Task>();
                    foreach (Task task in taskList)
                    {
                        if (task.isComplete)
                        { oldTasks.Add(task); }
                        else
                        {
                            Console.WriteLine("No completed tasks.");
                        }

                        for (int i = 0; i < oldTasks.Count; i++)
                        {
                            Console.WriteLine(oldTasks[i].ID + ". " + oldTasks[i].Entry);
                            Console.WriteLine("\nNotes:\n- ");
                            Console.WriteLine(oldTasks[i].Notes);
                            Console.WriteLine(oldTasks[i].isComplete ? "[Complete]" : "[Active]");
                            Console.WriteLine("------------------------");
                        }
                    }
                    Console.ReadLine();
                }
                else if (string.Equals(entry.ToLower().Trim(), "r"))//clean
                {
                    Console.WriteLine("Which task should be removed?");

                    for (int i = 0; i < taskList.Count; i++)
                    {
                        Console.WriteLine(taskList[i].ID + ". " + taskList[i].Entry);
                    }
                    string remove = Console.ReadLine();
                    if (string.IsNullOrEmpty(remove))
                    {
                        Console.WriteLine("Nothing was entered.");
                    }
                    else
                    {
                        int num = int.Parse(remove); //add parse to not break if not a number
                        for (int i = 0; i < taskList.Count; i++)
                        {
                            if (num == taskList[i].ID)
                            {
                                Task selected = taskList[i];
                                Console.Clear();
                                Console.WriteLine("Task selected:\n");
                                Console.WriteLine(selected.ID + ". " + selected.Entry);
                                Console.WriteLine("\nNotes:\n");
                                Console.WriteLine("- " + selected.Notes);
                                if (selected.isComplete)
                                {
                                    Console.WriteLine("\n[Active]");
                                }
                                else
                                {
                                    Console.WriteLine("\n[Complete]");
                                }

                                Console.WriteLine("Enter DELETE to remove task.\n");
                                string notes = Console.ReadLine();
                                if (string.IsNullOrEmpty(notes))
                                {
                                    Console.WriteLine("Nothing was entered. Press [Enter] again to go back.");
                                }
                                else if (string.Equals(notes, "DELETE"))
                                {
                                    selected.CancelTask();
                                    Console.WriteLine("Task completed.");
                                }
                                else
                                {
                                    Console.WriteLine("Key entered incorrectly");
                                    Console.ReadLine();
                                }
                            }
                        }
                    }
                } 
                else if (string.Equals(entry.ToLower().Trim(), "q"))
                {
                    appRunning = false;
                }
                else if(string.Equals(entry.ToLower().Trim(), "c"))
                {
                    Console.WriteLine("Cleared all tasks");
                    taskList.Clear();
                }
                else
                {
                    Task newTask = new Task(entry);
                    newTask.AddTask();
                }
            }
            WriteData();
            Console.WriteLine("Exited gracefully");
        }

        public static void WriteData()
        {
            string json = JsonSerializer.Serialize(taskList, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(_saveFile, json);
        }

        public static void LoadData()
        {
            string json = File.ReadAllText(_saveFile);
            List<Task> tasks = JsonSerializer.Deserialize<List<Task>>(json);
            foreach (var item in tasks)
            {
                taskList.Add(item);
            }
        }
    }

    public class Task
    {
        private static int _counter;

        public int ID { get; set; }
        public string Entry { get; set; }
        public string Notes { get; set; }
        public bool isComplete { get; set; }

        public Task() { }

        public Task(string taskName) 
        {
            this.ID = Interlocked.Increment(ref _counter);
            this.Entry = taskName;
            this.Notes = string.Empty;
            this.isComplete = false;
        }

        public override string ToString()
        {
            return this.ID + " - " + this.Entry + "\n" + this.Notes + "\n[Completed = " + this.isComplete + "]\n";
        }

        public void AddTask()
        {
            this.isComplete = false;
            Program.taskList.Add(this);
        }

        public void CompleteTask()
        {
            this.isComplete = true;
        }

        public void CancelTask()
        {
            this.isComplete = true;
            Program.taskList.Remove(this);
        }
    }
}