namespace AndOS.Application.Entities;

public class Process(Window window, Program program)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Window Window { get; set; } = window;
    public Program Program { get; set; } = program;
}
