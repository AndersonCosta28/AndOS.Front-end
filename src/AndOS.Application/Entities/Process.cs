namespace AndOS.Application.Entities;

public class Process
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Window Window { get; set; }
    public Program Program { get; set; }

    public Process(Program program)
    {
        this.Program = program;
    }

    public Process(Window window, Program program)
    {
        this.Window = window;
        this.Program = program;
    }
}
